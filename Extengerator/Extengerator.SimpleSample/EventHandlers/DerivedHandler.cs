namespace Extengerator.SimpleSample.EventHandlers;

// Extengerator: this class does not explicitly implement the marker interface
// and will therefore not be part of the generate code.
// It implements the interface implicitly only by deriving from AbstractHandler
// Only classes implementing the marker interface explicitly will be part of the generated code.
public sealed class DerivedHandler: AbstractHandler
{
    public override void HandleEvent(bool eventData)
    {
        //do something
    }
}