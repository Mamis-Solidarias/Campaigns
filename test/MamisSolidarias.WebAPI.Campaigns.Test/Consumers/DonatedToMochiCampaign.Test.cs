using System;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.Infrastructure.Campaigns.Models.Base;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

internal class DonatedToMochiCampaign_Test : ConsumerTest<DonatedToMochiCampaign>
{
    protected override DonatedToMochiCampaign CreateConsumer()
    {
        return new DonatedToMochiCampaign(_dbContext);
    }

    [TestCase(Messages.Campaigns.Abrigaditos)]
    [TestCase(Messages.Campaigns.Misiones)]
    [TestCase(Messages.Campaigns.Navidemos)]
    [TestCase(Messages.Campaigns.JuntosALaPar)]
    [TestCase(Messages.Campaigns.NavidadCompartida)]
    [TestCase(Messages.Campaigns.ApadrinaMiSonriza)]
    public async Task IncorrectCampaign_ShouldReturn(Messages.Campaigns campaignName)
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), participant.DonorId, participant.Id, participant.CampaignId, campaignName, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().NotThrowAsync();

        var result = await _dbContext.MochiParticipants.SingleAsync(t => t.Id == participant.Id);

        result.Should().BeEquivalentTo(participant);
    }

    [Test]
    public async Task ParticipantIdIsNull_ShouldReturn()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), participant.DonorId, null, participant.CampaignId, Messages.Campaigns.UnaMochiComoLaTuya, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().NotThrowAsync();

        var result = await _dbContext.MochiParticipants.SingleAsync(t => t.Id == participant.Id);

        result.Should().BeEquivalentTo(participant);
    }

    [Test]
    public async Task ParticipantDoesNotExists_ShouldThrowException()
    {
        // Arrange
        var participant = DataFactory.GetMochiParticipant()
            .AsNoDonationAssigned()
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), participant.DonorId, participant.Id, participant.CampaignId,
                Messages.Campaigns.UnaMochiComoLaTuya, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task ParticipantDoesNotHaveDonorAssigned_ShouldThrowException()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonorAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), participant.DonorId, participant.Id, participant.CampaignId,
                Messages.Campaigns.UnaMochiComoLaTuya, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task ParticipantDoesHasDonationAssigned_ShouldThrowException()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), participant.DonorId, participant.Id, participant.CampaignId,
                Messages.Campaigns.UnaMochiComoLaTuya, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task DonorsDoNotMatch_ShouldThrowArgumentException()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();
        var otherDonorId = participant.DonorId + 100;

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), otherDonorId, participant.Id, participant.CampaignId,
                Messages.Campaigns.UnaMochiComoLaTuya, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task CampaignsDoNotMatch_ShouldThrowArgumentException()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();
        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();
        var otherCampaignId = participant.CampaignId + 100;

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                Guid.NewGuid(), participant.DonorId, participant.Id, otherCampaignId,
                Messages.Campaigns.UnaMochiComoLaTuya, 100, Currency.ARS
            )
        );

        // Act
        var action = async () => await Consumer.Consume(context);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task MessageIsValid_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateMochiCampaign()
            .WithoutParticipants()
            .Build();

        var participant = _dataFactory.GenerateMochiParticipant()
            .AsNoDonationAssigned()
            .WithCampaignId(campaign.Id)
            .Build();

        var donationId = Guid.NewGuid();

        var context = MockExtensions.MockConsumeContext(
            new DonationAddedToCampaign(
                donationId, participant.DonorId, participant.Id, participant.CampaignId,
                Messages.Campaigns.UnaMochiComoLaTuya, 100, Currency.ARS
            )
        );

        // Act
        await Consumer.Consume(context);

        // Assert
        var result = await _dbContext.MochiParticipants.SingleAsync(t => t.Id == participant.Id);
        result.DonationId.Should().Be(donationId);
        result.State.Should().Be(ParticipantState.DonationReceived);
    }
}