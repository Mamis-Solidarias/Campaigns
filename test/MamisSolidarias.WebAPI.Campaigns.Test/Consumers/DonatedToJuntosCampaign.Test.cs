using System;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class DonatedToJuntosCampaign_Test : ConsumerTest<DonatedToJuntosCampaign>
{
    protected override DonatedToJuntosCampaign CreateConsumer()
    {
        return new DonatedToJuntosCampaign(_dbContext);
    }

    [TestCase(Messages.Campaigns.Abrigaditos)]
    [TestCase(Messages.Campaigns.Misiones)]
    [TestCase(Messages.Campaigns.Navidemos)]
    [TestCase(Messages.Campaigns.UnaMochiComoLaTuya)]
    [TestCase(Messages.Campaigns.NavidadCompartida)]
    [TestCase(Messages.Campaigns.ApadrinaMiSonriza)]
    public async Task IncorrectCampaign_ShouldReturn(Messages.Campaigns campaignName)
    {
        // Arrange
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .WithoutDonations()
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), null, null, campaign.Id, campaignName, 100, Currency.ARS
            )
        );

        // Act
        await Consumer.Consume(context);

        // Assert
        var result = await _dbContext.JuntosCampaigns.SingleAsync(t => t.Id == campaign.Id);

        result.Donations.Should().HaveCount(0);
    }

    [Test]
    public async Task CampaignDoesNotExists_ShouldThrowInvalidOperation()
    {
        // Arrange
        const int campaignId = 123;
        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), null, null, campaignId, Messages.Campaigns.JuntosALaPar, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Test]
    public async Task CurrencyIsNotARS_ShouldThrowNotSupported()
    {
        // Arrange
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .WithoutDonations()
            .Build();
        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), null, null, campaign.Id, Messages.Campaigns.JuntosALaPar, 100, Currency.EUR
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<NotSupportedException>();
    }

    [Test]
    public async Task DonationIsRepeated_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var donation = Guid.NewGuid();
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .WithDonations(donation)
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donation, null, null, campaign.Id, Messages.Campaigns.JuntosALaPar, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task DonationIsValid_ShouldSucceed()
    {
        // Arrange
        var donation = Guid.NewGuid();
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .WithoutDonations()
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donation, null, null, campaign.Id, Messages.Campaigns.JuntosALaPar, 100, Currency.ARS
            )
        );

        // Act
        await Consumer.Consume(context);

        // Assert
        var result = await _dbContext.JuntosCampaigns.SingleAsync(t => t.Id == campaign.Id);
        result.Donations.Should().HaveCount(1);
        result.Donations.Should().ContainEquivalentOf((CampaignDonation)donation);
    }
}