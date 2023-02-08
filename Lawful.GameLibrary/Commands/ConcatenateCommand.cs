using System.Xml;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class ConcatenateCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("Concatenate Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.NextLine();
		GameConsole.WriteLine(
			"USAGE: cat [filepath]\n" +
			"\n" +
			"WHERE:\n" +
			"   path -> Path to a file to display on the console");
	}

	public static void Invoke(InputQuery Query)
	{
		if (Query.Arguments.Count == 0)
		{
			GameConsole.WriteLine("Insufficient arguments");
			return;
		}

		if (!FSAPI.TryGetNode(Player.CurrentSession, Query.Arguments[0], FSNodeType.File, out XmlNode File))
		{
			GameConsole.WriteLine($"Could not find file '{Query.Arguments[0]}'");
			return;
		}

		if (!FSAPI.UserHasFilePermissions(Player.CurrentSession, File, FilePermission.Read))
		{
			GameConsole.WriteLine($"'{Player.CurrentSession.User.Username}' is not permitted read that file");
			return;
		}

		if (File.Attributes["Content"] is not null)
		{

		}

		GameConsole.WriteLine(File.InnerText.Trim());

		EventManager.JSE.SetValue("G_Path", File.GetPath());
		EventManager.JSE.SetValue("G_HostIP", Player.CurrentSession.Host.Address);

		EventManager.HandleEventsByTrigger(Trigger.ReadFile);

		// People make fun of JS for wacky shit
		// Meanwhile, I'm here in C# casting null to a type???
		// I HAVE to do this or it will error out!

		// "Ah yes, here is your Delegate-flavored null reference"

		EventManager.JSE.SetValue("G_Path", GameAPI.EmptyDelegate);
		EventManager.JSE.SetValue("G_HostIP", GameAPI.EmptyDelegate);
	}
}
