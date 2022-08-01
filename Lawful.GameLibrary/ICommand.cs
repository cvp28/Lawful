using Lawful.InputParser;

namespace Lawful.GameLibrary;
	
public interface ICommand : IComponent
{
	public string ExecutableName { get; }

	public void Help();

	public void Invoke(InputQuery Query, User Player, ComputerStructure Computers, EventManager Events);
}
