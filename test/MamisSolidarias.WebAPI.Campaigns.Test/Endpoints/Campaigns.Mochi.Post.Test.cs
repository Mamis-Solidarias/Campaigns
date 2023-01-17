using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Sqlite;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using MassTransit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiPostTest
{
    private readonly Mock<IBus> _mockBus = new();
    private DataFactory _dataFactory = null!;
    private CampaignsDbContext _db = null!;
    private Endpoint _endpoint = null!;

    [SetUp]
    public void Setup()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<CampaignsDbContext>()
            .UseSqlite(connection)
            .UseExceptionProcessor()
            .Options;

        _db = new CampaignsDbContext(options);
        _db.Database.EnsureCreated();

        _dataFactory = new DataFactory(_db);
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_db, _mockBus.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
        _mockBus.Reset();
    }

    [Test]
    public async Task WithValidParameters_ManyParticipants_Successful()
    {
        // Arrange
        MochiCampaign campaign = DataFactory.GetMochiCampaign();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.BeneficiaryId),
            Description = campaign.Description,
            Provider = campaign.Provider
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        var result = await _db.MochiCampaigns.SingleOrDefaultAsync(
            t => t.CommunityId == campaign.CommunityId && t.Edition == campaign.Edition);

        result.Should().NotBeNull();
        result!.Edition.Should().Be(campaign.Edition);
        result.CommunityId.Should().Be(campaign.CommunityId);
        result.Description.Should().Be(campaign.Description);
        result.Provider.Should().Be(campaign.Provider);
    }

    [Test]
    public async Task WithValidParameters_NoParticipants_Successful()
    {
        // Arrange
        MochiCampaign campaign = DataFactory.GetMochiCampaign()
            .WithoutParticipants();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id),
            Description = campaign.Description,
            Provider = campaign.Provider
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        var result = await _db.MochiCampaigns.SingleOrDefaultAsync(
            t => t.CommunityId == campaign.CommunityId && t.Edition == campaign.Edition);

        result.Should().NotBeNull();
        result!.Edition.Should().Be(campaign.Edition);
        result.CommunityId.Should().Be(campaign.CommunityId);
        result.Description.Should().Be(campaign.Description);
        result.Provider.Should().Be(campaign.Provider);
    }


    [Test]
    public async Task WithInvalidParameters_RepeatedEditionAndCommunity_Fails()
    {
        // Arrange

        var campaign = _dataFactory
            .GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}