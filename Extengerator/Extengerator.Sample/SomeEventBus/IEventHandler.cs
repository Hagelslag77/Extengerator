namespace Extengerator.Sample.SomeEventBus;

public interface IEventHandler<in TEvent>
{
    void HandleEvent(TEvent eventData);
}