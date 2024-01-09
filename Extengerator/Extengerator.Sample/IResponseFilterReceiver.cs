namespace Api.OGame.ResponseFilters
{
    public interface IResponseFilterReceiver
    {
        void AddResponseFilterFirst(ResponseFilter filter);
        void AddResponseFilterLast(ResponseFilter filter);
        bool RemoveResponseFilter(ResponseFilter filter);
    }
}