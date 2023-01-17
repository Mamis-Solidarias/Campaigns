using EntityFramework.Exceptions.Sqlite;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MassTransit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal abstract class Test 
{
    protected DataFactory _dataFactory = null!;
    protected CampaignsDbContext _dbContext = null!;
    protected readonly Mock<IBus> _mockBus = new();
    protected readonly Mock<IGraphQlClient> _mockGraphQl = new();
    
    [SetUp]
    public virtual void Setup()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<CampaignsDbContext>()
            .UseSqlite(connection)
            .UseExceptionProcessor()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        _dbContext = new CampaignsDbContext(options);
        _dbContext.Database.EnsureCreated();

        _dataFactory = new DataFactory(_dbContext);
    }

    [TearDown]
    public void Teardown()
    {
        _mockBus.Reset();
        _mockGraphQl.Reset();
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}