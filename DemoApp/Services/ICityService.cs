using DemoApp.Lib;

namespace DemoApp.Services;

public interface ICityService
{
    Task<IEnumerable<City>> GetCitiesAsync(string prefix = "");
}