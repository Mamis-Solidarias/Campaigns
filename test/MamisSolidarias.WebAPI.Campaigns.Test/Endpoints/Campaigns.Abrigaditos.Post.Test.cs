using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsAbrigaditosPostTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext, _mockGraphQl.Object, _mockBus.Object };

    [Test]
    public async Task WithValidParameters_ManyParticipants_Successful()
    {
        // Arrange
        var campaign = DataFactory.GetAbrigaditosCampaign()
            .WithParticipants(DataFactory.GetAbrigaditosParticipants(10))
            .Build();
        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);
        var req = Map(campaign);

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        _dbContext.AbrigaditosCampaigns.Should().HaveCount(1);
        var result = await _dbContext.AbrigaditosCampaigns
            .Include(t => t.Donations)
            .SingleAsync(t => t.Id == _endpoint.Response.Id);

        result.Should()
            .BeEquivalentTo(campaign, opt =>
                opt.Excluding(t => t.Id)
                    .Excluding(t => t.Donations)
                    .Excluding(t => t.Participants)
            );
    }

    [Test]
    public async Task CommunityDoesNotExists_Fails()
    {
        // Arrange
        var campaign = DataFactory.GetAbrigaditosCampaign()
            .Build();

        _mockGraphQl.MockEmptyResponse(t => t.GetCommunity.ExecuteAsync(
            It.Is<string>(r => r == campaign.CommunityId),
            It.IsAny<CancellationToken>())
        );

        var req = Map(campaign);

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
        _dbContext.AbrigaditosCampaigns.Should().BeEmpty();
    }

    [Test]
    public async Task WithValidParameters_NoParticipants_Successful()
    {
        // Arrange
        var campaign = DataFactory
            .GetAbrigaditosCampaign()
            .WithParticipants(new List<AbrigaditosParticipant>())
            .Build();

        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);

        var req = Map(campaign);

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        _dbContext.AbrigaditosCampaigns.Should().HaveCount(1);
        var result = await _dbContext.AbrigaditosCampaigns
            .Include(t => t.Donations)
            .SingleAsync(t => t.Id == _endpoint.Response.Id);

        result.Should()
            .BeEquivalentTo(campaign, opt =>
                opt.Excluding(t => t.Id)
                    .Excluding(t => t.Donations)
                    .Excluding(t => t.Participants)
            );
    }


    [Test]
    public async Task WithInvalidParameters_UserDoesNotHavePermission_Fails()
    {
        // Arrange
        var campaign = DataFactory.GetAbrigaditosCampaign()
            .WithParticipants(DataFactory.GetAbrigaditosParticipants(10));

        _mockGraphQl.MockAuthenticationError(
            t => t.GetCommunity.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )
        );

        var req = Map(campaign);

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(403);
        _dbContext.AbrigaditosCampaigns.Should().BeEmpty();
    }

    [Test]
    public async Task WithInvalidParameters_RepeatedEditionAndCommunity_Fails()
    {
        // Arrange
        var existingCampaign = _dataFactory.GenerateAbrigaditosCampaign().Build();
        var newCampaign = DataFactory.GetAbrigaditosCampaign()
            .WithEdition(existingCampaign.Edition)
            .WithCommunityId(existingCampaign.CommunityId)
            .Build();

        var req = Map(newCampaign);

        _mockGraphQl.MockGetCommunity(t => t == newCampaign.CommunityId, newCampaign.CommunityId);


        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }

    private static Request Map(AbrigaditosCampaign campaign)
    {
        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.BeneficiaryId),
            Description = campaign.Description,
            FundraiserGoal = campaign.FundraiserGoal,
            Provider = campaign.Provider
        };
        return req;
    }
}