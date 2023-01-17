using FastEndpoints;
using EndpointFactory = MamisSolidarias.Utils.Test.EndpointFactory;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal abstract class EndpointTest<TEndpoint> : Test
    where TEndpoint : class, IEndpoint
{
    protected TEndpoint _endpoint = null!;
    protected abstract object?[] ConstructorArguments { get; }

    public override void Setup()
    {
        base.Setup();
        _endpoint = EndpointFactory.CreateEndpoint<TEndpoint>(
            ConstructorArguments
        );
    }
}