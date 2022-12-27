using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MamisSolidarias.GraphQlClient;
using MassTransit;
using Moq;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal static class MockExtensions
{
    public static ConsumeContext<T> MockConsumeContext<T>(T message) where T : class
    {
        var context = new Mock<ConsumeContext<T>>();
        context.SetupGet(t => t.Message)
            .Returns(message);

        return context.Object;
    }

    public static void MockGetDonor(this Mock<IGraphQlClient> client, 
        Expression<Func<int, bool>> matcher,
        string donorName)
    {
        client.MockRequest<IGetDonorResult, IGetDonor_Donor>(
            t => t.Donor,
            new GetDonor_Donor_Donor(donorName),
            t => t.GetDonor.ExecuteAsync(
                It.Is(matcher),
                It.IsAny<CancellationToken>()
            )
        );
    }

    public static void MockGetCommunity(this Mock<IGraphQlClient> client,
        Expression<Func<string, bool>> matcher,
        string? communityId)
    {
        client.MockRequest<IGetCommunityResult, IGetCommunity_Community>(
            t => t.Community,
            new GetCommunity_Community_Community(communityId),
            t => t.GetCommunity.ExecuteAsync(
                It.Is(matcher),
                It.IsAny<CancellationToken>()
            )
        );
    }

    public static void MockGetBeneficiaryWithClothes(this Mock<IGraphQlClient> client,
        Expression<Func<int, bool>> idMatcher
        , BeneficiaryGender gender, int? shoeSize)
    {
        var result = new GetBeneficiaryWithClothes_Beneficiary_Beneficiary(
            gender,
            new GetBeneficiaryWithClothes_Beneficiary_Clothes_Clothes(shoeSize)
        );

        client.MockRequest<IGetBeneficiaryWithClothesResult, IGetBeneficiaryWithClothes_Beneficiary>(
            t => t.Beneficiary,
            result,
            t => t.GetBeneficiaryWithClothes.ExecuteAsync(
                It.Is(idMatcher),
                It.IsAny<CancellationToken>())
        );
    }

    public static void MockGetBeneficiaryWithEducation(this Mock<IGraphQlClient> client,
        Expression<Func<int, bool>> idMatcher,
        string firstName, string lastName,
        BeneficiaryGender gender, SchoolCycle? schoolCycle)
    {
        var result = new GetBeneficiaryWithEducation_Beneficiary_Beneficiary(
            firstName,lastName,gender,
            schoolCycle is null ? null : new GetBeneficiaryWithEducation_Beneficiary_Education_Education(schoolCycle)
        );

        client.MockRequest<IGetBeneficiaryWithEducationResult, IGetBeneficiaryWithEducation_Beneficiary>(
            t => t.Beneficiary,
            result,
            t => t.GetBeneficiaryWithEducation.ExecuteAsync(
                It.Is(idMatcher),
                It.IsAny<CancellationToken>())
        );
    }

    public static void MockRequest<T, TR>(this Mock<IGraphQlClient> client,
        Expression<Func<T, TR?>> resultSelector,
        TR result,
        Expression<Func<IGraphQlClient, Task<IOperationResult<T>>>> expression
    ) where T : class
    {
        var mockResult = new Mock<T>();
        mockResult.Setup(resultSelector)
            .Returns(result);

        var operationResult = new Mock<IOperationResult<T>>();
        operationResult.SetupGet(t => t.Data)
            .Returns(mockResult.Object);
        operationResult.SetupGet(t => t.Errors)
            .Returns(new List<IClientError>());

        client.Setup(expression)
            .ReturnsAsync(operationResult.Object);
    }

    public static void MockErrors<T>(this Mock<IGraphQlClient> client,
        Expression<Func<IGraphQlClient, Task<IOperationResult<T>>>> expression,
        params IClientError[] errors
    ) where T : class
    {
        var operationResult = new Mock<IOperationResult<T>>();

        operationResult.SetupGet(t => t.Data)
            .Returns((T?)null);
        operationResult.SetupGet(t => t.Errors)
            .Returns(errors);

        client.Setup(expression).ReturnsAsync(operationResult.Object);
    }
    
    public static void MockEmptyResponse<T>(this Mock<IGraphQlClient> client,
        Expression<Func<IGraphQlClient, Task<IOperationResult<T>>>> expression
    ) where T : class
    {
        var operationResult = new Mock<IOperationResult<T>>();

        operationResult.SetupGet(t => t.Data)
            .Returns(null as T);
        operationResult.SetupGet(t => t.Errors)
            .Returns(Enumerable.Empty<IClientError>().ToList);

        client.Setup(expression).ReturnsAsync(operationResult.Object);
    }

    public static void MockAuthenticationError<T>(this Mock<IGraphQlClient> client,
        Expression<Func<IGraphQlClient, Task<IOperationResult<T>>>> expression
    ) where T : class
    {
        MockErrors(client, expression, new ClientError("AUTH_NOT_AUTHORIZED", "AUTH_NOT_AUTHORIZED"));
    }
}


