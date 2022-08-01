
namespace Lawful.GameLibrary;

public interface IPlugin
{
	// Display name for the plugin
	public string Name { get; }

	// Internal ID for the plugin, omitting spaces
	public string ID { get; }

	public List<IComponent> GetComponents();
}
