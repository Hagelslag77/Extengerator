namespace Extengerator.SimpleSample.SomeDependencyInjectionFramework;

public interface IContainer
{
    public T Resolve<T>();
}