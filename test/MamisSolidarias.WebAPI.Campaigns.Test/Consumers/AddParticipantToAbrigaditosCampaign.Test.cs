using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class AddParticipantToAbrigaditosCampaign_Test
{
    private readonly Mock<IGraphQlClient> _mockGraphQlClient = new();
    private AddParticipantToAbrigaditosCampaign _consumer = null!;
    private DataFactory _dataFactory = null!;
    private CampaignsDbContext _dbContext = null!;

    [SetUp]
    public void SetUp()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<CampaignsDbContext>()
            .UseSqlite(connection)
            .Options;

        _dbContext = new CampaignsDbContext(options);
        _dbContext.Database.EnsureCreated();

        _dataFactory = new DataFactory(_dbContext);

        _consumer = new AddParticipantToAbrigaditosCampaign(_mockGraphQlClient.Object, _dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        _mockGraphQlClient.Reset();
    }

    [Test]
    public async Task BeneficiaryDoesNotExists_ShouldThrowArgumentException()
    {
        // Arrange
        const int beneficiaryId = 123;
        const int campaignId = 1;
        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToAbrigaditosCampaign(
                campaignId, beneficiaryId
            )
        );

        _mockGraphQlClient.MockEmptyResponse(t => t.GetBeneficiaryWithShirt.ExecuteAsync(
                It.Is<int>(r => r == beneficiaryId),
                It.IsAny<CancellationToken>()
            )
        );

        // Act
        var action = async () => await _consumer.Consume(context);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Test]
    public async Task UserNotAuthorized_ShouldThrowGraphQlException()
    {
        // Arrange
        const int beneficiaryId = 123;
        const int campaignId = 1;
        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToAbrigaditosCampaign(
                campaignId, beneficiaryId
            )
        );

        _mockGraphQlClient.MockAuthenticationError(t => t.GetBeneficiaryWithShirt.ExecuteAsync(
                It.Is<int>(r => r == beneficiaryId),
                It.IsAny<CancellationToken>()
            )
        );

        // Act
        var action = async () => await _consumer.Consume(context);

        // Assert
        await action.Should().ThrowExactlyAsync<GraphQLException>();
    }

    [Test]
    public async Task WithValidParameters_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign().Build();
        const int beneficiaryId = 123;
        const BeneficiaryGender beneficiaryGender = BeneficiaryGender.Male;
        const string firstName = "John";
        const string lastName = "Doe";
        const string shirtSize = "M";

        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToAbrigaditosCampaign(
                campaign.Id, beneficiaryId
            )
        );

        _mockGraphQlClient.MockGetBeneficiaryWithShirt(t => t == beneficiaryId,
            firstName, lastName, beneficiaryGender, shirtSize);

        // Act
        await _consumer.Consume(context);

        // Assert
        var participant = await _dbContext.AbrigaditosParticipants
            .Where(t => t.BeneficiaryId == beneficiaryId)
            .Where(t => t.CampaignId == campaign.Id)
            .SingleAsync();

        participant.ShirtSize.Should().Be(shirtSize);
        participant.BeneficiaryName.Should().Be($"{firstName} {lastName}");
        participant.BeneficiaryGender.Should().Be(beneficiaryGender.Map());
    }
}