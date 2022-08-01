using Haven;
using Lawful.GameLibrary;

namespace Lawful.UI;

internal static class Sections
{
	public static WidgetGroup MainMenu;
	public static WidgetGroup NewGame;
	public static WidgetGroup LoadGame;
	public static WidgetGroup Options;
	public static WidgetGroup Credits;
	public static WidgetGroup Game;
}

public static class UIManager
{
	private static WidgetGroup _current;
	public static WidgetGroup Current
	{
		get => _current;
		set
		{
			if (_current is not null)
				_current.Hide();

			_current = value;
			_current.Show();
		}
	}

	public static void Initialize()
	{
		ConstructMainMenu();
		ConstructNewGame();
		ConstructLoadGame();
		ConstructOptions();
		ConstructCredits();
		ConstructGame();

		Sections.MainMenu.Hide();
		Sections.NewGame.Hide();
		Sections.LoadGame.Hide();
		Sections.Options.Hide();
		Sections.Credits.Hide();
		Sections.Game.Hide();
	}

	public static void ConstructMainMenu()
	{
		Sections.MainMenu = new();

		Label Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		Label Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		Label Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		Label Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		Label Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		Label DevSg = new(1, 5, @"\ A game by Carson Ver Planck", ConsoleColor.Yellow, ConsoleColor.Black);

		Menu Main = new(1, 7) { SelectedOptionStyle = MenuStyle.Highlighted };
		Main.AddOption("New Game",	delegate () { Current = Sections.NewGame; });
		Main.AddOption("Load Game",	delegate () { Current = Sections.LoadGame; });
		Main.AddOption("Options",	delegate () { Current = Sections.Options; });
		Main.AddOption("Credits",	delegate () { Current = Sections.Credits; });
		Main.AddOption("Exit",		delegate () { Engine.SignalExit(); });

		Sections.MainMenu.Widgets.Add(Line1);
		Sections.MainMenu.Widgets.Add(Line2);
		Sections.MainMenu.Widgets.Add(Line3);
		Sections.MainMenu.Widgets.Add(Line4);
		Sections.MainMenu.Widgets.Add(Line5);
		Sections.MainMenu.Widgets.Add(DevSg);
		Sections.MainMenu.Widgets.Add(Main);

		Sections.MainMenu.OnShow = delegate (WidgetGroup wg) { Engine.FocusedWidget = Main; };
		Sections.MainMenu.OnHide = delegate (WidgetGroup wg) { Engine.FocusedWidget = null; };
	}

	public static void ConstructNewGame()
	{
		Sections.NewGame = new();

		Label Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		Label Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		Label Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		Label Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		Label Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		Label NewGm = new(1, 5, @"\ Start a new game", ConsoleColor.Yellow, ConsoleColor.Black);

		Label HelpLabel = new(1, 7, "Press [ESC] to go back");

		Label InstructionsLabel = new(1, 9, "Select an installed storyline to play");

		Menu StoryMenu = new(1, 10) { SelectedOptionStyle = MenuStyle.Highlighted };
		InputField ProfileNameField = new(1, 10, "> ");
		InputField PCNameField = new(1, 10, "> ");

		Sections.NewGame.Widgets.Add(Line1);
		Sections.NewGame.Widgets.Add(Line2);
		Sections.NewGame.Widgets.Add(Line3);
		Sections.NewGame.Widgets.Add(Line4);
		Sections.NewGame.Widgets.Add(Line5);
		Sections.NewGame.Widgets.Add(NewGm);

		Sections.NewGame.Widgets.Add(HelpLabel);
		Sections.NewGame.Widgets.Add(InstructionsLabel);

		Sections.NewGame.Widgets.Add(StoryMenu);
		Sections.NewGame.Widgets.Add(ProfileNameField);
		Sections.NewGame.Widgets.Add(PCNameField);

		string SelectedStory = string.Empty;
		string ProfileName = string.Empty;
		string PCName = string.Empty;

		ProfileNameField.OnInput = delegate (string Input)
		{
			if (Input.Length == 0) { return; }

			if (Directory.Exists(@$".\Content\Saves\{Input}"))
				return;

			ProfileName = Input;
			ProfileNameField.Visible = false;

			PCNameField.Visible = true;
			Engine.FocusedWidget = PCNameField;

			InstructionsLabel.Text = "Enter a PC name";
		};

		PCNameField.OnInput = delegate (string Input)
		{
			if (Input.Length == 0) { return; }

			PCName = Input;

			(bool Succeeded, string Story) = SaveAPI.InitSave(PCName, ProfileName, SelectedStory);

			if (!Succeeded)
			{
				
			}
		};

		Sections.NewGame.OnShow = delegate (WidgetGroup wg)
		{
			ProfileNameField.Visible = false;
			PCNameField.Visible = false;

			foreach (string StoryName in GameAPI.GetInstalledStorylines())
			{
				StoryMenu.AddOption(StoryName, delegate ()
				{
					SelectedStory = StoryName;
					StoryMenu.Visible = false;

					ProfileNameField.Visible = true;
					Engine.FocusedWidget = ProfileNameField;

					InstructionsLabel.Text = "Enter a profile name";
				});
			}

			Engine.AddUpdateTask("NewGameProcedure", NewGameProcedure);
			Engine.FocusedWidget = StoryMenu;
		};

		Sections.NewGame.OnHide = delegate (WidgetGroup wg)
		{
			StoryMenu.RemoveAllOptions();
			Engine.RemoveUpdateTask("NewGameProcedure");
			Engine.FocusedWidget = null;
		};
	}

	private static void NewGameProcedure(State s)
	{
		if (!s.KeyPressed) { return; }

		switch (s.KeyInfo.Key)
		{
			case ConsoleKey.Escape:
				Current = Sections.MainMenu;
				break;
		}
	}

	public static void ConstructLoadGame()
	{
		Sections.LoadGame = new();
	}

	public static void ConstructOptions()
	{
		Sections.Options = new();
	}

	public static void ConstructCredits()
	{
		Sections.Credits = new();
	}

	public static void ConstructGame()
	{
		Sections.Game = new();
	}
}