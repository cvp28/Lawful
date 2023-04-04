
using System.Xml.Serialization;

namespace Lawful.GameLibrary;

public class Config
{
	[XmlElement("ShowFPS")]
	public bool ShowFPS;

	[XmlElement("Renderer")]
	public RendererType SelectedRenderer;

	[XmlElement("EnableTypewriter")]
	public bool EnableTypewriter;

	[XmlElement("TypewriterVolume")]
	public float TypewriterVolume;

	public Config()
	{
		ShowFPS = true;
		SelectedRenderer = OperatingSystem.IsWindows() ? RendererType.WindowsNative : RendererType.CrossPlatform;
		EnableTypewriter = true;
		TypewriterVolume = 0.2f;
	}
}

public enum RendererType
{
	CrossPlatform,
	WindowsNative
}