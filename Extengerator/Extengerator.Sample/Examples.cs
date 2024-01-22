using Api.OGame.ResponseFilters;
using Zenject;

namespace Extengerator.Sample;

public class Examples
{

    public static void DoStuffWithGeneratedCode(IResponseFilterReceiver receiver, DiContainer container)
    {
        receiver.AddFilters(container);
    }

}