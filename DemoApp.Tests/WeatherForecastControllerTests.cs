using System.Net.Http.Json;
using DemoApp.Lib;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace DemoApp.Tests;

public class WeatherForecastControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly DemoDbContext _dbContext;

    public WeatherForecastControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        _dbContext = _factory.Services.GetRequiredService<DemoDbContext>();

        EnsureCleanDb();
    }

    [Fact]
    public async Task Get_WithoutPrefix_ReturnsAllCities()
    {
        // Arrange
        _dbContext.Cities.Add(new City { Name = "London", Lat = 51.5287398, Lon = -0.2664031 });
        _dbContext.Cities.Add(new City { Name = "Berlin", Lat = 52.5069386, Lon = 13.2599275 });
        _dbContext.Cities.Add(new City { Name = "Barcelona", Lat = 41.3927673, Lon = 2.0577888});
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/weatherforecast/cities");

        // Assert
        response.EnsureSuccessStatusCode();
        var cities = await response.Content.ReadFromJsonAsync<IEnumerable<City>>();
        cities.Should().HaveCount(3);
        cities.Should().Contain(c => c.Name == "London");
        cities.Should().Contain(c => c.Name == "Berlin");
        cities.Should().Contain(c => c.Name == "Barcelona");
    }

    private void EnsureCleanDb()
    {
        _dbContext.Database.CloseConnection();
        _dbContext.Database.OpenConnection();
        _dbContext.Database.EnsureCreated();
        _dbContext.ChangeTracker.Clear();
    }
}
