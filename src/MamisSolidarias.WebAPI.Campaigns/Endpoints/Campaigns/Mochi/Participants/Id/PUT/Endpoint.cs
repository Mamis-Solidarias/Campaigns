using FastEndpoints;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using Microsoft.EntityFrameworkCore;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.PUT;

internal sealed class Endpoint : Endpoint<Request, Response>
{
    private readonly CampaignsDbContext _db;
    private readonly IGraphQlClient _graphQlClient;

    public Endpoint(CampaignsDbContext dbContext, IGraphQlClient graphQlClient)
    {
        _graphQlClient = graphQlClient;
        _db = dbContext;
    }

    public override void Configure()
    {
        Put("campaigns/mochi/participants/{id}");
        Policies(Utils.Security.Policies.CanWrite);
        Description(t=> t.WithTags("Una Mochi como la Tuya"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        try
        {
            var participant = await _db.MochiParticipants
                .AsTracking()
                .SingleAsync(t => t.Id == req.Id, ct);

            var executor = await _graphQlClient.GetDonor.ExecuteAsync(req.DonorId, ct);

            var hasErrors = await executor.HandleErrors(
                async token => await SendForbiddenAsync(token),
                async (errors, token) => await SendGraphQlErrors(errors, token),
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
            participant.DonationDropOffPoint = req.DonationDropOffLocation;
            participant.DonationType = Enum.Parse<DonationType>(req.DonationType, true);
            participant.State = ParticipantState.MissingDonation;

            await _db.SaveChangesAsync(ct);
            await SendAsync(new Response(participant.State), cancellation: ct);
        }
        catch (InvalidOperationException)
        {
            await SendNotFoundAsync(ct);
        }
    }

    private async Task SendGraphQlErrors(IEnumerable<IClientError> errors, CancellationToken token)
    {
        foreach (var clientError in errors)
            AddError(clientError.Message);

        await SendErrorsAsync(cancellation: token);
    }
}