﻿using System.Xml.Serialization;

namespace Lawful.GameLibrary;

public class Chat
{
	[XmlElement("Message")]
	public List<ChatMessage> History = new();
}

public class ChatMessage
{
	[XmlAttribute("Username")]
	public string Username = string.Empty;

	[XmlText]
	public string Message = string.Empty;
}