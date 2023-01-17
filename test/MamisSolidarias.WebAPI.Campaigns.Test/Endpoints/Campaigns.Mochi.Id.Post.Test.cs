using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiIdPostTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext };

    [Test]
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        var previousCampaign = _dataFactory.GenerateMochiCampaign()
            .WithParticipants(DataFactory.GetMochiParticipants(5))
            .Build();

        var campaign = DataFactory.GetMochiCampaign().Build();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);

        var createdCampaign = await _dbContext.MochiCampaigns
            .Include(c => c.Participants)
            .Where(t => t.CommunityId == campaign.CommunityId)
            .Where(t => t.Edition == campaign.Edition)
            .SingleAsync();

        createdCampaign.Description.Should().Be(previousCampaign.Description);
        createdCampaign.Provider.Should().Be(previousCampaign.Provider);
        createdCampaign.Participants.Should().BeEquivalentTo(previousCampaign.Participants,
            options => options
                .Excluding(p => p.Id)
                .Excluding(p => p.CampaignId)
                .Excluding(t => t.DonorId)
                .Excluding(t => t.DonorName)
                .Excluding(t => t.DonationDropOffPoint)
                .Excluding(t => t.DonationType)
                .Excluding(t => t.DonationId)
                .Excluding(t => t.State)
        );
    }

    [Test]
    public async Task WithInvalidParameters_CampaignNotFound_Fails()
    {
        // Arrange
        var previousCampaign = DataFactory.GetMochiCampaign()
            .Build();
        MochiCampaign campaign = DataFactory.GetMochiCampaign();

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
        _dbContext.MochiCampaigns.Should().BeEmpty();
    }

    [Test]
    public async Task WithInvalidParameters_RepeatedCommunityAndEdition_Fails()
    {
        // Arrange
        var previousCampaign = _dataFactory.GenerateMochiCampaign().Build();

        var req = new Request
        {
            Edition = previousCampaign.Edition,
            CommunityId = previousCampaign.CommunityId,
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}