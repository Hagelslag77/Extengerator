using Extengerator.SimpleSample.SomeEventBus;

namespace Extengerator.SimpleSample.EventHandlers;

// Extengerator: this class implements the marker interface and will show up in the generated code
public sealed class BarHandler : IEventHandler<object>, IAutoRegisterHandler
{
    public void HandleEvent(object eventData)
    {
        //do something
    }
}