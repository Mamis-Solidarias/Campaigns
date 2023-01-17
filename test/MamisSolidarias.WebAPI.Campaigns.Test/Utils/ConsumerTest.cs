using MassTransit;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal abstract class ConsumerTest<TConsumer> : Test
    where TConsumer : class, IConsumer
{
    protected TConsumer Consumer { get; set; } = null!;
    protected abstract TConsumer CreateConsumer();

    public override void Setup()
    {
        base.Setup();
        Consumer = CreateConsumer();
    }
}