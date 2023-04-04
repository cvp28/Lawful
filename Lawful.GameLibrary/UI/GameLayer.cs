using Haven;
using Jint.Runtime.Debugger;
using Lawful.InputParser;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Lawful.GameLibrary.UI;

using static GameSession;

public class GameLayer : Layer
{
	public Func<string> PromptProvider;
	public static string GetDefaultPrompt() => $"{Player.CurrentSession.PathNode.GetPath()} # ";

	[Widget] private Label ConsoleTabLabel, NETChatTabLabel;
	[Widget] private Label Header;
	[Widget] public ScrollableTextBox GameConsole;
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

		Input = new(0, Console.WindowHeight - 1, "/ # ")
		{
			HighlightingEnabled = true
		};

		Input.OnRetrieveSuggestions = OnRetrieveSuggestions;
		Input.OnHighlight = OnHighlight;

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
	private void OnInput(State s)
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

			case ConsoleKey.PageUp:
				GameConsole.ScrollViewUp();
				break;

			case ConsoleKey.PageDown:
				GameConsole.ScrollViewDown();
				break;
		}
	}

	private void OnConsoleInput(string UserInput)
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

			EventManager.JSE.SetValue("G_UserQuery", "");
		}).Start();
	}

	private List<string> TempSuggestions = new();

	private IEnumerable<string> OnRetrieveSuggestions(Token Selected, IEnumerable<Token> CurrentTokens)
	{
		TempSuggestions.Clear();

		if (Selected.Content.Contains('/'))
		{
			bool Root = Selected.Content.StartsWith('/');

			var Tokens = Selected.Content.Split('/');
			var LastToken = Selected.Content.Split('/').Last();

			StringBuilder sb = new();

			for (int i = 0; i < Tokens.Length - 1; i++)
			{
				if (i == 0)
					sb.Append($"{(Root ? '/' : "")}{Tokens[i]}");
				else
					sb.Append($"/{Tokens[i]}");
			}

			var Children = (FSAPI.Locate(Player.CurrentSession, sb.ToString()) as XmlNode).ChildNodes;

			foreach (XmlNode n in Children)
			{
				if (n.Name == "Directory")
				{
					TempSuggestions.Add($"{sb}/{n.Attributes["Name"].Value}/");
				}
				else
				{
					TempSuggestions.Add($"{sb}/{n.Attributes["Name"].Value}");
				}

			}

			TempSuggestions.Add($"{sb}/..");
		}
		else
		{
			foreach (XmlNode x in Player.CurrentSession.PathNode.ChildNodes)
				TempSuggestions.Add(x.Attributes["Name"].Value);
		}


		return TempSuggestions.Where(m => Regex.IsMatch(m, $"^{Selected.Content.Replace(".", "\\.")}"));
	}

	private void OnHighlight(IEnumerable<Token> CurrentTokens)
	{
		if (!CurrentTokens.Any())
			return;

		foreach (Token t in CurrentTokens)
		{
			var Temp = FSAPI.Locate(Player.CurrentSession, t.Content);

			if (Temp is null)
				continue;

			// If file is executable
			if (Temp is XmlNode File && File.Attributes["Command"] is not null)
				t.HighlightForeground = ConsoleColor.Green;
			else
				t.HighlightForeground = ConsoleColor.Yellow;
		}

		{
			Token First = CurrentTokens.First();

			if (FSAPI.LocateFile(Player.CurrentSession, $"/bin/{First.Content}") is XmlNode File)
				if (File.Attributes["Command"] is not null)
					First.HighlightForeground = ConsoleColor.Green;
		}
	}

	public void ResetInput()
	{
		Input.Clear();
		Input.OnInput = OnConsoleInput;
		
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
