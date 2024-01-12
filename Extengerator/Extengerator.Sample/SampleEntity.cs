using Api.OGame.ResponseFilters;
//using Generators;
using Zenject;

namespace Extengerator.Sample;

// This code will not compile until you build the project with the Source Generators

public class Foo
{
}


//[Report]
public partial class SampleEntity : Foo
{
    public int Id { get; } = 42;
    public string? Name { get; } = "Sample";

    private readonly IResponseFilterReceiver? _filter = null;
    private readonly DiContainer _container = new ();
    
    void Foo()
    {
       _filter?.AddFilters(_container);
    }
}

