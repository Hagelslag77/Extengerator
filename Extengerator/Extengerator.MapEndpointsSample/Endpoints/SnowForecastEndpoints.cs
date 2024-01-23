using Extengerator.MapEndpointsSample.Models;

namespace Extengerator.MapEndpointsSample.Endpoints;

//Extengerator: implement IEndpoint to be recognized by the source generate
public class SnowForecastEndpoints : IEndpoint
{
    private readonly string[] _summaries =
    [
        "Snowstorm", "Light snow", "Chilly", "Thawing", "Rain",
    ];
    
    public void Map(WebApplication app)
    {
        app.MapGet("/snowforecast", GetSnowForecasts)
            .WithName("GetSnowForecast")
            .WithOpenApi();
        
        app.MapGet("/snowforecast/{city}", GetSnowForecastsForCity)
            .WithName("GetSnowForecastForCity")
            .WithOpenApi();
    }
    
    private WeatherForecast[] GetSnowForecasts()
    {
        return Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    _summaries[Random.Shared.Next(_summaries.Length)]
                ))
            .ToArray();
    }
    
    private WeatherForecast[] GetSnowForecastsForCity(string city)
    {
        return Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    $"{city}: {_summaries[Random.Shared.Next(_summaries.Length)]}"
                ))
            .ToArray();
    }
}