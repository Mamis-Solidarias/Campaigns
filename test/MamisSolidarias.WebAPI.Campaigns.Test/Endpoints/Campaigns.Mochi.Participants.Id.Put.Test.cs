using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiParticipantsIdPutTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext, _mockGraphQl.Object };

    [Test]
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign().Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonorAssigned()
            .WithCampaignId(campaign.Id)
            .Build();
        var request = new Request
        {
            DonorId = 123,
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = participant.Id
        };

        _mockGraphQl.MockGetDonor(r => r == request.DonorId, participant.DonorName!);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);

        var result = await _dbContext.MochiParticipants.SingleAsync(t => t.Id == participant.Id);
        result.DonorId.Should().Be(request.DonorId);
        result.DonationDropOffPoint.Should().Be(request.DonationDropOffLocation);
        result.DonationType.Should().Be(DonationType.Money);
        result.State.Should().Be(ParticipantState.MissingDonation);
    }

    [Test]
    public async Task WithInvalidParameters_ParticipantNotFound_Fails()
    {
        // Arrange
        var participant = DataFactory.GetMochiParticipant()
            .AsNoDonorAssigned()
            .Build();

        var request = new Request
        {
            DonorId = 123,
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = participant.Id
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }


    [Test]
    public async Task WithInvalidParameters_DonorNotFound_Fails()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign().Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonorAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var request = new Request
        {
            DonorId = 123,
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = participant.Id
        };

        _mockGraphQl.MockEmptyResponse(t => t.GetDonor.ExecuteAsync(
            It.Is<int>(r => r == request.DonorId),
            It.IsAny<CancellationToken>()
        ));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
        var result = await _dbContext.MochiParticipants.SingleAsync(t => t.Id == participant.Id);
        result.DonorId.Should().BeNull();
        result.DonationDropOffPoint.Should().BeNull();
        result.DonationType.Should().BeNull();
        result.State.Should().Be(ParticipantState.MissingDonor);
    }

    [Test]
    public async Task WithInvalidParameters_UserNotAuthorized_Fails()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign().Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonorAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var request = new Request
        {
            DonorId = 123,
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = participant.Id
        };

        _mockGraphQl.MockAuthenticationError(t => t.GetDonor.ExecuteAsync(
                It.Is<int>(r => r == request.DonorId),
                It.IsAny<CancellationToken>()
            )
        );

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(403);
        var result = await _dbContext.MochiParticipants.SingleAsync(t => t.Id == participant.Id);
        result.DonorId.Should().BeNull();
        result.DonationDropOffPoint.Should().BeNull();
        result.DonationType.Should().BeNull();
        result.State.Should().Be(ParticipantState.MissingDonor);
    }

    [Test]
    public async Task WithInvalidParameters_GraphQlError_Fails()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign().Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonorAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var request = new Request
        {
            DonorId = 123,
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = participant.Id
        };

        _mockGraphQl.MockErrors(t => t.GetDonor.ExecuteAsync(
                It.Is<int>(r => r == request.DonorId),
                It.IsAny<CancellationToken>()
            ),
            new ClientError("", "OTHER ERROR")
        );

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
}