using ENSEKTask.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ENSEKTask.UnitTests;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsFiveWeatherForecasts()
    {
        // Arrange
        var controller = new WeatherForecastController(NullLogger<WeatherForecastController>.Instance);

        // Act
        var result = controller.Get();

        // Assert
        Assert.Equal(5, result.Count());
    }

    [Fact]
    public void Get_ReturnsForecastsWithSummary()
    {
        var controller = new WeatherForecastController(NullLogger<WeatherForecastController>.Instance);

        var result = controller.Get();

        Assert.All(result, forecast => Assert.False(string.IsNullOrWhiteSpace(forecast.Summary)));
    }
}
