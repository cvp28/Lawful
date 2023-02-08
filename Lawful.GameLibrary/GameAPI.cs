using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics.CodeAnalysis;

using Haven;
using Un4seen.Bass;

using Lawful.InputParser;

namespace Lawful.GameLibrary;

using static GameSession;
using UI;

public static class GameAPI
{
	private static XmlSerializer ConfigSerializer = new(typeof(Config));

	private static List<string> NormalTypewriterStreams;
	private static Random Rand;
	public static bool TypewriterInitialized { get; private set; } = false;
	public static Renderer CurrentRenderer { get; set; }

	public static Delegate EmptyDelegate = delegate () { };

	// Base game configuration that gets auto-generated at runtime
	public static Config BaseConfig = new()
	{
		ShowFPS = false,
		SelectedRenderer = OperatingSystem.IsWindows() ? Renderer.WindowsNative : Renderer.CrossPlatform,
		EnableTypewriter = true,
		TypewriterVolume = 0.2f
	};

	public static string ToPlatformPath(this string Path)
	{
		char PathSeparator = OperatingSystem.IsWindows() ? '\\' : '/';

		Span<char> PathSpan = stackalloc char[Path.Length];

		for (int i = 0; i < PathSpan.Length; i++)
			if (Path[i] == '\\' || Path[i] == '/')
				PathSpan[i] = PathSeparator;
			else
				PathSpan[i] = Path[i];

		return PathSpan.ToString();
	}

	public static string[] GetInstalledStorylines()
	{
		XmlSerializer sStory = new(typeof(Story));
		XmlSerializer sCompStruct = new(typeof(ComputerStructure));
		List<string> ValidStories = new();

		foreach (string StoryDir in Directory.GetDirectories(@"Content\Stories".ToPlatformPath()))
		{
			bool HasComputers = File.Exists(@$"{StoryDir}\Computers.xml".ToPlatformPath());
			bool HasStory = File.Exists(@$"{StoryDir}\Story.xml".ToPlatformPath());

			if (!HasComputers || !HasStory)
				continue;

			using (XmlReader ComputersReader = XmlReader.Create($@"{StoryDir}\Computers.xml".ToPlatformPath()))
				if (!sCompStruct.CanDeserialize(ComputersReader))
					continue;

			using (XmlReader StoryReader = XmlReader.Create($@"{StoryDir}\Story.xml".ToPlatformPath()))
				if (!sStory.CanDeserialize(StoryReader))
					continue;

			ValidStories.Add(StoryDir.Split('\\', '/').Last());
		}

		return ValidStories.ToArray();
	}

	public static (string Name, string Path)[] GetSaves()
	{
		XmlSerializer sUser = new(typeof(User));
		XmlSerializer sCompStruct = new(typeof(ComputerStructure));
		List<(string Name, string Path)> ValidSaves = new();

		foreach (string SaveDir in Directory.GetDirectories(@"Content\Saves".ToPlatformPath()))
		{
			bool HasComputers = File.Exists($@"{SaveDir}\Computers.xml".ToPlatformPath());
			bool HasUser = File.Exists($@"{SaveDir}\User.xml".ToPlatformPath());

			if (!HasComputers || !HasUser)
				continue;

			using (XmlReader ComputersReader = XmlReader.Create($@"{SaveDir}\Computers.xml".ToPlatformPath()))
				if (!sCompStruct.CanDeserialize(ComputersReader))
					continue;

			using (XmlReader UserReader = XmlReader.Create($@"{SaveDir}\User.xml".ToPlatformPath()))
				if (!sUser.CanDeserialize(UserReader))
					continue;

			string Name = SaveDir.Split('\\', '/').Last();
			string Path = $"{SaveDir}\\User.xml".ToPlatformPath();

			ValidSaves.Add((Name, Path));
		}

		return ValidSaves.ToArray();
	}

	public static void InitializeBASS()
	{
		// Initialize BASS
		bool BassInit = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

		if (!BassInit)
		{
			Log.WriteLine($"failed: {Bass.BASS_ErrorGetCode()}");
			throw new Exception("Failed to initialize BASS");
		}
	}

	public static void InitSFX()
	{
		MainAudioOut.CreateStream("FriendRequestNotify", @"Content\Audio\SFX\FriendRequestNotify.wav".ToPlatformPath());
		MainAudioOut.CreateStream("UserOnlineNotify", @"Content\Audio\SFX\UserOnlineNotify.wav".ToPlatformPath());
		MainAudioOut.CreateStream("UserOfflineNotify", @"Content\Audio\SFX\UserOfflineNotify.wav".ToPlatformPath());
	}

