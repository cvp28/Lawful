using Haven;

namespace Lawful.GameLibrary.UI;

public class FPSLayer : Layer
{
	[Widget] Label FPSLabel;

	public FPSLayer()
	{
		FPSLabel = new(0, 0);

		AddWidgetsInternal();
	}

	public override void OnShow(App a, object[] Args) { }
	public override void OnHide(App a) { }

	[UpdateTask]
	private void Update(State s)
	{
		FPSLabel.Text = string.Create(null, stackalloc char[64], $"FPS: {s.FPS} :: {s.LastFrameTime} ms");
	}

	public override void UpdateLayout(Dimensions d)
	{
		FPSLabel.X = d.WindowWidth - FPSLabel.Text.Length - 1;
		FPSLabel.Y = 0;
	}
}
