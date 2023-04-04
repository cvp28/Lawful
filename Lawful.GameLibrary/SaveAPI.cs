using System.Diagnostics.CodeAnalysis;

namespace Lawful.GameLibrary;

using static GameSession;

public static class SaveAPI
{
	// We return a bool and a string containing:
	// a. if the save initialization was successful
	// and
	// b. the path to the user's save file
	public static (bool, string) InitSave(string UserPCName, string UserProfileName, string UserStorySelection)
	{
		Story UserStory = Story.DeserializeFromFile($@"Content\Stories\{UserStorySelection}\Story.xml".ToPlatformPath());

		User User = new()
		{
			ProfileName = UserProfileName,
			StoryID = UserStorySelection,
			CurrentMissionID = UserStory.StartMissionID,
			NETChatAccount = UserStory.DefaultNETChatAccount
		};

		// Define our path to the new user's save folder
		string PathToNewSaveFolder = Path.Combine(Directory.GetCurrentDirectory() + @$"\Content\Saves\{UserProfileName}".ToPlatformPath());

		// Create our new save folder if it does not exist already
		if (!Directory.Exists(PathToNewSaveFolder))
			Directory.CreateDirectory(PathToNewSaveFolder);
		else
			return (false, null);

		try
		{
			// Load in the default ComputerStructure from our selected storyline, update it to the user's selected values for PC and Profile name, and re-serialize it to their save folder
			ComputerStructure Computers = ComputerStructure.DeserializeFromFile($@"Content\Stories\{UserStorySelection}\Computers.xml".ToPlatformPath());

			Computer UserPC = Computers.GetComputer("REPLACETHIS");
			UserAccount UserAccount = UserPC.GetUser("REPLACETHIS");

			UserPC.Name = UserPCName;
			UserAccount.Username = UserProfileName;

			Computers.SerializeToFile($@"{PathToNewSaveFolder}\Computers.xml".ToPlatformPath());

			// Do some initialization on the User object now that we have the ComputerStructure loaded in and then serialize that to the user's save folder
			User.HomePC = UserPC;
			User.Account = UserAccount;

			// This will create the session that we are going to serialize with the rest of the user class that will have all of our connection data bundled in when we load the save
			User.InstantiateSession();

			User.SerializeToFile($@"{PathToNewSaveFolder}\User.xml".ToPlatformPath());

			// And we're done, report success and the path to the user's save file
			return (true, $@"{PathToNewSaveFolder}\User.xml".ToPlatformPath());
		}
		catch (Exception e)
		{
			Log.WriteLine("SaveAPI :: Caught exception while writing save data", ConsoleColor.Red, ConsoleColor.Black);
			Log.WriteLine(e.Message);
			Log.WriteLine(e.StackTrace);
			Log.WriteLine();

			return (false, null);
		}
	}

	public static bool LoadGameFromSave(string PathToSave)
	{
		// Initialize game objects
		try
		{
			Player = User.DeserializeFromFile(PathToSave);

			CurrentStory = Story.DeserializeFromFile($@"Content\Stories\{Player.StoryID}\Story.xml".ToPlatformPath());
			CurrentMission = CurrentStory.GetMission(Player.CurrentMissionID);

			Computers = ComputerStructure.DeserializeFromFile($@"Content\Saves\{Player.ProfileName}\Computers.xml".ToPlatformPath());
		}
		catch (Exception e)
		{
			Log.WriteLine($"SaveAPI :: Error loading save '{PathToSave}'", ConsoleColor.Red, ConsoleColor.Black);
			Log.WriteLine(e.Message);
			Log.WriteLine(e.StackTrace);
			Log.WriteLine();

			Player = null;
			CurrentStory = null;
			CurrentMission = null;
			Computers = null;
			return false;
		}

		// Initialize some connection-related values given the computer structure for this save
		Player.PostDeserializationInit(Computers);

		// Method that creates our Home PC user session and populates the CurrentSession field
		Player.InstantiateSession();

		EventManager.Initialize();

		// Load current mission
		MissionAPI.LoadMission(Player.CurrentMissionID);

		return true;
	}

	public static void UnloadCurrentSave()
	{
		Player.CloseCurrentSession();
		Player = null;

		CurrentStory.Missions.Clear();
		CurrentStory = null;

		Computers.Computers.Clear();
		Computers = null;
	}
}