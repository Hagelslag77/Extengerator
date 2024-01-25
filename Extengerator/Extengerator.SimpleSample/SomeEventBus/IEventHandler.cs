namespace Extengerator.SimpleSample.SomeEventBus;

public interface IEventHandler<in TEvent>
{
    void HandleEvent(TEvent eventData);
}