using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;

public class MochiCampaign : Campaign<MochiParticipant>
{
}

internal sealed class MochiConfigurator : CampaignConfigurator<MochiCampaign, MochiParticipant>
{
}