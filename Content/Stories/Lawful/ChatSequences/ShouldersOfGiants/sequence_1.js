AddMessage("Jason", "Hey, what's up?");

DelayMs(2000);

AddColoredMessage("[SERVER]", "User 'Jason' poked you.", ConsoleColor.Yellow)

DelayMs(1000);

AddMessage("Jason", "Come onnnn, I know you got that notification just now!");

DefineChoice1("Sorry, was in the restroom");
DefineChoice1Flag("sog_1_proper");

DefineChoice2("ALRIGHT alright im here. whats up?");
DefineChoice2Flag("sog_1_jeez");

// Blocking call. Waits for user to actually pick a response choice.
AwaitResponse();

if (FlagIsSet("sog_1_proper"))
{
	AddMessage("Jason", "proper response init");
}
else if (FlagIsSet("sog_1_jeez"))
{
	AddMessage("Jason", "jeez");
}