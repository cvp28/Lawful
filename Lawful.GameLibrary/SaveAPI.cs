namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class SaveAPI
{
	// We return a bool and a string containing:
	// a. if the save initialization was successful
	// and
	// b. the path to the user's save file
	public static (bool, string) InitSave(string UserPCName, string UserProfileName, string UserStorySelection)
	{
		Story UserStory = Story.DeserializeFromFile($@".\Content\Stories\{UserStorySelection}\Story.xml");

		User User = new()
		{
			ProfileName = UserProfileName,
			StoryID = UserStorySelection,
			CurrentMissionID = UserStory.StartMissionID
		};

		// Define our path to the new user's save folder
		string PathToNewSaveFolder = Path.Combine(Directory.GetCurrentDirectory() + @$"\Content\Saves\{UserProfileName}");

		// Create our new save folder if it does not exist already
		if (!Directory.Exists(PathToNewSaveFolder))
			Directory.CreateDirectory(PathToNewSaveFolder);
		else
			return (false, null);

		try
		{
			// Load in the default ComputerStructure from our selected storyline, update it to the user's selected values for PC and Profile name, and re-serialize it to their save folder
			ComputerStructure Computers = ComputerStructure.DeserializeFromFile($@".\Content\Stories\{UserStorySelection}\Computers.xml");

			Computer UserPC = Computers.GetComputer("REPLACETHIS");
			UserAccount UserAccount = UserPC.GetUser("REPLACETHIS");

			UserPC.Name = UserPCName;
			UserAccount.Username = UserProfileName;

			Computers.SerializeToFile($@"{PathToNewSaveFolder}\Computers.xml");

			// Do some initialization on the User object now that we have the ComputerStructure loaded in and then serialize that to the user's save folder
			User.HomePC = UserPC;
			User.Account = UserAccount;

			// This will create the session that we are going to serialize with the rest of the user class that will have all of our connection data bundled in when we load the save
			User.InstantiateSession();

			User.SerializeToFile($@"{PathToNewSaveFolder}\User.xml");

			// And we're done, report success and the path to the user's save file
			return (true, $@"{PathToNewSaveFolder}\User.xml");
		}
		catch (Exception e)
		{
			Log.WriteLine("Caught exception while writing save data", ConsoleColor.Red, ConsoleColor.Black);
			Log.WriteLine(e.Message);
			Log.WriteLine(e.StackTrace);
			Log.NextLine();

			return (false, null);
		}
	}

	public static void LoadGameFromSave(string PathToSave)
	{
		// Initialize game objects
		try
		{
			Player = User.DeserializeFromFile(PathToSave);

			CurrentStory = Story.DeserializeFromFile($@".\Content\Stories\{Player.StoryID}\Story.xml");
			CurrentMission = CurrentStory.GetMission(Player.CurrentMissionID);

			Computers = ComputerStructure.DeserializeFromFile($@".\Content\Saves\{Player.ProfileName}\Computers.xml");
		}
		catch (Exception e)
		{
			Log.WriteLine($"Error loading save '{PathToSave}'", ConsoleColor.Red, ConsoleColor.Black);
			Log.WriteLine(e.Message);
			Log.WriteLine(e.StackTrace);
			Log.NextLine();

			Player = null;
			CurrentStory = null;
			CurrentMission = null;
			Computers = null;
			return;
		}

		// Initialize some connection-related values given the computer structure for this save
		Player.PostDeserializationInit(Computers);

		// Method that creates our Home PC user session and populates the CurrentSession field
		Player.InstantiateSession();

		EventManager.Initialize();

		// Load current mission
		MissionAPI.LoadMission(Player.CurrentMissionID);
	}
}