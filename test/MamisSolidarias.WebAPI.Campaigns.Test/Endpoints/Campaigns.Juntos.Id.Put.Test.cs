using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsJuntosIdPutTest
{
    private readonly Mock<DbAccess> _mockDb = new();
    private Endpoint _endpoint = null!;
    private readonly Mock<IBus> _mockBus = new();

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_mockBus.Object, null, _mockDb.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _mockDb.Reset();
        _mockBus.Reset();
    }

    [Test]
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        var campaign = new JuntosCampaign();
        SetUpGetCampaign(campaign.Id, campaign);

        var request = new Request
        {
            Id = campaign.Id,
            Description = campaign.Description,
            Provider = campaign.Provider,
            AddedBeneficiaries = Enumerable.Range(1, 3),
            RemovedBeneficiaries = new List<int>(),
            FundraiserGoal = campaign.FundraiserGoal
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
    }
    
    [Test]
    public async Task WithInvalidParameters_CampaignDoesNotExists_Fails()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();

        SetUpGetCampaign(campaign.Id, null);

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

    private void SetUpGetCampaign(int id, JuntosCampaign? campaign)
    {
        _mockDb.Setup(t => t.GetCampaign(
                It.Is<int>(r => r == id),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(campaign);
    }
}