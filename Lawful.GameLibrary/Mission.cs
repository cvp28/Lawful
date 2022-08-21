using System.Xml.Serialization;

namespace Lawful.GameLibrary;
	
public class Mission
{
	[XmlAttribute("Name")]
	public string Name;

	[XmlAttribute("ID")]
	public string ID;

	[XmlAttribute("Event")]
	public List<Event> Events;
}