

namespace Api.OGame.ResponseFilters
{

    public sealed class UserBannedFilter : ResponseFilter, IResponseFilter
    {
        protected override bool Filter(IApiResponse response)
        {
            return true;
        }
    }
}