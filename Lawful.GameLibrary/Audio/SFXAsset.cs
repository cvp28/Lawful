using System.Xml.Serialization;
using System.Runtime.InteropServices;

using Un4seen.Bass;


namespace Lawful.GameLibrary;

using static GameSession;

public class SFXAsset
{
	[XmlAttribute("ID")]
	public string ID;

	[XmlAttribute("Path")]
	public string Path;

	[XmlAttribute("Volume")]
	public float Volume;

	[XmlIgnore]
	private GCHandle DataHandle;

	[XmlIgnore]
	public byte[] Data { get; private set; }

	private static readonly object bass_lock = new();
	private List<int> ActiveStreams;

	public bool Load()
	{
		ActiveStreams = new();

		try
		{
			Data = File.ReadAllBytes($"{GameSession.CurrentStoryRoot}\\{Path}".ToPlatformPath());
			DataHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
		}
		catch (Exception ex)
		{
			Log.WriteLine($"SFXAsset ({ID}) :: Failed to load with exception '{ex.Message}'");
			return false;
		}

		return true;
	}

	public void Play()
	{
		Task.Run(delegate ()
		{
			int Handle = 0;

			lock (bass_lock)
			{
				Handle = Bass.BASS_StreamCreateFile(DataHandle.AddrOfPinnedObject(), 0, Data.LongLength, BASSFlag.BASS_STREAM_AUTOFREE);
			}

			if (Handle == 0)
			{
				Log.WriteLine($"SFXAsset ({ID}) :: Failed to play with '{Bass.BASS_ErrorGetCode()}'");
				return;
			}

			Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_VOL, Volume);
			Bass.BASS_ChannelPlay(Handle, false);

			ActiveStreams.Add(Handle);

			while (Bass.BASS_ChannelIsActive(Handle) == BASSActive.BASS_ACTIVE_PLAYING)
				Thread.Sleep(50);

			ActiveStreams.Remove(Handle);
		});
	}

	public bool Free()
	{
		foreach (int Stream in ActiveStreams)
			Bass.BASS_ChannelStop(Stream);

		DataHandle.Free();

		Data = null;
		return true;
	}
}
