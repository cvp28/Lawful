AddMessage("Jason", "Hey, what's up?");

DelayMs(2000);

AddMessage("SERVER", "User 'Jason' poked you.")	

DelayMs(1000);

AddMessage("Jason", "Come onnnn, I know you got that notification just now!");

DefineChoice(1, "Sorry, was in the restroom");
DefineChoiceFlag(1, "sog_1_proper");

DefineChoice(2, "ALRIGHT alright im here. whats up?");
DefineChoiceFlag(2, "sog_1_jeez");

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