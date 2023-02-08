using System.Xml.Serialization;

namespace Lawful.GameLibrary;

public class Mission
{
	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("ID")]
	public string ID;

	[XmlElement("Event")]
	public List<Event> Events;

	[XmlElement("SFXAsset")]
	public List<SFXAsset> SoundEffects;

	[XmlElement("MusicAsset")]
	public List<MusicAsset> MusicAssets;

	public bool HasAssets => SoundEffects.Count + MusicAssets.Count > 0;

	public Mission()
	{
		SoundEffects = new();
		MusicAssets = new();
	}

	public bool LoadAssets()
	{
		bool Success = false;

		if (SoundEffects is not null)
			foreach (var asset in SoundEffects)
				Success = asset.Load();

		if (MusicAssets is not null)
			foreach (var asset in MusicAssets)
				Success = asset.Load();

		return Success;
	}

	public bool FreeAssets()
	{
		bool Success = false;

		if (SoundEffects is not null)
			foreach (var asset in SoundEffects)
				Success = asset.Free();

		if (MusicAssets is not null)
			foreach (var asset in MusicAssets)
				Success = asset.Free();

		return Success;
	}
}