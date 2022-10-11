using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

public static class MochiParticipantExtensions
{
    public static SchoolCycle? Map(this GraphQlClient.SchoolCycle? beneficiaryGender)
        => beneficiaryGender switch
        {
            GraphQlClient.SchoolCycle.PreSchool => SchoolCycle.PreSchool,
            GraphQlClient.SchoolCycle.PrimarySchool => SchoolCycle.PrimarySchool,
            GraphQlClient.SchoolCycle.MiddleSchool => SchoolCycle.MiddleSchool,
            GraphQlClient.SchoolCycle.HighSchool => SchoolCycle.HighSchool,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(beneficiaryGender), beneficiaryGender, null)
        };


    public static BeneficiaryGender Map(this GraphQlClient.BeneficiaryGender beneficiaryGender)
        => beneficiaryGender switch
        {
            GraphQlClient.BeneficiaryGender.Male => BeneficiaryGender.Male,
            GraphQlClient.BeneficiaryGender.Female => BeneficiaryGender.Female,
            GraphQlClient.BeneficiaryGender.Other => BeneficiaryGender.Other,
            _ => throw new ArgumentOutOfRangeException(nameof(beneficiaryGender), beneficiaryGender, null)
        };
    
    public static GraphQlClient.SchoolCycle? Map(this SchoolCycle? participantBeneficiaryGender)
    {
        return participantBeneficiaryGender switch
        {
            SchoolCycle.PreSchool => GraphQlClient.SchoolCycle.PreSchool,
            SchoolCycle.PrimarySchool => GraphQlClient.SchoolCycle.PrimarySchool,
            SchoolCycle.MiddleSchool => GraphQlClient.SchoolCycle.MiddleSchool,
            SchoolCycle.HighSchool => GraphQlClient.SchoolCycle.HighSchool,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(participantBeneficiaryGender),
                participantBeneficiaryGender, null)
        };
    }

    public static GraphQlClient.BeneficiaryGender Map(this BeneficiaryGender participantBeneficiaryGender)
    {
        return participantBeneficiaryGender switch
        {
            BeneficiaryGender.Male => GraphQlClient.BeneficiaryGender.Male,
            BeneficiaryGender.Female => GraphQlClient.BeneficiaryGender.Female,
            BeneficiaryGender.Other => GraphQlClient.BeneficiaryGender.Other,
            _ => throw new ArgumentOutOfRangeException(nameof(participantBeneficiaryGender),
                participantBeneficiaryGender, null)
        };
    }
}