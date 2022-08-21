using System.Xml;
using System.Xml.Serialization;

using Lawful.GameLibrary.UI;

namespace Lawful.GameLibrary;

public static class GameAPI
{
	public static string[] GetInstalledStorylines()
	{
		XmlSerializer sStory = new(typeof(Story));
		XmlSerializer sCompStruct = new(typeof(ComputerStructure));
		List<string> ValidStories = new();
		
		foreach (string StoryDir in Directory.GetDirectories(@".\Content\Stories"))
		{
			bool HasComputers = File.Exists(@$"{StoryDir}\Computers.xml");
			bool HasStory = File.Exists(@$"{StoryDir}\Story.xml");

			if (!HasComputers || !HasStory)
				continue;

			using (XmlReader ComputersReader = XmlReader.Create(@$"{StoryDir}\Computers.xml"))
				if (!sCompStruct.CanDeserialize(ComputersReader))
					continue;

			using (XmlReader StoryReader = XmlReader.Create($@"{StoryDir}\Story.xml"))
				if (!sStory.CanDeserialize(StoryReader))
					continue;

			ValidStories.Add(StoryDir.Split('\\').Last());
		}

		return ValidStories.ToArray();
	}

	public static string[] GetSaves()
	{
		XmlSerializer sUser = new(typeof(User));
		XmlSerializer sCompStruct = new(typeof(ComputerStructure));
		List<string> ValidSaves = new();

		foreach (string SaveDir in Directory.GetDirectories(@".\Content\Saves"))
		{
			bool HasComputers = File.Exists($@"{SaveDir}\Computers.xml");
			bool HasUser = File.Exists($@"{SaveDir}\User.xml");

			if (!HasComputers || !HasUser)
				continue;

			using (XmlReader ComputersReader = XmlReader.Create($@"{SaveDir}\Computers.xml"))
				if (!sCompStruct.CanDeserialize(ComputersReader))
					continue;

			using (XmlReader UserReader = XmlReader.Create($@"{SaveDir}\User.xml"))
				if (!sUser.CanDeserialize(UserReader))
					continue;

			ValidSaves.Add(SaveDir.Split('\\').Last());
		}

		return ValidSaves.ToArray();
	}

	public static void CommenceBootup()
	{
		
	}
}