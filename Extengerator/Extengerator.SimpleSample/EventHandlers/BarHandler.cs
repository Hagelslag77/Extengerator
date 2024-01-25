using Extengerator.SimpleSample.SomeEventBus;

namespace Extengerator.SimpleSample.EventHandlers;

public sealed class BarHandler : IEventHandler<object>, IAutoRegisterHandler
{
    public void HandleEvent(object eventData)
    {
        //do something
    }
}