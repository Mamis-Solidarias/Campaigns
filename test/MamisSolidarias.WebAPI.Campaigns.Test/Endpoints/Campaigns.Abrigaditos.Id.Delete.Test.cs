using System.Threading.Tasks;
using EntityFramework.Exceptions.PostgreSQL;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.Delete;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

public class Campaigns_Abrigaditos_Id_Delete_Test
{
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
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(
            _dbContext
        );
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task WithAValidCampaign_ShouldDeleteIt()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .Build();

        var request = new Request { Id = campaign.Id };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        _dbContext.AbrigaditosCampaigns.Should().BeEmpty();
    }

    [Test]
    public async Task CampaignDoesNotExists_ShouldFail()
    {
        // Arrange
        const int campaignId = 123;

        var request = new Request { Id = campaignId };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }
}