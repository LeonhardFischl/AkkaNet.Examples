using ChatServer.Services;
using ChatServerApp.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


/*
// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS,
// visit https://go.microsoft.com/fwlink/?linkid=2099682

   To avoid missing ALPN support issue on Mac. To work around this issue, configure Kestrel and the gRPC client to use HTTP/2 without TLS.
   You should only do this during development. Not using TLS will result in gRPC messages being sent without encryption.

   https://learn.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-7.0
*/
builder.WebHost.ConfigureKestrel(options =>
{
	// Setup a HTTP/2 endpoint without TLS.
	options.ListenLocalhost(8080, o => o.Protocols = HttpProtocols.Http2);
});


// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ChatRoomService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChatService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

Console.WriteLine($"gRPC server about to listening on port:8080");

app.Run();