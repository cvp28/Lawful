using Haven;

using Lawful.InputParser;

namespace Lawful.GameLibrary.UI;

using static GameSession;

public static class UIManager
{

	public static void Initialize()
	{
		//	ConstructMainMenu();
		//	ConstructNewGame();
		//	ConstructLoadGame();
		//	ConstructOptions();
		//	ConstructBootup();
		//	ConstructGame();
		//	ConstructNETChat();

		MainMenuLayer MainMenu = new();
		NewGameLayer NewGame = new();
		LoadGameLayer LoadGame = new();
		OptionsLayer Options = new();
		BootupLayer Bootup = new();
		GameLayer Game = new();
		NETChatLayer NETChat = new();
		NotifyLayer Notify = new();

		FPSLayer FPSOverlay = new();
		LogLayer LogView = new();

		Log = LogView.Log;
		GameConsole = Game.GameConsole;

		App.AddLayer("MainMenu", MainMenu);
		App.AddLayer("NewGame", NewGame);
		App.AddLayer("LoadGame", LoadGame);
		App.AddLayer("Options", Options);
		App.AddLayer("Bootup", Bootup);
		App.AddLayer("Game", Game);
		App.AddLayer("NETChat", NETChat);
		App.AddLayer("Notify", Notify);

		App.AddLayer("FPS", FPSOverlay);
		App.AddLayer("Log", LogView);

		if (CurrentConfig.ShowFPS)
			App.SetLayer(2, "FPS");

		App.AddUpdateTask("ToggleLog", ToggleLog);

		App.SetLayer(0, "MainMenu");

		// Add UI to Engine
		//	App.AddWidgets(Sections.MainMenu);
		//	App.AddWidgets(Sections.NewGame);
		//	App.AddWidgets(Sections.LoadGame);
		//	App.AddWidgets(Sections.Options);
		//	App.AddWidgets(Sections.Credits);
		//	App.AddWidgets(Sections.Bootup);
		//	App.AddWidgets(Sections.Game);
		//	App.AddWidgets(Sections.NETChat);
	}

	//	private static void EscapeProcedure(State s)
	//	{
	//		if (!s.KeyPressed) { return; }
	//	
	//		switch (s.KeyInfo.Key)
	//		{
	//			case ConsoleKey.Escape:
	//				Current = Sections.MainMenu;
	//				break;
	//		}
	//	}

	private static void ToggleLog(State s)
	{
		if (!s.KeyPressed)
			return;

		switch (s.KeyInfo.Key)
		{
			case ConsoleKey.F10:

				if (!App.IsLayerVisible("Log"))
					App.SetLayer(3, "Log");
				else
					App.SetLayer(3);

				break;
		}
	}

	private static void ConstructMainMenu()
	{
		//	Sections.MainMenu = new();
		//	
		//	Label Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label DevSg = new(1, 5, @"\ A game by Carson Ver Planck", ConsoleColor.Yellow, ConsoleColor.Black);
		//	
		//	Menu Main = new(1, 7) { SelectedOptionStyle = MenuStyle.Highlighted };
		//	Main.AddOption("New Game",			delegate () { Current = Sections.NewGame; });
		//	Main.AddOption("Load Game",			delegate () { Current = Sections.LoadGame; });
		//	Main.AddOption("Options",			delegate () { Current = Sections.Options; });
		//	Main.AddOption("Credits",			delegate () { Current = Sections.Credits; });
		//	Main.AddOption("Exit",				App.SignalExit);
		//	Main.AddOption("Modal Test",		delegate () { App.DoModal(ConsoleKey.Enter, "Hello", "This is a test of", "the modal dialog thingy", "", "this is a really long string of text meant to test the bit of logic that handles really long strings of text"); });
		//	
		//	Sections.MainMenu.Widgets.Add(Line1);
		//	Sections.MainMenu.Widgets.Add(Line2);
		//	Sections.MainMenu.Widgets.Add(Line3);
		//	Sections.MainMenu.Widgets.Add(Line4);
		//	Sections.MainMenu.Widgets.Add(Line5);
		//	Sections.MainMenu.Widgets.Add(DevSg);
		//	Sections.MainMenu.Widgets.Add(Main);
		//	
		//	Sections.MainMenu.OnShow = delegate (WidgetGroup wg) { App.FocusedWidget = Main; };
		//	Sections.MainMenu.OnHide = delegate (WidgetGroup wg) { App.FocusedWidget = null; };
	}

