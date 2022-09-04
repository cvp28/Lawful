//#define VS

using System.Xml;

using Haven;

using Lawful.GameLibrary;
using Lawful.GameLibrary.UI;

namespace Lawful;



public class Lawful
{
	static void Main()
	{
		Console.ReadKey(true);

#if VS
		string Root = @"C:\Users\Carson\source\repos\Lawful";
#else
		string Root = Directory.GetCurrentDirectory();
#endif

		Directory.SetCurrentDirectory(Root);

		if (File.Exists(@"runtimeconfig.xml"))
			ProcessRuntimeConfig();

		if (OperatingSystem.IsWindows() && !GameOptions.ForceCSRenderer)
			Engine.Initialize<NativeScreen>();
		else
			Engine.Initialize<Screen>();

		// Create UI
		UIManager.Initialize();
		UIManager.Current = Sections.MainMenu;

		Directory.SetCurrentDirectory(@"bin64");
		AudioManager.Initialize();
		Directory.SetCurrentDirectory(Root);

		if (GameOptions.EnableTypewriter)
			GameAPI.InitTypewriter();

		UIManager.Log.WriteLine("Running engine now...");

		// Run Engine
		Engine.Run();

		AudioManager.FreeAll();
	}

	static void ProcessRuntimeConfig()
	{
		XmlDocument RuntimeConfigDoc = new();
		RuntimeConfigDoc.Load(@"runtimeconfig.xml");

		GameOptions.ForceCSRenderer = RuntimeConfigDoc.SelectSingleNode("Config/ForceCSRenderer") is not null;
		GameOptions.ShowFPS = RuntimeConfigDoc.SelectSingleNode("Config/ShowFPS") is not null;
		GameOptions.EnableTypewriter =  RuntimeConfigDoc.SelectSingleNode("Config/EnableTypewriter") is not null;

		string TypewriterVolumeNodeValue = RuntimeConfigDoc.SelectSingleNode("Config/TypewriterVolume")?.Attributes["value"]?.Value.ToLower();

		bool SetDefaultVolume = TypewriterVolumeNodeValue is null || !float.TryParse(TypewriterVolumeNodeValue, out GameOptions.TypewriterVolume);

		if (SetDefaultVolume)
			GameOptions.TypewriterVolume = 0.2f;
	}
}