using System.Xml.Serialization;
using System.Runtime.InteropServices;

using Un4seen.Bass;

namespace Lawful.GameLibrary;

using static GameSession;

public class MusicAsset
{
	[XmlAttribute("ID")]
	public string ID;

	[XmlAttribute("Path")]
	public string Path;

	[XmlIgnore]
	public int BassHandle { get; private set; }

	[XmlIgnore]
	private GCHandle DataHandle;

	[XmlIgnore]
	public byte[] Data { get; private set; }

	public bool Load()
	{
		try
		{
			Data = File.ReadAllBytes($"{CurrentStoryRoot}\\{Path}".ToPlatformPath());
			DataHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
			BassHandle = Bass.BASS_StreamCreateFile(DataHandle.AddrOfPinnedObject(), 0, Data.LongLength, BASSFlag.BASS_DEFAULT);

			if (BassHandle == 0)
			{
				Log.WriteLine($"MusicAsset ({ID}) :: Failed to create BASS stream with '{Bass.BASS_ErrorGetCode()}'");
				return false;
			}
		}
		catch (Exception ex)
		{
			Log.WriteLine($"MusicAsset ({ID}) :: Failed to load with exception '{ex.Message}'");
			return false;
		}

		return true;
	}

	public bool Play(bool Restart) => Bass.BASS_ChannelPlay(BassHandle, Restart);

	public bool Pause() => Bass.BASS_ChannelPause(BassHandle);

	public bool Stop() => Bass.BASS_ChannelStop(BassHandle);

	public bool SetVolume(float Volume) => Bass.BASS_ChannelSetAttribute(BassHandle, BASSAttribute.BASS_ATTRIB_VOL, Volume);

	public bool Seek(double Seconds) => Bass.BASS_ChannelSetPosition(BassHandle, Seconds);

	public bool SlideVolume(float Volume, int Seconds) => Bass.BASS_ChannelSlideAttribute(BassHandle, BASSAttribute.BASS_ATTRIB_VOL, Volume, Seconds);

	public bool Free()
	{
		if (!Bass.BASS_StreamFree(BassHandle))
		{
			Log.WriteLine($"MusicAsset ({ID}) :: Failed to free '{Path}' with '{Bass.BASS_ErrorGetCode()}'");

			DataHandle.Free();
			return false;
		}

		DataHandle.Free();

		Data = null;
		return true;
	}
}
