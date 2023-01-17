using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class AddParticipantToAbrigaditosCampaign_Test : ConsumerTest<AddParticipantToAbrigaditosCampaign>
{
    protected override AddParticipantToAbrigaditosCampaign CreateConsumer()
    {
        return new(GraphQlClient, _dbContext);
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

        _mockGraphQl.MockEmptyResponse(t => t.GetBeneficiaryWithShirt.ExecuteAsync(
                It.Is<int>(r => r == beneficiaryId),
                It.IsAny<CancellationToken>()
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

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

        _mockGraphQl.MockAuthenticationError(t => t.GetBeneficiaryWithShirt.ExecuteAsync(
                It.Is<int>(r => r == beneficiaryId),
                It.IsAny<CancellationToken>()
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

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

        _mockGraphQl.MockGetBeneficiaryWithShirt(t => t == beneficiaryId,
            firstName, lastName, beneficiaryGender, shirtSize);

        // Act
        await Consumer.Consume(context);

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