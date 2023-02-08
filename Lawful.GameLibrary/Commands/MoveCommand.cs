using System.Xml;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class MoveCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("File System Object Mover Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.NextLine();
		GameConsole.WriteLine(
			"USAGE: mv [origin] [destination]\n" +
			"\n" +
			"WHERE:\n" +
			"   origin -> Any valid path resolving to one or more objects in the file system\n" +
			"   destination -> Path to destination. Must be a directory.");
	}

	public static void Invoke(InputQuery Query)
	{
		if (Query.Arguments.Count < 2)
		{
			GameConsole.WriteLine("Insufficient arguments");
			return;
		}

		// Find each query

		if (!FSAPI.TryGetNode(Player.CurrentSession, Query.Arguments[0], out dynamic Origin))
		{
			GameConsole.WriteLine($"Could not resolve origin query '{Query.Arguments[0]}'");
			return;
		}

		if (!FSAPI.TryGetNode(Player.CurrentSession, Query.Arguments[1], FSNodeType.Directory, out XmlNode Destination))
		{
			GameConsole.WriteLine($"Directory '{Query.Arguments[1]}' not found");
			return;
		}

		if (Destination == Origin)
		{
			GameConsole.WriteLine("Origin and Destination cannot be the same");
			return;
		}

		// Both origin and destination need to have MODIFY permissions for the current user
		bool DestinationHasModifyPerms = FSAPI.UserHasDirectoryPermissions(Player.CurrentSession, Destination, DirectoryPermission.Modify);
		if (!DestinationHasModifyPerms)
		{
			GameConsole.WriteLine($"{Player.CurrentSession.User.Username} is not permitted to perform that action on {Destination.GetPath()}");
			return;
		}

		switch (Origin)
		{
			case XmlNode o:
				XmlNode Parent = o.ParentNode;
				if (!FSAPI.UserHasDirectoryPermissions(Player.CurrentSession, Parent, DirectoryPermission.Modify))
				{
					GameConsole.WriteLine($"{Player.CurrentSession.User.Username} is not permitted to perform that action on {o.GetPath()}");
					return;
				}

				Destination.AppendChild(o);
				break;

			case XmlNodeList nl:
				int count = nl.Count;

				for (int i = 0; i < count; i++)
				{
					XmlNode CurrentParent = nl[i].ParentNode;
					if (!FSAPI.UserHasDirectoryPermissions(Player.CurrentSession, CurrentParent, DirectoryPermission.Modify))
					{
						GameConsole.WriteLine($"{Player.CurrentSession.User.Username} is not permitted to perform that action on {nl[i].GetPath()}");
						continue;
					}

					//if (nl[i] == Player.CurrentShell.PathNode) // An origin query involving multiple items may include the current directory so we must check for that
					//    Player.CurrentShell.PathNode = d;

					Destination.AppendChild(nl[0]);
				}
				break;
		}
	}
}