	private static void ConstructNewGame()
	{
		//	Sections.NewGame = new();
		//	
		//	Label Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label NewGm = new(1, 5, @"\ Start a new game", ConsoleColor.Yellow, ConsoleColor.Black);
		//	
		//	Label HelpLabel = new(1, 7);
		//	
		//	Label StatusLabel = new(1, 9);
		//	
		//	Menu StoryMenu = new(1, 10) { SelectedOptionStyle = MenuStyle.Highlighted };
		//	InputField ProfileNameField = new(1, 10, "> ");
		//	InputField PCNameField = new(1, 10, "> ");
		//	
		//	Sections.NewGame.Widgets.Add(Line1);
		//	Sections.NewGame.Widgets.Add(Line2);
		//	Sections.NewGame.Widgets.Add(Line3);
		//	Sections.NewGame.Widgets.Add(Line4);
		//	Sections.NewGame.Widgets.Add(Line5);
		//	Sections.NewGame.Widgets.Add(NewGm);
		//	
		//	Sections.NewGame.Widgets.Add(HelpLabel);
		//	Sections.NewGame.Widgets.Add(StatusLabel);
		//	
		//	Sections.NewGame.Widgets.Add(StoryMenu);
		//	Sections.NewGame.Widgets.Add(ProfileNameField);
		//	Sections.NewGame.Widgets.Add(PCNameField);
		//	
		//	string SelectedStory = string.Empty;
		//	string ProfileName = string.Empty;
		//	string PCName = string.Empty;
		//	
		//	ProfileNameField.OnInput = delegate (string Input)
		//	{
		//		if (Input.Length == 0) { return; }
		//	
		//		if (Directory.Exists(@$".\Content\Saves\{Input}"))
		//			return;
		//	
		//		ProfileName = Input;
		//		ProfileNameField.Visible = false;
		//	
		//		PCNameField.Visible = true;
		//		App.FocusedWidget = PCNameField;
		//	
		//		StatusLabel.Text = "(3/3) Enter a PC name";
		//	};
		//	
		//	PCNameField.OnInput = delegate (string Input)
		//	{
		//		Task.Run(delegate ()
		//		{
		//			if (Input.Length == 0) { return; }
		//	
		//			PCName = Input;
		//	
		//			StatusLabel.Text = "Creating save... ";
		//	
		//			(bool Succeeded, string StoryPath) = SaveAPI.InitSave(PCName, ProfileName, SelectedStory);
		//	
		//			if (!Succeeded)
		//				StatusLabel.Text = "Creating save... failed. Check logs in game directory for more details.";
		//			else
		//				StatusLabel.Text = "Creating save... successful.";
		//		});
		//	};
		//	
		//	Sections.NewGame.OnShow = delegate (WidgetGroup wg)
		//	{
		//		Task.Run(delegate ()
		//		{
		//			ProfileNameField.Visible = false;
		//			PCNameField.Visible = false;
		//	
		//			ProfileNameField.Clear();
		//			PCNameField.Clear();
		//	
		//			HelpLabel.Text = string.Empty;
		//			StatusLabel.Text = "Getting installed stories...";
		//	
		//			foreach (string StoryName in GameAPI.GetInstalledStorylines())
		//			{
		//				StoryMenu.AddOption(StoryName, delegate ()
		//				{
		//					SelectedStory = StoryName;
		//					StoryMenu.Visible = false;
		//	
		//					ProfileNameField.Visible = true;
		//					App.FocusedWidget = ProfileNameField;
		//	
		//					StatusLabel.Text = "(2/3) Enter a profile name";
		//				});
		//			}
		//	
		//			StatusLabel.Text = "(1/3) Select an installed story to play";
		//	
		//			App.AddUpdateTask("NewGameEscape", EscapeProcedure);
		//			HelpLabel.Text = "Press [ESC] to go back";
		//		});
		//		App.FocusedWidget = StoryMenu;
		//	};
		//	
		//	Sections.NewGame.OnHide = delegate (WidgetGroup wg)
		//	{
		//		StoryMenu.RemoveAllOptions();
		//		App.RemoveUpdateTask("NewGameEscape");
		//		App.FocusedWidget = null;
		//	};
	}

