
using System.Xml.Serialization;

namespace Lawful.GameLibrary;

public class Config
{
	[XmlElement("ShowFPS")]
	public bool ShowFPS;

	[XmlElement("Renderer")]
	public Renderer SelectedRenderer;

	[XmlElement("EnableTypewriter")]
	public bool EnableTypewriter;

	[XmlElement("TypewriterVolume")]
	public float TypewriterVolume;

	public Config()
	{
		ShowFPS = true;
		SelectedRenderer = OperatingSystem.IsWindows() ? Renderer.WindowsNative : Renderer.CrossPlatform;
		EnableTypewriter = true;
		TypewriterVolume = 0.2f;
	}
}

public enum Renderer
{
	CrossPlatform,
	WindowsNative
}