using Jint;
using Jint.Runtime.Interop;

using Haven;

namespace Lawful.GameLibrary;

using static UI.UIManager;

public static class EventManager
{
	private static Jint.Engine JavaScriptEngine;
	private static List<Event> Events;

	public static void Initialize()
	{
		JavaScriptEngine = new(options =>
		{
			options.AddExtensionMethods(typeof(Util));
		});

		JavaScriptEngine.SetValue("BootupConsole", BootupConsole);
		JavaScriptEngine.SetValue("GameConsole", GameConsole);

		JavaScriptEngine.SetValue("ConsoleColor", TypeReference.CreateTypeReference(JavaScriptEngine, typeof(ConsoleColor)));
		JavaScriptEngine.SetValue("Sleep", new Action<int>(Thread.Sleep));

		JavaScriptEngine.SetValue("BWriteDynamic", new Action<string, int>(BootupConsole.WriteDynamic));
		JavaScriptEngine.SetValue("BWriteDynamicColor", new Action<string, int, ConsoleColor>(BootupConsole.WriteDynamicColor));
		JavaScriptEngine.SetValue("BWriteColor", new Action<string, ConsoleColor>(BootupConsole.WriteColor));
		JavaScriptEngine.SetValue("BWriteLineColor", new Action<string, ConsoleColor>(BootupConsole.WriteLineColor));

		JavaScriptEngine.SetValue("GWriteDynamic", new Action<string, int>(GameConsole.WriteDynamic));
		JavaScriptEngine.SetValue("GWriteDynamicColor", new Action<string, int, ConsoleColor>(GameConsole.WriteDynamicColor));
		JavaScriptEngine.SetValue("GWriteColor", new Action<string, ConsoleColor>(GameConsole.WriteColor));
		JavaScriptEngine.SetValue("GWriteLineColor", new Action<string, ConsoleColor>(GameConsole.WriteLineColor));

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

			JavaScriptEngine.Execute(ScriptSource);
		}
	}
}
