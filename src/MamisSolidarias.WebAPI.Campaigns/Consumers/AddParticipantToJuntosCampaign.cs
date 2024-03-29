using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.JuntosALaPar;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MassTransit;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class AddParticipantToJuntosCampaign : IConsumer<ParticipantAddedToJuntosCampaign>
{
    private readonly CampaignsDbContext _db;
    private readonly IGraphQlClient _graphQlClient;

    public AddParticipantToJuntosCampaign(IGraphQlClient graphQlClient, CampaignsDbContext db)
    {
        _graphQlClient = graphQlClient;
        _db = db;
    }

    public async Task Consume(ConsumeContext<ParticipantAddedToJuntosCampaign> context)
    {
        var beneficiaryId = context.Message.BeneficiaryId;
        var campaignId = context.Message.CampaignId;

        var response = await _graphQlClient
            .GetBeneficiaryWithClothes
            .ExecuteAsync(beneficiaryId, context.CancellationToken);

        var hasErrors = await response.HandleErrors(
            _ => Task.CompletedTask,
            (_, _) => Task.CompletedTask,
            default
        );

        if (hasErrors)
            throw new GraphQLException("Error getting beneficiary with clothes");

        if (response.Data?.Beneficiary is null)
            throw new ArgumentException("Beneficiary not found");

        var entry = new JuntosParticipant
        {
            BeneficiaryGender = response.Data.Beneficiary.Gender.Map(),
            ShoeSize = response.Data.Beneficiary.Clothes?.ShoeSize,
            BeneficiaryId = beneficiaryId,
            CampaignId = campaignId,
            BeneficiaryName = $"{response.Data.Beneficiary.FirstName} {response.Data.Beneficiary.LastName}"
        };

        await _db.JuntosParticipants.AddAsync(entry, context.CancellationToken);
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}