	private static void ConstructLoadGame()
	{
		//	Sections.LoadGame = new();
		//	
		//	Label Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label LodGm = new(1, 5, @"\ Load a saved game", ConsoleColor.Yellow, ConsoleColor.Black);
		//	
		//	Label HelpLabel = new(1, 7);
		//	
		//	Label StatusLabel = new(1, 9);
		//	Menu SaveMenu = new(1, 10) { SelectedOptionStyle = MenuStyle.Highlighted };
		//	bool LoadingGame = false;
		//	
		//	Sections.LoadGame.Widgets.Add(Line1);
		//	Sections.LoadGame.Widgets.Add(Line2);
		//	Sections.LoadGame.Widgets.Add(Line3);
		//	Sections.LoadGame.Widgets.Add(Line4);
		//	Sections.LoadGame.Widgets.Add(Line5);
		//	Sections.LoadGame.Widgets.Add(LodGm);
		//	Sections.LoadGame.Widgets.Add(HelpLabel);
		//	Sections.LoadGame.Widgets.Add(StatusLabel);
		//	Sections.LoadGame.Widgets.Add(SaveMenu);
		//	
		//	Sections.LoadGame.OnShow = delegate (WidgetGroup wg)
		//	{
		//		Task.Run(delegate ()
		//		{
		//			HelpLabel.Text = string.Empty;
		//			StatusLabel.Text = "Getting saved games...";
		//	
		//			int Count = 0;
		//	
		//			foreach (var Save in GameAPI.GetSaves())
		//			{
		//				Count++;
		//				SaveMenu.AddOption(Save.Name, delegate ()
		//				{
		//					if (LoadingGame)
		//						return;
		//	
		//					LoadingGame = true;
		//	
		//					Task.Run(delegate ()
		//					{
		//						StatusLabel.Text = $"Loading saved game '{Save.Name}'...";
		//	
		//						bool SaveSucceeded = SaveAPI.LoadGameFromSave(Save.Path);
		//	
		//						if (SaveSucceeded)
		//							Current = Sections.Bootup;
		//						else
		//							StatusLabel.Text = $"Error loading saved game '{Save.Name}' - press F10 for details";
		//	
		//						LoadingGame = false;
		//					});
		//				});
		//			}
		//	
		//			StatusLabel.Text = "Select a saved game to load";
		//	
		//			App.AddUpdateTask("LoadGameEscape", EscapeProcedure);
		//			HelpLabel.Text = "Press [ESC] to go back";
		//		});
		//	
		//		App.FocusedWidget = SaveMenu;
		//	};
		//	
		//	Sections.LoadGame.OnHide = delegate (WidgetGroup wg)
		//	{
		//		SaveMenu.RemoveAllOptions();
		//		App.RemoveUpdateTask("LoadGameEscape");
		//		App.FocusedWidget = null;
		//	};
	}

