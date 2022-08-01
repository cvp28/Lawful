
namespace Lawful.GameLibrary;

public interface IComponent
{
	// Display name for the component
	// Something akin to "Konym.ListCommand" or anything following [Username].[ComponentName]
	public string Name { get; }

	// Internal ID for the component, omitting spaces
	public string ID { get; }

	public ComponentType Type { get; }
}