using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using FluentAssertions;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiPostTest
{
    private Endpoint _endpoint = null!;
    private readonly Mock<DbAccess> _mockDb = new();
    private readonly Mock<IGraphQlClient> _mockGraphQl = new();

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(null, _mockGraphQl.Object, _mockDb.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _mockDb.Reset();
        _mockGraphQl.Reset();
    }

    [Test]
    public async Task WithValidParameters_ManyParticipants_Successful()
    {
        // Arrange
        Mochi campaign = DataFactory.GetMochi();

        foreach (var participant in campaign.Participants)
        {
            var getBeneficiaryResult = new Mock<IGetBeneficiaryResult>();
            getBeneficiaryResult.Setup(t => t.Beneficiary)
                .Returns(new GetBeneficiary_Beneficiary_Beneficiary(
                        participant.BeneficiaryName, "",
                        participant.BeneficiaryGender.Map(),
                        new GetBeneficiary_Beneficiary_Education_Education(participant.SchoolCycle.Map())
                    )
                );

            var operationResult = new Mock<IOperationResult<IGetBeneficiaryResult>>();
            operationResult.SetupGet(t => t.Data)
                .Returns(getBeneficiaryResult.Object);
            operationResult.SetupGet(t => t.Errors)
                .Returns(new List<IClientError>());
            
            _mockGraphQl.Setup(t => t.GetBeneficiary.ExecuteAsync(
                        It.Is<int>(r => r == participant.BeneficiaryId),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(operationResult.Object);
        }

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
        _endpoint.Response.Community.Should().Be(campaign.CommunityId);
        _endpoint.Response.Edition.Should().Be(campaign.Edition);
    }

    [Test]
    public async Task WithValidParameters_NoParticipants_Successful()
    {
        // Arrange
        Mochi campaign = DataFactory.GetMochi().WithParticipants(new List<MochiParticipant>());

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
        _endpoint.Response.Community.Should().Be(campaign.CommunityId);
        _endpoint.Response.Edition.Should().Be(campaign.Edition);
    }

    [Test]
    public async Task WithInvalidParameters_ParticipantDoesNotExists_Fails()
    {
        // Arrange
        Mochi campaign = DataFactory.GetMochi()
            .WithParticipants(Enumerable.Range(0, 3).Select(_ => new MochiParticipantBuilder().Build())
            );

        var operationResult = new Mock<IOperationResult<IGetBeneficiaryResult>>();

        operationResult.SetupGet(t => t.Data)
            .Returns((IGetBeneficiaryResult?) null);
        operationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());

        _mockGraphQl.Setup(t => t.GetBeneficiary.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(operationResult.Object);


        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id)
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(409);
    }

    [Test]
    public async Task WithInvalidParameters_UserDoesNotHavePermission_Fails()
    {
        // Arrange
        Mochi campaign = DataFactory.GetMochi()
            .WithParticipants(Enumerable.Range(0, 3).Select(_ => new MochiParticipantBuilder().Build())
            );

        var operationResult = new Mock<IOperationResult<IGetBeneficiaryResult>>();

        operationResult.SetupGet(t => t.Data)
            .Returns((IGetBeneficiaryResult?) null);
        operationResult.SetupGet(t => t.Errors)
            .Returns(new[] {new ClientError("Auth invalid", "AUTH_NOT_AUTHORIZED")});

        _mockGraphQl.Setup(t => t.GetBeneficiary.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(operationResult.Object);


        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id)
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
        Mochi campaign = DataFactory.GetMochi().WithParticipants(new List<MochiParticipant>());

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id)
        };

        _mockDb.Setup(r => r.AddMochiCampaign(
                It.Is<Mochi>(t => t.CommunityId == campaign.CommunityId && t.Edition == campaign.Edition),
                CancellationToken.None)
            )
            .ThrowsAsync(new UniqueConstraintException());

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}