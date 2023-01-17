using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StrawberryShake;
using MockExtensions = MamisSolidarias.WebAPI.Campaigns.Utils.MockExtensions;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class JuntosParticipantUpdateTest : ConsumerTest<JuntosParticipantUpdated>
{
    protected override JuntosParticipantUpdated CreateConsumer()
    {
        return new JuntosParticipantUpdated(_dbContext, GraphQlClient);
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
        const GraphQlClient.BeneficiaryGender gender = MamisSolidarias.GraphQlClient.BeneficiaryGender.Male;
        const int shoeSize = 35;

        _mockGraphQl.MockGetBeneficiaryWithClothes(
            i => i == beneficiaryId,
            gender, shoeSize
        );

        var context = MockExtensions.MockConsumeContext(
            new BeneficiaryUpdated(beneficiaryId)
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().NotThrowAsync<DbUpdateException>();

        var participants = _dbContext.JuntosParticipants
            .Where(t => t.BeneficiaryId == beneficiaryId)
            .ToList();

        participants.Should().Contain(t => t.ShoeSize == shoeSize);
        participants.Should().Contain(t => t.BeneficiaryGender == gender.Map());
    }
}