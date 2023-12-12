using DemoApp.Lib;
using DemoApp.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DemoApp.Tests;

public class CityServiceTests
{
    private readonly DemoDbContext _dbContext;

    public CityServiceTests()
    {
        var options = new DbContextOptionsBuilder<DemoDbContext>()
            .UseInMemoryDatabase(nameof(CityServiceTests))
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
            .EnableSensitiveDataLogging()
            .Options;
        _dbContext = new DemoDbContext(options);
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetCities_WithoutPrefix_ReturnsAllCities()
    {
        // Arrange
        _dbContext.Cities.Add(new City { Name = "London", Lat = 51.5287398, Lon = -0.2664031 });
        _dbContext.Cities.Add(new City { Name = "Berlin", Lat = 52.5069386, Lon = 13.2599275 });
        _dbContext.Cities.Add(new City { Name = "Barcelona", Lat = 41.3927673, Lon = 2.0577888});
        await _dbContext.SaveChangesAsync();

        // Act
        var service = new CityService(_dbContext);
        var cities = await service.GetCitiesAsync();

        // Assert
        cities.Should().HaveCount(3);
    }
}