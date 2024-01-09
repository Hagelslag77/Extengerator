namespace Api.OGame.ResponseFilters
{
    interface IResponseFilter
    {
        
    }
    
    public abstract class ResponseFilter : IResponseFilter
    {
        protected abstract bool Filter(IApiResponse request);
        
    }
}