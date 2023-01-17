using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

public class CampaignsJuntosIdDeleteTest
{
    private readonly Mock<DbAccess> _mockDb = new();
    private Endpoint _endpoint = null!;

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
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();
        _mockDb.Setup(x => x.CampaignExists(
                It.Is<int>(t => t == campaign.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(true);

        _mockDb.Setup(t => t.DeleteCampaign(
                It.Is<int>(r => r == campaign.Id),
                It.IsAny<CancellationToken>()
            )
        ).Returns(Task.CompletedTask);

        var req = new Request { Id = campaign.Id };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task CampaignDoesNotExists_Fails()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();
        _mockDb.Setup(x => x.CampaignExists(
                It.Is<int>(t => t == campaign.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(false);


        var req = new Request { Id = campaign.Id };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }

    [Test]
    public async Task UnknownDatabaseError_Fails()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();
        _mockDb.Setup(x => x.CampaignExists(
                It.Is<int>(t => t == campaign.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(true);

        _mockDb.Setup(t => t.DeleteCampaign(
                It.Is<int>(r => r == campaign.Id),
                It.IsAny<CancellationToken>()
            )
        ).ThrowsAsync(new DbUpdateException());

        var req = new Request { Id = campaign.Id };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(500);
    }
}