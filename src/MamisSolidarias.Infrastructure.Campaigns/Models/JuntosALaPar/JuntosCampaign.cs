using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;

public class JuntosCampaign : CampaignWithDonations<JuntosParticipant>
{
}

internal sealed class JuntosCampaignConfigurator : CampaignWithDonationsConfigurator<JuntosCampaign, JuntosParticipant>
{
    public JuntosCampaignConfigurator() : base("JuntosDonations")
    {
    }
}