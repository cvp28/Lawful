using Jint;

namespace Lawful.GameLibrary;

using static UI.UIManager;

public static class EventManager
{
	private static Engine JSE;
	private static List<Event> Events;

	public static void Initialize()
	{
		JSE = new(options =>
		{
			options.AddExtensionMethods(typeof(Util));
		});

		JSE.SetValue("BootupConsole", BootupConsole);
		JSE.SetValue("GameConsole", GameConsole);

		JSE.SetValue("KeyAvailable", ReadKey.KeyAvailable);
		JSE.SetValue("ReadKey", ReadKey.ReadKey);
		JSE.SetValue("ConsoleColor", typeof(ConsoleColor));
		JSE.SetValue("Sleep", new Action<int>(Thread.Sleep));

		Events = new();
	}

	public static void AddMissionEvents(Mission m)
	{
		foreach (Event e in m.Events)
			Events.Add(e);
	}

	public static void ClearEvents() => Events.Clear();

	public static void HandleEventsByTrigger(Trigger t)
	{
		Event[] TriggerEvents = Events.Where(e => e.Trigger == t).ToArray();

		foreach (Event e in TriggerEvents)
		{
			string ScriptSource = e.ScriptSource;

			if (ScriptSource.Length == 0)
				continue;

			Log.WriteLine($"EventManager :: Executing '{e.ScriptPath}' for '{t}'...");

			try
			{
				JSE.Execute(ScriptSource);
			}
			catch (Exception ex)
			{
				Log.WriteLine($"EventManager :: Script exception for '{e.ScriptPath}'");
				Log.WriteLine($"   {ex.Message}", ConsoleColor.Red, ConsoleColor.Black);
				Log.WriteLine($"{ex.StackTrace}");
			}
		}
	}
}
