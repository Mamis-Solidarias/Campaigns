using System;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class DonatedToAbrigaditosCampaign_Test : ConsumerTest<DonatedToAbrigaditosCampaign>
{
    protected override DonatedToAbrigaditosCampaign CreateConsumer()
    {
        return new DonatedToAbrigaditosCampaign(_dbContext);
    }

    [TestCase(Messages.Campaigns.JuntosALaPar)]
    [TestCase(Messages.Campaigns.Misiones)]
    [TestCase(Messages.Campaigns.Navidemos)]
    [TestCase(Messages.Campaigns.UnaMochiComoLaTuya)]
    [TestCase(Messages.Campaigns.NavidadCompartida)]
    [TestCase(Messages.Campaigns.ApadrinaMiSonriza)]
    public async Task IncorrectCampaign_ShouldReturn(Messages.Campaigns campaignName)
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .WithoutDonations()
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), null, null, campaign.Id, campaignName,
                100, Currency.ARS
            )
        );

        // Act
        await Consumer.Consume(context);

        // Assert
        var result = await _dbContext
            .AbrigaditosCampaigns
            .SingleAsync(t => t.Id == campaign.Id);

        result.Donations.Should().HaveCount(0);
    }

    [Test]
    public async Task CampaignDoesNotExists_ShouldThrowInvalidOperation()
    {
        // Arrange
        const int campaignId = 123;
        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), null, null, campaignId, Messages.Campaigns.Abrigaditos,
                100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task DonationIsRepeated_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var donation = Guid.NewGuid();
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .WithDonations(donation)
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donation, null, null, campaign.Id, Messages.Campaigns.Abrigaditos,
                100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Test]
    public async Task CurrencyIsNotARS_ShouldThrowNotSupportedException()
    {
        // Arrange
        var donation = Guid.NewGuid();
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .WithDonations(donation)
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donation, null, null, campaign.Id, Messages.Campaigns.Abrigaditos,
                100, Currency.EUR
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<NotSupportedException>();
    }

    [Test]
    public async Task DonationIsValid_ShouldSucceed()
    {
        // Arrange
        var donation = Guid.NewGuid();
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .WithoutDonations()
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donation, null, null, campaign.Id, Messages.Campaigns.Abrigaditos, 100, Currency.ARS
            )
        );

        // Act
        await Consumer.Consume(context);

        // Assert
        var result = await _dbContext
            .AbrigaditosCampaigns
            .SingleAsync(t => t.Id == campaign.Id);
        result.Donations.Should().HaveCount(1);
        result.Donations.Should().ContainEquivalentOf((CampaignDonation)donation);
    }
}