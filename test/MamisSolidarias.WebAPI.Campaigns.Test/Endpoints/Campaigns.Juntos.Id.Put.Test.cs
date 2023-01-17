using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsJuntosIdPutTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments
        => new object?[] { _dbContext, _mockBus.Object };


    [Test]
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .Build();

        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = "New Provider",
            AddedBeneficiaries = Enumerable.Range(1, 3),
            RemovedBeneficiaries = new List<int>(),
            FundraiserGoal = 1000,
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var updatedCampaign = await _dbContext.JuntosCampaigns.SingleAsync(t=> t.Id == campaign.Id);
        updatedCampaign.Description.Should().Be(request.Description);
        updatedCampaign.Provider.Should().Be(request.Provider);
        updatedCampaign.FundraiserGoal.Should().Be(request.FundraiserGoal);
    }

    [Test]
    public async Task WithInvalidParameters_CampaignDoesNotExists_Fails()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();
        
        var request = new Request
        {
            Id = campaign.Id,
            Description = campaign.Description,
            Provider = campaign.Provider,
            AddedBeneficiaries = Enumerable.Range(1, 3),
            RemovedBeneficiaries = Enumerable.Range(4, 5),
            FundraiserGoal = campaign.FundraiserGoal
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }
    

}