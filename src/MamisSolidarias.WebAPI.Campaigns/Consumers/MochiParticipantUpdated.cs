using HotChocolate;
using MamisSolidarias.GraphQlClient;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.Messages;
using MamisSolidarias.WebAPI.Campaigns.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MamisSolidarias.WebAPI.Campaigns.Consumers;

public class MochiParticipantUpdated : IConsumer<BeneficiaryUpdated>
{
    private readonly CampaignsDbContext _dbContext;
    private readonly IGraphQlClient _graphQlClient;

    public MochiParticipantUpdated(CampaignsDbContext dbContext, IGraphQlClient graphQlClient)
    {
        _dbContext = dbContext;
        _graphQlClient = graphQlClient;
    }


    public async Task Consume(ConsumeContext<BeneficiaryUpdated> context)
    {
        var token = context.CancellationToken;
        var beneficiaryId = context.Message.BeneficiaryId;

        var operationResult = await _graphQlClient.GetBeneficiaryWithEducation.ExecuteAsync(beneficiaryId, token);

        var hasErrors = await operationResult.HandleErrors(
            _ => Task.CompletedTask,
            (_, _) => Task.CompletedTask,
            token
        );

        if (hasErrors || operationResult.Data?.Beneficiary is null)
            throw new GraphQLException("Error getting beneficiary");

        var beneficiary = operationResult.Data.Beneficiary;

        var newSchoolCycle = beneficiary.Education?.Cycle.Map();

        await _dbContext.MochiParticipants
            .Where(t => t.BeneficiaryId == beneficiaryId)
            .ExecuteUpdateAsync(t =>
                    t.SetProperty(r => r.SchoolCycle, r => newSchoolCycle)
                        .SetProperty(r => r.BeneficiaryGender, beneficiary.Gender.Map())
                        .SetProperty(r => r.BeneficiaryName, $"{beneficiary.FirstName} {beneficiary.LastName}")
                , token);
    }
}