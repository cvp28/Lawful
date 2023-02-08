using Haven;
using Lawful.InputParser;

namespace Lawful.GameLibrary.UI;

using static GameSession;

public class NETChatLayer : Layer
{
	[Widget] private Label ConsoleTabLabel, NETChatTabLabel;
	[Widget] private Label Line1, Line2, Line3, Line4, Line5, ChtFr;

	[Widget] private Label FriendsHeader, RequestsHeader;
	[Widget] private Menu FriendsMenu, RequestsMenu;

	private List<NETChatContact> Friends;

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
			SelectedOptionStyle = MenuStyle.Highlighted
		};

		RequestsMenu = new(1, 10)
		{
			SelectedOptionStyle = MenuStyle.Highlighted,
			Visible = false
		};

		Friends = new();

		AddWidgetsInternal();
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

	public override void OnShow(App a, object[] Args)
	{
		a.FocusedWidget = FriendsMenu;
	}

	public override void OnHide(App a)
	{
		a.FocusedWidget = null;
	}

	[UpdateTask]
	private void UpdateUI(State s)
	{
		#region Update Friends
		FriendsMenu.RemoveAllOptions();

		var OnlineFriends = Player.NETChatAccount.Contacts.Where(c => c.Online).ToList();
		OnlineFriends.Sort();

		var OfflineFriends = Player.NETChatAccount.Contacts.Where(c => !c.Online).ToList();
		OfflineFriends.Sort();

		foreach (var f in OnlineFriends)
			FriendsMenu.AddOption(f.Username, delegate () { }, ConsoleColor.DarkGreen);

		foreach (var f in OfflineFriends)
			FriendsMenu.AddOption(f.Username, delegate () { }, ConsoleColor.DarkRed);

		FriendsHeader.Text = $"Your Friends ({OnlineFriends.Count} Online, {OfflineFriends.Count} Offline)";
		#endregion

		#region Update Friend Requests

		if (Player.NETChatAccount.PendingFriendRequests.Count > 0)
		{
			RequestsHeader.Visible = true;
			RequestsMenu.Visible = true;
			App.Instance.AddUpdateTask("NETChatLayer.MenuSwitcher", MenuSwitcher);

			RequestsMenu.RemoveAllOptions();

			// Sort the friend requests alphabetically
			Player.NETChatAccount.PendingFriendRequests.Sort();

			foreach (var request in Player.NETChatAccount.PendingFriendRequests)
				RequestsMenu.AddOption(request, delegate () { }, ConsoleColor.Yellow);

			int Count = Player.NETChatAccount.PendingFriendRequests.Count;
			RequestsHeader.Text = $"{Count} new friend {(Count > 1 ? "requests" : "request")}";
		}
		else
		{
			RequestsHeader.Visible = false;
			RequestsMenu.Visible = false;
			App.Instance.RemoveUpdateTask("NETChatLayer.MenuSwitcher");
		}

		#endregion
	}

	private void MenuSwitcher(State s)
	{
		if (!s.KeyPressed)
			return;

		switch (s.KeyInfo.Key)
		{
			case ConsoleKey.Tab:
				if (FriendsMenu.Focused)
					App.Instance.FocusedWidget = RequestsMenu;
				else
					App.Instance.FocusedWidget = FriendsMenu;

				return;
		}
	}

	public override void UpdateLayout(Dimensions d)
	{
		RequestsHeader.Y = 11 + FriendsMenu.OptionCount;
		RequestsMenu.Y = RequestsHeader.Y + 1;
	}
}
