using DemoApp.Lib;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.Services;

public class CityService : ICityService
{
    private readonly DemoDbContext _demoDb;

    public CityService(DemoDbContext demoDb)
    {
        _demoDb = demoDb;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(string prefix = "")
    {
        return await _demoDb.Cities.ToListAsync();
    }
}
