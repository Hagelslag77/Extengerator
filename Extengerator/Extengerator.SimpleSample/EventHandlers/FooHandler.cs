using Extengerator.SimpleSample.SomeEventBus;

namespace Extengerator.SimpleSample.EventHandlers;

// Extengerator: this class implements the marker interface and will show up in the generated code
public sealed class FooHandler : IEventHandler<string>, IAutoRegisterHandler
{
    public void HandleEvent(string eventData)
    {
        //do something
    }
}