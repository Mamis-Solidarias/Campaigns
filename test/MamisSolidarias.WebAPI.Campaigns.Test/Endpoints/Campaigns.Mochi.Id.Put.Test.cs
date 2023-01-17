using MamisSolidarias.GraphQlClient;
using Moq;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal sealed class CampaignsMochiIdPutTest
{
    // private Endpoint _endpoint = null!;
    private readonly Mock<IGraphQlClient> _mockGraphQl = new();

    [SetUp]
    public void Setup()
    {
        // _endpoint = EndpointFactory.CreateEndpoint<Endpoint>(_mockGraphQl.Object,null, _mockDb.Object);
    }

    [TearDown]
    public void Teardown()
    {
        _mockGraphQl.Reset();
    }

    // [Test]
    // public async Task WithValidParameters_Succeeds()
    // {
    //     
    // }
    //
    // [Test]
    // public async Task WithInvalidParameters_UserDoesNotHavePermission_Succeeds()
    // {
    //     
    // }
    //
    // [Test]
    // public async Task WithInvalidParameters_BeneficiaryDoesNotExists_Succeeds()
    // {
    //     
    // }
}