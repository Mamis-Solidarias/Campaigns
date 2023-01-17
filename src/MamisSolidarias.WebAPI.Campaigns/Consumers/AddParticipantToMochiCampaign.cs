using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Infrastructure.Campaigns.Models.Mochi;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MassTransit;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class AddParticipantToMochiCampaign : IConsumer<ParticipantAddedToMochiCampaign>
{
    private readonly CampaignsDbContext _dbContext;
    private readonly IGraphQlClient _graphQlClient;

    public AddParticipantToMochiCampaign(IGraphQlClient graphQlClient, CampaignsDbContext dbContext)
    {
        _graphQlClient = graphQlClient;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ParticipantAddedToMochiCampaign> context)
    {
        var beneficiaryId = context.Message.BeneficiaryId;
        var campaignId = context.Message.CampaignId;

        var response = await _graphQlClient
            .GetBeneficiaryWithEducation
            .ExecuteAsync(beneficiaryId, context.CancellationToken);

        var hasErrors = await response.HandleErrors(
            _ => Task.CompletedTask,
            (_, _) => Task.CompletedTask,
            context.CancellationToken
        );

        if (hasErrors)
            throw new GraphQLException("Error getting beneficiary with education");

        if (response.Data?.Beneficiary is null)
            throw new GraphQLException("Beneficiary not found");

        var entry = new MochiParticipant
        {
            BeneficiaryGender = response.Data.Beneficiary.Gender.Map(),
            BeneficiaryId = beneficiaryId,
            BeneficiaryName =
                $"{response.Data.Beneficiary.FirstName.ToLower()} {response.Data.Beneficiary.LastName.ToLower()}",
            SchoolCycle = response.Data.Beneficiary.Education?.Cycle.Map(),
            CampaignId = campaignId
        };

        await _dbContext.MochiParticipants.AddAsync(entry, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }
}