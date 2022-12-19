using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Participants.Id.PUT;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(IGraphQlClient graphQlClient,CampaignsDbContext dbContext, DbAccess? db = null)
    {
        _graphQlClient = graphQlClient;
        _db = db ?? new(dbContext);
    }

    public override void Configure()
    {
        Put("/campaigns/juntos/participants/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var participant = await _db.GetParticipant(req.Id,ct);

        if (participant is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var executor = await _graphQlClient.GetDonor.ExecuteAsync(req.DonorId, ct);
        var hasErrors = await executor.HandleErrors(
            async token => await SendForbiddenAsync(token),
            async (errors, token) => await SendGraphQlErrors(errors, token),
            ct);

        if (hasErrors)
        {
            return;
        }
        
        if (executor.Data?.Donor is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        participant.DonorId = req.DonorId;
        participant.DonorName = executor.Data.Donor.Name;
        participant.State = ParticipantState.MissingDonation;
        
        await _db.SaveChanges(ct);
        await SendAsync(new Response(participant.State), cancellation:ct);
    }
    
    private async Task SendGraphQlErrors(IEnumerable<IClientError> errors, CancellationToken token)
    {
        foreach (var clientError in errors)
            AddError(clientError.Message);
        
        await SendErrorsAsync(cancellation: token);
    }
}