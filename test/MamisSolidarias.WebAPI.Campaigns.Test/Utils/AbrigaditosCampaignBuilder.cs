using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal sealed class AbrigaditosCampaignBuilder 
    : CampaignWithDonationsBuilder<AbrigaditosCampaign,AbrigaditosParticipant>
{
    public AbrigaditosCampaignBuilder(CampaignsDbContext? db) : base(db)
    {
    }

    public AbrigaditosCampaignBuilder(AbrigaditosCampaign obj) : base(obj)
    {
    }
}