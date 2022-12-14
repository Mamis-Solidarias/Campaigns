using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using FluentAssertions;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiIdPostTest
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
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        MochiCampaign previousCampaign = DataFactory.GetMochiCampaign().WithId(123);
        
        MochiCampaign campaign = DataFactory.GetMochiCampaign();
        
        _mockDb.Setup(t=> t.GetCampaignAsync(
                It.Is<int>(r => r == previousCampaign.Id), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousCampaign);

        foreach (var participant in previousCampaign.Participants)
        {
            var getBeneficiaryResult = new Mock<IGetBeneficiaryWithEducationResult>();
            getBeneficiaryResult.Setup(t => t.Beneficiary)
                .Returns(new GetBeneficiaryWithEducation_Beneficiary_Beneficiary(
                        participant.BeneficiaryName, "",
                        participant.BeneficiaryGender.Map(),
                        new GetBeneficiaryWithEducation_Beneficiary_Education_Education(participant.SchoolCycle.Map())
                    )
                );

            var operationResult = new Mock<IOperationResult<IGetBeneficiaryWithEducationResult>>();
            operationResult.SetupGet(t => t.Data)
                .Returns(getBeneficiaryResult.Object);
            
            operationResult.SetupGet(t => t.Errors)
                .Returns(new List<IClientError>());
            
            _mockGraphQl.Setup(t => t.GetBeneficiaryWithEducation.ExecuteAsync(
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
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(201);
    }

    [Test]
    public async Task WithInvalidParameters_UserNotAuthenticated_Fails()
    {
        // Arrange
        MochiCampaign previousCampaign = DataFactory.GetMochiCampaign()
            .WithId(123)
            .WithParticipants(Enumerable.Range(0, 3).Select(_ => new MochiParticipantBuilder().Build())
            );
        MochiCampaign campaign = DataFactory.GetMochiCampaign();
        
        _mockDb.Setup(t=> t.GetCampaignAsync(
                It.Is<int>(r => r == previousCampaign.Id), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousCampaign);

        var operationResult = new Mock<IOperationResult<IGetBeneficiaryWithEducationResult>>();

        operationResult.SetupGet(t => t.Data)
            .Returns((IGetBeneficiaryWithEducationResult?) null);
        operationResult.SetupGet(t => t.Errors)
            .Returns(new[] {new ClientError("Auth invalid", "AUTH_NOT_AUTHORIZED")});

        _mockGraphQl.Setup(t => t.GetBeneficiaryWithEducation.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(operationResult.Object);


        var req = new Request
        {
            Edition = campaign.Edition,
            CommunityId = campaign.CommunityId,
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(403);
    }

    [Test]
    public async Task WithInvalidParameters_CampaignNotFound_Fails()
    {
        // Arrange
        MochiCampaign previousCampaign = DataFactory.GetMochiCampaign()
            .WithId(123);
        MochiCampaign campaign = DataFactory.GetMochiCampaign();
        
        _mockDb.Setup(t=> t.GetCampaignAsync(
                It.Is<int>(r => r == previousCampaign.Id), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MochiCampaign?)null);


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
    }
    
    [Test]
    public async Task WithInvalidParameters_RepeatedCommunityAndEdition_Fails()
    {
        
        // Arrange
        MochiCampaign previousCampaign = DataFactory.GetMochiCampaign().WithId(123);
        
        MochiCampaign campaign = DataFactory.GetMochiCampaign();
        
        _mockDb.Setup(t=> t.GetCampaignAsync(
                It.Is<int>(r => r == previousCampaign.Id), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousCampaign);

        _mockDb.Setup(t => t.SaveCampaignAsync(
                It.IsAny<MochiCampaign>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new UniqueConstraintException());

        foreach (var participant in previousCampaign.Participants)
        {
            var getBeneficiaryResult = new Mock<IGetBeneficiaryWithEducationResult>();
            getBeneficiaryResult.Setup(t => t.Beneficiary)
                .Returns(new GetBeneficiaryWithEducation_Beneficiary_Beneficiary(
                        participant.BeneficiaryName, "",
                        participant.BeneficiaryGender.Map(),
                        new GetBeneficiaryWithEducation_Beneficiary_Education_Education(participant.SchoolCycle.Map())
                    )
                );

            var operationResult = new Mock<IOperationResult<IGetBeneficiaryWithEducationResult>>();
            operationResult.SetupGet(t => t.Data)
                .Returns(getBeneficiaryResult.Object);
            
            operationResult.SetupGet(t => t.Errors)
                .Returns(new List<IClientError>());
            
            _mockGraphQl.Setup(t => t.GetBeneficiaryWithEducation.ExecuteAsync(
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
            PreviousCampaignId = previousCampaign.Id
        };

        // Act
        await _endpoint.HandleAsync(req, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
    
}