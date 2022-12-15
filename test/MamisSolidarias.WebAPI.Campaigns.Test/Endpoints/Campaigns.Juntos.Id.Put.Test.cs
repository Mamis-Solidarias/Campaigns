using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using BeneficiaryGender = MamisSolidarias.Infrastructure.Campaigns.Models.BeneficiaryGender;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsJuntosIdPutTest
{
    private readonly Mock<DbAccess> _mockDb = new();
    private readonly Mock<IGraphQlClient> _mockGraphQl = new();
    private Endpoint _endpoint = null!;

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_mockGraphQl.Object, null, _mockDb.Object);
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

        foreach (var beneficiary in request.AddedBeneficiaries)
            SetUpGetBeneficiaryWithClothes(beneficiary,BeneficiaryGender.Female,38);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task WithInvalidParameters_UserDoesNotHavePermission_Fails()
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

        foreach (var beneficiary in request.AddedBeneficiaries)
            SetUpGetBeneficiaryWithClothesReturnNull(beneficiary, new[]
            {
                new ClientError("", "AUTH_NOT_AUTHORIZED")
            });

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(403);
    }

    [Test]
    public async Task WithInvalidParameters_BeneficiaryDoesNotExists_Fails()
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

        foreach (var beneficiary in request.AddedBeneficiaries)
            SetUpGetBeneficiaryWithClothesReturnNull(beneficiary);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(409);
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

    private void SetUpGetBeneficiaryWithClothes(int beneficiaryId, BeneficiaryGender gender, int? shoeSize)
    {
        var getBeneficiaryResult = new Mock<IGetBeneficiaryWithClothesResult>();
        getBeneficiaryResult.Setup(t => t.Beneficiary)
            .Returns(
                new GetBeneficiaryWithClothes_Beneficiary_Beneficiary(
                    gender.Map(),
                    new GetBeneficiaryWithClothes_Beneficiary_Clothes_Clothes(shoeSize)
                )
            );

        var operationResult = new Mock<IOperationResult<IGetBeneficiaryWithClothesResult>>();
        operationResult.SetupGet(t => t.Data)
            .Returns(getBeneficiaryResult.Object);
        operationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());

        _mockGraphQl.Setup(t => t.GetBeneficiaryWithClothes.ExecuteAsync(
                    It.Is<int>(r => r == beneficiaryId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(operationResult.Object);
    }

    private void SetUpGetBeneficiaryWithClothesReturnNull(int beneficiaryId, IEnumerable<IClientError>? errors = default)
    {
        var getBeneficiaryResult = new Mock<IGetBeneficiaryWithClothesResult>();
        getBeneficiaryResult.Setup(t => t.Beneficiary)
            .Returns(null as GetBeneficiaryWithClothes_Beneficiary_Beneficiary);

        var operationResult = new Mock<IOperationResult<IGetBeneficiaryWithClothesResult>>();
        operationResult.SetupGet(t => t.Data)
            .Returns(getBeneficiaryResult.Object);
        operationResult.SetupGet(t => t.Errors)
            .Returns(errors?.ToList() ?? new List<IClientError>());

        _mockGraphQl.Setup(t => t.GetBeneficiaryWithClothes.ExecuteAsync(
                    It.Is<int>(r => r == beneficiaryId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(operationResult.Object);
    }
}