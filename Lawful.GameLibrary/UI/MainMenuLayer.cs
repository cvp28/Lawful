using Haven;

namespace Lawful.GameLibrary.UI;

public class MainMenuLayer : Layer
{
	[Widget] Label Line1, Line2, Line3, Line4, Line5, DevSg;

	[Widget] Menu Main;

	public MainMenuLayer() : base()
	{
		Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		DevSg = new(1, 5, @"\ A game by Carson Ver Planck /", ConsoleColor.Yellow, ConsoleColor.Black);

		Main = new(1, 7)
		{
			SelectedOptionStyle = MenuStyle.Highlighted,
			TextAlignment = Alignment.Center
		};

		Main.AddOption(" Start new game  ",		delegate () { App.Instance.SetLayer(0, "NewGame"); });
		Main.AddOption("Load a saved game",		delegate () { App.Instance.SetLayer(0, "LoadGame"); });
		Main.AddOption("     Options     ",		delegate () { App.Instance.SetLayer(0, "Options"); });
		Main.AddOption("      Exit       ",		App.Instance.SignalExit);

		AddWidgetsInternal();
	}

	public override void OnShow(App a, object[] Args)
	{
		a.FocusedWidget = Main;
	}

	public override void OnHide(App a)
	{
		a.FocusedWidget = null;
	}

	public override void UpdateLayout(Dimensions d)
	{
		Line1.CenterTo(d, 0, -8);
		Line2.CenterTo(d, 0, -7);
		Line3.CenterTo(d, 0, -6);
		Line4.CenterTo(d, 0, -5);
		Line5.CenterTo(d, 0, -4);
		DevSg.CenterTo(d, 0, -3);

		Main.CenterTo(d, 0, 2);
	}
}
