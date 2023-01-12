using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class DonatedToJuntosCampaign : IConsumer<DonationAddedToCampaign>
{
    private readonly CampaignsDbContext _dbContext;

    public DonatedToJuntosCampaign(CampaignsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<DonationAddedToCampaign> context)
    {
        var message = context.Message;
        var token = context.CancellationToken;

        if (message.Campaign is not Messages.Campaigns.JuntosALaPar)
            return;

        var campaign = await _dbContext.JuntosCampaigns
            .AsTracking()
            .SingleAsync(t => t.Id == message.CampaignId, token);
        
            campaign.Donations.Add(message.DonationId);
            await _dbContext.SaveChangesAsync(token);
            
    }
}