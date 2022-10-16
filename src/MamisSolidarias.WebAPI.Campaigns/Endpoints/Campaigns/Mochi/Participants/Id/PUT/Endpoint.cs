using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.PUT;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly DbAccess _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(IGraphQlClient graphQlClient, CampaignsDbContext dbContext, DbAccess? dbAccess = null)
    {
        _graphQlClient = graphQlClient;
        _db = dbAccess ?? new DbAccess(dbContext);
    }

    public override void Configure()
    {
        Put("campaigns/mochi/participants/{id}");
        Policies(Utils.Security.Policies.CanWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        MochiParticipant? participant = await _db.GetParticipantAsync(req.Id, ct);
        if (participant is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var executor = await _graphQlClient.GetDonor.ExecuteAsync(req.DonorId, ct);

        var hasErrors = await executor.HandleErrors(
            async token => await SendForbiddenAsync(token),
            async (errors,token) => await SendGraphQlErrors(errors,token),
            ct
        );
        if (hasErrors)
            return;
        
        if (executor.Data?.Donor is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        participant.DonorId = req.DonorId;
        participant.DonorName = executor.Data.Donor.Name;
        participant.DonationDropOffLocation = req.DonationDropOffLocation;
        participant.DonationType = Enum.Parse<MochiDonationType>(req.DonationType, true);
        participant.State = MochiParticipantState.MissingDonation;

        await _db.SaveParticipantAsync(participant, ct);
        await SendAsync(new Response(participant.State), cancellation: ct);
    }

    private async Task SendGraphQlErrors(IEnumerable<IClientError> errors, CancellationToken token)
    {
        foreach (var clientError in errors)
            AddError(clientError.Message);
        
        await SendErrorsAsync(cancellation: token);
    }
}