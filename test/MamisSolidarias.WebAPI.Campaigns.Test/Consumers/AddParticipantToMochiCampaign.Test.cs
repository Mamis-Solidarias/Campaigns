using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using BeneficiaryGender = MamisSolidarias.GraphQlClient.BeneficiaryGender;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;
using SchoolCycle = MamisSolidarias.GraphQlClient.SchoolCycle;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class AddParticipantToMochiCampaignTest
{
    private readonly Mock<IGraphQlClient> _mockGraphQlClient = new();
    private AddParticipantToMochiCampaign _consumer = null!;
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

        _consumer = new AddParticipantToMochiCampaign(_mockGraphQlClient.Object, _dbContext);
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
            new ParticipantAddedToMochiCampaign(1, 1)
        );

        _mockGraphQlClient.MockErrors
        (
            t => t.GetBeneficiaryWithEducation.ExecuteAsync(
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
        const BeneficiaryGender gender = BeneficiaryGender.Male;
        const string firstName = "Lucas";
        const string lastName = "dell";
        const SchoolCycle schoolCycle = SchoolCycle.HighSchool;
        
        const int campaignId = 456;

        _mockGraphQlClient.MockGetBeneficiaryWithEducation(
            i => i == beneficiaryId,
            firstName,lastName,gender,schoolCycle
        );

        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToMochiCampaign(beneficiaryId,campaignId)
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
        const BeneficiaryGender gender = BeneficiaryGender.Male;
        const string firstName = "Lucas";
        const string lastName = "dell";
        const SchoolCycle schoolCycle = SchoolCycle.HighSchool;
        
        var campaign = _dataFactory.GenerateMochiCampaign().Build();
        
        _mockGraphQlClient.MockGetBeneficiaryWithEducation(
            i => i == beneficiaryId,
            firstName,lastName,gender,schoolCycle
        );

        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToMochiCampaign(beneficiaryId,campaign.Id)
        );

        // Act
        await _consumer.Consume(context);

        // Assert

        var participant = await _dbContext.MochiParticipants.FirstOrDefaultAsync(t =>
            t.BeneficiaryId == beneficiaryId && t.CampaignId == campaign.Id);
        participant.Should().NotBeNull();
        participant?.BeneficiaryId.Should().Be(beneficiaryId);
        participant?.CampaignId.Should().Be(campaign.Id);
        participant?.BeneficiaryGender.Should().Be(Infrastructure.Campaigns.Models.BeneficiaryGender.Male);
        participant?.BeneficiaryName.Should().Be($"{firstName} {lastName}".ToLower());
        participant?.SchoolCycle.Should().Be(Infrastructure.Campaigns.Models.SchoolCycle.HighSchool);
        participant?.State.Should().Be(ParticipantState.MissingDonor);
    }
}