using System;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class DonatedToJuntosCampaign_Test
{
    private DonatedToJuntosCampaign _consumer = null!;
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

        _consumer = new DonatedToJuntosCampaign(_dbContext);
    }

    [TearDown]
    protected void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
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
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), null, null, campaign.Id, campaignName
            )
        );

        // Act
        await _consumer.Consume(context);

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
                Guid.NewGuid(), null, null, campaignId, Messages.Campaigns.JuntosALaPar
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
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .WithDonations(donation)
            .Build();
        
        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donation, null, null, campaign.Id, Messages.Campaigns.JuntosALaPar
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
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donation, null, null, campaign.Id, Messages.Campaigns.JuntosALaPar
            )
        );
        
        // Act
        await _consumer.Consume(context);
        
        // Assert
        var result = await _dbContext.JuntosCampaigns.SingleAsync(t => t.Id == campaign.Id);
        result.Donations.Should().HaveCount(1);
        result.Donations.Should().ContainEquivalentOf((Donation)donation);
    }
}