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
using StrawberryShake;
using BeneficiaryGender = MamisSolidarias.Infrastructure.Campaigns.Models.Base.BeneficiaryGender;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class AbrigaditosParticipantUpdatesTest
{
    private readonly Mock<IGraphQlClient> _mockGraphQlClient = new();
    private AbrigaditosParticipantUpdated _consumer = null!;
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

        _consumer = new AbrigaditosParticipantUpdated(_dbContext, _mockGraphQlClient.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task InvalidParameter_BeneficiaryDoesNotExists_Fails()
    {
        // Arrange
        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(1)
        );

        _mockGraphQlClient.MockErrors
        (
            t => t.GetBeneficiaryWithShirt.ExecuteAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ),
            new ClientError("Beneficiary not found", "BENEFICIARY_NOT_FOUND")
        );

        // Act
        var action = () => _consumer.Consume(context);

        // Assert
        await action.Should().ThrowExactlyAsync<GraphQLException>();
    }

    [Test]
    public async Task ValidParameters_UpdatesParticipants()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign().Build();

        var participant = _dataFactory.GenerateAbrigaditosParticipant()
            .WithShirtSize(null)
            .WithCampaignId(campaign.Id)
            .WithGender(BeneficiaryGender.Other)
            .Build();

        var beneficiaryId = participant.BeneficiaryId;
        const GraphQlClient.BeneficiaryGender gender = GraphQlClient.BeneficiaryGender.Male;
        const string firstName = "John";
        const string lastName = "Doe";
        const string shirtSize = "L";

        _mockGraphQlClient.MockGetBeneficiaryWithShirt(
            i => i == beneficiaryId,
            firstName, lastName, gender, shirtSize
        );

        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(beneficiaryId)
        );

        // Act
        var action = async () => await _consumer.Consume(context);

        // Assert
        await action.Should().NotThrowAsync<DbUpdateException>();

        var participants = _dbContext.AbrigaditosParticipants
            .Where(t => t.BeneficiaryId == beneficiaryId)
            .ToList();

        participants.Should().Contain(t => t.BeneficiaryName == $"{firstName} {lastName}");
        participants.Should().Contain(t => t.BeneficiaryGender == gender.Map());
        participants.Should().Contain(t => t.ShirtSize == shirtSize);
    }
}