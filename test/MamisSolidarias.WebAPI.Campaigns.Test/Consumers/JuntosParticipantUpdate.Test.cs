using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
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

public class JuntosParticipantUpdateTest
{
    private readonly Mock<IGraphQlClient> _mockGraphQlClient = new();
    private JuntosParticipantUpdated _consumer = null!;
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

        _consumer = new JuntosParticipantUpdated(_dbContext, _mockGraphQlClient.Object);
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
    public async Task ValidParameters_UpdatesParticipants()
    {
        // Arrange
        var campaign = _dataFactory.GenerateJuntosCampaign()
            .WithParticipants(new List<JuntosParticipant>()).Build();
        var participant = _dataFactory.GenerateJuntosParticipant()
            .WithShoeSize(null)
            .WithCampaignId(campaign.Id)
            .WithGender(BeneficiaryGender.Other)
            .Build();

        var beneficiaryId = participant.BeneficiaryId;
        const GraphQlClient.BeneficiaryGender gender = GraphQlClient.BeneficiaryGender.Male;
        const int shoeSize = 35;

        _mockGraphQlClient.MockGetBeneficiaryWithClothes(
            i => i == beneficiaryId,
            gender, shoeSize
        );

        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(beneficiaryId)
        );

        // Act
        var action = async () => await _consumer.Consume(context);

        // Assert
        await action.Should().NotThrowAsync<DbUpdateException>();

        var participants = _dbContext.JuntosParticipants
            .Where(t => t.BeneficiaryId == beneficiaryId)
            .ToList();

        participants.Should().Contain(t => t.ShoeSize == shoeSize);
        participants.Should().Contain(t => t.BeneficiaryGender == gender.Map());
    }
}