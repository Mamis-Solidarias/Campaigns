using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class MochiParticipantUpdateTest : ConsumerTest<MochiParticipantUpdated>
{
    protected override MochiParticipantUpdated CreateConsumer()
    {
        return new MochiParticipantUpdated(_dbContext, GraphQlClient);
    }

    [Test]
    public async Task InvalidParameter_BeneficiaryDoesNotExists_Fails()
    {
        // Arrange
        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(1)
        );

        _mockGraphQl.MockErrors
        (
            t => t.GetBeneficiaryWithEducation.ExecuteAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ),
            new ClientError("Beneficiary not found", "BENEFICIARY_NOT_FOUND")
        );

        // Act
        var action = () => Consumer.Consume(context);

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
            .WithSchoolCycle(SchoolCycle.PreSchool)
            .WithGender(BeneficiaryGender.Female)
            .WithBeneficiaryName("Test 123")
            .WithCampaignId(campaign.Id)
            .Build();

        var beneficiaryId = participant.BeneficiaryId;
        const string firstName = "John";
        const string lastName = "Doe";
        const GraphQlClient.SchoolCycle schoolCycle = MamisSolidarias.GraphQlClient.SchoolCycle.HighSchool;
        const GraphQlClient.BeneficiaryGender gender = MamisSolidarias.GraphQlClient.BeneficiaryGender.Male;

        _mockGraphQl.MockGetBeneficiaryWithEducation(
            i => i == beneficiaryId,
            firstName, lastName, gender, schoolCycle
        );

        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(beneficiaryId)
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().NotThrowAsync<DbUpdateException>();

        var participants = _dbContext.MochiParticipants
            .Where(t => t.BeneficiaryId == beneficiaryId)
            .ToList();

        participants.Should().Contain(t => t.BeneficiaryName == $"{firstName} {lastName}");
        participants.Should().Contain(t => t.SchoolCycle.Map() == schoolCycle);
        participants.Should().Contain(t => t.BeneficiaryGender == gender.Map());
    }
}