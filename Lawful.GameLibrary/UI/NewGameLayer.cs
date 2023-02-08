using Haven;
using System.Diagnostics;

namespace Lawful.GameLibrary.UI;

public class NewGameLayer : Layer
{
	[Widget] Label Line1, Line2, Line3, Line4, Line5, NewGm;

	[Widget] Label HelpLabel, StatusLabel;

	[Widget] Menu StoryMenu;
	[Widget] InputField InputField;

	private string SelectedStory;
	private string ProfileName;
	private string PCName;

	public NewGameLayer() : base()
	{
		Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		NewGm = new(1, 5, @"\ Start a new game /", ConsoleColor.Yellow, ConsoleColor.Black);

		HelpLabel = new(1, 7);

		StatusLabel = new(1, 9);

		StoryMenu = new(1, 10)
		{
			SelectedOptionStyle = MenuStyle.Highlighted,
			TextAlignment = Alignment.Center
		};

		InputField = new(1, 10, "> ");

		AddWidgetsInternal();
	}

	public override void OnShow(App a, object[] Args)
	{
		// Reset widgets to default state
		StoryMenu.Visible = true;

		InputField.Visible = false;
		InputField.Clear();
		InputField.OnInput = OnProfileNameInput;

		HelpLabel.Text = string.Empty;

		// Run background worker task to retrieve installed stories (IO) and populate the menu
		Task.Run(delegate () { InitMenu(a); });
	}

	private void InitMenu(App a)
	{
		StatusLabel.Text = "Getting installed stories...";

		foreach (string StoryName in GameAPI.GetInstalledStorylines())
		{
			StoryMenu.AddOption(StoryName, delegate ()
			{
				SelectedStory = StoryName;
				StoryMenu.Visible = false;

				InputField.Visible = true;
				App.Instance.FocusedWidget = InputField;

				StatusLabel.Text = "(2/3) Enter a profile name";
			});
		}

		StatusLabel.Text = "(1/3) Select an installed story to play";
		HelpLabel.Text = "Press [ESC] to go back";

		a.FocusedWidget = StoryMenu;
		a.AddUpdateTask("NewGameEscape", EscapeProcedure);
	}

	private void OnProfileNameInput(string Input)
	{
		if (Input.Length == 0) { return; }

		if (Directory.Exists(@$".\Content\Saves\{Input}"))
			return;

		ProfileName = Input;

		StatusLabel.Text = "(3/3) Enter a PC name";
		InputField.OnInput = OnPCNameInput;
	}

	private void OnPCNameInput(string Input)
	{
		if (Input.Length == 0) { return; }

		PCName = Input;

		Task.Run(delegate ()
		{
			StatusLabel.Text = "Creating save... ";

			(bool Succeeded, string StoryPath) = SaveAPI.InitSave(PCName, ProfileName, SelectedStory);

			if (!Succeeded)
				StatusLabel.Text = "Creating save... failed. Press F10 for more details.";
			else
				StatusLabel.Text = "Creating save... successful.";
		});
	}

	private void EscapeProcedure(State s)
	{
		if (!s.KeyPressed)
			return;

		if (s.KeyInfo.Key == ConsoleKey.Escape)
			App.Instance.SetLayer(0, "MainMenu");
	}

	public override void OnHide(App a)
	{
		StoryMenu.RemoveAllOptions();
		a.RemoveUpdateTask("NewGameEscape");
		a.FocusedWidget = null;
	}

	public override void UpdateLayout(Dimensions d)
	{
		Line1.CenterTo(d, 0, -8);
		Line2.CenterTo(d, 0, -7);
		Line3.CenterTo(d, 0, -6);
		Line4.CenterTo(d, 0, -5);
		Line5.CenterTo(d, 0, -4);
		NewGm.CenterTo(d, 0, -3);

		HelpLabel.CenterTo(d, 0, -1);

		StatusLabel.CenterTo(d, 0, 1);

		StoryMenu.CenterTo(d, 0, 3 + (int) Math.Ceiling(StoryMenu.OptionCount / 2.0f));
		InputField.CenterTo(d, 0, 3);
	}
}
