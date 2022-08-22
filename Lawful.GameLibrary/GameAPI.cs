using Lawful.GameLibrary.UI;
using Lawful.InputParser;
using System.Xml;
using System.Xml.Serialization;

namespace Lawful.GameLibrary;

using static UI.UIManager;

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
		if (GameSession.SkipBootupSequence)
		{
			Current = Sections.Game;
			return;
		}

		Task.Run(delegate ()
		{
			BootupConsole.CursorVisible = false;
			BootupConsole.Clear();

			EventManager.HandleEventsByTrigger(Trigger.BootupSequenceStarted);

			BootupConsole.WriteLineColor("V Systems Company", ConsoleColor.Yellow);
			BootupConsole.WriteLineColor("(C) 2018", ConsoleColor.Yellow);
			BootupConsole.NextLine();

			Thread.Sleep(750);

			BootupConsole.Write("Checking RAM");
			for (int i = 0; i < 20; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLineColor(" 8192 MB OK", ConsoleColor.Green);
			BootupConsole.NextLine();

			Thread.Sleep(500);

			BootupConsole.Write("Building device list... ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 16, 16, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("OK", ConsoleColor.Green);
			Thread.Sleep(250);


			BootupConsole.WriteLineColor("    Found Device!", ConsoleColor.Green);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    ID                : 0x01", ConsoleColor.Yellow);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    Type              : Mechanical", ConsoleColor.Yellow);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    Vendor            : Toshiba", ConsoleColor.Yellow);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    Reported Capacity : 2 TB (2,199,023,255,552 bytes)", ConsoleColor.Yellow);

			Thread.Sleep(500);
			BootupConsole.NextLine();


			BootupConsole.WriteLineColor("    Found Device!", ConsoleColor.Green);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    ID                : 0x02", ConsoleColor.Yellow);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    Type              : SSD", ConsoleColor.Yellow);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    Vendor            : Samsung", ConsoleColor.Yellow);

			Thread.Sleep(50);
			BootupConsole.Write("        ");
			BootupConsole.BeginCharacterAnimation(new char[8] { '|', '/', '-', '\\', '|', '/', '-', '\\' }, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLineColor("    Reported Capacity : 500 GB (536,870,912,000 bytes)", ConsoleColor.Yellow);

			Thread.Sleep(250);
			BootupConsole.WriteLine("Done!");
			BootupConsole.NextLine();

			Thread.Sleep(500);

			BootupConsole.WriteLineColor("Booting from EFI partition on device '0x02'...", ConsoleColor.Yellow);
			Thread.Sleep(1000);

			BootupConsole.Clear();
			BootupConsole.CursorVisible = true;

			Thread.Sleep(1500);

			BootupConsole.NextLine();

			Thread.Sleep(500);

			BootupConsole.CursorVisible = false;
			BootupConsole.Clear();

			BootupConsole.WriteLine("Kennedy Computers Microprocessor Kernel");
			BootupConsole.WriteLine("(C) 2020");
			BootupConsole.NextLine();

			Thread.Sleep(750);

			BootupConsole.WriteLine("Loading modules... ");
			Thread.Sleep(500);

			BootupConsole.WriteColor("  [fs.sys]      Common Filesystem Driver", ConsoleColor.Yellow);
			for (int i = 0; i < 20; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLineColor(" loaded", ConsoleColor.Green);

			BootupConsole.WriteColor("  [netman.sys]  Network Management Driver", ConsoleColor.Yellow);
			for (int i = 0; i < 10; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLineColor(" loaded", ConsoleColor.Green);

			BootupConsole.WriteColor("  [kcon.sys]    Kennedy Console Driver", ConsoleColor.Yellow);
			for (int i = 0; i < 25; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLineColor(" loaded", ConsoleColor.Green);

			BootupConsole.WriteColor("  [session.sys] User-Space Session Handler Driver", ConsoleColor.Yellow);
			for (int i = 0; i < 15; i++)
			{
				BootupConsole.Write('.');
				Thread.Sleep(50);
			}
			BootupConsole.WriteLineColor(" loaded", ConsoleColor.Green);
			BootupConsole.NextLine();

			Thread.Sleep(250);

			BootupConsole.WriteColor("  Module reliability checking... ", ConsoleColor.Yellow);
			Thread.Sleep(500);
			BootupConsole.WriteLineColor(" 0 errors", ConsoleColor.Green);

			Thread.Sleep(250);

			BootupConsole.WriteLine("Module load finished");

			Thread.Sleep(750);

			BootupConsole.NextLine();
			BootupConsole.NextLine();

			BootupConsole.Write("Initiating user session... ");
			Thread.Sleep(500);
			BootupConsole.WriteLineColor("done", ConsoleColor.Green);
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

			BootupConsole.CursorVisible = true;

			EventManager.HandleEventsByTrigger(Trigger.BootupSequenceFinished);

			Current = Sections.Game;
		});

	}

	public static void HandleUserInput(InputQuery Query)
	{

	}
}