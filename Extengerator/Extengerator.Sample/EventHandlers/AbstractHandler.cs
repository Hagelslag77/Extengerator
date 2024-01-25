using Extengerator.Sample.SomeEventBus;

namespace Extengerator.Sample.EventHandlers;

public abstract class AbstractHandler : IEventHandler<bool>, IAutoRegisterHandler
{
    public virtual void HandleEvent(bool eventData)
    {
        //do something
    }
}