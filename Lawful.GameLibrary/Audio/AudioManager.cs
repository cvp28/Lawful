using Un4seen.Bass;

namespace Lawful.GameLibrary;

using static GameSession;


public class AudioManager
{
	private Dictionary<string, int> ActiveStreams;

	public AudioManager()
	{
		ActiveStreams = new();
	}

	/// <summary>
	/// Creates a stream for an audio file and assigns it an ID
	/// </summary>
	/// <param name="ID">Internal ID for this stream</param>
	/// <param name="Path">Game-root-relative path to the relevant file</param>
	/// <returns>BASS handle to the stream</returns>
	public void CreateStream(string ID, string Path)
	{
		int TryHandle = Bass.BASS_StreamCreateFile($@"{Path}", 0, 0, BASSFlag.BASS_DEFAULT);

		if (TryHandle == 0)
			Log.WriteLine($"AudioManager :: Failed to create BASS stream '{ID}' at '{Path}'");
		else
		{
			Log.WriteLine($"AudioManager :: Created BASS stream '{ID}' at '{Path}'");
			ActiveStreams.Add(ID, TryHandle);
		}
	}

	public int GetStream(string ID)
	{
		if (!ActiveStreams.ContainsKey(ID))
			return 0;

		return ActiveStreams[ID];
	}

	public bool FreeStream(string ID)
	{
		if (!ActiveStreams.ContainsKey(ID))
			return false;

		bool Freed = Bass.BASS_StreamFree(ActiveStreams[ID]);
		ActiveStreams.Remove(ID);
		Log.WriteLine($"AudioManager :: Freed stream '{ID}'");

		return Freed;
	}

	public void FreeAll()
	{
		foreach (var kvp in ActiveStreams)
		{
			if (Bass.BASS_StreamFree(kvp.Value))
				ActiveStreams.Remove(kvp.Key);
			else
				Log.WriteLine($"AudioManager :: BASS_StreamFree failed with '{Bass.BASS_ErrorGetCode()}'");
		}
	}

	public bool Play(string ID, bool RestartCurrent)
	{
		if (!ActiveStreams.ContainsKey(ID))
			return false;

		bool Play = Bass.BASS_ChannelPlay(ActiveStreams[ID], RestartCurrent);

		if (!Play)
			Log.WriteLine($"AudioManager :: BASS_ChannelPlay failed with '{Bass.BASS_ErrorGetCode()}'");

		return Play;
	}

	public bool Pause(string ID)
	{
		if (!ActiveStreams.ContainsKey(ID))
			return false;

		bool Pause = Bass.BASS_ChannelPause(ActiveStreams[ID]);

		if (!Pause)
			Log.WriteLine($"AudioManager :: BASS_ChannelPause failed with '{Bass.BASS_ErrorGetCode()}'");

		return Pause;
	}

	public bool SetVolume(string ID, float Volume)
	{
		if (!ActiveStreams.ContainsKey(ID))
			return false;

		bool SetVolume = Bass.BASS_ChannelSetAttribute(ActiveStreams[ID], BASSAttribute.BASS_ATTRIB_VOL, Volume);

		if (!SetVolume)
			Log.WriteLine($"AudioManager :: BASS_ChannelSetAttribute failed with '{Bass.BASS_ErrorGetCode()}'");

		return SetVolume;
	}

	public bool Stop(string ID)
	{
		if (!ActiveStreams.ContainsKey(ID))
			return false;

		bool Stop = Bass.BASS_ChannelStop(ActiveStreams[ID]);

		if (!Stop)
			Log.WriteLine($"AudioManager :: BASS_ChannelStop failed with '{Bass.BASS_ErrorGetCode()}'");

		return Stop;
	}


	public bool StopAll()
	{
		bool Stop = false;

		foreach (var kvp in ActiveStreams)
		{
			Stop = Bass.BASS_ChannelStop(kvp.Value);

			if (!Stop)
				Log.WriteLine($"AudioManager :: BASS_ChannelStop failed with '{Bass.BASS_ErrorGetCode()}'");
		}

		return Stop;
	}
}
