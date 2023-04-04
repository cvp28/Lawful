using Haven;
using System.Runtime.InteropServices.JavaScript;

namespace Lawful.GameLibrary.UI;

using static GameSession;

public class NETChatLayer : Layer
{
	[Widget] private Label ConsoleTabLabel, NETChatTabLabel;
	[Widget] private Label Line1, Line2, Line3, Line4, Line5, ChtFr;

	[Widget] public ScrollableTextBox ChatView;

	[Widget] private Label FriendsHeader, RequestsHeader;
	[Widget] private Menu FriendsMenu, RequestsMenu;
	[Widget] private Label AcceptLabel;

	private string CurrentFriendRequest;

	public NETChatLayer() : base()
	{
		ConsoleTabLabel = new(1, 0, "Console", ConsoleColor.White, ConsoleColor.Black);
		NETChatTabLabel = new(9, 0, "NETChat", ConsoleColor.Black, ConsoleColor.White);

		Line1 = new(1, 1, @"    _   __ ______ ______ ______ __            __ ", ConsoleColor.Red, ConsoleColor.Black);
		Line2 = new(1, 2, @"   / | / // ____//_  __// ____// /_   ______ / /_", ConsoleColor.Red, ConsoleColor.Black);
		Line3 = new(1, 3, @"  /  |/ // __/    / /  / /    / __ \ / __  // __/", ConsoleColor.Red, ConsoleColor.Black);
		Line4 = new(1, 4, @" / /|  // /___   / /  / /___ / / / // /_/ // /_  ", ConsoleColor.Red, ConsoleColor.Black);
		Line5 = new(1, 5, @"/_/ |_//_____/  /_/   \____//_/ /_/ \__/\_\\__/  ", ConsoleColor.Red, ConsoleColor.Black);
		ChtFr = new(1, 6, @"\ Chat with friends", ConsoleColor.Yellow, ConsoleColor.Black);

		FriendsHeader = new(1, 9, "Your Friends");
		RequestsHeader = new(1, 9, "Friend Requests")
		{
			Visible = false
		};

		FriendsMenu = new(1, 10)
		{
			SelectedOptionStyle = MenuStyle.Highlighted,
			KeyActionProcessingMode = KeyActionMode.After
		};

		FriendsMenu.AddKeyAction(ConsoleKey.Tab, delegate ()
		{
			if (RequestsMenu.Visible)
				App.Instance.FocusedWidget = RequestsMenu;
		});

		RequestsMenu = new(1, 10)
		{
			SelectedOptionStyle = MenuStyle.Highlighted,
			KeyActionProcessingMode = KeyActionMode.After,
			Visible = false
		};

		RequestsMenu.AddKeyAction(ConsoleKey.Tab, delegate ()
		{
			App.Instance.FocusedWidget = FriendsMenu;
		});

		ChatView = new(0, 0, 70, 20)
		{
			Visible = false,
			CursorVisible = false,
			KeyActionProcessingMode = KeyActionMode.After
		};

		ChatView.AddKeyAction(ConsoleKey.Escape, delegate ()
		{
			ChatView.Visible = false;
			ChatView.Clear();
			App.Instance.FocusedWidget = FriendsMenu;
			App.Instance.RemoveUpdateTask("NETChatLayer.ChatViewControls");
		});

		ChatView.AddKeyAction(ConsoleKey.UpArrow, ChatView.ScrollViewUp);
		ChatView.AddKeyAction(ConsoleKey.DownArrow, ChatView.ScrollViewDown);

		AcceptLabel = new(0, 0, "Accept? (Y/N)")
		{
			KeyActionProcessingMode = KeyActionMode.After
		};

		AcceptLabel.AddKeyAction(ConsoleKey.Y, delegate ()
		{
			// Add as friend
			Player.NETChatAccount.Contacts.Add(new()
			{
				Username = CurrentFriendRequest,
				Online = false,
				Chat = new()
			});

			// Remove friend request
			Player.NETChatAccount.PendingFriendRequests.Remove(CurrentFriendRequest);

			UpdateLists();

			if (Player.NETChatAccount.PendingFriendRequests.Count == 0)
				App.Instance.FocusedWidget = FriendsMenu;
			else
				App.Instance.FocusedWidget = RequestsMenu;

			// Do notification
			App.GetLayer<NotifyLayer>().PushNotification("Friend request accepted", "FriendRequestAccept", delegate () { }, 2);

			// Trigger event
			EventManager.JSE.SetValue("G_Username", CurrentFriendRequest);
			EventManager.HandleEventsByTrigger(Trigger.FriendRequestAccepted);
			EventManager.JSE.SetValue("G_Username", "");

			// Reset UI
			AcceptLabel.Visible = false;
			CurrentFriendRequest = string.Empty;
		});
		AcceptLabel.AddKeyAction(ConsoleKey.N, delegate ()
		{
			// Remove friend request
			Player.NETChatAccount.PendingFriendRequests.Remove(CurrentFriendRequest);

			UpdateLists();

			if (Player.NETChatAccount.PendingFriendRequests.Count == 0)
				App.Instance.FocusedWidget = FriendsMenu;
			else
				App.Instance.FocusedWidget = RequestsMenu;

			// Do notification
			App.GetLayer<NotifyLayer>().PushNotification("Friend request denied", "FriendRequestDeny", delegate () { }, 2);

			// Trigger event
			EventManager.JSE.SetValue("G_Username", CurrentFriendRequest);
			EventManager.HandleEventsByTrigger(Trigger.FriendRequestDenied);
			EventManager.JSE.SetValue("G_Username", "");

			// Reset UI
			AcceptLabel.Visible = false;
			CurrentFriendRequest = string.Empty;
		});

		AddWidgetsInternal();
	}

