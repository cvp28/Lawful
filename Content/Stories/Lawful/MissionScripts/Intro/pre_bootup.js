function BetterReadKey()
{
	while (BootupConsole.KeyAvailable) { BootupConsole.ReadKey(true); }
	BootupConsole.ReadKey(true);
}

Sleep(2000);
BootupConsole.CursorVisible = false;

BootupConsole.WriteDynamic("End of Sophomore Year, 2018\n", 50);
Sleep(750);
BootupConsole.WriteDynamic("Lawful Estate\n", 50);
Sleep(750);
BootupConsole.WriteDynamic("10:34 AM\n", 50);
BootupConsole.NextLine();
Sleep(1000);

BootupConsole.WriteLine("Dad: Alex! Can you come out here for a second?");
BetterReadKey();
BootupConsole.WriteLine("Alex: Yeah?");
BetterReadKey();
BootupConsole.WriteLine("Dad: Report card came.");
BetterReadKey();
BootupConsole.WriteLine("Alex: Don't bother. Nothing but F's on that paper.");
Sleep(2000);
BootupConsole.NextLine();
BootupConsole.WriteLine("Dad: ...");
Sleep(1250);
BootupConsole.WriteLine("Alex: ...");
Sleep(1250);
BootupConsole.WriteLineColor("(dramatic envelope ripping noises)", ConsoleColor.Yellow);
Sleep(1000);
BootupConsole.NextLine();
BootupConsole.WriteLine("Dad: Let's see here... WOW. All A's, huh?");
BetterReadKey();
BootupConsole.WriteLine("Alex: Hmm, must have been sent to the wrong address.");
BetterReadKey();
BootupConsole.WriteLine("Dad: Don't sell yourself short, bud. Remember our deal?");
BetterReadKey();
BootupConsole.WriteLine("Alex: I do actually, I'll go get ready!");
BetterReadKey();

Sleep(750);
BootupConsole.NextLine();
BootupConsole.WriteDynamic("Wavetronics Retail Outlet, Aisle 10\n", 50);
Sleep(750);
BootupConsole.WriteDynamic("11:42 AM\n", 50);
BootupConsole.NextLine();
Sleep(1000);

BootupConsole.WriteLine("Dad: Alex, you have been staring at that shelf for 10 minutes.");
BetterReadKey();
BootupConsole.WriteLine("Alex: wait wha-");
BetterReadKey();
BootupConsole.WriteLine("Dad: Don't tell me you were daydreaming about a computer of all things.");
BetterReadKey();
BootupConsole.WriteLine("Alex: ");
BetterReadKey();
BootupConsole.WriteLine("Dad: Just hurry up and pick a computer already, the A/C in this place blows too cold.");
BetterReadKey();
BootupConsole.Write("Alex: ");
Sleep(600);
BootupConsole.WriteColor("Points", ConsoleColor.Yellow);
BootupConsole.Write(' ');
Sleep(600);
BootupConsole.WriteDynamic("That one.\n", 25);

Sleep(1000);
BootupConsole.NextLine();
BootupConsole.WriteDynamic("Law Estate\n", 50);
Sleep(750);
BootupConsole.WriteDynamic("1:28 PM\n", 50);
BootupConsole.NextLine();
Sleep(750);

BootupConsole.WriteLine("Alex: Alright, cables are hooked up and everything looks to be in order.");
BetterReadKey();
BootupConsole.WriteLine("Alex: Let's see what this button does...");

Sleep(1500);
BootupConsole.Clear();
Sleep(2000);