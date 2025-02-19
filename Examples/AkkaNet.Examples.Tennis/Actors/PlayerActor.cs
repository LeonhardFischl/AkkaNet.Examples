using Akka.Actor;

using AkkaNet.Examples.Tennis.Events;

namespace AkkaNet.Examples.Tennis.Actors;

/// <summary>
/// The PlayerActor is responsible for serving the ball.
/// </summary>
public class PlayerActor : ReceiveActor
{
	private readonly string _name;

	/// <summary>
	/// Ctor
	/// </summary>
	/// <param name="name"></param>
	public PlayerActor(string name)
	{
		this._name = name;

		this.Receive<ServeBall>(message =>
		{
			Console.WriteLine($"{this._name} served the ball!");
			this.Sender.Tell(new ServeBall()); // Send ball back to the MatchActor
		});


		this.Receive<StopMatch>(message =>
		{
			Console.WriteLine($"{this._name} stopping!");
			Context.Stop(this.Self);
		});
	}


}