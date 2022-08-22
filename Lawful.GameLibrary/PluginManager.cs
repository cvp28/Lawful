using System.Runtime.Loader;

namespace Lawful.GameLibrary;

public static class PluginManager
{
	public static Dictionary<string, AssemblyLoadContext> LoadedPlugins;

	public static void Initialize()
	{
		LoadedPlugins = new();
	}

	public static void LoadPlugin(string FullFilename)
	{
		if (!File.Exists(FullFilename))
		{
			Console.WriteLine(@$"[PluginManager] Could not find plugin '{FullFilename}'");
			return;
		}

		AssemblyLoadContext TempContext = new(null, true);

		try
		{
			TempContext.LoadFromAssemblyPath(Path.GetFullPath(FullFilename));
		}
		catch (Exception ex)
		{
			Environment.ExitCode = -1;
			Console.WriteLine($"[PluginManager] Exception while loading plugin assembly '{FullFilename}'");
			Console.WriteLine(ex.Message);
			Console.ReadKey(true);
			Environment.Exit(Environment.ExitCode);
		}

		// Get any type that implements IPlugin
		Type ImportedPluginType = TempContext.Assemblies.First()
			.GetTypes()
			.FirstOrDefault(type => type.IsAssignableTo(typeof(IPlugin)));

		if (ImportedPluginType is null)
		{
			Console.WriteLine($"[PluginManager] Error instantiating plugin from assembly '{FullFilename}'");
			Console.WriteLine( "                No types found that implement IPlugin");
			return;
		}

		// Create an instance of it
		IPlugin Plugin = Activator.CreateInstance(ImportedPluginType) as IPlugin;

		Console.WriteLine($"[PluginManager] Loading plugin '{Plugin.ID}'...");

		// Register each of the plugin's components with the ComponentManager
		foreach (IComponent Component in Plugin.GetComponents())
		{
			//if (ComponentManager.RegisterComponent(Component))
				//Util.WriteLineColor($"[PluginManager] Exported and registered {Component.Type} '{Component.Name}' with ID '{Component.ID}'", ConsoleColor.Yellow);
		}

		LoadedPlugins.Add(Plugin.ID, TempContext);
		//Util.WriteLineColor($"[PluginManager] finished.", ConsoleColor.Green);
	}
}
