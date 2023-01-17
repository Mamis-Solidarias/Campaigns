using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class AbrigaditosParticipantUpdatesTest : ConsumerTest<AbrigaditosParticipantUpdated>
{
    protected override AbrigaditosParticipantUpdated CreateConsumer()
    {
        return new(_dbContext, GraphQlClient);
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
            t => t.GetBeneficiaryWithShirt.ExecuteAsync(
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
        var campaign = _dataFactory.GenerateAbrigaditosCampaign().Build();

        var participant = _dataFactory.GenerateAbrigaditosParticipant()
            .WithShirtSize(null)
            .WithCampaignId(campaign.Id)
            .WithGender(BeneficiaryGender.Other)
            .Build();

        var beneficiaryId = participant.BeneficiaryId;
        const GraphQlClient.BeneficiaryGender gender = MamisSolidarias.GraphQlClient.BeneficiaryGender.Male;
        const string firstName = "John";
        const string lastName = "Doe";
        const string shirtSize = "L";

        _mockGraphQl.MockGetBeneficiaryWithShirt(
            i => i == beneficiaryId,
            firstName, lastName, gender, shirtSize
        );

        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(beneficiaryId)
        );

        // Act
        var action = async () => await Consumer.Consume(context);

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