using Jint;
using Jint.Runtime.Interop;

using Haven;
using Lawful.GameLibrary.UI;

namespace Lawful.GameLibrary;

using static UI.UIManager;

public static class EventManager
{
	private static Jint.Engine JSE;
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
		JSE.SetValue("ReadKey", new Func<bool, ConsoleKeyInfo>(ReadKey.ReadKey));
		JSE.SetValue("ConsoleColor", TypeReference.CreateTypeReference(JSE, typeof(ConsoleColor)));
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

			JSE.Execute(ScriptSource);
		}
	}
}
