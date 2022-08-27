using System.Xml;
using System.Xml.Serialization;

using Lawful.InputParser;
using Lawful.GameLibrary.UI;

namespace Lawful.GameLibrary;

using static UI.UIManager;
using static GameSession;

public static class GameAPI
{
	public static string[] GetInstalledStorylines()
	{
		XmlSerializer sStory = new(typeof(Story));
		XmlSerializer sCompStruct = new(typeof(ComputerStructure));
		List<string> ValidStories = new();
		
		foreach (string StoryDir in Directory.GetDirectories(@".\Content\Stories"))
		{
			bool HasComputers = File.Exists(@$"{StoryDir}\Computers.xml");
			bool HasStory = File.Exists(@$"{StoryDir}\Story.xml");

			if (!HasComputers || !HasStory)
				continue;

			using (XmlReader ComputersReader = XmlReader.Create(@$"{StoryDir}\Computers.xml"))
				if (!sCompStruct.CanDeserialize(ComputersReader))
					continue;

			using (XmlReader StoryReader = XmlReader.Create($@"{StoryDir}\Story.xml"))
				if (!sStory.CanDeserialize(StoryReader))
					continue;

			ValidStories.Add(StoryDir.Split('\\').Last());
		}

		return ValidStories.ToArray();
	}

	public static (string Name, string Path)[] GetSaves()
	{
		XmlSerializer sUser = new(typeof(User));
		XmlSerializer sCompStruct = new(typeof(ComputerStructure));
		List<(string Name, string Path)> ValidSaves = new();

		foreach (string SaveDir in Directory.GetDirectories(@".\Content\Saves"))
		{
			bool HasComputers = File.Exists($@"{SaveDir}\Computers.xml");
			bool HasUser = File.Exists($@"{SaveDir}\User.xml");

			if (!HasComputers || !HasUser)
				continue;

			using (XmlReader ComputersReader = XmlReader.Create($@"{SaveDir}\Computers.xml"))
				if (!sCompStruct.CanDeserialize(ComputersReader))
					continue;

			using (XmlReader UserReader = XmlReader.Create($@"{SaveDir}\User.xml"))
				if (!sUser.CanDeserialize(UserReader))
					continue;

			string Name = SaveDir.Split('\\').Last();
			string Path = $"{SaveDir}\\User.xml";

			ValidSaves.Add((Name, Path));
		}

		return ValidSaves.ToArray();
	}

