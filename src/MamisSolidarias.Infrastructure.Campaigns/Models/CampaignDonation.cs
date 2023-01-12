namespace MamisSolidarias.Infrastructure.Campaigns.Models;

public class CampaignDonation
{
    public Guid Id { get; set; }

    public static implicit operator Guid(CampaignDonation campaignDonation) => campaignDonation.Id;

    public static implicit operator CampaignDonation(Guid id) => new() { Id = id };
}