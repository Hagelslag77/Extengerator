

namespace Api.OGame.ResponseFilters
{

    public sealed class UserBannedFilter : ResponseFilter
    {
        protected override bool Filter(IApiResponse response)
        {
            return true;
        }
    }
}