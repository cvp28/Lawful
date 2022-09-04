using System.Collections.Concurrent;
using Un4seen.Bass;

namespace Lawful.GameLibrary;

using static UI.UIManager;

public class StreamGroup
{
	public ConcurrentDictionary<string, int> Streams;

	public StreamGroup() => Streams = new();

	public bool AddStream(string ID, int Stream) => Streams.TryAdd(ID, Stream);

	public void FreeAllStreams()
	{
		foreach (var kvp in Streams)
		{
			Bass.BASS_StreamFree(kvp.Value);
			Streams.TryRemove(kvp);
		}
	}
}

public static class AudioManager
{
	private static Dictionary<string, int> ActiveStreams;
	private static ConcurrentDictionary<string, StreamGroup> ActiveStreamGroups;

	public static void Initialize()
	{
		ActiveStreams = new();

		Log.Write("AudioManager :: Initializing BASS... ");

		bool BassInit = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

		if (!BassInit)
			Log.WriteLine($"failed: {Bass.BASS_ErrorGetCode()}");
		else
			Log.WriteLine("done.");
	}

	public static bool AddStreamGroup(string ID, StreamGroup sg) => ActiveStreamGroups.TryAdd(ID, sg);

	public static StreamGroup GetStreamGroup(string ID)
	{
		if (!ActiveStreamGroups.TryGetValue(ID, out StreamGroup sg))
			return null;
		else
			return sg;
	}

	

	/// <summary>
	/// Creates a stream for an audio file and assigns it an ID
	/// </summary>
	/// <param name="ID">Internal ID for this stream</param>
	/// <param name="Path">Game-root-relative path to the relevant file</param>
	/// <returns>BASS handle to the stream</returns>
	public static int CreateStream(string ID, string Path)
	{
		int TryHandle = Bass.BASS_StreamCreateFile($@"{Path}", 0, 0, BASSFlag.BASS_DEFAULT);

		if (TryHandle == 0)
			Log.WriteLine($"AudioManager :: Failed to create BASS stream '{ID}' at '{Path}'");
		else
		{
			Log.WriteLine($"AudioManager :: Created BASS stream '{ID}' at '{Path}'");
			ActiveStreams.Add(ID, TryHandle);
		}

		return TryHandle;
	}

	public static bool GetStream(string ID, out int Stream)
	{
		if (!ActiveStreams.ContainsKey(ID))
		{
			Stream = 0;
			return false;
		}

		Stream = ActiveStreams[ID];
		return true;
	}

	public static bool FreeStream(string ID)
	{
		if (!ActiveStreams.ContainsKey(ID))
			return false;

		return Bass.BASS_StreamFree(ActiveStreams[ID]);
	}

	public static void FreeAll()
	{
		foreach (var kvp in ActiveStreams)
			Bass.BASS_StreamFree(kvp.Value);
	}
}
