using System.Collections.Concurrent;

using Haven;

namespace Lawful.GameLibrary.UI;

using static GameSession;

public class NotifyLayer : Layer
{
	[Widget] private TextBox NotifyBox;

	private ConcurrentQueue<Notification> NotificationQueue;
	private Thread NotifyThread;
	private bool NotifyActive;

	public NotifyLayer() : base()
	{
		NotifyBox = new(0, 0, 5, 3)
		{
			Visible = false,
			CursorVisible = false
		};

		NotificationQueue = new();

		AddWidgetsInternal();
	}

	private void NotificationThread()
	{
		while (NotifyActive)
		{
			if (NotificationQueue.Count == 0)
			{
				Thread.Sleep(100);
				continue;
			}

			// Retrieve the next notification in the queue
			NotificationQueue.TryDequeue(out Notification CurrentNotification);

			// Resize box
			NotifyBox.Resize(CurrentNotification.MaxLineLength + 1, CurrentNotification.LineCount);

			// Set the box to be visible, play the notification noise, and start flashing the text
			NotifyBox.Visible = true;

			CurrentNotification.NotificationAction();
			MainAudioOut.Play(CurrentNotification.SoundStreamID, true);

			for (int i = 0; i < CurrentNotification.FlashCount; i++)
			{
				NotifyBox.Clear();

				// Flash yellow for 1/2 a second
				NotifyBox.Write(CurrentNotification.Text, ConsoleColor.Yellow, ConsoleColor.Black);
				Thread.Sleep(500);

				NotifyBox.Clear();

				// Flash white for 1/2 a second
				NotifyBox.Write(CurrentNotification.Text, ConsoleColor.White, ConsoleColor.Black);
				Thread.Sleep(500);
			}

			// At the end of the notification, make the box invisible
			NotifyBox.Visible = false;
		}
	}

	public override void OnShow(App a, object[] Args)
	{ }

	public override void OnHide(App a)
	{ }

	/// <summary>
	/// Pushes a notification into the notification queue
	/// </summary>
	/// <param name="NotificationText">The text for the notification</param>
	/// <param name="NotificationAction">The code to execute for the notification (gets queued up to execute when the notification is actually on screen)</param>
	/// <param name="FlashCount">The amount of times to flash the notification white & yellow</param>
	public void PushNotification(string NotificationText, string SoundStreamID, Action NotificationAction, int FlashCount = 3)
	{
		NotificationQueue.Enqueue(new(NotificationText, SoundStreamID, FlashCount, NotificationAction));
	}

	public void StartNotifyThread()
	{
		Log.WriteLine("NotifyLayer :: Starting Notify thread...");

		NotifyActive = true;

		NotifyThread = new(NotificationThread);
		NotifyThread.Start();
	}

	public void StopNotifyThread()
	{
		Log.WriteLine("NotifyLayer :: Stopping Notify thread...");

		NotifyActive = false;
		NotifyThread.Join();
	}

	public override void UpdateLayout(Dimensions d)
	{
		NotifyBox.X = d.WindowWidth - 2 - (NotifyBox.Width + 2);
		NotifyBox.Y = 2;
	}
}

public struct Notification
{
	public string Text;
	public string SoundStreamID;
	public int FlashCount;
	public Action NotificationAction;

	public int MaxLineLength => GetMaxLineLength();
	public int LineCount => Text.Count(c => c == '\n') + 1;

	public Notification(string Text, string SoundStreamID, int FlashCount, Action NotificationAction)
	{
		this.Text = Text;
		this.SoundStreamID = SoundStreamID;
		this.FlashCount = FlashCount;
		this.NotificationAction = NotificationAction;
	}

	private int GetMaxLineLength()
	{
		int MaxLength = 0;
		int CurrentLength = 0;

		for (int i = 0; i < Text.Length; i++)
		{
			if (Text[i] == '\n')
				CurrentLength = 0;
			else
				CurrentLength++;

			if (CurrentLength > MaxLength)
				MaxLength = CurrentLength;
		}

		return MaxLength;
	}
}