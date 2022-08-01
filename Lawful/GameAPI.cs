using System.Xml;
using System.Xml.Serialization;
using Lawful.GameLibrary;

namespace Lawful;

public static class GameAPI
{
	public static string[] GetInstalledStorylines()
	{
		XmlSerializer sSt = new(typeof(Story));
		XmlSerializer sCS = new(typeof(ComputerStructure));
		List<string> ValidStories = new();
		
		foreach (string StoryDir in Directory.GetDirectories(@".\Content\Stories"))
		{
			bool HasComputers = File.Exists(@$"{StoryDir}\Computers.xml");
			bool HasStory = File.Exists(@$"{StoryDir}\Story.xml");

			if (!HasComputers || !HasStory)
				continue;

			using (XmlReader ComputersReader = XmlReader.Create(@$"{StoryDir}\Computers.xml"))
				if (!sCS.CanDeserialize(ComputersReader))
					continue;

			using (XmlReader StoryReader = XmlReader.Create($@"{StoryDir}\Story.xml"))
				if (!sSt.CanDeserialize(StoryReader))
					continue;

			ValidStories.Add(StoryDir.Split('\\').Last());
		}

		return ValidStories.ToArray();
	}
}