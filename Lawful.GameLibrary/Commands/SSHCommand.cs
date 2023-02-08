using System.Net;
using System.Xml;
using Esprima.Ast;
using Lawful.GameLibrary.UI;
using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class SSHCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("SSH Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.NextLine();
		GameConsole.WriteLine(
			"USAGE: ssh [username]@[hostname] [password]\n" +
			"\n" +
			"WHERE:\n" +
			"	username -> Name of user you intend to login to\n" +
			"	hostname -> IP/Domain of remote system you intend to connect to\n" +
			"   password -> Password for the specified user");
	}

	public static void Invoke(InputQuery Query)
	{
		if (Query.Arguments.Count < 1)
		{
			GameConsole.WriteLine("Insufficient arguments");
			return;
		}

		string[] LoginQuery = Query.Arguments[0].Split('@', StringSplitOptions.RemoveEmptyEntries);

		if (LoginQuery.Length < 2)
		{
			GameConsole.WriteLine("Insufficient arguments");
			return;
		}

		string Username = LoginQuery[0];
		string Hostname = LoginQuery[1];

		// Error checking
		if (!IPAddress.TryParse(Hostname, out IPAddress TryIP))
		{
			GameConsole.WriteLine("Invalid IP address specified", ConsoleColor.Red, ConsoleColor.Black);
			return;
		}

		if (Player.CurrentSession.Host.Address == TryIP.ToString())
		{
			GameConsole.WriteLine("Already connected to that machine, cannot perform SSH connection", ConsoleColor.Red, ConsoleColor.Black);
			return;
		}

		GameConsole.Write($"Trying connection to '{TryIP}' ", ConsoleColor.Yellow, ConsoleColor.Black);

		Util.BeginCharacterAnimation(GameConsole, Util.LoadingAnimFrames, 8, 32, 75, GameConsole.GetCursorPosition());

		GameConsole.Write("- ");

		if (!Computers.HasComputer(TryIP.ToString()))
		{
			GameConsole.WriteLine($"could not find a node at the IP address: '{TryIP}'", ConsoleColor.Red, ConsoleColor.Black);
			return;
		}

		// Hostname is valid at this point, check Username

		Computer TryPC = Computers.GetComputer(TryIP.ToString());

		if (!TryPC.HasUser(Username))
		{
			GameConsole.WriteLine($"node at IP '{TryIP}' does not contain user '{Username}'", ConsoleColor.Red, ConsoleColor.Black);
			return;
		}

		GameConsole.WriteLine("connected", ConsoleColor.Green, ConsoleColor.Black);

		// Both are valid at this point, check if user has a password. If so, start login.
		UserAccount TryUser = TryPC.GetUser(Username);
		var Layer = App.GetLayer<GameLayer>("Game");

		// Login bit goes here

		int Tries = 0;
		int MaxTries = 3;

		if (TryUser.Password.Length == 0)
		{
			Login();
			return;
		}

		Layer.PromptProvider = GetLoginPrompt;
		Layer.Input.OnInput = delegate (string Password)
		{
			Tries++;

			if (Password != TryUser.Password)
			{
				GameConsole.WriteLine($"Invalid password supplied ({Tries}/{MaxTries})", ConsoleColor.Red, ConsoleColor.Black);
				
				if (Tries == MaxTries)
				{
					GameConsole.WriteLine($"Login failed for {Username}@{TryIP} :: max attempts reached", ConsoleColor.Red, ConsoleColor.Black);

					Layer.ResetInput();
					return;
				}

				return;
			}

			Login();
		};

		void Login()
		{
			GameConsole.WriteLine($"Logged in as user '{Username}' at the connected node '{TryIP}'", ConsoleColor.Green, ConsoleColor.Black);

			Player.CloseCurrentSession();
			TryPC.TryOpenSession(Username, out Player.CurrentSession);

			EventManager.HandleEventsByTrigger(Trigger.SSHConnect);

			Layer.ResetInput();
		}

		string GetLoginPrompt() => $"Login to {TryUser.Username}@{TryPC.Address}: ";
	}
}
