using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Sqlite;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using MassTransit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

public class Campaigns_Abrigaditos_Id_Put_Test
{
    private readonly Mock<IBus> _mockBus = new();
    private DataFactory _dataFactory = null!;
    private CampaignsDbContext _dbContext = null!;
    private Endpoint _endpoint = null!;

    [SetUp]
    public void SetUp()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<CampaignsDbContext>()
            .UseSqlite(connection)
            .UseExceptionProcessor()
            .Options;

        _dbContext = new CampaignsDbContext(options);
        _dbContext.Database.EnsureCreated();

        _dataFactory = new DataFactory(_dbContext);
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_dbContext, _mockBus.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockBus.Reset();
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task WhenTheCampaignDoesNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var campaign = DataFactory.GetAbrigaditosCampaign().Build();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = null,
            FundraiserGoal = 999
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }

    [Test]
    public async Task WhenTheCampaignExists_UpdatingOnlyValues_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign().Build();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = "New Provider",
            FundraiserGoal = 999
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var updatedCampaign = await _dbContext.AbrigaditosCampaigns.SingleAsync(t => t.Id == campaign.Id);
        updatedCampaign.Description.Should().Be(request.Description);
        updatedCampaign.Provider.Should().Be(request.Provider);
        updatedCampaign.FundraiserGoal.Should().Be(request.FundraiserGoal);
    }

    [Test]
    public async Task WhenTheCampaignExists_RemovingParticipants_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .WithParticipants(DataFactory.GetAbrigaditosParticipants(5))
            .Build();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = "New Provider",
            FundraiserGoal = 999,
            RemovedBeneficiaries = campaign.Participants.Select(t => t.BeneficiaryId)
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var updatedCampaign = await _dbContext.AbrigaditosCampaigns
            .Include(t => t.Participants)
            .SingleAsync(t => t.Id == campaign.Id);
        updatedCampaign.Description.Should().Be(request.Description);
        updatedCampaign.Provider.Should().Be(request.Provider);
        updatedCampaign.FundraiserGoal.Should().Be(request.FundraiserGoal);
        updatedCampaign.Participants.Should().BeEmpty();
    }

    [Test]
    public async Task WhenTheCampaignExists_AddingParticipants_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .Build();
        var participants = DataFactory.GetAbrigaditosParticipants(5)
            .Select(t => t.Build())
            .ToArray();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = "New Provider",
            FundraiserGoal = 999,
            AddedBeneficiaries = participants.Select(t => t.BeneficiaryId)
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var updatedCampaign = await _dbContext.AbrigaditosCampaigns
            .Include(t => t.Participants)
            .SingleAsync(t => t.Id == campaign.Id);
        updatedCampaign.Description.Should().Be(request.Description);
        updatedCampaign.Provider.Should().Be(request.Provider);
        updatedCampaign.FundraiserGoal.Should().Be(request.FundraiserGoal);
    }
}