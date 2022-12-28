using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
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

public class MochiParticipantUpdateTest
{
    private readonly Mock<IGraphQlClient> _mockGraphQlClient = new();
    private MochiParticipantUpdate _consumer = null!;
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

        _consumer = new(_dbContext,_mockGraphQlClient.Object);
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
    public async Task ValidParameters_UpdatesParticipants()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithParticipants(new List<MochiParticipant>()).Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .WithSchoolCycle(Infrastructure.Campaigns.Models.SchoolCycle.PreSchool)
            .WithBeneficiaryGender(Infrastructure.Campaigns.Models.BeneficiaryGender.Female)
            .WithBeneficiaryName("Test 123")
            .WithCampaignId(campaign.Id)
            .Build();
        
        var beneficiaryId = participant.BeneficiaryId;
        const string firstName = "John";
        const string lastName = "Doe";
        const SchoolCycle schoolCycle = SchoolCycle.HighSchool;
        const BeneficiaryGender gender = BeneficiaryGender.Male;

        _mockGraphQlClient.MockGetBeneficiaryWithEducation(
            i => i == beneficiaryId,
            firstName,lastName, gender, schoolCycle
        );

        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(beneficiaryId)
        );

        // Act
        var action = async () => await _consumer.Consume(context);

        // Assert
        await action.Should().NotThrowAsync<DbUpdateException>();
        
        var participants = _dbContext.MochiParticipants
            .Where(t=> t.BeneficiaryId == beneficiaryId)
            .ToList();
        
        participants.Should().Contain(t=> t.BeneficiaryName == $"{firstName} {lastName}");
        participants.Should().Contain(t=> t.SchoolCycle.Map() == schoolCycle);
        participants.Should().Contain(t => t.BeneficiaryGender == gender.Map());
    }
}