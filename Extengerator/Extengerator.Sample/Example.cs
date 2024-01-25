using System;
using Extengerator.Sample.EventHandlers;
using Extengerator.Sample.SomeDependencyInjectionFramework;
using Extengerator.Sample.SomeEventBus;

namespace Extengerator.Sample;

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