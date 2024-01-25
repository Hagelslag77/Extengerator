using Extengerator.SimpleSample.SomeEventBus;

namespace Extengerator.SimpleSample.EventHandlers;

public sealed class FooHandler : IEventHandler<string>, IAutoRegisterHandler
{
    public void HandleEvent(string eventData)
    {
        //do something
    }
}