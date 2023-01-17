using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;
using MamisSolidarias.WebAPI.Campaigns.Utils.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils.Abrigaditos;

internal sealed class AbrigaditosCampaignBuilder
    : CampaignWithDonationsBuilder<AbrigaditosCampaign, AbrigaditosParticipant>
{
    public AbrigaditosCampaignBuilder(CampaignsDbContext? db) : base(db)
    {
    }

    public AbrigaditosCampaignBuilder(AbrigaditosCampaign obj) : base(obj)
    {
    }
}