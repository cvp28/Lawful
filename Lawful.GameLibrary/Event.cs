
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
	CommandExecuted
}

public class Event
{
	[XmlAttribute("Trigger")]
	public Trigger Trigger { get; private set; }

	[XmlAttribute("ScriptPath")]
	public string ScriptPath { get; private set; }

	public string ScriptSource
	{
		get
		{
			string PathToScript = $@"{CurrentMissionRoot}\{ScriptPath}";

			if (!File.Exists(PathToScript))
				return string.Empty;
			else
				return File.ReadAllText(PathToScript);
		}
	}

	[XmlAttribute("Attrib")]
	public string Attributes { get; private set; }
}
