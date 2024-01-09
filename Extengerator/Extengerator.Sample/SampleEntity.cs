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

    public IResponseFilterReceiver filter;
    public DiContainer Container = new ();
    
    void Foo()
    {
       // filter.AddFilters(Container);
    }
}

