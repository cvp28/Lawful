using System.Reflection;
using System.Runtime.Loader;

namespace Lawful.GameLibrary;

// Convenient place to store all session-specific data
public static class GameSession
{
	public static User Player;
	public static EventManager Events;
	public static ComputerStructure Computers;

	public static Story CurrentStory;
	public static Mission CurrentMission;

	public static bool SkipBootupSequence;

	public static string CurrentMissionRoot
	{
		get => $@".\Content\Story\{CurrentMission.Name}";
	}
}
