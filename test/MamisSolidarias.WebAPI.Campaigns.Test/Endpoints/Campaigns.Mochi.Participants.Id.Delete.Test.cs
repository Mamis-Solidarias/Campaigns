using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiParticipantsIdDeleteTest : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext };

    [Test]
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign().Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var request = new Request
        {
            Id = participant.Id
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        _endpoint.Response.ParticipantId.Should().Be(participant.Id);

        var result = await _dbContext.MochiParticipants.SingleAsync(t => t.Id == participant.Id);
        result.State.Should().Be(ParticipantState.MissingDonor);
        result.DonorId.Should().BeNull();
        result.DonorName.Should().BeNull();
        result.DonationType.Should().BeNull();
        result.DonationDropOffPoint.Should().BeNull();
    }

    [Test]
    public async Task ParticipantDoesNotExists_Fails()
    {
        // Arrange
        var participant = DataFactory.GetMochiParticipant()
            .Build();

        var request = new Request
        {
            Id = participant.Id
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }
}