#define DEV

using Lawful.GameLibrary;
using Lawful.GameLibrary.UI;

using Haven;

Console.ReadKey(true);

#if DEV
	string Root = @"C:\Users\Carson\source\repos\Lawful";
#else
	string Root = Directory.GetCurrentDirectory();
#endif

int ProcessRCElapsed = 0;
int InitializeEngineElapsed = 0;
int InitializeUIElapsed = 0;
int InitializeBASSElapsed = 0;
int InitializeEventManagerElapsed = 0;
int InitializeChatSequenceInterpreterElapsed = 0;

GameSession.SkipBootupSequence = true;

Directory.SetCurrentDirectory(Root);

#region runtimeconfig.xml
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
#endregion

#region Haven Init
// Initialize Haven
Console.Write("Initializing Haven... ");
InitializeEngineElapsed = Util.ExecTimed(delegate ()
{
	switch (GameSession.CurrentConfig.SelectedRenderer)
	{
		case RendererType.WindowsNative:
			GameSession.App = App.Create<WindowsNativeRenderer>(4);
			GameAPI.CurrentRenderer = RendererType.WindowsNative;
			break;

		case RendererType.CrossPlatform:
			GameSession.App = App.Create<ConWriteRenderer>(4);
			GameAPI.CurrentRenderer = RendererType.CrossPlatform;
			break;
	}
});
Console.WriteLine("done.");
#endregion

#region UI Init
// Initialize UI
Console.Write("Initializing UI... ");
InitializeUIElapsed = Util.ExecTimed(UIManager.Initialize);
Console.WriteLine("done.");
#endregion

#region BASS Init
// Initialize BASS
Console.Write("Initializing BASS... ");
InitializeBASSElapsed = Util.ExecTimed(GameAPI.InitBASS);
Console.WriteLine("done.");
#endregion

#region EventManager Init
Console.Write("Initializing EventManager... ");
InitializeEventManagerElapsed = Util.ExecTimed(EventManager.Initialize);
Console.WriteLine("done.");
#endregion

#region ChatSequenceInterpreter Init
Console.Write("Initializing ChatSequenceInterpreter... ");
InitializeChatSequenceInterpreterElapsed = Util.ExecTimed(GameAPI.InitChatSequenceInterpreter);
Console.WriteLine("done.");
#endregion

// Create Main AudioManager
GameSession.MainAudioOut = new();

if (GameSession.CurrentConfig.EnableTypewriter)
	GameAPI.InitTypewriter();

GameAPI.InitSFX();

GameSession.Log.WriteLine($"Lawful :: Process runtimeconfig.xml took {ProcessRCElapsed} ms");
GameSession.Log.WriteLine($"Lawful :: Initialize Haven took {InitializeEngineElapsed} ms");
GameSession.Log.WriteLine($"Lawful :: Initialize UI took {InitializeUIElapsed} ms");
GameSession.Log.WriteLine($"Lawful :: Initialize BASS took {InitializeBASSElapsed} ms");
GameSession.Log.WriteLine($"Lawful :: Initialize EventManager took {InitializeEventManagerElapsed} ms");
GameSession.Log.WriteLine($"Lawful :: Initialize ChatSequenceInterpreter took {InitializeChatSequenceInterpreterElapsed} ms");

GameSession.Log.WriteLine("Lawful :: Running Haven now...");

// Run Engine
GameSession.App.Run();

GameSession.MainAudioOut.FreeAll();