using Akka.Actor;
using Akka.Configuration;

namespace Akka.Net.Examples.Remote.Server;

public class Program
{
	public static void Main(string[] args)
	{
		var config = ConfigurationFactory.ParseString(@"
            akka {  
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }
                remote {
                    dot-netty.tcp {
                        port = 8080
                        hostname = 127.0.0.1
                    }
                }
            }");

		using var system = ActorSystem.Create("RemoteSystem", config);
		Console.WriteLine($"Start listening to any messages coming in...(time: {DateTime.Now.ToLongTimeString()})");
		var actor = system.ActorOf(Props.Create(() => new RemoteEchoActor()), "remoteEcho");

		// Keep the application running
		Console.WriteLine("Press Enter to exit...");
		Console.ReadLine();
	}
}