	private static void ConstructOptions()
	{
		//	Sections.Options = new();
		//	
		//	Label Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		//	Label CfgOp = new(1, 5, @"\ Configure game options", ConsoleColor.Yellow, ConsoleColor.Black);
		//	
		//	Menu OptsMenu = new(1, 7);
		//	
		//	Menu RendererMenu = new(36, 9);
		//	RendererMenu.SelectedOptionStyle = MenuStyle.Highlighted;
		//	
		//	InputField TWVolumeInput = new(26, 8, " -> New Volume (0.0 < x <= 1.0): ");
		//	TWVolumeInput.Filter = InputFilter.NumericsWithSingleDot;
		//	
		//	TWVolumeInput.OnInput = delegate (string Input)
		//	{
		//		if (!float.TryParse(Input, out float NewVolume))
		//			return;
		//	
		//		if (NewVolume <= 0.0 || NewVolume > 1.0)
		//			return;
		//	
		//		CurrentConfig.TypewriterVolume = NewVolume;
		//	
		//		GameAPI.UpdateTypewriterVolume();
		//		GameAPI.WriteCurrentConfig();
		//	
		//		OptsMenu.EditOption(1, $"Typewriter Volume  : {CurrentConfig.TypewriterVolume}");
		//		TWVolumeInput.X = OptsMenu.GetOptionText(1).Length + 2;
		//	
		//		App.FocusedWidget = OptsMenu;
		//		TWVolumeInput.Visible = false;
		//	};
		//	
		//	OptsMenu.AddOption($"Typewriter         : ", delegate ()
		//	{
		//		CurrentConfig.EnableTypewriter = !CurrentConfig.EnableTypewriter;
		//		OptsMenu.EditOption(0, $"Typewriter         : {(CurrentConfig.EnableTypewriter ? "Enabled" : "Disabled")}");
		//	
		//		if (!GameAPI.TypewriterInitialized)
		//			GameAPI.InitTypewriter();
		//	
		//		if (CurrentConfig.EnableTypewriter)
		//			App.AddUpdateTask("Typewriter", GameAPI.TypewriterTask);
		//		else
		//			App.RemoveUpdateTask("Typewriter");
		//	
		//		GameAPI.WriteCurrentConfig();
		//	});
		//	
		//	OptsMenu.AddOption($"Typewriter Volume  : ", delegate ()
		//	{
		//		TWVolumeInput.Visible = true;
		//		App.FocusedWidget = TWVolumeInput;
		//	});
		//	
		//	OptsMenu.AddOption($"Selected Renderer  : ", delegate ()
		//	{
		//		RendererMenu.Visible = true;
		//		App.FocusedWidget = RendererMenu;
		//	});
		//	
		//	OptsMenu.AddOption($"FPS Display        : ", delegate ()
		//	{
		//		CurrentConfig.ShowFPS = !CurrentConfig.ShowFPS;
		//		OptsMenu.EditOption(3, $"FPS Display        : {(CurrentConfig.ShowFPS ? "Enabled" : "Disabled")}");
		//	
		//		if (CurrentConfig.ShowFPS)
		//		{
		//			App.AddWidget(FPSLabel);
		//			//App.AddWidget(RenderDiagLabel);
		//			App.AddUpdateTask("UpdateFPS", FPSUpdateTask);
		//		}
		//		else
		//		{
		//			App.RemoveWidget(FPSLabel);
		//			//App.RemoveWidget(RenderDiagLabel);
		//			App.RemoveUpdateTask("UpdateFPS");
		//		}
		//	});
		//	
		//	RendererMenu.AddOption("Cross Platform", delegate ()
		//	{
		//		CurrentConfig.SelectedRenderer = Renderer.CrossPlatform;
		//		GameAPI.WriteCurrentConfig();
		//	
		//		if (Renderer.CrossPlatform != GameAPI.CurrentRenderer)
		//			OptsMenu.EditOption(2, $"Selected Renderer  : {CurrentConfig.SelectedRenderer} - restart needed");
		//		else
		//			OptsMenu.EditOption(2, $"Selected Renderer  : {CurrentConfig.SelectedRenderer}");
		//	
		//		RendererMenu.X = OptsMenu.GetOptionText(2).Length + 2;
		//	
		//		RendererMenu.Visible = false;
		//		App.FocusedWidget = OptsMenu;
		//	});
		//	
		//	if (OperatingSystem.IsWindows())
		//		RendererMenu.AddOption("Windows Native", delegate ()
		//		{
		//			CurrentConfig.SelectedRenderer = Renderer.WindowsNative;
		//			GameAPI.WriteCurrentConfig();
		//	
		//			if (Renderer.WindowsNative != GameAPI.CurrentRenderer)
		//				OptsMenu.EditOption(2, $"Selected Renderer  : {CurrentConfig.SelectedRenderer} - restart needed");
		//			else
		//				OptsMenu.EditOption(2, $"Selected Renderer  : {CurrentConfig.SelectedRenderer}");
		//	
		//			RendererMenu.X = OptsMenu.GetOptionText(2).Length + 2;
		//	
		//			RendererMenu.Visible = false;
		//			App.FocusedWidget = OptsMenu;
		//		});
		//	
		//	Sections.Options.Widgets.Add(Line1);
		//	Sections.Options.Widgets.Add(Line2);
		//	Sections.Options.Widgets.Add(Line3);
		//	Sections.Options.Widgets.Add(Line4);
		//	Sections.Options.Widgets.Add(Line5);
		//	Sections.Options.Widgets.Add(CfgOp);
		//	Sections.Options.Widgets.Add(OptsMenu);
		//	Sections.Options.Widgets.Add(RendererMenu);
		//	Sections.Options.Widgets.Add(TWVolumeInput);
		//	
		//	Sections.Options.OnShow = delegate (WidgetGroup wg)
		//	{
		//		RendererMenu.Visible = false;
		//		TWVolumeInput.Visible = false;
		//	
		//		// Construct options
		//		OptsMenu.EditOption(0, $"Typewriter         : {(CurrentConfig.EnableTypewriter ? "Enabled" : "Disabled")}");
		//		OptsMenu.EditOption(1, $"Typewriter Volume  : {CurrentConfig.TypewriterVolume}");
		//	
		//		if (CurrentConfig.SelectedRenderer != GameAPI.CurrentRenderer)
		//			OptsMenu.EditOption(2, $"Selected Renderer  : {CurrentConfig.SelectedRenderer} - restart needed");
		//		else
		//			OptsMenu.EditOption(2, $"Selected Renderer  : {CurrentConfig.SelectedRenderer}");
		//	
		//		OptsMenu.EditOption(3, $"FPS Display        : {(CurrentConfig.ShowFPS ? "Enabled" : "Disabled")}");
		//	
		//		App.AddUpdateTask("OptionsEscape", EscapeProcedure);
		//		App.FocusedWidget = OptsMenu;
		//	};
		//	
		//	Sections.Options.OnHide = delegate (WidgetGroup wg)
		//	{
		//		RendererMenu.Visible = false;
		//		TWVolumeInput.Visible = false;
		//		App.RemoveUpdateTask("OptionsEscape");
		//		App.FocusedWidget = null;
		//	};
	}

