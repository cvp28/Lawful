using Haven;
using Lawful.InputParser;

namespace Lawful.GameLibrary.UI;

using static GameSession;

public class GameLayer : Layer
{
	public Func<string> PromptProvider;
	public static string GetDefaultPrompt() => $"{Player.CurrentSession.PathNode.GetPath()} # ";

	[Widget] private Label ConsoleTabLabel, NETChatTabLabel;
	[Widget] private Label Header;
	[Widget] public TextBox GameConsole;
	[Widget] public InputField Input;

	public GameLayer() : base()
	{
		ConsoleTabLabel = new(1, 0, "Console", ConsoleColor.Black, ConsoleColor.White);
		NETChatTabLabel = new(9, 0, "NETChat", ConsoleColor.White, ConsoleColor.Black);

		Header = new(0, 0);

		GameConsole = new(0, 1, Console.WindowWidth - 2, Console.WindowHeight - 4)
		{
			CursorBlinkIntervalMs = 500
		};

		Input = new(0, Console.WindowHeight - 1, "/ # ");

		PromptProvider = GetDefaultPrompt;

		AddWidgetsInternal();
	}

	[UpdateTask]
	private void UpdateUI(State s)
	{
		Header.Text = $"{Player.CurrentSession.User.Username} @ {Player.CurrentSession.Host.Address} ({Player.CurrentSession.Host.Name})";
		Input.Prompt = PromptProvider();
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

				App.Instance.SetLayer(0, "Game");
				break;

			case ConsoleKey.F2:

				if (App.Instance.IsLayerVisible("NETChat"))
					return;

				App.Instance.SetLayer(0, "NETChat");
				break;
		}
	}

	private void OnInput(string UserInput)
	{
		InputQuery UserQuery = Parser.Parse(UserInput);

		GameConsole.WriteLine($"{Input.Prompt}{UserInput}");

		new Thread(delegate ()
		{
			EventManager.JSE.SetValue("G_UserQuery", UserQuery);

			EventManager.HandleEventsByTrigger(Trigger.CommandEntered);

			Log.WriteLine($"UIManager :: HandleUserInput task for '{UserInput}' starting...");
			GameAPI.HandleUserInput(UserQuery);
			Log.WriteLine($"UIManager :: HandleUserInput task for '{UserInput}' finished");

			EventManager.HandleEventsByTrigger(Trigger.CommandExecuted);
		}).Start();
	}

	public void ResetInput()
	{
		Input.Clear();
		Input.OnInput = OnInput;

		PromptProvider = GetDefaultPrompt;
	}

	public override void OnShow(App a, object[] Args)
	{
		try
		{
			bool ResetConsole = (bool) Args[0];

			if (ResetConsole)
			{
				GameConsole.Clear();
				ResetInput();
			}
		}
		catch (Exception)
		{ }

		a.FocusedWidget = Input;
	}

	public override void OnHide(App a)
	{
		App.Instance.FocusedWidget = null;
	}

	public override void UpdateLayout(Dimensions d)
	{
		Header.X = d.HorizontalCenter - (Header.Text.Length / 2);

		GameConsole.Resize(d.WindowWidth - 2, d.WindowHeight - 4);
		Input.Y = d.WindowHeight - 1;
	}
}
