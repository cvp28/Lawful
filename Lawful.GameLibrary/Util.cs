using Haven;

namespace Lawful.GameLibrary;

using static GameSession;
using static UI.UIManager;

public static class Util
{
	public static void PrintPrompt()
	{
		GameConsole.Write("/ [");
		GameConsole.WriteColor(Player.CurrentSession.User.Username, ConsoleColor.Green);
		GameConsole.Write("] [");
		GameConsole.WriteColor(Player.CurrentSession.Host.Address, ConsoleColor.Green);
		GameConsole.WriteLine(']');

		GameConsole.Write($"\\ {Player.CurrentSession.PathNode.GetPath()} > ");

		//	Console.Write($"\\ [{Player.ConnectionInfo.Drive.Label}] {Player.ConnectionInfo.Path} > ");
		//	Console.WriteLine($"/ [{Player.ConnectionInfo.User.Username} @ {Player.ConnectionInfo.PC.Address}]");
	}

	public static void WriteColor(this HConsole Con, string text, ConsoleColor color)
	{
		ConsoleColor initialcolor = Con.ForegroundColor;
		Con.ForegroundColor = color;
		Con.Write(text);
		Con.ForegroundColor = initialcolor;
	}

	public static void WriteLineColor(this HConsole Con, string text, ConsoleColor color)
	{
		ConsoleColor initialcolor = Con.ForegroundColor;
		Con.ForegroundColor = color;
		Con.WriteLine(text);
		Con.ForegroundColor = initialcolor;
	}

	public static void WriteDynamic(this HConsole Con, string text, int delaymiliseconds)
	{
		foreach (char c in text)
		{
			Con.Write(c);
			Thread.Sleep(delaymiliseconds);
		}
	}

	public static void WriteDynamicColor(this HConsole Con, string text, int delaymiliseconds, ConsoleColor color)
	{
		ConsoleColor initialcolor = Con.ForegroundColor;
		Con.ForegroundColor = color;
		Con.WriteDynamic(text, delaymiliseconds);
		Con.ForegroundColor = initialcolor;
	}

	//	public static string ReadLineSecret()
	//	{
	//		ConsoleColor RevertTo = Console.ForegroundColor;
	//		GameConsole.ForegroundColor = Console.BackgroundColor;
	//	
	//		GameConsole.CursorVisible = false;
	//		string Input = GameConsole.ReadLine();
	//		GameConsole.CursorY--;
	//		GameConsole.CursorVisible = true;
	//	
	//	
	//		Console.ForegroundColor = RevertTo;
	//	
	//		return Input;
	//	}

	//	public static void BeginSpinningCursorAnimation(string[] Frames, int CycleCountLowerLimit, int CycleCountUpperLimit, int SleepIntervalMilliseconds, (int X, int Y) Position)
	//	{
	//		Console.CursorVisible = false;
	//	
	//		Random rand = new(DateTime.UtcNow.Second);
	//	
	//		int Count = 0;
	//	
	//		for (int i = 0; i < rand.Next(CycleCountLowerLimit, CycleCountUpperLimit); i++)
	//		{
	//			if (Count == Frames.Length)
	//			{
	//				Count = 0;
	//				Console.SetCursorPosition(Position.X, Position.Y);
	//			}
	//	
	//			Console.SetCursorPosition(Position.X, Position.Y);
	//	
	//			Console.Write(Frames[Count]);
	//	
	//			Count++;
	//	
	//			Thread.Sleep(SleepIntervalMilliseconds);
	//		}
	//	
	//		Console.SetCursorPosition(Position.X, Position.Y);
	//	
	//		Console.CursorVisible = true;
	//	}

	public static void BeginCharacterAnimation(this HConsole Con, char[] Frames, int CycleCountLowerLimit, int CycleCountUpperLimit, int SleepIntervalMilliseconds, (int X, int Y) Position)
	{
		Con.CursorVisible = false;
	
		Random rand = new(DateTime.UtcNow.Second);
	
		int Count = 0;
	
		for (int i = 0; i < rand.Next(CycleCountLowerLimit, CycleCountUpperLimit); i++)
		{
			if (Count == Frames.Length)
			{
				Count = 0;
				Con.SetCursorPosition(Position.X, Position.Y);
			}

			Con.SetCursorPosition(Position.X, Position.Y);

			Con.Write(Frames[Count]);
	
			Count++;
	
			Thread.Sleep(SleepIntervalMilliseconds);
		}

		Con.SetCursorPosition(Position.X, Position.Y);

		Con.CursorVisible = true;
	}

	// Basic login handler with a max tries counter
	// MaxTries set to 0 means infinite tries
	//	public static bool TryUserLogin(UserAccount Account, int MaxTries = 0)
	//	{
	//		if (Account.Password.Length == 0)
	//			return true;
	//	
	//		string Password;
	//		int Tries = 0;
	//	
	//		do
	//		{
	//			if (MaxTries > 0)
	//				if (Tries == MaxTries)
	//					return false;
	//	
	//			if (MaxTries > 0)
	//				Console.Write($"({MaxTries - Tries}) ");
	//	
	//			Console.WriteLine($"Password for '{Account.Username}': ");
	//			Password = ReadLineSecret();
	//	
	//			Tries++;
	//	
	//			if (Password == "$cancel")
	//				return false;
	//		}
	//		while (Password != Account.Password);
	//	
	//		return true;
	//	}
}