
namespace GrpcTestProject.Data.DbInitializer;
public class DbInitializer
{

    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var _dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();

            _dbContext.Database.EnsureCreated();

            if (!_dbContext.Location.Any())
            {
                var locations = new List<Models.Location>
                {
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
                };
                await _dbContext.Location.AddRangeAsync(locations);
                await _dbContext.SaveChangesAsync();
            }

            if (!_dbContext.Traffic.Any())
            {
                var traffic = new List<Models.Traffic>
                {
                     new Models.Traffic
                    {

                        Id = 1,
                        LocationId = 1,
                        Note = "All clear",
                        TrafficStatus = TrafficResponse.Types.TrafficStatus.TrafficClear
                    },
                    new Models.Traffic
                    {
                        Id = 2,
                        LocationId = 2,
                        Note = "Umerena guzva na bulevaru Zorana Djindjica.",
                        TrafficStatus = TrafficResponse.Types.TrafficStatus.TrafficModerate
                    },
                    new Models.Traffic
                    {
                        Id = 3,
                        LocationId = 1,
                        Note = "Severe traffic on Sesame st.",
                        TrafficStatus = TrafficResponse.Types.TrafficStatus.TrafficSevere
                    }
                };
                await _dbContext.Traffic.AddRangeAsync(traffic);
                await _dbContext.SaveChangesAsync();
            }

        }
    }


}