using GrpcTestProject.Data;
using Microsoft.EntityFrameworkCore;

namespace Tremium.WebAPI.Data.Extensions;

public static class ServiceScope
{
    public static void SetupData(this IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }
}
