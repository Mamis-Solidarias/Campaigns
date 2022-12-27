using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Participants.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsJuntosParticipantsIdPutTest
{
    private Endpoint _endpoint = null!;
    private readonly Mock<DbAccess> _mockDb = new();
    private readonly Mock<IGraphQlClient> _mockGraphQl = new();

    [SetUp]
    public void Setup()
    {
        _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_mockGraphQl.Object,null, _mockDb.Object);
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
        var request = new Request
        {
            DonorId = 123,
            Id = 999
        };
        JuntosParticipant participant = DataFactory.GetJuntosParticipant().WithId(request.Id);

        _mockDb.Setup(x => x.GetParticipant(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);
        
        _mockGraphQl.MockGetDonor(t => t == request.DonorId, participant.DonorName!);

        _mockDb.Setup(t => t.SaveChanges(
                It.IsAny<CancellationToken>()
            )
        ).Returns(Task.CompletedTask);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        _endpoint.Response.State.Should().Be(ParticipantState.MissingDonation);

    }

    [Test]
    public async Task WithInvalidParameters_ParticipantNotFound_Fails()
    {
        // Arrange
        var request = new Request
        {
            DonorId = 123,
            Id = 999
        };

        _mockDb.Setup(x => x.GetParticipant(
                    It.Is<int>(t => t == request.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(null as JuntosParticipant);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);

    }
    

    [Test]
    public async Task WithInvalidParameters_DonorNotFound_Fails()
    {
        // Arrange
        var request = new Request
        {
            DonorId = 123,
            Id = 999
        };
        JuntosParticipant participant = DataFactory.GetJuntosParticipant().WithId(request.Id);

        _mockDb.Setup(x => x.GetParticipant(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);

        _mockGraphQl.MockEmptyResponse(t => t.GetDonor.ExecuteAsync(
                It.Is<int>(r => r == request.DonorId),
                It.IsAny<CancellationToken>())
        );

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);

    }

    [Test]
    public async Task WithInvalidParameters_UserNotAuthorized_Fails()
    {
        // Arrange
        var request = new Request
        {
            DonorId = 123,
            Id = 999
        };
        JuntosParticipant participant = DataFactory.GetJuntosParticipant().WithId(request.Id);

        _mockDb.Setup(x => x.GetParticipant(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);

        _mockGraphQl.MockAuthenticationError(t => t.GetDonor.ExecuteAsync(
            It.Is<int>(r => r == request.DonorId),
            It.IsAny<CancellationToken>())
        );
        
        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(403);

    }

    [Test]
    public async Task WithInvalidParameters_GraphQlError_Fails()
    {
        // Arrange
        var request = new Request
        {
            DonorId = 123,
            Id = 999
        };
        JuntosParticipant participant = DataFactory.GetJuntosParticipant().WithId(request.Id);

        _mockDb.Setup(x => x.GetParticipant(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);
        
        _mockGraphQl.MockErrors(t => t.GetDonor.ExecuteAsync( 
            It.Is<int>(r => r == request.DonorId),
            It.IsAny<CancellationToken>()),
            new ClientError("","OTHER ERROR")
        );

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);
        
        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(400);
    }
    
    
}