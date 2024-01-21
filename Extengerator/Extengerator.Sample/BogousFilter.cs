
namespace Api.OGame.ResponseFilters
{

    public abstract class BogousFilter : ResponseFilter, IResponseFilter
    {
        protected override bool Filter(IApiResponse response)
        {
            return true;
        }
    }
}