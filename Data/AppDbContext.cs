using GrpcTestProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcTestProject.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Models.Gallery> Galleries => Set<Models.Gallery>();
}