using Akka.Actor;

using AkkaNet.Examples.Tennis.Events;

namespace AkkaNet.Examples.Tennis.Actors;

/// <summary>
/// The MatchActor is responsible for coordinating the match between two players.
/// </summary>
public class MatchActor : ReceiveActor
{
	private readonly IActorRef _player1;
	private readonly IActorRef _player2;
	private int _player1Score;
	private int _player2Score;
	private int _turns;

	/// <summary>
	/// Ctor AND starts the match
	/// </summary>
	/// <param name="player1"></param>
	/// <param name="player2"></param>
	public MatchActor(IActorRef player1, IActorRef player2)
	{
		this._player1 = player1;
		this._player2 = player2;
		this._player1Score = 0;
		this._player2Score = 0;
		Context.SetReceiveTimeout(TimeSpan.FromSeconds(5));
		this.ReceiveAsync<ReceiveTimeout>(async _ => await this.WaitingForNextPlayerScore());

		this.Receive<StartMatch>(message =>
		{
			Console.WriteLine("Match has started!");

			// Start the game by having the first player serve after 
			Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(5), this.Self, new ServeBall(), this.Self);
		});

		this.Receive<StopMatch>(message =>
		{
			Console.WriteLine("Stopping match.");
			this._player1.Tell(new StopMatch());
			this._player2.Tell(new StopMatch());
			Context.Stop(this.Self);
		});

		this.Receive<ServeBall>(message =>
		{
			this._turns++;
			if (this._turns > 5)
			{
				this.DecideWinner();
				return;
			}
			// Randomly decide who scores
			if (new Random().NextDouble() > 0.5)
			{
				this._player1Score++;
				Console.WriteLine($"Player 1 scores a point! Current Score: Player 1 - {this._player1Score}, Player 2 - {this._player2Score}");
			}
			else
			{
				this._player2Score++;
				Console.WriteLine($"Player 2 scores a point! Current Score: Player 1 - {this._player1Score}, Player 2 - {this._player2Score}");
			}

			// Alternate serving
			if (this._turns % 2 == 1)
			{
				this._player1.Tell(message, this.Self);
			}
			else
			{
				this._player2.Tell(message, this.Self);
			}

			// Continue the game
			this.Self.Tell(new ServeBall());
		});

		this.Receive<PlayerScored>(message =>
		{
			this._player1Score++;
			Console.WriteLine($"Player 1 scores! Current Score: Player 1 - {this._player1Score}, Player 2 - {this._player2Score}");

			// Restart the timer if needed
			Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(5), this.Self, new PlayerScored(), this.Self);
		});


	}

	private void DecideWinner()
	{
		if (this._player1Score > this._player2Score)
		{
			Console.WriteLine("Player 1 wins the match!");
		}
		else if (this._player2Score > this._player1Score)
		{
			Console.WriteLine("Player 2 wins the match!");
		}
		else
		{
			Console.WriteLine("It's a draw!");
		}
	}

	private async Task WaitingForNextPlayerScore()
	{
		Console.WriteLine("Waiting for the next score!");

		this.Self.Tell(new PlayerScored());
	}
}