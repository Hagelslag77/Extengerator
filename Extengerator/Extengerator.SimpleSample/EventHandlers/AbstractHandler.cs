using Extengerator.SimpleSample.SomeEventBus;

namespace Extengerator.SimpleSample.EventHandlers;

public abstract class AbstractHandler : IEventHandler<bool>, IAutoRegisterHandler
{
    public virtual void HandleEvent(bool eventData)
    {
        //do something
    }
}