	private static void ConstructBootup()
	{
		//	Sections.Bootup = new();
		//	
		//	BootupConsole = new();
		//	
		//	Sections.Bootup.Widgets.Add(BootupConsole);
		//	
		//	Sections.Bootup.OnShow = delegate (WidgetGroup wg)
		//	{
		//		Current = Sections.Game;
		//	
		//		//	App.FocusedWidget = ReadKey;
		//		//	BootupConsole.Clear();
		//		//	GameAPI.CommenceBootupTask();
		//	};
		//	
		//	Sections.Bootup.OnHide = delegate (WidgetGroup wg)
		//	{
		//		App.FocusedWidget = null;
		//		BootupConsole.Clear();
		//	};
	}

	//	private static bool ReinitializeGameSection = true;

	private static void ConstructGame()
	{
		//	Sections.Game = new();
		//	
		//	GameConsole = new(0, 1, Console.WindowWidth - 2, Console.WindowHeight - 4);
		//	GameConsole.CursorBlinkIntervalMs = 500;
		//	
		//	Label ConsoleTabLabel = new(1, 0, "Console", ConsoleColor.Black, ConsoleColor.White);
		//	Label NETChatTabLabel = new(9, 0, "NETChat", ConsoleColor.White, ConsoleColor.Black);
		//	
		//	InputHeader = new(0, 0);
		//	GameInput = new(0, Console.WindowHeight - 1, "# ");
		//	
		//	GameInput.OnInput = OnInput;
		//	
		//	Sections.Game.Widgets.Add(GameConsole);
		//	Sections.Game.Widgets.Add(InputHeader);
		//	Sections.Game.Widgets.Add(GameInput);
		//	Sections.Game.Widgets.Add(ConsoleTabLabel);
		//	Sections.Game.Widgets.Add(NETChatTabLabel);
		//	
		//	Sections.Game.OnShow = delegate (WidgetGroup wg)
		//	{
		//		if (ReinitializeGameSection)
		//		{
		//			GameConsole.Clear();
		//			GameInput.Clear();
		//			GamePromptProvider = GetDefaultPrompt;
		//			ReinitializeGameSection = false;
		//		}
		//	
		//		App.FocusedWidget = GameInput;
		//		App.AddUpdateTask("UpdateGameUI", UpdateGameUI);
		//		App.AddUpdateTask("AppSwitchTask", AppSwitchTask);
		//	};
		//	
		//	Sections.Game.OnHide = delegate (WidgetGroup wg)
		//	{
		//		App.FocusedWidget = null;
		//		App.RemoveUpdateTask("UpdateGameUI");
		//		App.RemoveUpdateTask("AppSwitchTask");
		//		ReinitializeGameSection = true;
		//	};
	}

	//	private static void AppSwitchTask(State s)
	//	{
	//		if (!s.KeyPressed)
	//			return;
	//	
	//		switch (s.KeyInfo.Key)
	//		{
	//			case ConsoleKey.F1:
	//				if (Current == Sections.Game)
	//					return;
	//				else
	//				{
	//					ReinitializeGameSection = false;
	//					Current = Sections.Game;
	//				}
	//				break;
	//	
	//			case ConsoleKey.F2:
	//				if (Current == Sections.NETChat)
	//					return;
	//				else
	//					Current = Sections.NETChat;
	//				break;
	//		}
	//	}

	//	private static void CenterInputHeader()
	//	{
	//		string Username = Player.CurrentSession.User.Username;
	//	
	//		InputHeader.Text = $"{Username} @ {Player.CurrentSession.Host.Address} ({Player.CurrentSession.Host.Name})";
	//		InputHeader.X = (Console.WindowWidth / 2) - (InputHeader.Text.Length / 2);
	//	}
	//	
	//	public static void UpdateGameUI(State s)
	//	{
	//		CenterInputHeader();
	//		GameInput.Prompt = GamePromptProvider();
	//	}

