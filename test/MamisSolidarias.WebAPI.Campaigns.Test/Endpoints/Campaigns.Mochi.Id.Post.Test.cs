using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Sqlite;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiIdPostTest
{
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
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_db);
    }

    [TearDown]
    public void Teardown()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }

    [Test]
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        var previousCampaign = _dataFactory.GenerateMochiCampaign()
            .WithParticipants(DataFactory.GetMochiParticipants(5))
            .Build();

        var campaign = DataFactory.GetMochiCampaign().Build();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);

        var createdCampaign = await _db.MochiCampaigns
            .Include(c => c.Participants)
            .Where(t => t.CommunityId == campaign.CommunityId)
            .Where(t => t.Edition == campaign.Edition)
            .SingleAsync();

        createdCampaign.Description.Should().Be(previousCampaign.Description);
        createdCampaign.Provider.Should().Be(previousCampaign.Provider);
        createdCampaign.Participants.Should().BeEquivalentTo(previousCampaign.Participants,
            options => options
                .Excluding(p => p.Id)
                .Excluding(p => p.CampaignId)
                .Excluding(t => t.DonorId)
                .Excluding(t => t.DonorName)
                .Excluding(t => t.DonationDropOffPoint)
                .Excluding(t => t.DonationType)
                .Excluding(t => t.DonationId)
                .Excluding(t => t.State)
        );
    }

    [Test]
    public async Task WithInvalidParameters_CampaignNotFound_Fails()
    {
        // Arrange
        var previousCampaign = DataFactory.GetMochiCampaign()
            .Build();
        MochiCampaign campaign = DataFactory.GetMochiCampaign();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
        _db.MochiCampaigns.Should().BeEmpty();
    }

    [Test]
    public async Task WithInvalidParameters_RepeatedCommunityAndEdition_Fails()
    {
        // Arrange
        var previousCampaign = _dataFactory.GenerateMochiCampaign().Build();

        var req = new Request
        {
            Edition = previousCampaign.Edition,
            CommunityId = previousCampaign.CommunityId,
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}