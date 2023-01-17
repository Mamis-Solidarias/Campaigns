using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class AddParticipantToJuntosCampaignTest : ConsumerTest<AddParticipantToJuntosCampaign>
{
    protected override AddParticipantToJuntosCampaign CreateConsumer()
    {
        return new AddParticipantToJuntosCampaign(GraphQlClient, _dbContext);
    }

    [Test]
    public async Task InvalidParameter_BeneficiaryDoesNotExists_Fails()
    {
        // Arrange
        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToJuntosCampaign(1, 1)
        );

        _mockGraphQl.MockErrors
        (
            t => t.GetBeneficiaryWithClothes.ExecuteAsync(
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
    public async Task InvalidParameter_CampaignDoesNotExists_Fails()
    {
        // Arrange
        const int beneficiaryId = 123;
        const int campaignId = 456;

        _mockGraphQl.MockGetBeneficiaryWithClothes(
            i => i == beneficiaryId,
            BeneficiaryGender.Male, 123
        );

        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToJuntosCampaign(campaignId, beneficiaryId)
        );

        // Act
        var action = async () => await Consumer.Consume(context);

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

        _mockGraphQl.MockGetBeneficiaryWithClothes(
            i => i == beneficiaryId,
            beneficiaryGender, beneficiariesShoeSize
        );

        var context = MockExtensions.MockConsumeContext(
            new ParticipantAddedToJuntosCampaign(campaign.Id, beneficiaryId)
        );

        // Act
        await Consumer.Consume(context);

        // Assert

        var participant = await _dbContext.JuntosParticipants.FirstOrDefaultAsync(t =>
            t.BeneficiaryId == beneficiaryId && t.CampaignId == campaign.Id);
        participant.Should().NotBeNull();
        participant?.BeneficiaryId.Should().Be(beneficiaryId);
        participant?.CampaignId.Should().Be(campaign.Id);
        participant?.BeneficiaryGender.Should().Be(Infrastructure.Campaigns.Models.Base.BeneficiaryGender.Male);
        participant?.ShoeSize.Should().Be(beneficiariesShoeSize);
    }
}