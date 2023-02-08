using Haven;

namespace Lawful.GameLibrary.UI;

using static GameSession;

public class OptionsLayer : Layer
{
	[Widget] Label Line1, Line2, Line3, Line4, Line5, CfgOp;

	[Widget] Menu OptionsMenu, RendererMenu;

	[Widget] InputField VolumeInput;

	public OptionsLayer() : base()
	{
		Line1 = new(1, 0, @"    __                           ____            __", ConsoleColor.Red, ConsoleColor.Black);
		Line2 = new(1, 1, @"   / /     ______   _      __   / __/  __  __   / /", ConsoleColor.Red, ConsoleColor.Black);
		Line3 = new(1, 2, @"  / /     / __  /  | | /| / /  / /_   / / / /  / / ", ConsoleColor.Red, ConsoleColor.Black);
		Line4 = new(1, 3, @" / /___  / /_/ /   | |/ |/ /  / __/  / /_/ /  / /  ", ConsoleColor.Red, ConsoleColor.Black);
		Line5 = new(1, 4, @"/_____/  \__/\_\   |__/|__/  /_/     \____/  /_/   ", ConsoleColor.Red, ConsoleColor.Black);
		CfgOp = new(1, 5, @"\ Configure game options /", ConsoleColor.Yellow, ConsoleColor.Black);

		OptionsMenu = new(1, 7);
		OptionsMenu.AddOption("Typewriter         : ", ToggleTypewriter);
		OptionsMenu.AddOption("Typewriter Volume  : ", ChangeTypewriterVolume);
		OptionsMenu.AddOption("Selected Renderer  : ", ChangeRenderer);
		OptionsMenu.AddOption("FPS Display        : ", ToggleFPSOverlay);

		RendererMenu = new(1, 8) { SelectedOptionStyle = MenuStyle.Highlighted };
		RendererMenu.AddOption("Cross Platform", SetCrossPlatformRenderer);

		if (OperatingSystem.IsWindows())
			RendererMenu.AddOption("Windows Native", SetWindowsNativeRenderer);

		VolumeInput = new(1, 9, "New Volume (0.0 < x <= 1.0): ")
		{
			Filter = InputFilter.NumericsWithSingleDot,
			OnInput = OnVolumeInput
		};

		AddWidgetsInternal();
	}

	private void ToggleTypewriter()
	{
		CurrentConfig.EnableTypewriter = !CurrentConfig.EnableTypewriter;
		OptionsMenu[0].Text = $"Typewriter         : {(CurrentConfig.EnableTypewriter ? "Enabled" : "Disabled")}";

		if (!GameAPI.TypewriterInitialized)
			GameAPI.InitTypewriter();

		if (CurrentConfig.EnableTypewriter)
			App.AddUpdateTask("Typewriter", GameAPI.TypewriterTask);
		else
			App.RemoveUpdateTask("Typewriter");

		GameAPI.WriteCurrentConfig();
	}

	private void ChangeTypewriterVolume()
	{
		OptionsMenu.Visible = false;
		VolumeInput.Visible = true;

		App.Instance.FocusedWidget = VolumeInput;
	}

	private void ChangeRenderer()
	{
		OptionsMenu.Visible = false;
		RendererMenu.Visible = true;
		App.Instance.FocusedWidget = RendererMenu;
	}

	private void ToggleFPSOverlay()
	{
		CurrentConfig.ShowFPS = !CurrentConfig.ShowFPS;
		GameAPI.WriteCurrentConfig();

		OptionsMenu[3].Text = $"FPS Display        : {(CurrentConfig.ShowFPS ? "Enabled" : "Disabled")}";

		if (CurrentConfig.ShowFPS)
			App.Instance.SetLayer(2, "FPS");
		else
			App.Instance.SetLayer(2);
	}

