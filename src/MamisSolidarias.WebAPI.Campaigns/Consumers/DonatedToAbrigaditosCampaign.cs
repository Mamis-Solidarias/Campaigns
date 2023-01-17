using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class DonatedToAbrigaditosCampaign : IConsumer<DonationAddedToCampaign>
{
    private readonly CampaignsDbContext _db;

    public DonatedToAbrigaditosCampaign(CampaignsDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<DonationAddedToCampaign> context)
    {
        var token = context.CancellationToken;
        var message = context.Message;

        if (message.Campaign is not Messages.Campaigns.Abrigaditos)
            return;

        var campaign = await _db.AbrigaditosCampaigns
            .AsTracking()
            .SingleAsync(t => t.Id == message.CampaignId, token);

        campaign.Donations.Add(message.DonationId);
        await _db.SaveChangesAsync(token);
    }
}