	//	public static Func<string> GamePromptProvider;
	//	
	//	public static string GetDefaultPrompt() => $"{Player.CurrentSession.PathNode.GetPath()} # ";

	//	public static void OnInput(string Input)
	//	{
	//		InputQuery UserQuery = Parser.Parse(Input);
	//	
	//		GameConsole.WriteLine($"{GameInput.Prompt}{Input}");
	//	
	//		new Thread(delegate ()
	//		{
	//			EventManager.JSE.SetValue("G_UserQuery", UserQuery);
	//	
	//			EventManager.HandleEventsByTrigger(Trigger.CommandEntered);
	//	
	//			Log.WriteLine($"UIManager :: HandleUserInput task for '{Input}' starting...");
	//			GameAPI.HandleUserInput(UserQuery);
	//			Log.WriteLine($"UIManager :: HandleUserInput task for '{Input}' finished");
	//	
	//			EventManager.HandleEventsByTrigger(Trigger.CommandExecuted);
	//		}).Start();
	//	
	//		//	Task.Run(delegate ()
	//		//	{
	//		//		EventManager.JSE.SetValue("UserQuery", UserQuery);
	//		//	
	//		//		EventManager.HandleEventsByTrigger(Trigger.CommandEntered);
	//		//	
	//		//		Log.WriteLine($"UIManager :: HandleUserInput task for '{Input}' starting...");
	//		//		GameAPI.HandleUserInput(UserQuery);
	//		//		Log.WriteLine($"UIManager :: HandleUserInput task for '{Input}' finished");
	//		//	
	//		//		EventManager.HandleEventsByTrigger(Trigger.CommandExecuted);
	//		//	});
	//	
	//	}

	//	public static void ConstructNETChat()
	//	{
	//		Sections.NETChat = new();
	//	
	//		Label ConsoleTabLabel = new(1, 0, "Console", ConsoleColor.White, ConsoleColor.Black);
	//		Label NETChatTabLabel = new(9, 0, "NETChat", ConsoleColor.Black, ConsoleColor.White);
	//	
	//		// NETChat UI
	//	
	//		Label Line1 = new(1, 0, @"    _   __ ______ ______ ______ __            __ ", ConsoleColor.Yellow, ConsoleColor.Black);
	//		Label Line2 = new(1, 1, @"   / | / // ____//_  __// ____// /_   ______ / /_", ConsoleColor.Yellow, ConsoleColor.Black);
	//		Label Line3 = new(1, 2, @"  /  |/ // __/    / /  / /    / __ \ / __  // __/", ConsoleColor.Yellow, ConsoleColor.Black);
	//		Label Line4 = new(1, 3, @" / /|  // /___   / /  / /___ / / / // /_/ // /_  ", ConsoleColor.Yellow, ConsoleColor.Black);
	//		Label Line5 = new(1, 4, @"/_/ |_//_____/  /_/   \____//_/ /_/ \__/\_\\__/  ", ConsoleColor.Yellow, ConsoleColor.Black);
	//		Label Line6 = new(1, 5, @"\ Select an option", ConsoleColor.White, ConsoleColor.Black);
	//	
	//		BulletList ContactsList = new(1, 7);
	//	
	//		Sections.NETChat.AddWidgets(ConsoleTabLabel, NETChatTabLabel);			// Add top tabs
	//		Sections.NETChat.AddWidgets(Line1, Line2, Line3, Line4, Line5, Line6);  // Add UI header
	//		Sections.NETChat.AddWidgets(ContactsList);								// Add contacts list
	//	
	//		Sections.NETChat.OnShow = delegate (WidgetGroup wg)
	//		{
	//			foreach (var Contact in Player.NETChatAccount.Contacts)
	//				ContactsList.AddChild(Contact.Username, Contact.Username, Contact.Online ? ConsoleColor.Green : ConsoleColor.Red);
	//	
	//			App.AddUpdateTask("AppSwitchTask", AppSwitchTask);
	//		};
	//	
	//		Sections.NETChat.OnHide = delegate (WidgetGroup wg)
	//		{
	//			ContactsList.Clear();
	//	
	//			App.RemoveUpdateTask("AppSwitchTask");
	//			App.FocusedWidget = null;
	//		};
	//	}
}