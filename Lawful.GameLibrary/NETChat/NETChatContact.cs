
using System.Xml.Serialization;

namespace Lawful.GameLibrary;

public class NETChatContact : IComparable<NETChatContact>
{
	[XmlAttribute("Username")]
	public string Username;

	// This flag will be controlled by script calls to set a certain contact's status to online or offline
	[XmlAttribute("Online")]
	public bool Online;

	[XmlElement("Chat")]
	public Chat Chat;

	[XmlAttribute("HasPendingChatRequest")]
	public bool HasPendingChatRequest;

	[XmlElement("PathToSequenceJS")]
	public string PathToSequenceJS;

	[XmlIgnore]
	public string SequenceJSSource => File.ReadAllText(PathToSequenceJS);

	public int CompareTo(NETChatContact other) => Username.CompareTo(other.Username);
}
