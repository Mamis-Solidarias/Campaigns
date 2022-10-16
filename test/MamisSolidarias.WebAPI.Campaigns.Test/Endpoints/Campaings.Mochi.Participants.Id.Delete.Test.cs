using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.DELETE;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaingsMochiParticipantsIdDeleteTest
{
    private Endpoint _endpoint = null!;
    private readonly Mock<DbAccess> _mockDb = new();

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(null, _mockDb.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _mockDb.Reset();
    }

    [Test]
    public async Task WithValidParameters_Succeeds()
    {
        // Arrange
        MochiParticipant participant = DataFactory.GetMochiParticipant();

        _mockDb.Setup(t => t.GetParticipantAsync(
                It.Is<int>(r => r == participant.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(participant);

        var request = new Request
        {
            Id = participant.Id
        };

        _mockDb.Setup(t => t.UpdateParticipantAsync(
                It.Is<MochiParticipant>(r => r == participant),
                It.IsAny<CancellationToken>()
            )
        ).Returns(Task.CompletedTask);
        
        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        _endpoint.Response.ParticipantId.Should().Be(participant.Id);
        participant.State.Should().Be(MochiParticipantState.MissingDonor);
        participant.DonorId.Should().BeNull();
        participant.DonorName.Should().BeNull();
        participant.DonationType.Should().BeNull();
        participant.DonationDropOffLocation.Should().BeNull();
    }

    [Test]
    public async Task ParticipantDoesNotExists_Fails()
    {
        // Arrange
        MochiParticipant participant = DataFactory.GetMochiParticipant();

        _mockDb.Setup(t => t.GetParticipantAsync(
                It.Is<int>(r => r == participant.Id),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(null as MochiParticipant);

        var request = new Request
        {
            Id = participant.Id
        };
        
        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
        participant.DonorId.Should().NotBeNull();
        participant.DonorName.Should().NotBeNull();
        participant.DonationType.Should().NotBeNull();
        participant.DonationDropOffLocation.Should().NotBeNull();
    }
}