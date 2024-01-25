using Extengerator.Sample.SomeEventBus;

namespace Extengerator.Sample.EventHandlers;

public sealed class BarHandler : IEventHandler<object>, IAutoRegisterHandler
{
    public void HandleEvent(object eventData)
    {
        //do something
    }
}