	public static void CommenceBootupTask()
	{
		if (SkipBootupSequence)
		{
			Current = Sections.Game;
			return;
		}

		Task.Run(delegate ()
		{
			BootupConsole.CursorVisible = false;
			BootupConsole.Clear();

			//EventManager.HandleEventsByTrigger(Trigger.BootupSequenceStarted);

			BootupConsole.WriteLine("V Systems Company", ConsoleColor.Yellow, ConsoleColor.Black);
			BootupConsole.WriteLine("(C) 2018", ConsoleColor.Yellow, ConsoleColor.Black);
			BootupConsole.NextLine();

			Thread.Sleep(750);

			BootupConsole.Write("Checking RAM");
			for (int i = 0; i < 20; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLine(" 8192 MB OK", ConsoleColor.Green, ConsoleColor.Black);
			BootupConsole.NextLine();

			Thread.Sleep(500);

			BootupConsole.Write("Building device list... ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 16, 16, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("OK", ConsoleColor.Green, ConsoleColor.Black);
			Thread.Sleep(250);


			BootupConsole.WriteLine("    Found Device!", ConsoleColor.Green, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("ID                : 0x01", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Type              : Mechanical", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Vendor            : Toshiba", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Reported Capacity : 2 TB (2,199,023,255,552 bytes)", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(500);
			BootupConsole.NextLine();


			BootupConsole.WriteLine("    Found Device!", ConsoleColor.Green, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("ID                : 0x02", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Type              : SSD", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Vendor            : Samsung", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Reported Capacity : 500 GB (536,870,912,000 bytes)", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(250);
			BootupConsole.WriteLine("Done!");
			BootupConsole.NextLine();

			Thread.Sleep(500);

			BootupConsole.WriteLine("Booting from EFI partition on device '0x02'...", ConsoleColor.Yellow, ConsoleColor.Black);
			Thread.Sleep(1000);

			BootupConsole.Clear();
			BootupConsole.CursorVisible = true;

			Thread.Sleep(1500);

			BootupConsole.NextLine();

			Thread.Sleep(500);

			BootupConsole.CursorVisible = false;
			BootupConsole.Clear();

			BootupConsole.WriteLine("Kennedy Computers Microprocessor Kernel");
			BootupConsole.WriteLine("(C) 2018");
			BootupConsole.NextLine();

			Thread.Sleep(750);

			BootupConsole.WriteLine("Loading modules... ");
			Thread.Sleep(500);

			BootupConsole.Write("  [fs.sys]      Common Filesystem Driver", ConsoleColor.Yellow, ConsoleColor.Black);
			for (int i = 0; i < 20; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLine(" loaded", ConsoleColor.Green, ConsoleColor.Black);

			BootupConsole.Write("  [netman.sys]  Network Management Driver", ConsoleColor.Yellow, ConsoleColor.Black);
			for (int i = 0; i < 10; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLine(" loaded", ConsoleColor.Green, ConsoleColor.Black);

			BootupConsole.Write("  [kcon.sys]    Kennedy Console Driver", ConsoleColor.Yellow, ConsoleColor.Black);
			for (int i = 0; i < 25; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLine(" loaded", ConsoleColor.Green, ConsoleColor.Black);

			BootupConsole.Write("  [session.sys] User-Space Session Handler Driver", ConsoleColor.Yellow, ConsoleColor.Black);
			for (int i = 0; i < 15; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLine(" loaded", ConsoleColor.Green, ConsoleColor.Black);
			BootupConsole.NextLine();

			Thread.Sleep(250);

			BootupConsole.Write("  Module reliability checking... ", ConsoleColor.Yellow, ConsoleColor.Black);
			Thread.Sleep(500);
			BootupConsole.WriteLine(" 0 errors", ConsoleColor.Green, ConsoleColor.Black);

			Thread.Sleep(250);

			BootupConsole.WriteLine("Module load finished");

			Thread.Sleep(750);

			BootupConsole.NextLine();
			BootupConsole.NextLine();

			BootupConsole.Write("Initiating user session... ");
			Thread.Sleep(500);
			BootupConsole.WriteLine("done", ConsoleColor.Green, ConsoleColor.Black);
			BootupConsole.NextLine();

			Thread.Sleep(250);

			BootupConsole.NextLine();

			Thread.Sleep(500);

			BootupConsole.WriteLine("Welcome.");
			BootupConsole.NextLine();

			Thread.Sleep(1500);

			BootupConsole.Write("[kcon]::AllocateConsole : Allocating console... ");

			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 16, 48, 50, BootupConsole.GetCursorPosition());

			BootupConsole.CursorVisible = false;

			Random random = new();
			int num = random.Next(10000000, 99999999);
			string HexString = num.ToString("X");

			BootupConsole.WriteLine("done.");
			BootupConsole.WriteLine($"kcon handle: 0x{HexString}");

			BootupConsole.NextLine();

			Thread.Sleep(500);

			EventManager.HandleEventsByTrigger(Trigger.BootupSequenceFinished);

			BootupConsole.CursorVisible = true;

			Current = Sections.Game;
		});

	}

	/// <summary>
	/// Intended to be executed in seperate thread from caller
	/// </summary>
	/// <param name="Query">The user query to handle</param>
	public static void HandleUserInput(InputQuery Query)
	{
		if (Query.Command.Length == 0)
			return;

		switch (Query.Command.ToUpper())
		{
			case "EXIT":
				Current = Sections.MainMenu;
				return;

			case "QUERY":
				if (Query.Arguments.Count == 0)
					return;

				GameConsole.WriteLine($"RSI Status : {Remote.TryGetRSI(Query.Arguments[0], out UserAccount User, out Computer Host, out string Path)}");
				GameConsole.WriteLine($"Username   : {(User is not null ? User.Username : "null")}");
				GameConsole.WriteLine($"Host       : {(Host is not null ? Host.Name : "null")}");
				GameConsole.WriteLine($"             {(Host is not null ? Host.Address : "null")}");
				GameConsole.WriteLine($"Path       : {Path}");

				return;

			case "CURRENTMISSION":
				GameConsole.WriteLine($"'{CurrentMission.Name}'");

				foreach (Event e in CurrentMission.Events)
					GameConsole.WriteLine($"  Event: On '{e.Trigger}' -> {e.ScriptPath}");

				return;

			case "CLEAR":
				GameConsole.Clear();
				return;

			case "UITEST":
				GameConsole.Write("Running MT UI test... ");

				GameConsole.WriteCharInPlace('1');
				Computers.Computers[0].TryOpenSession("jason", out Player.CurrentSession);
				Thread.Sleep(1000);

				GameConsole.WriteCharInPlace('2');
				Player.CurrentSession.PathNode = Player.CurrentSession.Host.GetNodeFromPath("/home/jason");
				Thread.Sleep(1000);

				GameConsole.WriteCharInPlace('3');
				Computers.Computers[1].TryOpenSession("Konym", out Player.CurrentSession);
				Thread.Sleep(1000);

				GameConsole.WriteCharInPlace('4');
				Player.CurrentSession.PathNode = Player.CurrentSession.Host.GetNodeFromPath("/home");
				Thread.Sleep(1000);

				GameConsole.WriteCharInPlace('5');
				Computers.Computers[0].TryOpenSession("jason", out Player.CurrentSession);
				Thread.Sleep(1000);

				GameConsole.WriteCharInPlace('6');
				Player.CurrentSession.PathNode = Player.CurrentSession.Host.GetNodeFromPath("/home/jason");
				Thread.Sleep(1000);

				GameConsole.WriteCharInPlace('7');
				Computers.Computers[1].TryOpenSession("Konym", out Player.CurrentSession);
				Thread.Sleep(1000);

				GameConsole.WriteCharInPlace('8');
				Player.CurrentSession.PathNode = Player.CurrentSession.Host.GetNodeFromPath("/home/Konym");

				GameConsole.Write("done.");
				GameConsole.NextLine();
				return;
		}

		return;
	}
}