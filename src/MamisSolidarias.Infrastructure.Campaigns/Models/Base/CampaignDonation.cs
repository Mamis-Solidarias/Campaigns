namespace MamisSolidarias.Infrastructure.Campaigns.Models.Base;

public class CampaignDonation
{
    public Guid Id { get; set; }

    public static implicit operator Guid(CampaignDonation campaignDonation)
    {
        return campaignDonation.Id;
    }

    public static implicit operator CampaignDonation(Guid id)
    {
        return new CampaignDonation { Id = id };
    }
}