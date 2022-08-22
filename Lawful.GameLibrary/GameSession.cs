using System.Reflection;
using System.Runtime.Loader;

namespace Lawful.GameLibrary;

// Convenient place to store all session-specific data
public static class GameSession
{
	public static User Player;
	public static ComputerStructure Computers;

	public static Story CurrentStory;
	public static Mission CurrentMission;

	public static bool SkipBootupSequence;

	public static string CurrentStoryRoot
	{
		get => $@".\Content\Stories\{CurrentStory.Name}";
	}
}
