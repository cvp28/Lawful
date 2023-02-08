using System.Collections.Concurrent;
using Haven;

namespace Lawful.GameLibrary;

public class ReadKeyBridge : Widget
{
	private ConcurrentQueue<ConsoleKeyInfo> InputBuffer;
	public bool KeyAvailable => InputBuffer.Count > 0;

	public ReadKeyBridge() : base()
	{
		InputBuffer = new();
		Visible = false;
	}

	public override void Draw(Haven.Renderer s) { }

	public override void OnConsoleKey(ConsoleKeyInfo cki) => InputBuffer.Enqueue(cki);

	public ConsoleKeyInfo ReadKey(bool DiscardCurrentBuffer)
	{
		ConsoleKeyInfo cki;

		if (DiscardCurrentBuffer)
			InputBuffer.Clear();

		while (!KeyAvailable)
			Thread.Sleep(10);

		while (!InputBuffer.TryDequeue(out cki))
			Thread.Sleep(10);

		return cki;
	}
}
