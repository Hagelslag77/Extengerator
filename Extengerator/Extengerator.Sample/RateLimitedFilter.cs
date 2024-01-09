
namespace Api.OGame.ResponseFilters
{

    public sealed class RateLimitedFilter : ResponseFilter
    {
        protected override bool Filter(IApiResponse response)
        {
            return true;
        }
    }
}