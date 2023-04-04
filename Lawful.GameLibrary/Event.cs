
using System.Xml;
using System.Xml.Serialization;

namespace Lawful.GameLibrary;

using static GameSession;

public enum Trigger
{
	BootupSequenceStarted,
	BootupSequenceFinished,

	CreateFile,
	DeleteFile,
	ReadFile,
	RenameFile,

	CreateDirectory,
	DeleteDirectory,
	ChangeDirectory,
	RenameDirectory,

	SSHConnect,
	SSHDisconnect,

	CommandEntered,
	CommandExecuted,

	FriendRequestAccepted,
	FriendRequestDenied
}

public class Event
{
	[XmlAttribute("Trigger")]
	public Trigger Trigger { get; set; }

	[XmlAttribute("ScriptPath")]
	public string ScriptPath { get; set; }

	[XmlIgnore]
	public string ScriptSource
	{
		get
		{
			string PathToScript = $"{CurrentStoryRoot}\\{ScriptPath}".ToPlatformPath();

			if (!File.Exists(PathToScript))
				return string.Empty;
			else
				return File.ReadAllText(PathToScript);
		}
	}
}
