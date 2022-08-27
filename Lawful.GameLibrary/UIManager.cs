using Esprima.Ast;
using Haven;
using Lawful.InputParser;

namespace Lawful.GameLibrary.UI;

public static class Sections
{
	public static WidgetGroup MainMenu;
	public static WidgetGroup NewGame;
	public static WidgetGroup LoadGame;
	public static WidgetGroup Options;
	public static WidgetGroup Credits;
	public static WidgetGroup Bootup;
	public static WidgetGroup Game;
}

public static class UIManager
{
	public static TextBox Log;
	public static Label FPSLabel;
	public static TextBox BootupConsole;
	public static TextBox GameConsole;
	public static InputField GameInput;
	public static ReadKeyBridge ReadKey;
	public static Label InputHeader;

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
		ConstructBootup();
		ConstructGame();

		// False to supress event firing
		Sections.MainMenu.Hide(false);
		Sections.NewGame.Hide(false);
		Sections.LoadGame.Hide(false);
		Sections.Options.Hide(false);
		Sections.Credits.Hide(false);
		Sections.Bootup.Hide(false);
		Sections.Game.Hide(false);

		Log = new(ScreenSpace.Full);
		Log.Visible = false;

		ReadKey = new();

		FPSLabel = new(Console.WindowWidth - 18, 0);

		// Add UI to Engine
		Engine.AddWidgets(Sections.MainMenu);
		Engine.AddWidgets(Sections.NewGame);
		Engine.AddWidgets(Sections.LoadGame);
		Engine.AddWidgets(Sections.Options);
		Engine.AddWidgets(Sections.Credits);
		Engine.AddWidgets(Sections.Bootup);
		Engine.AddWidgets(Sections.Game);

		if (GameOptions.ShowFPS)
		{
			Engine.AddWidget(FPSLabel);
			Engine.AddUpdateTask("UpdateFPS", delegate (State s) { FPSLabel.Text = $"{s.FPS,-4} FPS :: {s.LastFrameTime,-1} ms"; });
		}

		Engine.AddWidget(Log);

		Log.WriteLine("UI initialized!");

