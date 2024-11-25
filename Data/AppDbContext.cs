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
    public DbSet<Models.Photo> Photos => Set<Models.Photo>();




}