using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcTestProject.Data;

namespace GrpcTestProject.Services;
public class ChatService : Chat.ChatBase
{
    private readonly ILogger<ChatService> _logger;
    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
    }
    public override async  Task SendMessage 
        (IAsyncStreamReader<ClientMessage> requestStream, 
        IServerStreamWriter<ServerMessage> responseStream, 
        ServerCallContext context)
    {
        var clientToServerTask = HandleClientRequest(requestStream, context);
        var serverToClientTask = HandleServerResponse(responseStream, context);
        
        await Task.WhenAll(clientToServerTask, serverToClientTask);
    }

    private async Task HandleClientRequest(IAsyncStreamReader<ClientMessage> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            var message = requestStream.Current;
            _logger.LogInformation($"Client said {message.Text}");
        }
    }

    private static async Task<int> HandleServerResponse(IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
    {
        var pingCount = 0;

        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(
            new ServerMessage
            {
                Text = $"Server said hi {++pingCount} times.",
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            });

            await Task.Delay(1000);
        }

        return pingCount;
    }
}