	public override void OnShow(App a, object[] Args)
	{
		AcceptLabel.Visible = false;

		UpdateLists();

		if (ChatView.Visible)
			a.FocusedWidget = ChatView;
		else
			a.FocusedWidget = FriendsMenu;
	}

	public override void OnHide(App a)
	{
		a.FocusedWidget = null;
	}

	private static readonly object FriendsListLock = new();

	public void UpdateLists()
	{
		lock (FriendsListLock)
		{
			#region Update Friends List
			// Reset friends menu
			FriendsMenu.RemoveAllOptions();

			// Sort friends list
			var OnlineFriends = Player.NETChatAccount.Contacts.Where(c => c.Online).ToList();
			OnlineFriends.Sort();

			var OfflineFriends = Player.NETChatAccount.Contacts.Where(c => !c.Online).ToList();
			OfflineFriends.Sort();

			// Add friends list options
			foreach (var f in OnlineFriends)
				FriendsMenu.AddOption(f.Username, delegate ()
				{
					App.Instance.FocusedWidget = ChatView;

					if (f.HasPendingChatRequest)
					{

					}

					ChatView.WriteLine($"{f.Username}'s Chat\n", ConsoleColor.Yellow);

					foreach (var m in f.Chat.History)
					{
						ChatView.Write('[');

						if (f.Username == "SERVER")
							ChatView.Write(f.Username, ConsoleColor.DarkGreen);
						else
							ChatView.Write(f.Username, ConsoleColor.Yellow);

						ChatView.Write("] ");

						ChatView.WriteLine(m.Message);
					}

					ChatView.Visible = true;
				}, ConsoleColor.DarkGreen);

			foreach (var f in OfflineFriends)
				FriendsMenu.AddOption(f.Username, delegate ()
				{
					App.Instance.FocusedWidget = ChatView;

					ChatView.WriteLine($"{f.Username}'s Chat\n", ConsoleColor.Yellow);

					foreach (var m in f.Chat.History)
					{
						ChatView.Write('[');

						if (f.Username == "SERVER")
							ChatView.Write(f.Username, ConsoleColor.DarkGreen);
						else
							ChatView.Write(f.Username, ConsoleColor.Yellow);

						ChatView.Write("] ");

						ChatView.WriteLine(m.Message);
					}

					ChatView.Visible = true;
				}, ConsoleColor.DarkRed);

			// Update header
			FriendsHeader.Text = $"Your Friends ({OnlineFriends.Count} Online, {OfflineFriends.Count} Offline)";
			#endregion

			#region Update Friend Requests
			// Reset requests menu
			RequestsMenu.RemoveAllOptions();

			// Sort the friend requests alphabetically
			Player.NETChatAccount.PendingFriendRequests.Sort();

			// Add friend requests options
			foreach (var request in Player.NETChatAccount.PendingFriendRequests)
				RequestsMenu.AddOption(request, delegate ()
				{
					CurrentFriendRequest = request;

					AcceptLabel.X = RequestsMenu.X + RequestsMenu[RequestsMenu.SelectedOption].Text.Length + 1;
					AcceptLabel.Y = RequestsMenu.Y + RequestsMenu.SelectedOption;

					AcceptLabel.Visible = true;

					App.Instance.FocusedWidget = AcceptLabel;
				}, ConsoleColor.Yellow);

			// Update header
			int Count = Player.NETChatAccount.PendingFriendRequests.Count;
			RequestsHeader.Text = $"{Count} new friend {(Count > 1 ? "requests" : "request")}";
			#endregion
		}
	}

	[UpdateTask]
	private void AppSwitchTask(State s)
	{
		if (!s.KeyPressed)
			return;

		switch (s.KeyInfo.Key)
		{
			case ConsoleKey.F1:

				if (App.Instance.IsLayerVisible("Game"))
					return;

				App.Instance.SetLayer(0, "Game", false);
				break;

			case ConsoleKey.F2:

				if (App.Instance.IsLayerVisible("NETChat"))
					return;

				App.Instance.SetLayer(0, "NETChat");
				break;
		}
	}

	private int LastFriendCount = 0;
	private int LastRequestsCount = 0;

	[UpdateTask]
	private void UpdateUI(State s)
	{
		bool FriendsListChanged = Player.NETChatAccount.Contacts.Count != LastFriendCount;
		bool RequestCountChanged = Player.NETChatAccount.PendingFriendRequests.Count != LastRequestsCount;

		if (FriendsListChanged || RequestCountChanged)
			UpdateLists();

		LastFriendCount = Player.NETChatAccount.Contacts.Count;
		LastRequestsCount = Player.NETChatAccount.PendingFriendRequests.Count;

		if (Player.NETChatAccount.PendingFriendRequests.Count > 0)
		{
			RequestsHeader.Visible = true;
			RequestsMenu.Visible = true;
		}
		else
		{
			RequestsHeader.Visible = false;
			RequestsMenu.Visible = false;
		}
	}

	public override void UpdateLayout(Dimensions d)
	{
		RequestsHeader.Y = 11 + FriendsMenu.OptionCount;
		RequestsMenu.Y = RequestsHeader.Y + 1;

		ChatView.X = d.WindowWidth - (ChatView.Width + 2) - 1;
		ChatView.Y = d.WindowHeight - (ChatView.Height + 2) - 1;
	}
}
