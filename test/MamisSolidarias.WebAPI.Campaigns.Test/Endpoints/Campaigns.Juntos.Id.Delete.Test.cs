using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal class CampaignsJuntosIdDeleteTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext };

    [Test]
    public async Task CampaignExists_Success()
    {
        // Arrange
        var campaign = _dataFactory.GenerateJuntosCampaign().Build();
        var req = new Request { Id = campaign.Id };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var dbCampaign = await _dbContext.JuntosCampaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);
        dbCampaign.Should().BeNull();
    }

    [Test]
    public async Task CampaignDoesNotExists_Fails()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();
        var req = new Request { Id = campaign.Id };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }
}