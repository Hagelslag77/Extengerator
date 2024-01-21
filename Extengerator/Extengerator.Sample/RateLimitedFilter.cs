
namespace Api.OGame.ResponseFilters
{

    public sealed class RateLimitedFilter : ResponseFilter, IResponseFilter
    {
        protected override bool Filter(IApiResponse response)
        {
            return true;
        }
    }
}