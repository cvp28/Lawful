using System.Xml;
using Haven;

using Lawful.UI;

namespace Lawful;

class Lawful
{
	static Label FPSLabel;
	static bool ForceCSRenderer = false;
	static bool ShowFPS = false;

	static void Main()
	{
		Console.ReadKey(true);

		Directory.SetCurrentDirectory(@"C:\Users\Carson\source\repos\Lawful");

		if (File.Exists(@".\runtimeconfig.xml"))
			ProcessRuntimeConfig();

		if (OperatingSystem.IsWindows() && !ForceCSRenderer)
			Engine.Initialize<NativeScreen>();
		else
			Engine.Initialize<Screen>();

		// Create UI
		UIManager.Initialize();
		UIManager.Current = Sections.MainMenu;

		FPSLabel = new(0, Dimensions.Current.WindowHeight - 1);

		// Add UI to Engine
		Engine.AddWidgets(Sections.MainMenu);
		Engine.AddWidgets(Sections.NewGame);
		Engine.AddWidgets(Sections.LoadGame);
		Engine.AddWidgets(Sections.Options);
		Engine.AddWidgets(Sections.Credits);
		Engine.AddWidgets(Sections.Game);

		if (ShowFPS)
		{
			Engine.AddWidget(FPSLabel);

			Engine.AddUpdateTask("UpdateFPS", delegate (State s) { FPSLabel.Text = $"{s.FPS,-4} FPS :: {s.LastFrameTime,-1} ms"; });
		}

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
			ForceCSRenderer = ForceCSRendererNodeValue == "true";

		if (ShowFPSNodeValue is not null)
			ShowFPS = ShowFPSNodeValue == "true";
	}
}