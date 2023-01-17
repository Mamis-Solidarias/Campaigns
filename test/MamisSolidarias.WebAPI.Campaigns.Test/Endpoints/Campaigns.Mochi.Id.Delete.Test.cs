using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Sqlite;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiIdDeleteTest
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
    public async Task CampaignExists_Success()
    {
        // Arrange
        var mochi = _dataFactory.GenerateMochiCampaign().Build();

        var req = new Request { Id = mochi.Id };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        _db.MochiCampaigns.Where(t => t.Id == mochi.Id).Should().BeEmpty();
    }

    [Test]
    public async Task CampaignDoesNotExists_Fails()
    {
        // Arrange
        const int id = 1;

        var req = new Request { Id = id };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }
}