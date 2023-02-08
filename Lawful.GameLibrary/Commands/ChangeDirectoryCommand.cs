using System.Xml;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class ChangeDirectoryCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("Change Directory Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.NextLine();
		GameConsole.WriteLine(
			"USAGE: cd [directoryname]\n" +
			"\n" +
			"WHERE:\n" +
			"   directoryname -> Path to directory");
	}

	public static void Invoke(InputQuery Query)
	{
		// Handle no arguments
		if (Query.Arguments.Count == 0)
		{
			GameConsole.WriteLine("Insufficient arguments");
			return;
		}

		// Handle query
		if (!FSAPI.TryGetNode(Player.CurrentSession, Query.Arguments[0], FSNodeType.Directory, out XmlNode TryDirectory))
		{
			GameConsole.WriteLine($"Directory '{Query.Arguments[0]}' not found");
			return;
		}

		if (!FSAPI.UserHasDirectoryPermissions(Player.CurrentSession, TryDirectory, DirectoryPermission.Enter))
		{
			GameConsole.WriteLine($"'{Player.CurrentSession.User.Username}' is not permitted to enter that directory");
			return;
		}

		Player.CurrentSession.PathNode = TryDirectory;
	}
}