	private void SetCrossPlatformRenderer()
	{
		CurrentConfig.SelectedRenderer = Renderer.CrossPlatform;
		GameAPI.WriteCurrentConfig();

		if (Renderer.CrossPlatform != GameAPI.CurrentRenderer)
			OptionsMenu[2].Text = $"Selected Renderer  : {CurrentConfig.SelectedRenderer} - restart needed";
		else
			OptionsMenu[2].Text = $"Selected Renderer  : {CurrentConfig.SelectedRenderer}";

		RendererMenu.X = OptionsMenu[2].Text.Length + 2;

		RendererMenu.Visible = false;
		App.FocusedWidget = OptionsMenu;
		OptionsMenu.Visible = true;
	}

	private void SetWindowsNativeRenderer()
	{
		CurrentConfig.SelectedRenderer = Renderer.WindowsNative;
		GameAPI.WriteCurrentConfig();

		if (Renderer.WindowsNative != GameAPI.CurrentRenderer)
			OptionsMenu[2].Text = $"Selected Renderer  : {CurrentConfig.SelectedRenderer} - restart needed";
		else
			OptionsMenu[2].Text = $"Selected Renderer  : {CurrentConfig.SelectedRenderer}";

		RendererMenu.X = OptionsMenu[2].Text.Length + 2;

		RendererMenu.Visible = false;
		App.FocusedWidget = OptionsMenu;
		OptionsMenu.Visible = true;
	}

	public override void OnShow(App a, object[] Args)
	{
		RendererMenu.Visible = false;
		VolumeInput.Visible = false;

		// Construct options
		OptionsMenu[0].Text = $"Typewriter         : {(CurrentConfig.EnableTypewriter ? "Enabled" : "Disabled")}";
		OptionsMenu[1].Text = $"Typewriter Volume  : {CurrentConfig.TypewriterVolume}";

		if (CurrentConfig.SelectedRenderer != GameAPI.CurrentRenderer)
			OptionsMenu[2].Text = $"Selected Renderer  : {CurrentConfig.SelectedRenderer} - restart needed";
		else
			OptionsMenu[2].Text = $"Selected Renderer  : {CurrentConfig.SelectedRenderer}";

		OptionsMenu[3].Text = $"FPS Display        : {(CurrentConfig.ShowFPS ? "Enabled" : "Disabled")}";

		App.FocusedWidget = OptionsMenu;
	}

	public override void OnHide(App a)
	{
		// Reset controls to default state
		RendererMenu.Visible = false;
		VolumeInput.Visible = false;
		OptionsMenu.Visible = true;

		a.FocusedWidget = null;
	}

	[UpdateTask]
	private void EscapeProcedure(State s)
	{
		if (!s.KeyPressed)
			return;

		if (s.KeyInfo.Key == ConsoleKey.Escape)
			App.Instance.SetLayer(0, "MainMenu");
	}

	private void OnVolumeInput(string Input)
	{
		if (!float.TryParse(Input, out float NewVolume))
			return;

		if (NewVolume <= 0.0 || NewVolume > 1.0)
			return;

		CurrentConfig.TypewriterVolume = NewVolume;

		GameAPI.UpdateTypewriterVolume();
		GameAPI.WriteCurrentConfig();

		OptionsMenu[1].Text = $"Typewriter Volume  : {CurrentConfig.TypewriterVolume}";
		VolumeInput.X = OptionsMenu[1].Text.Length + 2;

		VolumeInput.Visible = false;

		App.FocusedWidget = OptionsMenu;
		OptionsMenu.Visible = true;
	}

	public override void UpdateLayout(Dimensions d)
	{
		Line1.CenterTo(d, 0, -8);
		Line2.CenterTo(d, 0, -7);
		Line3.CenterTo(d, 0, -6);
		Line4.CenterTo(d, 0, -5);
		Line5.CenterTo(d, 0, -4);
		CfgOp.CenterTo(d, 0, -3);

		OptionsMenu.CenterTo(d, 0, 1);
		RendererMenu.CenterTo(d, 0, 1);
		VolumeInput.CenterTo(d, 0, 1);
	}

}
