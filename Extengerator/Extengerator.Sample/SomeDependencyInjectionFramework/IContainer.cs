
namespace Extengerator.Sample.SomeDependencyInjectionFramework;

public interface IContainer
{
    public T Resolve<T>();
}