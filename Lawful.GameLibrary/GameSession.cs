using Haven;
using Lawful.GameLibrary.UI;

namespace Lawful.GameLibrary;

// Convenient place to store all session-specific data
public static class GameSession
{
	public static App App;
	public static TextBox Log;
	public static TextBox GameConsole;

	public static AudioManager MainAudioOut;

	public static User Player;
	public static ComputerStructure Computers;

	public static Story CurrentStory;
	public static Mission CurrentMission;

	public static bool SkipBootupSequence;

	// Used to temporarily disable the typewriter
	public static bool DoTypewriter;

	public static string CurrentStoryRoot
	{
		get => $@"Content\Stories\{CurrentStory.Name}".ToPlatformPath();
	}

	public static Config CurrentConfig;
}
