using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Abrigaditos;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MassTransit;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class AddParticipantToAbrigaditosCampaign : IConsumer<ParticipantAddedToAbrigaditosCampaign>
{
    private readonly CampaignsDbContext _db;
    private readonly IGraphQlClient _graphQlClient;

    public AddParticipantToAbrigaditosCampaign(IGraphQlClient graphQlClient, CampaignsDbContext db)
    {
        _graphQlClient = graphQlClient;
        _db = db;
    }

    public async Task Consume(ConsumeContext<ParticipantAddedToAbrigaditosCampaign> context)
    {
        var beneficiaryId = context.Message.BeneficiaryId;
        var campaignId = context.Message.CampaignId;

        var response = await _graphQlClient
            .GetBeneficiaryWithShirt
            .ExecuteAsync(beneficiaryId, context.CancellationToken);

        var hasErrors = await response.HandleErrors(
            _ => Task.CompletedTask,
            (_, _) => Task.CompletedTask,
            default
        );

        if (hasErrors)
            throw new GraphQLException("Error getting beneficiary");

        if (response.Data?.Beneficiary is null)
            throw new ArgumentException("Beneficiary not found");

        var entry = new AbrigaditosParticipant
        {
            BeneficiaryGender = response.Data.Beneficiary.Gender.Map(),
            ShirtSize = response.Data.Beneficiary.Clothes?.ShirtSize,
            BeneficiaryId = beneficiaryId,
            CampaignId = campaignId,
            BeneficiaryName = $"{response.Data.Beneficiary.FirstName} {response.Data.Beneficiary.LastName}",
        };

        await _db.AbrigaditosParticipants.AddAsync(entry, context.CancellationToken);
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}