	public static void InitTypewriter()
	{
		NormalTypewriterStreams = new();
		Rand = new(DateTime.Now.Millisecond);

		NormalTypewriterStreams.Add("TypewriterN1");
		NormalTypewriterStreams.Add("TypewriterN2");
		NormalTypewriterStreams.Add("TypewriterN3");
		NormalTypewriterStreams.Add("TypewriterN4");
		NormalTypewriterStreams.Add("TypewriterN5");

		MainAudioOut.CreateStream("TypewriterN1", @"Content\Audio\Typewriter\N1.wav".ToPlatformPath());
		MainAudioOut.CreateStream("TypewriterN2", @"Content\Audio\Typewriter\N2.wav".ToPlatformPath());
		MainAudioOut.CreateStream("TypewriterN3", @"Content\Audio\Typewriter\N3.wav".ToPlatformPath());
		MainAudioOut.CreateStream("TypewriterN4", @"Content\Audio\Typewriter\N4.wav".ToPlatformPath());
		MainAudioOut.CreateStream("TypewriterN5", @"Content\Audio\Typewriter\N5.wav".ToPlatformPath());

		MainAudioOut.CreateStream("TypewriterEnter", @"Content\Audio\Typewriter\Enter.wav".ToPlatformPath());
		MainAudioOut.CreateStream("TypewriterSpacebar", @"Content\Audio\Typewriter\Spacebar.wav".ToPlatformPath());

		UpdateTypewriterVolume();

		DoTypewriter = true;
		App.AddUpdateTask("Typewriter", TypewriterTask);

		TypewriterInitialized = true;
	}

	public static void UpdateTypewriterVolume()
	{
		foreach (string Stream in NormalTypewriterStreams)
			MainAudioOut.SetVolume(Stream, CurrentConfig.TypewriterVolume);

		MainAudioOut.SetVolume("TypewriterEnter", CurrentConfig.TypewriterVolume);
		MainAudioOut.SetVolume("TypewriterSpacebar", CurrentConfig.TypewriterVolume);
	}

	public static void TypewriterTask(State s)
	{
		if (!s.KeyPressed || !DoTypewriter)
			return;

		switch (s.KeyInfo.Key)
		{
			case ConsoleKey.Spacebar:
				MainAudioOut.Play("TypewriterSpacebar", true);
				break;

			case ConsoleKey.Enter:
				MainAudioOut.Play("TypewriterEnter", true);
				break;

			default:
				string Stream = NormalTypewriterStreams[Rand.Next(NormalTypewriterStreams.Count)];
				MainAudioOut.Play(Stream, true);
				break;
		}
	}

