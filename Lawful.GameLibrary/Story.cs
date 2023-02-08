using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Lawful.GameLibrary;

public class Story
{
	[XmlAttribute("Name")]
	public string Name { get; set; }

	[XmlAttribute("Start")]
	public string StartMissionID { get; set; }

	[XmlElement("Mission")]
	public List<Mission> Missions;

	[XmlElement("NETChatAccount")]
	public NETChatAccount DefaultNETChatAccount;

	public Story() => Missions = new();

	public bool HasMission(string ID) => Missions.Any(m => m.ID == ID);

	public Mission GetMission(string ID) => Missions.FirstOrDefault(m => m.ID == ID);

	[RequiresUnreferencedCode("")]
	public void SerializeToFile(string Path)
	{
		using FileStream fs = new(Path, FileMode.Create);

		XmlSerializer xs = new(typeof(Story));

		xs.Serialize(fs, this);
	}

	[RequiresUnreferencedCode("")]
	public static Story DeserializeFromFile(string Path)
	{
		if (!File.Exists(Path))
			throw new Exception($"Could not find file referenced by '{Path}'");

		using FileStream fs = new(Path, FileMode.Open);

		XmlSerializer xs = new(typeof(Story));

		return xs.Deserialize(fs) as Story;
	}
}