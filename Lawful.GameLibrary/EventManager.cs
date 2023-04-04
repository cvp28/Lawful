using Jint;

namespace Lawful.GameLibrary;

using static GameSession;
using UI;

public static class EventManager
{
	public static Engine JSE;
	private static List<Event> Events;
	private static bool RemoveCurrentEvent = false;

	public static void Initialize()
	{
		Events = new();

		JSE = new(options =>
		{
			options.AddExtensionMethods(typeof(Util));
		});

		var BootupConsole = App.GetLayer<BootupLayer>().BootupConsole;
		var ReadKey = App.GetLayer<BootupLayer>().ReadKey;

		var GameConsole = App.GetLayer<GameLayer>("Game").GameConsole;

		JSE.SetValue("BootupConsole", BootupConsole);
		JSE.SetValue("GameConsole", GameConsole);
		
		JSE.SetValue("KeyAvailable", ReadKey.KeyAvailable);
		JSE.SetValue("ReadKey", ReadKey.ReadKey);

		JSE.SetValue("ConsoleColor", typeof(ConsoleColor));
		JSE.SetValue("Sleep", new Action<int>(Thread.Sleep));
		
		JSE.SetValue("TypewriterOff", delegate () { DoTypewriter = false; });
		JSE.SetValue("TypewriterOn", delegate () { DoTypewriter = true; });
		JSE.SetValue("ToggleTypewriter", delegate () { DoTypewriter = !DoTypewriter; });

		#region Sound API
		JSE.SetValue("PlayMusic", delegate (string ID, bool Restart)
		{
			var TryAsset = CurrentMission.MusicAssets.FirstOrDefault(asset => asset.ID == ID);

			if (TryAsset is null)
			{
				Log.WriteLine($"EventManager :: A script called 'PlayMusic' but ID '{ID}' was not found");
				return false;
			}

			return TryAsset.Play(Restart);
		});
		JSE.SetValue("PauseMusic", delegate (string ID)
		{
			var TryAsset = GameSession.CurrentMission.MusicAssets.FirstOrDefault(asset => asset.ID == ID);

			if (TryAsset is null)
			{
				Log.WriteLine($"EventManager :: A script called 'PauseMusic' but ID '{ID}' was not found");
				return false;
			}

			return TryAsset.Pause();
		});
		JSE.SetValue("StopMusic", delegate (string ID)
		{
			var TryAsset = GameSession.CurrentMission.MusicAssets.FirstOrDefault(asset => asset.ID == ID);

			if (TryAsset is null)
			{
				Log.WriteLine($"EventManager :: A script called 'StopMusic' but ID '{ID}' was not found");
				return false;
			}

			return TryAsset.Stop();
		});
		JSE.SetValue("SetMusicVolume", delegate (string ID, float Volume)
		{
			var TryAsset = GameSession.CurrentMission.MusicAssets.FirstOrDefault(asset => asset.ID == ID);

			if (TryAsset is null)
			{
				Log.WriteLine($"EventManager :: A script called 'SetMusicVolume' but ID '{ID}' was not found");
				return false;
			}

			return TryAsset.SetVolume(Volume);
		});
		JSE.SetValue("SlideMusicVolume", delegate (string ID, float Volume, int Seconds)
		{
			var TryAsset = GameSession.CurrentMission.MusicAssets.FirstOrDefault(asset => asset.ID == ID);

			if (TryAsset is null)
			{
				Log.WriteLine($"EventManager :: A script called 'SlideMusicVolume' but ID '{ID}' was not found");
				return false;
			}

			return TryAsset.SlideVolume(Volume, Seconds);
		});
		JSE.SetValue("SeekMusic", delegate (string ID, double Seconds)
		{
			var TryAsset = GameSession.CurrentMission.MusicAssets.FirstOrDefault(asset => asset.ID == ID);

			if (TryAsset is null)
			{
				Log.WriteLine($"EventManager :: A script called 'SeekMusic' but ID '{ID}' was not found");
				return false;
			}

			return TryAsset.Seek(Seconds);
		});

		JSE.SetValue("PlaySFX", delegate (string ID)
		{
			var TryAsset = GameSession.CurrentMission.SoundEffects.FirstOrDefault(asset => asset.ID == ID);

			if (TryAsset is null)
			{
				Log.WriteLine($"EventManager :: A script called 'PlaySFX' but ID '{ID}' was not found");
				return;
			}

			TryAsset.Play();
		});
		#endregion

		JSE.SetValue("ChangeMission", delegate (string ID)
		{
			if (!CurrentStory.HasMission(ID))
				return;

			MissionAPI.UnloadCurrentMission();
			MissionAPI.LoadMission(ID);
		});

		JSE.SetValue("AddChatSequence", delegate (string Path)
		{

		});

		JSE.SetValue("SetUserOnline", delegate (string Username)
		{
			NETChatContact TryContact = Player.NETChatAccount.Contacts.FirstOrDefault(c => c.Username == Username);

			if (TryContact is null)
			{
				Log.WriteLine($"EventManager :: A script called 'SetUserOnline' but Username '{Username}' was not found");
				return;
			}

			App.GetLayer<NotifyLayer>().PushNotification($"{Username} is now online", "UserOnlineNotify", delegate()
			{
				TryContact.Online = true;

				if (App.IsLayerVisible("NETChat"))
					App.GetLayer<NETChatLayer>().UpdateLists();
			});
		});

		JSE.SetValue("SetUserOffline", delegate (string Username)
		{
			NETChatContact TryContact = Player.NETChatAccount.Contacts.FirstOrDefault(c => c.Username == Username);

			if (TryContact is null)
			{
				Log.WriteLine($"EventManager :: A script called 'SetUserOffline' but Username '{Username}' was not found");
				return;
			}


			App.GetLayer<NotifyLayer>().PushNotification($"{Username} is now offline", "UserOfflineNotify", delegate()
			{
				TryContact.Online = false;

				if (App.IsLayerVisible("NETChat"))
					App.GetLayer<NETChatLayer>().UpdateLists();
			});
		});

		JSE.SetValue("DoFriendRequest", delegate (string Username)
		{
			if (Player.NETChatAccount.Contacts.FirstOrDefault(fr => fr.Username == Username) is not null)
			{
				Log.WriteLine($"EventManager :: A script called 'DoFriendRequest' but a contact for '{Username}' already exists");
				return;
			}

			if (Player.NETChatAccount.PendingFriendRequests.FirstOrDefault(fr => fr == Username) is not null)
			{
				Log.WriteLine($"EventManager :: A script called 'DoFriendRequest' but a request for '{Username}' was already pending");
				return;
			}

			App.GetLayer<NotifyLayer>().PushNotification($"New friend request from {Username}!\n\nPress F2 for details", "FriendRequestNotify", delegate ()
			{
				Player.NETChatAccount.PendingFriendRequests.Add(Username);

				if (App.IsLayerVisible("NETChat"))
					App.GetLayer<NETChatLayer>().UpdateLists();
			});
		});

		JSE.SetValue("RemoveMe", delegate () { RemoveCurrentEvent = true; });

		JSE.SetValue("AddEvent", delegate (Trigger t, string ScriptPath)
		{
			CurrentMission.Events.Add(new() { Trigger = t, ScriptPath = ScriptPath });
		});

		JSE.SetValue("Trigger", typeof(Trigger));
	}

