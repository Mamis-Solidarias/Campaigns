

using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;

public class AbrigaditosCampaign : CampaignWithDonations<AbrigaditosParticipant>
{

}

internal sealed class AbrigaditosCampaignConfigurator : CampaignWithDonationsConfigurator<AbrigaditosCampaign,AbrigaditosParticipant>
{
    public AbrigaditosCampaignConfigurator() : 
        base("AbrigaditosDonations") { }
}