	public static void WriteBaseConfig()
	{
		using (var stream = XmlWriter.Create("runtimeconfig.xml", new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
			ConfigSerializer.Serialize(stream, BaseConfig);
	}

	public static void WriteCurrentConfig()
	{
		using (var stream = XmlWriter.Create("runtimeconfig.xml", new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
			ConfigSerializer.Serialize(stream, CurrentConfig);
	}

	public static bool TryDeserializeConfig(out Config cfg)
	{
		using (var stream = File.OpenRead("runtimeconfig.xml"))
		{
			cfg = ConfigSerializer.Deserialize(stream) as Config;

			//	try
			//	{
			//	}
			//	catch (Exception)
			//	{
			//		goto fail;
			//	}

			if (cfg.TypewriterVolume < 0.0 || cfg.TypewriterVolume >= 1.0)
				goto fail;
		}

		return true;

	fail:
		cfg = BaseConfig;
		return false;
	}

	public static void CommenceBootupTask()
	{
		if (SkipBootupSequence)
		{
			App.GetLayer<NotifyLayer>().StartNotifyThread();

			App.SetLayer(0, "Game", true);
			App.SetLayer(1, "Notify");
			return;
		}

		var BootupConsole = App.GetLayer<BootupLayer>().BootupConsole;

		Task.Run(delegate ()
		{
			DoTypewriter = false;
			BootupConsole.CursorVisible = false;
			BootupConsole.Clear();

			EventManager.HandleEventsByTrigger(Trigger.BootupSequenceStarted);

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
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 16, 16, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("OK", ConsoleColor.Green, ConsoleColor.Black);
			Thread.Sleep(250);


			BootupConsole.WriteLine("    Found Device!", ConsoleColor.Green, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("ID                : 0x01", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Type              : Mechanical", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Vendor            : Toshiba", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Reported Capacity : 2 TB (2,199,023,255,552 bytes)", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(500);
			BootupConsole.NextLine();


			BootupConsole.WriteLine("    Found Device!", ConsoleColor.Green, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("ID                : 0x02", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Type              : SSD", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
			BootupConsole.WriteLine("Vendor            : Samsung", ConsoleColor.Yellow, ConsoleColor.Black);

			Thread.Sleep(50);
			BootupConsole.Write("                ");
			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 6, 6, 50, BootupConsole.GetCursorPosition());
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

			BootupConsole.BeginCharacterAnimation(Util.LoadingAnimFrames, 16, 48, 50, BootupConsole.GetCursorPosition());

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
			DoTypewriter = true;

			App.GetLayer<NotifyLayer>().StartNotifyThread();

			App.SetLayer(0, "Game", true);
			App.SetLayer(1, "Notify");
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

		// Built-in commands
		switch (Query.Command.ToUpper())
		{
			case "EXIT":
				App.GetLayer<NotifyLayer>().StopNotifyThread();

				App.SetLayer(1);
				App.SetLayer(0, "MainMenu");
				
				MissionAPI.UnloadCurrentMission();
				SaveAPI.UnloadCurrentSave();

				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

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

			case "DC":
				if (Player.CurrentSession.Host == Player.HomePC)
					return;

				Player.HomePC.TryOpenSession(Player.ProfileName, out Player.CurrentSession);
				EventManager.HandleEventsByTrigger(Trigger.SSHDisconnect);
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

		XmlNode TryExecuteBin = Player.CurrentSession.Host.GetNodeFromPath($"bin/{Query.Command}");
		XmlNode TryExecuteLocal = FSAPI.LocateFile(Player.CurrentSession, Query.Command);
		XmlNode TryExecute;

		if (TryExecuteBin is not null)
		{
			TryExecute = TryExecuteBin;
			goto ExecuteCommand;
		}

		if (TryExecuteLocal is not null)
		{
			TryExecute = TryExecuteLocal;
			goto ExecuteCommand;
		}

		GameConsole.WriteLine($"File not found '/bin/{Query.Command}' and './{Query.Command}'");
		return;

	ExecuteCommand:

		if (!FSAPI.UserHasFilePermissions(Player.CurrentSession, TryExecute, FilePermission.Execute))
		{
			GameConsole.WriteLine($"Execute permission denied for '{TryExecute.GetPath()}'");
			return;
		}

		if (TryExecute.Attributes["Command"] is null)
		{
			GameConsole.WriteLine($"File is not an executable '{TryExecute.GetPath()}'");
			return;
		}

		string Command = TryExecute.Attributes["Command"].Value.ToUpper();

		switch (Command.ToUpper())
		{
			case "SUS":
				if (Query.Flags.Contains("help"))
					SusCommand.Help();
				else
					SusCommand.Invoke(Query);
				break;

			case "LS":
				if (Query.Flags.Contains("help"))
					ListCommand.Help();
				else
					ListCommand.Invoke(Query);
				break;

			case "CD":
				if (Query.Flags.Contains("help"))
					ChangeDirectoryCommand.Help();
				else
					ChangeDirectoryCommand.Invoke(Query);
				break;

			case "CAT":
				if (Query.Flags.Contains("help"))
					ConcatenateCommand.Help();
				else
					ConcatenateCommand.Invoke(Query);
				break;

			case "MKDIR":
				if (Query.Flags.Contains("help"))
					MakeDirectoryCommand.Help();
				else
					MakeDirectoryCommand.Invoke(Query);
				break;

			case "RM":
				if (Query.Flags.Contains("help"))
					RemoveCommand.Help();
				else
					RemoveCommand.Invoke(Query);
				break;

			case "SU":
				if (Query.Flags.Contains("help"))
					SwitchUserCommand.Help();
				else
					SwitchUserCommand.Invoke(Query);
				break;

			case "SSH":
				if (Query.Flags.Contains("help"))
					SSHCommand.Help();
				else
					SSHCommand.Invoke(Query);
				break;

			case "MV":
				if (Query.Flags.Contains("help"))
					MoveCommand.Help();
				else
					MoveCommand.Invoke(Query);
				break;
		}
	}
}