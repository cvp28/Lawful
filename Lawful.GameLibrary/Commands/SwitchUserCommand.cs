using System.Xml;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using UI;

public static class SwitchUserCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("User Switching Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.WriteLine();
		GameConsole.WriteLine(
			"USAGE: su [username] [password]\n" +
			"\n" +
			"WHERE:\n" +
			"	username -> Name of user you intend to switch to\n" +
			"	password -> The specified user's password");
	}

	public static void Invoke(InputQuery Query)
	{
		if (Query.Arguments.Count < 1)
		{
			GameConsole.WriteLine("Insufficient arguments");
			return;
		}

		var Layer = App.GetLayer<GameLayer>("Game");
		UserAccount TryAccount = Player.CurrentSession.Host.GetUser(Query.Arguments[0]);

		if (TryAccount is null)
		{
			GameConsole.WriteLine($"No user found '{Query.Arguments[0]}'");
			return;
		}

		if (TryAccount.Password.Length == 0)
		{
			Switch();
			return;
		}

		int Tries = 0;
		int MaxTries = 3;

		Layer.PromptProvider = GetLoginPrompt;
		Layer.Input.OnInput = delegate (string Password)
		{
			Tries++;

			if (Password != TryAccount.Password)
			{
				GameConsole.WriteLine($"Invalid password supplied ({Tries}/{MaxTries})", ConsoleColor.Red, ConsoleColor.Black);

				if (Tries == MaxTries)
				{
					GameConsole.WriteLine($"Login failed for {TryAccount.Username} :: max attempts reached", ConsoleColor.Red, ConsoleColor.Black);

					Layer.ResetInput();
					return;
				}

				return;
			}

			Switch();
			return;
		};

		void Switch()
		{
			Player.CurrentSession.User = TryAccount;
			GameConsole.WriteLine($"Successfully changed user to '{TryAccount.Username}'", ConsoleColor.Green, ConsoleColor.Black);

			Layer.ResetInput();
		}

		string GetLoginPrompt() => $"Login to {TryAccount.Username}: ";
	}
}
