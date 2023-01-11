namespace MamisSolidarias.Infrastructure.Campaigns.Models;

public class Donation
{
    public Guid Id { get; set; }

    public static implicit operator Guid(Donation donation) => donation.Id;

    public static implicit operator Donation(Guid id) => new() { Id = id };
}