namespace Extengerator.MapEndpointsSample.Endpoints;

//Extengerator: this is the marker interface as configured in Extengerator.settings.yaml
public interface IEndpoint
{
    void Map(WebApplication app);
}