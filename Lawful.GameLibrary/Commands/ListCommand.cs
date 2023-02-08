using System.Xml;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class ListCommand
{
	public static void Help()
	{
		GameConsole.WriteLine("File System List Utility v1.0", ConsoleColor.Yellow, ConsoleColor.Black);
		GameConsole.NextLine();
		GameConsole.WriteLine(
			"USAGE: ls [objectname]\n" +
			"\n" +
			"WHERE:\n" +
			"   objectname -> Optional. Path to directory or file.\n" +
			"                 If directory, it will list its contents\n" +
			"                 If file, it will give details about the file");
	}

	public static void Invoke(InputQuery Query)
	{
		if (Query.Flags.Count != 0)
		{
			if (Query.Flags[0].ToUpper() == "HELP")
			{
				Help();
				return;
			}
		}

		XmlNode NodeToList = Player.CurrentSession.PathNode;

		if (Query.Arguments.Count == 0)
		{
			ListDirectory(NodeToList);
			return;
		}

		NodeToList = FSAPI.Locate(Player.CurrentSession, Query.Arguments[0]);

		switch (NodeToList)
		{
			case XmlNode n:
				if (n.Name == "File")
					ListFileDetails(n);
				else
					ListDirectory(n);
				break;

			default:
				GameConsole.WriteLine($"Could not resolve query '{Query.Arguments[0]}'");
				break;
		}
	}

	private static void ListDirectory(XmlNode Node)
	{
		XmlNodeList Folders = Node.SelectNodes("Directory");
		XmlNodeList Files = Node.SelectNodes("File");

		if (Folders != null)
			foreach (XmlNode Folder in Folders)
				GameConsole.WriteLine(Folder.Attributes["Name"].Value, ConsoleColor.Yellow, ConsoleColor.Black);

		if (Files != null)
			foreach (XmlNode File in Files)
				if (File.Attributes["Command"] is not null)
					GameConsole.WriteLine(File.Attributes["Name"].Value, ConsoleColor.Green, ConsoleColor.Black);
				else
					GameConsole.WriteLine(File.Attributes["Name"].Value);
	}

	private static void ListFileDetails(XmlNode Node)
	{
		if (Node.Attributes["Command"] is not null)
		{
			GameConsole.WriteLine("ELF x86-64 binary (stripped, no symbols)");
			return;
		}

		int Length = Node.InnerText.Trim().Length;
		GameConsole.Write("Length".PadRight(10));
		GameConsole.WriteLine(Length.ToString() + (Length == 1 ? " character" : " characters"), ConsoleColor.Yellow, ConsoleColor.Black);
	}
}
