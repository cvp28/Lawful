using System.Xml;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class MakeDirectoryCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("Directory Creation Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.NextLine();
		GameConsole.WriteLine(
			"USAGE: md [directoryname]\n" +
			"\n" +
			"WHERE:\n" +
			"   directoryname -> Name of directory to create inside the current working directory\n");
	}

	public static void Invoke(InputQuery Query)
	{
		if (Query.Arguments.Count == 0)
		{
			Console.WriteLine("Insufficient arguments");
			return;
		}

		XmlNode WhereToCreate = default;

		Query.NamedArguments.TryGetValue("p", out string NamedArgP);
		Query.NamedArguments.TryGetValue("P", out string NamedArgPC);
		Query.NamedArguments.TryGetValue("path", out string NamedArgPath);
		Query.NamedArguments.TryGetValue("PATH", out string NamedArgPathC);

		if (NamedArgP is not null)
			WhereToCreate = FSAPI.LocateDirectory(Player.CurrentSession, NamedArgP);
		else if (NamedArgPC is not null)
			WhereToCreate = FSAPI.LocateDirectory(Player.CurrentSession, NamedArgPC);
		else if (NamedArgPath is not null)
			WhereToCreate = FSAPI.LocateDirectory(Player.CurrentSession, NamedArgPath);
		else if (NamedArgPathC is not null)
			WhereToCreate = FSAPI.LocateDirectory(Player.CurrentSession, NamedArgPathC);

		if (WhereToCreate is null)
		{
			GameConsole.WriteLine("The specified path is invalid");
			return;
		}

		foreach (string Argument in Query.Arguments)
		{
			bool NameConflict = WhereToCreate.GetNodeFromPath(Argument) != null;

			if (NameConflict)
			{
				GameConsole.WriteLine($"An object '{Argument}' already exists");
				return;
			}

			// Check for MODIFY permissions in the current directory here
			bool HasPermissions = FSAPI.UserHasDirectoryPermissions(Player.CurrentSession, WhereToCreate, DirectoryPermission.Modify);

			if (!HasPermissions)
			{
				GameConsole.WriteLine($"'{Player.CurrentSession.User.Username}' is not permitted to modify that directory");
				return;
			}

			// We're all good at this point, create the directory

			if (Argument.Contains('/'))
			{
				GameConsole.WriteLine("Name cannot contain slashes");
				GameConsole.WriteLine("To create in a directory besides the CWD, specify the p= or path= named argument with the directory you wish to create in");
				return;
			}

			XmlDocument CurrentDocument = Player.CurrentSession.PathNode.OwnerDocument;

			XmlNode NewDirectory = CurrentDocument.CreateElement("Directory");

			XmlAttribute NewDirectoryNameAttribute = CurrentDocument.CreateAttribute("Name");
			NewDirectoryNameAttribute.InnerText = Argument;

			// By default, we give the new directory the following permissions:
			//
			// The user who created it is the owner
			// The root user will have full permissions
			// Any other user will be able to enter and list but not modify this new directory

			XmlAttribute NewDirectoryPermissionsAttribute = CurrentDocument.CreateAttribute("Perms");
			NewDirectoryPermissionsAttribute.InnerText = $"{Player.ProfileName}:elm:el";

			NewDirectory.Attributes.Append(NewDirectoryNameAttribute);
			NewDirectory.Attributes.Append(NewDirectoryPermissionsAttribute);

			WhereToCreate.AppendChild(NewDirectory);

			EventManager.HandleEventsByTrigger(Trigger.CreateDirectory);
		}
	}
}