		Engine.AddUpdateTask("ToggleLog", ToggleLog);
	}

	private static void EscapeProcedure(State s)
	{
		if (!s.KeyPressed) { return; }

		switch (s.KeyInfo.Key)
		{
			case ConsoleKey.Escape:
				Current = Sections.MainMenu;
				break;
		}
	}

	private static void ToggleLog(State s)
	{
		if (!s.KeyPressed)
			return;

		switch (s.KeyInfo.Key)
		{
			case ConsoleKey.F10:
				Log.Visible = !Log.Visible;
				break;
		}
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

		Label StatusLabel = new(1, 9);

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
		Sections.NewGame.Widgets.Add(StatusLabel);

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

			StatusLabel.Text = "(3/3) Enter a PC name";
		};

		PCNameField.OnInput = delegate (string Input)
		{
			if (Input.Length == 0) { return; }

			PCName = Input;

			StatusLabel.Text = "Creating save... ";
			(bool Succeeded, string StoryPath) = SaveAPI.InitSave(PCName, ProfileName, SelectedStory);

			if (!Succeeded)
				StatusLabel.Text = "Creating save... failed. Check logs in game directory for more details.";
			else
				StatusLabel.Text = "Creating save... successful.";
		};

		Sections.NewGame.OnShow = delegate (WidgetGroup wg)
		{
			ProfileNameField.Visible = false;
			PCNameField.Visible = false;

			ProfileNameField.Clear();
			PCNameField.Clear();

			StatusLabel.Text = "Getting installed stories...";

			foreach (string StoryName in GameAPI.GetInstalledStorylines())
			{
				StoryMenu.AddOption(StoryName, delegate ()
				{
					SelectedStory = StoryName;
					StoryMenu.Visible = false;

					ProfileNameField.Visible = true;
					Engine.FocusedWidget = ProfileNameField;

					StatusLabel.Text = "(2/3) Enter a profile name";
				});
			}

			StatusLabel.Text = "(1/3) Select an installed story to play";

			Engine.AddUpdateTask("NewGameEscape", EscapeProcedure);
			Engine.FocusedWidget = StoryMenu;
		};

		Sections.NewGame.OnHide = delegate (WidgetGroup wg)
		{
			StoryMenu.RemoveAllOptions();
			Engine.RemoveUpdateTask("NewGameEscape");
			Engine.FocusedWidget = null;
		};
	}

	public static void ConstructLoadGame()
	{
		Sections.LoadGame = new();

		Label Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		Label Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		Label Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		Label Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		Label Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		Label LodGm = new(1, 5, @"\ Load a saved game", ConsoleColor.Yellow, ConsoleColor.Black);

		Label HelpLabel = new(1, 7, "Press [ESC] to go back");

		Label StatusLabel = new(1, 9);
		Menu SaveMenu = new(1, 10) { SelectedOptionStyle = MenuStyle.Highlighted };

		Sections.LoadGame.Widgets.Add(Line1);
		Sections.LoadGame.Widgets.Add(Line2);
		Sections.LoadGame.Widgets.Add(Line3);
		Sections.LoadGame.Widgets.Add(Line4);
		Sections.LoadGame.Widgets.Add(Line5);
		Sections.LoadGame.Widgets.Add(LodGm);
		Sections.LoadGame.Widgets.Add(HelpLabel);
		Sections.LoadGame.Widgets.Add(StatusLabel);
		Sections.LoadGame.Widgets.Add(SaveMenu);

		Sections.LoadGame.OnShow = delegate (WidgetGroup wg)
		{
			StatusLabel.Text = "Getting installed stories...";

			int Count = 0;

			foreach (var Save in GameAPI.GetSaves())
			{
				Count++;
				SaveMenu.AddOption(Save.Name, delegate ()
				{
					SaveAPI.LoadGameFromSave(Save.Path);
					GameSession.SkipBootupSequence = true;
					Current = Sections.Bootup;
				});
			}

			StatusLabel.Text = "Select a saved game to load";

			Engine.AddUpdateTask("LoadGameEscape", EscapeProcedure);
			Engine.FocusedWidget = SaveMenu;
		};

		Sections.LoadGame.OnHide = delegate (WidgetGroup wg)
		{
			SaveMenu.RemoveAllOptions();
			Engine.RemoveUpdateTask("LoadGameEscape");
			Engine.FocusedWidget = null;
		};
	}

	public static void ConstructOptions()
	{
		Sections.Options = new();

		Sections.Options.OnShow = delegate (WidgetGroup wg)
		{
			Engine.AddUpdateTask("OptionsEscape", EscapeProcedure);
		};

		Sections.Options.OnHide = delegate (WidgetGroup wg)
		{
			Engine.RemoveUpdateTask("OptionsEscape");
			Engine.FocusedWidget = null;
		};
	}

	public static void ConstructCredits()
	{
		Sections.Credits = new();

		Sections.Credits.OnShow = delegate (WidgetGroup wg)
		{
			Engine.AddUpdateTask("CreditsEscape", EscapeProcedure);
		};

		Sections.Credits.OnHide = delegate (WidgetGroup wg)
		{
			Engine.RemoveUpdateTask("CreditsEscape");
			Engine.FocusedWidget = null;
		};
	}

	public static void ConstructBootup()
	{
		Sections.Bootup = new();

		BootupConsole = new();

		Sections.Bootup.Widgets.Add(BootupConsole);

		Sections.Bootup.OnShow = delegate (WidgetGroup wg)
		{
			Engine.FocusedWidget = ReadKey;
			BootupConsole.Clear();
			GameAPI.CommenceBootupTask();
		};

		Sections.Bootup.OnHide = delegate (WidgetGroup wg)
		{
			Engine.FocusedWidget = null;
			BootupConsole.Clear();
		};
	}

	private static bool DoGameUIUpdate;

	public static void ConstructGame()
	{
		Sections.Game = new();

		GameConsole = new(0, 1, Console.WindowWidth - 2, Console.WindowHeight - 4);
		GameConsole.CursorBlinkIntervalMs = 500;

		InputHeader = new(0, 0);
		GameInput = new(0, Console.WindowHeight - 1, "# ");

		GameInput.OnInput = OnInput;

		Thread UpdateGameUIThread = new(UpdateGameUI) { Name = "UpdateGameUI" };
		
		Sections.Game.Widgets.Add(GameConsole);
		Sections.Game.Widgets.Add(InputHeader);
		Sections.Game.Widgets.Add(GameInput);

		Sections.Game.OnShow = delegate (WidgetGroup wg)
		{
			GameConsole.Clear();
			GameInput.Clear();

			if (UpdateGameUIThread is null)
				UpdateGameUIThread = new(UpdateGameUI) { Name = "UpdateGameUI" };

			DoGameUIUpdate = true;
			UpdateGameUIThread.Start();

			Engine.FocusedWidget = GameInput;
			Engine.AddUpdateTask("GameEscape", EscapeProcedure);
		};

		Sections.Game.OnHide = delegate (WidgetGroup wg)
		{
			DoGameUIUpdate = false;
			UpdateGameUIThread.Join();
			UpdateGameUIThread = null;

			Engine.FocusedWidget = null;
			Engine.RemoveUpdateTask("GameEscape");
		};
	}

	private static void UpdateGameUI()
	{
		while (DoGameUIUpdate)
		{
			string Username = GameSession.Player.CurrentSession.User.Username;

			InputHeader.Text = $"{Username} @ {GameSession.Player.CurrentSession.Host.Address} ({GameSession.Player.CurrentSession.Host.Name})";
			InputHeader.X = (Console.WindowWidth / 2) - (InputHeader.Text.Length / 2);

			GameInput.Prompt = $"{GameSession.Player.CurrentSession.PathNode.GetPath()} # ";

			Thread.Sleep(100);
		}
	}

	private static void OnInput(string Input)
	{
		InputQuery UserQuery = Parser.Parse(Input);

		GameConsole.WriteLine($"{GameInput.Prompt}{Input}");

		Task.Run(delegate ()
		{
			GameAPI.HandleUserInput(UserQuery);
		});
	}
}