using Haven;

namespace Lawful.GameLibrary.UI;

public class LogLayer : Layer
{
	[Widget] public TextBox Log;

	public LogLayer()
	{
		Log = new(0, 0, Console.WindowWidth - 2, Console.WindowHeight - 2);
		Log.SetLogFile(@".\game_log.txt");

		Log.WriteLine("Lawful");
		Log.WriteLine("Dev Build - Feb. 2023");
		Log.NextLine();

		AddWidgetsInternal();
	}

	public override void OnShow(App a, object[] Args) { }
	public override void OnHide(App a) { }

	public override void UpdateLayout(Dimensions d)
	{
		Log.Resize(d.WindowWidth - 2, d.WindowHeight - 2);
	}
}
