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

		// This will have to be changed to whatever directory the Content folder and runtimeconfig.xml file are stored in.
		// Line will be removed in production versions of the game.
		Directory.SetCurrentDirectory(@"C:\Users\Carson\source\repos\Lawful");

		if (File.Exists(@".\runtimeconfig.xml"))
			ProcessRuntimeConfig();

		if (OperatingSystem.IsWindows() && !GameOptions.ForceCSRenderer)
			Engine.Initialize<NativeScreen>();
		else
			Engine.Initialize<Screen>();

		// Create UI
		UIManager.Initialize();
		UIManager.Current = Sections.MainMenu;

		UIManager.Log.WriteLine("Running engine now...");

		// Run Engine
		Engine.Run();
	}

	static void ProcessRuntimeConfig()
	{
		XmlDocument RuntimeConfigDoc = new();
		RuntimeConfigDoc.Load(@".\runtimeconfig.xml");

		string ForceCSRendererNodeValue = RuntimeConfigDoc.SelectSingleNode("Config/ForceCSRenderer")?.Attributes["value"]?.Value.ToLower();
		string ShowFPSNodeValue = RuntimeConfigDoc.SelectSingleNode("Config/ShowFPS")?.Attributes["value"]?.Value.ToLower();

		if (ForceCSRendererNodeValue is not null)
			GameOptions.ForceCSRenderer = ForceCSRendererNodeValue == "true";

		if (ShowFPSNodeValue is not null)
			GameOptions.ShowFPS = ShowFPSNodeValue == "true";
	}
}