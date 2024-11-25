using GrpcTestProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcTestProject.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Models.Gallery> Galleries => Set<Models.Gallery>();
    public DbSet<Models.Traffic> Traffic => Set<Models.Traffic>();
    public DbSet<Models.Location> Location => Set<Models.Location>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Location>()
        .HasData(
            new Models.Location
            {
                Id = 1,
                Name = "Sesame street",
                Description = ""
            },
            new Models.Location
            {
                Id = 2,
                Name = "Bulevar Zorana Djindjica"
            }
        );

    }
}