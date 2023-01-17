using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.Utils.Test;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Moq;
using NUnit.Framework;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiParticipantsIdPutTest
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
        var request = new Request
        {
            DonorId = 123,
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = 999
        };
        
        var participant = DataFactory.GetMochiParticipant()
            .WithId(request.Id)
            .Build();

        _mockDb.Setup(x => x.GetParticipantAsync(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);

        _mockGraphQl.MockGetDonor(r => r == request.DonorId, participant.DonorName!);

        _mockDb.Setup(t => t.SaveParticipantAsync(
                It.Is<MochiParticipant>(r => r.Id == participant.Id),
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
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = 999
        };
        var participant = DataFactory.GetMochiParticipant()
            .WithId(request.Id)
            .Build();

        _mockDb.Setup(x => x.GetParticipantAsync(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(null as MochiParticipant);

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
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = 999
        };
        var participant = DataFactory.GetMochiParticipant()
            .WithId(request.Id)
            .Build();

        _mockDb.Setup(x => x.GetParticipantAsync(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);

        _mockGraphQl.MockEmptyResponse(t => t.GetDonor.ExecuteAsync(
            It.Is<int>(r => r == request.DonorId),
            It.IsAny<CancellationToken>()
        ));

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
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = 999
        };
        var participant = DataFactory.GetMochiParticipant()
            .WithId(request.Id)
            .Build();

        _mockDb.Setup(x => x.GetParticipantAsync(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);

        _mockGraphQl.MockAuthenticationError(t => t.GetDonor.ExecuteAsync(
                It.Is<int>(r => r == request.DonorId),
                It.IsAny<CancellationToken>()
            )
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
            DonationDropOffLocation = "La casa de la abuela",
            DonationType = DonationType.Money.ToString(),
            Id = 999
        };
        MochiParticipant participant = DataFactory.GetMochiParticipant()
            .WithId(request.Id)
            .Build();

        _mockDb.Setup(x => x.GetParticipantAsync(
                    It.Is<int>(t => t == participant.Id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(participant);

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