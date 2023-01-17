using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Sqlite;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiParticipantsIdDeleteTest
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
        var campaign = _dataFactory.GenerateMochiCampaign().Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var request = new Request
        {
            Id = participant.Id
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        _endpoint.Response.ParticipantId.Should().Be(participant.Id);

        var result = await _db.MochiParticipants.SingleAsync(t => t.Id == participant.Id);
        result.State.Should().Be(ParticipantState.MissingDonor);
        result.DonorId.Should().BeNull();
        result.DonorName.Should().BeNull();
        result.DonationType.Should().BeNull();
        result.DonationDropOffPoint.Should().BeNull();
    }

    [Test]
    public async Task ParticipantDoesNotExists_Fails()
    {
        // Arrange
        var participant = DataFactory.GetMochiParticipant()
            .Build();

        var request = new Request
        {
            Id = participant.Id
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }
}