using Haven;

namespace Lawful.GameLibrary.UI;

public class LoadGameLayer : Layer
{
	[Widget] Label Line1, Line2, Line3, Line4, Line5, LodGm;

	[Widget] Label HelpLabel, StatusLabel;

	[Widget] Menu SaveMenu;

	private bool LoadingGame;

	public LoadGameLayer() : base()
	{
		Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		LodGm = new(1, 5, @"\ Load a saved game /", ConsoleColor.Yellow, ConsoleColor.Black);

		HelpLabel = new(1, 7);
		StatusLabel = new(1, 9);

		SaveMenu = new(1, 10) { SelectedOptionStyle = MenuStyle.Highlighted };

		AddWidgetsInternal();
	}

	public override void OnShow(App a, object[] Args)
	{
		LoadingGame = false;
		HelpLabel.Text = string.Empty;
		StatusLabel.Text = "Getting saved games...";

		Task.Run(InitSaveMenu);
	}

	public override void OnHide(App a)
	{
		SaveMenu.RemoveAllOptions();
		a.RemoveUpdateTask("LoadGameEscape");
		a.FocusedWidget = null;
	}

	private void InitSaveMenu()
	{
		foreach (var Save in GameAPI.GetSaves())
			SaveMenu.AddOption(Save.Name, delegate ()
			{
				if (LoadingGame)
					return;

				LoadingGame = true;

				Task.Run(delegate ()
				{
					StatusLabel.Text = $"Loading saved game '{Save.Name}'...";

					bool SaveSucceeded = SaveAPI.LoadGameFromSave(Save.Path);

					if (SaveSucceeded)
						App.Instance.SetLayer(0, "Bootup");
					else
						StatusLabel.Text = $"Error loading saved game '{Save.Name}' - press F10 for details";

					LoadingGame = false;
				});
			});

		App.Instance.AddUpdateTask("LoadGameEscape", EscapeProcedure);

		StatusLabel.Text = "Select a saved game to load";
		HelpLabel.Text = "Press [ESC] to go back";

		App.Instance.FocusedWidget = SaveMenu;

	}

	private void EscapeProcedure(State s)
	{
		if (!s.KeyPressed)
			return;

		if (s.KeyInfo.Key == ConsoleKey.Escape)
			App.Instance.SetLayer(0, "MainMenu");
	}

	public override void UpdateLayout(Dimensions d)
	{
		Line1.CenterTo(d, 0, -8);
		Line2.CenterTo(d, 0, -7);
		Line3.CenterTo(d, 0, -6);
		Line4.CenterTo(d, 0, -5);
		Line5.CenterTo(d, 0, -4);
		LodGm.CenterTo(d, 0, -3);

		HelpLabel.CenterTo(d, 0, -1);

		StatusLabel.CenterTo(d, 0, 1);

		SaveMenu.CenterTo(d, 0, 3 + (int)Math.Ceiling(SaveMenu.OptionCount / 2.0f));
	}
}
