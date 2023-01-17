using System;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class DonatedToAbrigaditosCampaign_Test
{
    private DonatedToAbrigaditosCampaign _consumer = null!;
    private DataFactory _dataFactory = null!;
    private CampaignsDbContext _dbContext = null!;

    [SetUp]
    protected void SetUp()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<CampaignsDbContext>()
            .UseSqlite(connection)
            .Options;

        _dbContext = new CampaignsDbContext(options);
        _dbContext.Database.EnsureCreated();

        _dataFactory = new DataFactory(_dbContext);

        _consumer = new DonatedToAbrigaditosCampaign(_dbContext);
    }

    [TearDown]
    protected void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
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
                Guid.NewGuid(), null, null, campaign.Id, campaignName
            )
        );

        // Act
        await _consumer.Consume(context);

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
                Guid.NewGuid(), null, null, campaignId, Messages.Campaigns.Abrigaditos
            )
        );

        // Act
        var action = async () => await _consumer.Consume(context);

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
                donation, null, null, campaign.Id, Messages.Campaigns.Abrigaditos
            )
        );

        // Act
        var action = async () => await _consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
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
                donation, null, null, campaign.Id, Messages.Campaigns.Abrigaditos
            )
        );

        // Act
        await _consumer.Consume(context);

        // Assert
        var result = await _dbContext
            .AbrigaditosCampaigns
            .SingleAsync(t => t.Id == campaign.Id);
        result.Donations.Should().HaveCount(1);
        result.Donations.Should().ContainEquivalentOf((CampaignDonation)donation);
    }
}