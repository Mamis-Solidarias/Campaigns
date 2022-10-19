using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using FluentAssertions;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using BeneficiaryGender = MamisSolidarias.GraphQlClient.BeneficiaryGender;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsJuntosPostTest
{
    private Endpoint _endpoint = null!;
    private readonly Mock<DbAccess> _mockDb = new();
    private readonly Mock<IGraphQlClient> _mockGraphQl = new();

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>( _mockGraphQl.Object,null, _mockDb.Object);
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
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign();

        foreach (var participant in campaign.Participants)
        {
            var getBeneficiaryResult = new Mock<IGetBeneficiaryWithClothesResult>();
            getBeneficiaryResult.Setup(t => t.Beneficiary)
                .Returns(new GetBeneficiaryWithClothes_Beneficiary_Beneficiary(
                        BeneficiaryGender.Female,
                        new GetBeneficiaryWithClothes_Beneficiary_Clothes_Clothes(participant.ShoeSize)
                    )
                );

            var operationResult = new Mock<IOperationResult<IGetBeneficiaryWithClothesResult>>();
            operationResult.SetupGet(t => t.Data)
                .Returns(getBeneficiaryResult.Object);
            operationResult.SetupGet(t => t.Errors)
                .Returns(new List<IClientError>());
            
            _mockGraphQl.Setup(t => t.GetBeneficiaryWithClothes.ExecuteAsync(
                        It.Is<int>(r => r == participant.BeneficiaryId),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(operationResult.Object);
        }
        
        var communityResult = new Mock<IGetCommunityResult>();
        communityResult.Setup(t => t.Community)
            .Returns(new GetCommunity_Community_Community(campaign.CommunityId));
        
        var communityOperationResult = new Mock<IOperationResult<IGetCommunityResult>>();
        communityOperationResult.SetupGet(t => t.Data)
            .Returns(communityResult.Object);
        communityOperationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());
            
        _mockGraphQl.Setup(t => t.GetCommunity.ExecuteAsync(
                    It.Is<string>(r => r == campaign.CommunityId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(communityOperationResult.Object);

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
        
        var communityResult = new Mock<IGetCommunityResult>();
        communityResult.Setup(t => t.Community)
            .Returns(new GetCommunity_Community_Community(campaign.CommunityId));
        
        var communityOperationResult = new Mock<IOperationResult<IGetCommunityResult>>();
        communityOperationResult.SetupGet(t => t.Data)
            .Returns(communityResult.Object);
        communityOperationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());
            
        _mockGraphQl.Setup(t => t.GetCommunity.ExecuteAsync(
                    It.Is<string>(r => r == campaign.CommunityId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(communityOperationResult.Object);

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
    public async Task WithInvalidParameters_ParticipantDoesNotExists_Fails()
    {
        // Arrange
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign()
            .WithParticipants(Enumerable.Range(0, 3).Select(_ => new JuntosParticipantBuilder().Build())
            );

        var communityResult = new Mock<IGetCommunityResult>();
        communityResult.Setup(t => t.Community)
            .Returns(new GetCommunity_Community_Community(campaign.CommunityId));
        
        var communityOperationResult = new Mock<IOperationResult<IGetCommunityResult>>();
        communityOperationResult.SetupGet(t => t.Data)
            .Returns(communityResult.Object);
        communityOperationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());
            
        _mockGraphQl.Setup(t => t.GetCommunity.ExecuteAsync(
                    It.Is<string>(r => r == campaign.CommunityId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(communityOperationResult.Object);
        
        var operationResult = new Mock<IOperationResult<IGetBeneficiaryWithClothesResult>>();

        operationResult.SetupGet(t => t.Data)
            .Returns((IGetBeneficiaryWithClothesResult?) null);
        operationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());

        _mockGraphQl.Setup(t => t.GetBeneficiaryWithClothes.ExecuteAsync(
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
        JuntosCampaign campaign = DataFactory.GetJuntosCampaign()
            .WithParticipants(Enumerable.Range(0, 3).Select(_ => new JuntosParticipantBuilder().Build())
            );
        
        var communityResult = new Mock<IGetCommunityResult>();
        communityResult.Setup(t => t.Community)
            .Returns(new GetCommunity_Community_Community(campaign.CommunityId));
        
        var communityOperationResult = new Mock<IOperationResult<IGetCommunityResult>>();
        communityOperationResult.SetupGet(t => t.Data)
            .Returns(null as IGetCommunityResult);
        communityOperationResult.SetupGet(t => t.Errors)
            .Returns(new[] {new ClientError("Auth invalid", "AUTH_NOT_AUTHORIZED")});
        
        _mockGraphQl.Setup(t => t.GetCommunity.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(communityOperationResult.Object);


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
        MochiCampaign campaign = DataFactory.GetMochi().WithParticipants(new List<MochiParticipant>());

        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            Beneficiaries = campaign.Participants.Select(t => t.Id)
        };
        
        var communityResult = new Mock<IGetCommunityResult>();
        communityResult.Setup(t => t.Community)
            .Returns(new GetCommunity_Community_Community(campaign.CommunityId));
        
        var communityOperationResult = new Mock<IOperationResult<IGetCommunityResult>>();
        communityOperationResult.SetupGet(t => t.Data)
            .Returns(communityResult.Object);
        communityOperationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());
            
        _mockGraphQl.Setup(t => t.GetCommunity.ExecuteAsync(
                    It.Is<string>(r => r == campaign.CommunityId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(communityOperationResult.Object);

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