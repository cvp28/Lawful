using System.Xml;

namespace Lawful.GameLibrary;

public static class EventManager
{
	private static List<Event> Events;

	public static void Initialize()
	{
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

	}
}
