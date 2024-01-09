
namespace Api.OGame.ResponseFilters
{

    public abstract class BogousFilter : ResponseFilter
    {
        protected override bool Filter(IApiResponse response)
        {
            return true;
        }
    }
}