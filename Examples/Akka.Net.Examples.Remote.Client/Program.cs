using Akka.Actor;
using Akka.Configuration;

namespace Akka.Net.Examples.Remote.Client;

internal class Program
{
	static void Main(string[] args)
	{
		var config = ConfigurationFactory.ParseString(@"
            akka {  
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }
                remote {
                    dot-netty.tcp {
                        port = 8081
                        hostname = 127.0.0.1
                    }
                }
            }");
		var system = ActorSystem.Create("ClientSystem", config);
		var remoteAddress = Address.Parse("akka.tcp://RemoteSystem@127.0.0.1:8080");

		var remoteActor = system.ActorSelection(remoteAddress + "/user/remoteEcho");

		Console.WriteLine($"Say 'Hello, Remote Actor!' (time: {DateTime.Now.ToLongTimeString()}");
		remoteActor.Tell("Hello, Remote Actor!");

		Console.ReadLine();
	}
}
