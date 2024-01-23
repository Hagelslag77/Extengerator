using Extengerator.MapEndpointsSample.Models;

namespace Extengerator.MapEndpointsSample.Endpoints;

//Extengerator: implement IEndpoint to be recognized by the source generate
public sealed class WeatherForecastEndpoints : IEndpoint
{
    private readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    ];
    
    public void Map(WebApplication app)
    {
        app.MapGet("/weatherforecast", GetWeatherForecasts)
            .WithName("GetWeatherForecast")
            .WithOpenApi();
    }

    private WeatherForecast[] GetWeatherForecasts()
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
}