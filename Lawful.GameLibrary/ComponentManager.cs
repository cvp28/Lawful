
namespace Lawful.GameLibrary;

public static class ComponentManager
{
	public static List<IComponent> Components;

	public static void Initialize()
	{
		Components = new();
	}

	public static bool RegisterComponent(IComponent Component)
	{
		if (Components.Any(cmp => cmp.ID == Component.ID))
		{
			Util.WriteLineColor($"[ComponentManager] Failed to register component '{Component.Name}' with ID '{Component.ID}'", ConsoleColor.Red);
			Util.WriteLineColor($"                   Internal component list already contains a component with that ID", ConsoleColor.Red);
			return false;
		}

		Components.Add(Component);
		return true;
	}

	public static void UnregisterComponent(string ID) => Components.RemoveAll(cmp => cmp.ID == ID);

	public static bool GetCommands(out List<ICommand> Commands)
	{
		Commands = new();

		IEnumerable<IComponent> Temp = Components.Where(cmp => cmp.Type == ComponentType.Command);
		
		foreach (var Command in Temp)
			Commands.Add(Command as ICommand);

		if (Commands.Count > 0)
			return true;
		else
			return false;
	}
}
