using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiPostTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext, _mockBus.Object };

    [Test]
    public async Task WithValidParameters_ManyParticipants_Successful()
    {
        // Arrange
        MochiCampaign campaign = DataFactory.GetMochiCampaign();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.BeneficiaryId),
            Description = campaign.Description,
            Provider = campaign.Provider
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        var result = await _dbContext.MochiCampaigns.SingleOrDefaultAsync(
            t => t.CommunityId == campaign.CommunityId && t.Edition == campaign.Edition);

        result.Should().NotBeNull();
        result!.Edition.Should().Be(campaign.Edition);
        result.CommunityId.Should().Be(campaign.CommunityId);
        result.Description.Should().Be(campaign.Description);
        result.Provider.Should().Be(campaign.Provider);
    }

    [Test]
    public async Task WithValidParameters_NoParticipants_Successful()
    {
        // Arrange
        MochiCampaign campaign = DataFactory.GetMochiCampaign()
            .WithoutParticipants();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id),
            Description = campaign.Description,
            Provider = campaign.Provider
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
        var result = await _dbContext.MochiCampaigns.SingleOrDefaultAsync(
            t => t.CommunityId == campaign.CommunityId && t.Edition == campaign.Edition);

        result.Should().NotBeNull();
        result!.Edition.Should().Be(campaign.Edition);
        result.CommunityId.Should().Be(campaign.CommunityId);
        result.Description.Should().Be(campaign.Description);
        result.Provider.Should().Be(campaign.Provider);
    }


    [Test]
    public async Task WithInvalidParameters_RepeatedEditionAndCommunity_Fails()
    {
        // Arrange

        var campaign = _dataFactory
            .GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}