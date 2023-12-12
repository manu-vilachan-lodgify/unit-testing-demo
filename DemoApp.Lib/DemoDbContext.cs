using Microsoft.EntityFrameworkCore;

namespace DemoApp.Lib;

public class DemoDbContext : DbContext
{
    public DbSet<City> Cities { get; set; }

    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoDbContext).Assembly);
    }
}
