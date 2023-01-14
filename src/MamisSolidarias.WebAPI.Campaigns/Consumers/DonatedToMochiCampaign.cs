using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class DonatedToMochiCampaign : IConsumer<DonationAddedToCampaign>
{
    private readonly CampaignsDbContext _db;

    public DonatedToMochiCampaign(CampaignsDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<DonationAddedToCampaign> context)
    {
        var message = context.Message;
        var token = context.CancellationToken;
        if (message.Campaign is not Messages.Campaigns.UnaMochiComoLaTuya)
            return;

        if (message.ParticipantId is null)
            return;

        var participant = await _db.MochiParticipants
            .AsTracking()
            .SingleAsync(t => t.Id == message.ParticipantId, token);


        if (participant.State is not ParticipantState.MissingDonation)
            throw new InvalidOperationException("Participant is not in MissingDonation state");

        ArgumentNullException.ThrowIfNull(participant.DonorId);

        if (participant.DonationId is not null)
            throw new InvalidOperationException("Participant already has a donation");

        if (participant.DonorId != message.DonorId)
            throw new ArgumentException("Donor is not the expected one");

        if (participant.CampaignId != message.CampaignId)
            throw new ArgumentException("Campaign is not the expected one");

        participant.State = ParticipantState.DonationReceived;
        participant.DonationId = message.DonationId;

        await _db.SaveChangesAsync(token);
    }
}