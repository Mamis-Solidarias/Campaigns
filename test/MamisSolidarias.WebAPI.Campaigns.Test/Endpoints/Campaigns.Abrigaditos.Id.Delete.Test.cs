using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.Delete;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal class Campaigns_Abrigaditos_Id_Delete_Test : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext };

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