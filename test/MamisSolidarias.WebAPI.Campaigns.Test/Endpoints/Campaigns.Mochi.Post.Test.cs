using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiPostTest
{
    private Endpoint _endpoint = null!;
    private readonly Mock<DbAccess> _mockDb = new();
    private readonly Mock<IBus> _mockBus = new();

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(null, _mockBus.Object, _mockDb.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _mockDb.Reset();
        _mockBus.Reset();
    }

    [Test]
    public async Task WithValidParameters_ManyParticipants_Successful()
    {
        // Arrange
        MochiCampaign campaign = DataFactory.GetMochiCampaign();
        
        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.BeneficiaryId)
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
    }

    [Test]
    public async Task WithValidParameters_NoParticipants_Successful()
    {
        // Arrange
        MochiCampaign campaign = DataFactory.GetMochiCampaign().WithParticipants(new List<MochiParticipant>());

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id)
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
    }
    


    [Test]
    public async Task WithInvalidParameters_RepeatedEditionAndCommunity_Fails()
    {
        // Arrange
        MochiCampaign campaign = DataFactory.GetMochiCampaign().WithParticipants(new List<MochiParticipant>());

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id)
        };

        _mockDb.Setup(r => r.AddMochiCampaign(
                It.Is<MochiCampaign>(t => t.CommunityId == campaign.CommunityId && t.Edition == campaign.Edition),
                CancellationToken.None)
            )
            .ThrowsAsync(new UniqueConstraintException());

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}