using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcTestProject.Data;
using Microsoft.EntityFrameworkCore;

namespace GrpcTestProject.Services;

public class TrafficService : Traffic.TrafficBase
{
    private readonly ILogger<TrafficService> _logger;
    private readonly AppDbContext _context;
    public TrafficService(AppDbContext context, ILogger<TrafficService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task GetTrafficInformation
        (TrafficRequest request,
         IServerStreamWriter<TrafficResponse> responseStream,
         ServerCallContext context)
    {

        for (int i = 0; i < 30; i++)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("The request was forcibly cancelled.");
                break;
            }
            var traffic = await _context.Traffic
                .FirstOrDefaultAsync(x => x.LocationId == request.LocationId);

            await responseStream.WriteAsync(new TrafficResponse
            {
                TrafficStatus = traffic!.TrafficStatus,
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                Note = traffic.Note
            });
            await Task.Delay(3000);
        }

    }

}
