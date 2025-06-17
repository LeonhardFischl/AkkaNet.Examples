using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Akka.Net.Examples.Robotics.Console;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddLogging();
builder.Services.AddSingleton<ActorSystem>();
builder.Services.AddHostedService<GrpcServerService>();

var host = builder.Build();

await host.RunAsync();




