
namespace Lawful.GameLibrary;

using static GameSession;

public static class MissionAPI
{
    public static void LoadMission(string MissionID)
    {
        Log.WriteLine($"MissionAPI :: Loading mission '{MissionID}'...");

        Mission TryMission = CurrentStory.GetMission(MissionID);

        if (TryMission is null)
        {
            Log.WriteLine($"MissionAPI :: Error loading mission - '{CurrentStory.Name}' does not contain mission by ID '{MissionID}'");
            return;
        }

        CurrentMission = TryMission;

        if (CurrentMission.HasAssets)
        {
            int Elapsed = Util.ExecTimed(delegate ()
            {
                CurrentMission.LoadAssets();
            });

            Log.WriteLine($"MissionAPI :: Loaded mission assets for '{MissionID}' in {Elapsed} ms");
        }

        EventManager.AddMissionEvents(TryMission);

        Log.WriteLine($"MissionAPI :: Finished loading '{MissionID}'");
    }

    public static void UnloadCurrentMission()
    {
        Log.WriteLine($"MissionAPI :: Unloading mission '{CurrentMission.ID}'");

        EventManager.ClearEvents();
        CurrentMission.Events.Clear();
        CurrentMission.FreeAssets();

        Log.WriteLine($"MissionAPI :: Finished unloading '{CurrentMission.ID}'");
    }
}