using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.WebAPI.Campaigns.Utils.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils.JuntosALaPar;

internal sealed class JuntosBuilder : CampaignWithDonationsBuilder<JuntosCampaign, JuntosParticipant>
{
    public JuntosBuilder(CampaignsDbContext? db)
        : base(db)
    {
    }

    public JuntosBuilder(JuntosCampaign obj)
        : base(obj)
    {
    }
}