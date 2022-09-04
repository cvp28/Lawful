
namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class MissionAPI
{
    public static void LoadMission(string MissionID)
    {
        Mission TryMission = CurrentStory.GetMission(MissionID);

        if (TryMission is null)
		{
			Log.WriteLine($"Could not load mission by ID '{MissionID}'");
			Log.WriteLine($"'{CurrentStory.Name}' does not contain mission by ID '{MissionID}'");
		}

        EventManager.AddMissionEvents(TryMission);
    }

    public static void UnloadCurrentMission()
    {
        try
        {
            EventManager.ClearEvents();
        }
        catch (Exception e)
        {
            Log.WriteLine("Error unloading the current mission assembly", ConsoleColor.Red, ConsoleColor.Black);
            Log.WriteLine(e.Message);
            Log.WriteLine(e.StackTrace);
            Log.NextLine();
        }
    }

    //  public static T GetMissionData<T>(string Name)
    //  {
    //      Type TryType = CurrentMissionAssembly.GetType($"{MissionAssemblyNamespace}.{MissionAssemblyDataClassName}");
    //      FieldInfo TryField = TryType.GetRuntimeField(Name);
    //  
    //      if (TryType is not null && TryField is not null)
    //          return (T)TryField.GetValue(0);
    //      else
    //          return default;
    //  }
    //  
    //  public static void SetMissionData(string Name, object Value)
    //  {
    //      Type TryType = CurrentMissionAssembly.GetType($"{MissionAssemblyNamespace}.{MissionAssemblyDataClassName}");
    //      FieldInfo TryField = TryType.GetRuntimeField(Name);
    //  
    //      if (TryType is not null && TryField is not null)
    //          TryField.SetValue(null, Value);
    //  }
}