	public static void AddEvent(Event e)
	{
		Events.Add(e);
	}

	public static void AddMissionEvents(Mission m)
	{
		foreach (Event e in m.Events)
			Events.Add(e);
	}

	public static void ClearEvents() => Events.Clear();

	private static List<Event> EventsToRemove = new();

	public static void HandleEventsByTrigger(Trigger t)
	{
		var TriggerEvents = Events.Where(e => e.Trigger == t).ToList();

		foreach (Event e in TriggerEvents)
		{
			string ScriptSource = e.ScriptSource;

			if (ScriptSource.Length == 0)
				continue;

			Log.WriteLine($"EventManager :: Executing '{e.ScriptPath}' for '{t}'...");

			try
			{
				JSE.Execute(ScriptSource);

				if (RemoveCurrentEvent)
				{
					EventsToRemove.Add(e);
					RemoveCurrentEvent = false;
				}
			}
			catch (Exception ex)
			{
				Log.WriteLine($"EventManager :: Script exception for '{e.ScriptPath}'");
				Log.WriteLine($"   {ex.Message}", ConsoleColor.Red, ConsoleColor.Black);
				Log.WriteLine($"{ex.StackTrace}");
			}
		}

		if (EventsToRemove.Count > 0)
		{
			foreach (Event e in EventsToRemove)
				Events.Remove(e);

			EventsToRemove.Clear();
		}
	}
}
