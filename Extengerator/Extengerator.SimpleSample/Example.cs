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
        // Extengerator: this extension method is auto generated for all classes implementing IAutoRegisterHandler
        // see configuration file 'Extengerator.settings.yaml'
        // in addition extra arguments are passed as parameters
        _eventBus.RegisterAllHandlers(container);
    }

    public void Dispose()
    {
        // Extengerator: this extension method is auto generated for all classes implementing IAutoRegisterHandler
        // see configuration file 'Extengerator.settings.yaml'
        _eventBus.DeregisterAllHandlers();
    }
}