//#define VS

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

using Haven;
using Jint;
using Lawful.GameLibrary;
using Lawful.GameLibrary.UI;

namespace Lawful;

public class Lawful
{
	static void Main()
	{

		//	Chat chat = new() { OtherUsername = "Jason" };
		//	
		//	chat.History.Add(new ChatMessage("Jason", "Hey, what's up?"));
		//	chat.History.Add(new ChatMessage("Alex", "Not much, just afkgadkfsjgaskjfhggkhjfasdajghsdaf"));
		//	
		//	NETChatAccount acc = new();
		//	acc.Username = "Alex";
		//	acc.ChatHistory.Add(chat);
		//	
		//	XmlSerializer xs = new(typeof(NETChatAccount));
		//	XmlWriter xw = XmlWriter.Create(File.OpenWrite("netchataccount.xml"), new() { Indent = true, OmitXmlDeclaration = true });
		//	
		//	xs.Serialize(xw, acc);

		Console.ReadKey(true);

#if VS
		string Root = @"C:\Users\Carson\source\repos\Lawful";
#else
		string Root = Directory.GetCurrentDirectory();
#endif

		int ProcessRCElapsed = 0;
		int InitializeEngineElapsed = 0;
		int InitializeUIElapsed = 0;
		int InitializeBASSElapsed = 0;

		//GameSession.SkipBootupSequence = true;

		Directory.SetCurrentDirectory(Root);

		// Process runtimeconfig.xml
		if (!File.Exists("runtimeconfig.xml"))
		{
			Console.Write("Config not found, writing base config... ");
			GameAPI.WriteBaseConfig();
			Console.WriteLine("done.");
		}

		Console.Write("Processing runtimeconfig.xml... ");
		ProcessRCElapsed = Util.ExecTimed(delegate ()
		{
			if (!GameAPI.TryDeserializeConfig(out GameSession.CurrentConfig))
			{
				Console.Write("error reading config, writing base config... ");
				GameAPI.WriteBaseConfig();
			}
		});
		Console.WriteLine("done.");

		// Initialize Haven
		Console.Write("Initializing Haven... ");
		InitializeEngineElapsed = Util.ExecTimed(delegate ()
		{
			switch (GameSession.CurrentConfig.SelectedRenderer)
			{
				case GameLibrary.Renderer.WindowsNative:
					GameSession.App = App.Create<WindowsNativeRenderer>(4);
					GameAPI.CurrentRenderer = GameLibrary.Renderer.WindowsNative;
					break;

				case GameLibrary.Renderer.CrossPlatform:
					GameSession.App = App.Create<ConWriteRenderer>(4);
					GameAPI.CurrentRenderer = GameLibrary.Renderer.CrossPlatform;
					break;
			}
		});
		Console.WriteLine("done.");

		// Initialize UI
		Console.Write("Initializing UI... ");
		InitializeUIElapsed = Util.ExecTimed(UIManager.Initialize);
		Console.WriteLine("done.");

		// Initialize BASS
		Console.Write("Initializing BASS... ");
		InitializeBASSElapsed = Util.ExecTimed(GameAPI.InitializeBASS);
		Console.WriteLine("done.");

		// Create Main AudioManager
		GameSession.MainAudioOut = new();

		if (GameSession.CurrentConfig.EnableTypewriter)
			GameAPI.InitTypewriter();

		GameAPI.InitSFX();

		GameSession.Log.WriteLine($"Lawful :: Process runtimeconfig.xml took {ProcessRCElapsed} ms");
		GameSession.Log.WriteLine($"Lawful :: Initialize Haven took {InitializeEngineElapsed} ms");
		GameSession.Log.WriteLine($"Lawful :: Initialize UI took {InitializeUIElapsed} ms");
		GameSession.Log.WriteLine($"Lawful :: Initialize BASS took {InitializeBASSElapsed} ms");
		
		GameSession.Log.WriteLine("Lawful :: Running Haven now...");

		// Run Engine
		GameSession.App.Run();

		GameSession.MainAudioOut.FreeAll();
	}
}