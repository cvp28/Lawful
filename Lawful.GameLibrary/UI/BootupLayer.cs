using Haven;

namespace Lawful.GameLibrary.UI;

public class BootupLayer : Layer
{
	[Widget] public TextBox BootupConsole;

	public ReadKeyBridge ReadKey;

	public BootupLayer() : base()
	{
		BootupConsole = new();
		ReadKey = new();

		AddWidgetsInternal();
	}

	public override void OnShow(App a, object[] Args)
	{
		// GameSession.SkipBootupSequence = true;

		App.Instance.FocusedWidget = ReadKey;
		GameAPI.CommenceBootupTask();
	}

	public override void OnHide(App a)
	{
		App.Instance.FocusedWidget = null;
		BootupConsole.Clear();
	}

	public override void UpdateLayout(Dimensions d)
	{
		BootupConsole.Resize(d.WindowWidth - 2, d.WindowHeight - 2);
	}
}
