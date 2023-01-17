using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiIdDeleteTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext };

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
        _dbContext.MochiCampaigns.Where(t => t.Id == mochi.Id).Should().BeEmpty();
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