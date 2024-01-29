using Extengerator.SimpleSample.SomeEventBus;

namespace Extengerator.SimpleSample.EventHandlers;

// Extengerator: abstract classes implementing the marker interface will be ignored in the generated code.
public abstract class AbstractHandler : IEventHandler<bool>, IAutoRegisterHandler
{
    public virtual void HandleEvent(bool eventData)
    {
        //do something
    }
}