using System.Xml;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class RemoveCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("File System Object Deletion Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.WriteLine();
		GameConsole.WriteLine(
			"USAGE: rm [path]\n" +
			"\n" +
			"WHERE:\n" +
			"   path -> Any valid path referencing one or more objects in the file system");
	}

	public static void Invoke(InputQuery Query)
	{
		if (Query.Arguments.Count == 0)
		{
			GameConsole.WriteLine("Insufficient arguments");
			return;
		}

		XmlNode Traverser = Player.CurrentSession.PathNode;

		switch (FSAPI.Locate(Player.CurrentSession, Query.Arguments[0]))
		{
			case XmlNode n:
				if (n.Name == "Root")
				{
					GameConsole.WriteLine("Cannot delete the root directory");
					return;
				}

				while (Traverser.Name != "Root")
				{
					if (Traverser == n)
					{
						GameConsole.WriteLine("Cannot delete a parent of the current working directory");
						return;
					}
					Traverser = Traverser.ParentNode;
				}

				if (!FSAPI.UserHasDirectoryPermissions(Player.CurrentSession, n.ParentNode, DirectoryPermission.Modify))
				{
					GameConsole.WriteLine($"{Player.CurrentSession.User.Username} is not permitted to perform that action on {n.GetPath()}");
					return;
				}

				n.ParentNode.RemoveChild(n);
				FireEvent(n);
				break;

			case XmlNodeList nl:
				XmlNode Parent = nl[0].ParentNode;

				while (Traverser.Name != "Root")
				{
					foreach (XmlNode n in nl)
						if (Traverser == n)
						{
							GameConsole.WriteLine("Cannot delete a parent of the current working directory");
							return;
						}

					Traverser = Traverser.ParentNode;
				}

				for (int i = nl.Count - 1; i >= 0; i--)
				{
					if (!FSAPI.UserHasDirectoryPermissions(Player.CurrentSession, nl[i].ParentNode, DirectoryPermission.Modify))
					{
						GameConsole.WriteLine($"{Player.CurrentSession.User.Username} is not permitted to perform that action on {nl[i].GetPath()}");
						continue;
					}
					Parent.RemoveChild(nl[i]);
					FireEvent(nl[i]);
				}

				break;

			default:
				GameConsole.WriteLine($"Could not resolve query '{Query.Arguments[0]}'");
				break;
		}

		static void FireEvent(XmlNode n)
		{
			if (n.Name == "Directory")
			{
				EventManager.JSE.SetValue("G_Path", n.GetPath());
				EventManager.JSE.SetValue("G_HostIP", Player.CurrentSession.Host.Address);

				EventManager.HandleEventsByTrigger(Trigger.DeleteDirectory);

				EventManager.JSE.SetValue("G_Path", GameAPI.EmptyDelegate);
				EventManager.JSE.SetValue("G_HostIP", GameAPI.EmptyDelegate);
			}

			if (n.Name == "File")
			{
				EventManager.JSE.SetValue("G_Path", n.GetPath());
				EventManager.JSE.SetValue("G_HostIP", Player.CurrentSession.Host.Address);

				EventManager.HandleEventsByTrigger(Trigger.DeleteFile);

				EventManager.JSE.SetValue("G_Path", GameAPI.EmptyDelegate);
				EventManager.JSE.SetValue("G_HostIP", GameAPI.EmptyDelegate);
			}
		}
	}
}
