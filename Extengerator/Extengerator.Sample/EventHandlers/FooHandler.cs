using Extengerator.Sample.SomeEventBus;

namespace Extengerator.Sample.EventHandlers;

public sealed class FooHandler : IEventHandler<string>, IAutoRegisterHandler
{
    public void HandleEvent(string eventData)
    {
        //do something
    }
}