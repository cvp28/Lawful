using System.Xml.Serialization;

namespace Lawful.GameLibrary;

public class NETChatAccount
{
	[XmlAttribute("Username")]
	public string Username = default;

	[XmlElement("Contact")]
	public List<NETChatContact> Contacts = new();

	[XmlElement("PendingFriendRequest")]
	public List<string> PendingFriendRequests = new();

	public NETChatAccount() { }

	//	public Chat GetChat(string Username)
	//	{
	//		if (Username == this.Username)
	//			return null;
	//	
	//		return ChatHistory.FirstOrDefault(chat => chat.Username == Username);
	//	}
}