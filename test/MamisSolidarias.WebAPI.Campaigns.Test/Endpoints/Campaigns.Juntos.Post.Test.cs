using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using FluentAssertions;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsJuntosPostTest
{
    private readonly Mock<IBus> _mockBus = new();
    private readonly Mock<DbAccess> _mockDb = new();
    private readonly Mock<IGraphQlClient> _mockGraphQl = new();
    private Endpoint _endpoint = null!;

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_mockBus.Object, _mockGraphQl.Object, null,
            _mockDb.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _mockDb.Reset();
        _mockBus.Reset();
        _mockGraphQl.Reset();
    }

    [Test]
    public async Task WithValidParameters_ManyParticipants_Successful()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();

        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.BeneficiaryId),
            Description = campaign.Description,
            FundraiserGoal = campaign.FundraiserGoal,
            Provider = campaign.Provider
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
        JuntosCampaign campaign = DataFactory
            .GetJuntosCampaign()
            .WithParticipants(new List<JuntosParticipant>());

        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id),
            Description = campaign.Description,
            FundraiserGoal = campaign.FundraiserGoal,
            Provider = campaign.Provider
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
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

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id),
            Description = campaign.Description,
            FundraiserGoal = campaign.FundraiserGoal,
            Provider = campaign.Provider
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(403);
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

        _mockGraphQl.MockGetCommunity(t => t == campaign.CommunityId, campaign.CommunityId);

        _mockDb.Setup(r => r.AddCampaign(
                It.Is<JuntosCampaign>(t => t.CommunityId == campaign.CommunityId && t.Edition == campaign.Edition),
                CancellationToken.None)
            )
            .ThrowsAsync(new UniqueConstraintException());

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}