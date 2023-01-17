using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class AddParticipantToJuntosCampaignTest
{
    private readonly Mock<IGraphQlClient> _mockGraphQlClient = new();
    private AddParticipantToJuntosCampaign _consumer = null!;
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

        _consumer = new AddParticipantToJuntosCampaign(_mockGraphQlClient.Object, _dbContext);
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
            new ParticipantAddedToJuntosCampaign(1, 1)
        );

        _mockGraphQlClient.MockErrors
        (
            t => t.GetBeneficiaryWithClothes.ExecuteAsync(
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
    public async Task InvalidParameter_CampaignDoesNotExists_Fails()
    {
        // Arrange
        const int beneficiaryId = 123;
        const int campaignId = 456;

        _mockGraphQlClient.MockGetBeneficiaryWithClothes(
            i => i == beneficiaryId,
            BeneficiaryGender.Male, 123
        );

        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToJuntosCampaign(campaignId, beneficiaryId)
        );

        // Act
        var action = async () => await _consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();

        var participant = await _dbContext.JuntosParticipants.FirstOrDefaultAsync(t =>
            t.BeneficiaryId == beneficiaryId && t.CampaignId == campaignId);
        participant.Should().BeNull();
    }

    [Test]
    public async Task ValidParameters_AddsParticipant()
    {
        // Arrange
        const int beneficiaryId = 123;
        const BeneficiaryGender beneficiaryGender = BeneficiaryGender.Male;
        const int beneficiariesShoeSize = 35;
        var campaign = _dataFactory.GenerateJuntosCampaign().Build();

        _mockGraphQlClient.MockGetBeneficiaryWithClothes(
            i => i == beneficiaryId,
            beneficiaryGender, beneficiariesShoeSize
        );

        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToJuntosCampaign(campaign.Id, beneficiaryId)
        );

        // Act
        await _consumer.Consume(context);

        // Assert

        var participant = await _dbContext.JuntosParticipants.FirstOrDefaultAsync(t =>
            t.BeneficiaryId == beneficiaryId && t.CampaignId == campaign.Id);
        participant.Should().NotBeNull();
        participant?.BeneficiaryId.Should().Be(beneficiaryId);
        participant?.CampaignId.Should().Be(campaign.Id);
        participant?.Gender.Should().Be(Infrastructure.Campaigns.Models.Base.BeneficiaryGender.Male);
        participant?.ShoeSize.Should().Be(beneficiariesShoeSize);
    }
}