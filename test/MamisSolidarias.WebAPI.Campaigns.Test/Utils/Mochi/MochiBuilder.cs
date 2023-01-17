using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.WebAPI.Campaigns.Utils.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Utils.Mochi;

internal sealed class MochiBuilder : CampaignBuilder<MochiCampaign,MochiParticipant>
{
    public MochiBuilder(CampaignsDbContext? db) : base(db)
    {
    }

    public MochiBuilder(MochiCampaign obj) : base(obj)
    {
    }
}