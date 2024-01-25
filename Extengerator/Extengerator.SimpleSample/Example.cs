using System;
using Extengerator.SimpleSample.EventHandlers;
using Extengerator.SimpleSample.SomeDependencyInjectionFramework;
using Extengerator.SimpleSample.SomeEventBus;

namespace Extengerator.SimpleSample;

public sealed class Example : IDisposable
{
    private readonly IEventBus _eventBus;

    public Example(IEventBus eventBus, IContainer container)
    {
        _eventBus = eventBus;
        _eventBus.RegisterAllHandlers(container);
    }

    public void Dispose()
    {
        _eventBus.DeregisterAllHandlers();
    }
}