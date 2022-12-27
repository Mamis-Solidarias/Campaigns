using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiIdDeleteTest
{
    private Endpoint _endpoint = null!;
    private readonly Mock<DbAccess> _mockDb = new();

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(null, _mockDb.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _mockDb.Reset();
    }

    [Test]
    public async Task CampaignExists_Success()
    {
        // Arrange
        MochiCampaign mochi = DataFactory.GetMochiCampaign();
        _mockDb.Setup(x => x.GetMochiAsync(
            It.Is<int>(t=> t == mochi.Id),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(mochi);

        _mockDb.Setup(t => t.DeleteMochiAsync(
                It.Is<MochiCampaign>(r => r.Id == mochi.Id),
                It.IsAny<CancellationToken>()
            )
        ).Returns(Task.CompletedTask);

        var req = new Request {Id = mochi.Id};
        
        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task CampaignDoesNotExists_Fails()
    {
        // Arrange
        MochiCampaign mochi = DataFactory.GetMochiCampaign();
        _mockDb.Setup(x => x.GetMochiAsync(
            It.Is<int>(t=> t == mochi.Id),
            It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((MochiCampaign?)null);


        var req = new Request {Id = mochi.Id};
        
        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }
}