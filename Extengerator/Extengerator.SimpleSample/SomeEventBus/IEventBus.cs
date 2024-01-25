namespace Extengerator.SimpleSample.SomeEventBus
{
    public interface IEventBus
    {
        void Register<TEvent>(IEventHandler<TEvent> eventHandler);
        
        void DeregisterAll<TEvent>();

    }
}