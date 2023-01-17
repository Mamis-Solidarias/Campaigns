using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsJuntosPostTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments => new object?[] { _dbContext, _mockBus.Object, _mockGraphQl.Object };

    [Test]
    public async Task WithValidParameters_ManyParticipants_Successful()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign()
            .WithParticipants(DataFactory.GetJuntosParticipants(5));

        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);

        var req = Map(campaign);

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        var result = await _dbContext.JuntosCampaigns
            .Where(t => t.Edition == campaign.Edition && t.CommunityId == campaign.CommunityId)
            .FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result!.CommunityId.Should().Be(campaign.CommunityId);
        result.Edition.Should().Be(campaign.Edition);
        result.Description.Should().Be(campaign.Description);
        result.FundraiserGoal.Should().Be(campaign.FundraiserGoal);
        result.Provider.Should().Be(campaign.Provider);
    }

    [Test]
    public async Task WithValidParameters_NoParticipants_Successful()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory
            .GetJuntosCampaign()
            .WithoutParticipants();

        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);

        var req = Map(campaign);

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        var result = await _dbContext.JuntosCampaigns
            .Where(t => t.Edition == campaign.Edition && t.CommunityId == campaign.CommunityId)
            .FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result!.CommunityId.Should().Be(campaign.CommunityId);
        result.Edition.Should().Be(campaign.Edition);
        result.Description.Should().Be(campaign.Description);
        result.FundraiserGoal.Should().Be(campaign.FundraiserGoal);
        result.Provider.Should().Be(campaign.Provider);
    }

    [Test]
    public async Task WithInvalidParameters_UserDoesNotHavePermission_Fails()
    {
        // Arrange
        var campaign = DataFactory.GetJuntosCampaign()
            .WithParticipants(DataFactory.GetJuntosParticipants(3))
            .Build();

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

        var result = await _dbContext.JuntosCampaigns
            .Where(t => t.Edition == campaign.Edition && t.CommunityId == campaign.CommunityId)
            .FirstOrDefaultAsync();

        result.Should().BeNull();
    }

    [Test]
    public async Task WithInvalidParameters_RepeatedEditionAndCommunity_Fails()
    {
        // Arrange
        JuntosCampaign campaign = _dataFactory.GenerateJuntosCampaign();

        var req = Map(campaign);

        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }

    private static Request Map(JuntosCampaign campaign)
    {
        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id),
            Description = campaign.Description,
            FundraiserGoal = campaign.FundraiserGoal,
            Provider = campaign.Provider
        };
        return req;
    }
}