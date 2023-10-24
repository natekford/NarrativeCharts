using static NarrativeCharts.Bookworm.BookwormBell;
using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Bookworm.P3;

public sealed class P3V2 : BookwormNarrativeChart
{
	public P3V2(BookwormTimeTracker time) : base(time)
	{
		Name = nameof(P3V2);
	}

	protected override void ProtectedCreate()
	{
		Chapter("Prologue");
		P3V2C01();
		Chapter("Discussing the Harvest Festival");
		P3V2C02();
		// Background event
		{
			// Ingo and Deid probably come back from Hasse since the priests are
			// able to move it, and Lutz/Myne never mention them being at the
			// monastery when they go there
			Add(Scene(LowerCityWorkshops).With(Deid, Ingo));
		}
		Chapter("Hasse's Monastery");
		P3V2C03();
		Chapter("The New Orphans");
		P3V2C04();
		Chapter("The Orphan's Treatment and Investigating the City");
		P3V2C05();
		Chapter("The Monastery's Barrier");
		P3V2C06();
		Chapter("A New Task and Winter Preparations");
		P3V2C07();
		Chapter("Opening the Italian Restaurant");
		P3V2C08();
		Chapter("Discussing How to Improve Hasse");
		P3V2C09();
		Chapter("Switching Places");
		P3V2C10();
		Chapter("Preparing for the Harvest Festival");
		P3V2C11();
		Chapter("Hasse's Contract");
		P3V2C12();
		Chapter("Starting Merchant Activities");
		P3V2C13();
		Update();
	}

	private void P3V2C01()
	{
		// Time: After the coming of age ceremony in summer, no clue about what exact day or time of day aside from not night
		SkipToDaysAhead(2, Lunch);
		Add(Scene(MynesHouse).With(Effa, Tuuli));
		// Time: "That night, while Gunther was drinking"
		SkipToCurrentDay(Bed);
		Add(Scene(MynesHouse).With(Effa, Gunther, Kamil));

		// Time: "It’s due in three days"
		SkipToDaysAhead(3, Lunch);
		Add(Scene(Temple).With(Damuel, Fran, Myne));
		var s1 = AddR(Scene(Temple).With(Benno, Effa, Lutz, Tuuli));
		Time.AddBell();
		Return(s1);
	}

	private void P3V2C02()
	{
		// Time: "There will be a meeting at third bell today." on the day after the autumn coming of age ceremony
		SkipToDaysAhead(2, Meetings);
		// fran explains the harvest festival to myne then they go to a meeting with the blue priests
		Add(Scene(Temple).With(Egmont, Ferdinand, Fran, Myne));
		// Time: "Not long after lunch"
		Time.AddBell();
		Add(Scene(Temple).With(Ferdinand, Myne, Zahm));
		Time.AddBell();
		// lutz and myne talk about gunther being assigned as a guard knight for the hasse carriages
		Add(Scene(Temple).With(Damuel, Gil, Lutz, Myne));
		Time.AddBell();
		// lutz tells gunther that he will be able to see myne
		Add(Scene(MynesHouse).With(Effa, Gunther, Lutz, Tuuli));
		Time.AddBell();
		Add(Scene(MerchantCompanies).With(Lutz));
	}

	private void P3V2C03()
	{
		// Time: probably the morning? no idea how many days later tho
		SkipToDaysAhead(2, Lunch);
		// myne sees gunther, the gray priests, and gilberta company off
		Add(Scene(Temple).With(Benno, Gunther, Lutz, Mark, Myne));
		// Time: the story says it's half a day or something to get to hasse via carriage?
		Time.AddDay();
		Add(Scene(Hasse).With(Benno, Gunther, Lutz, Mark));

		// Time: "I would be heading for Hasse three days from now" said the day that the grays depart
		SkipToDaysAhead(2, Morning);
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
		Time.AddBell();
		var s1 = AddR(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Myne));
		Time.AddBell();
		Return(s1);

		// Time: unknown, some time before myne comes back to hasse
		SkipToDaysAhead(2, Meetings);
		Add(Scene(MerchantCompanies).With(Benno, Mark, Lutz));
		Add(Scene(MynesHouse).With(Gunther));

		// Time: "The three days passed before I knew it"
		SkipToNextDay(Morning);
		Add(Scene(Hasse).With(Brigitte, Damuel, Gil, Fran, Ferdinand, Myne, Nicola));
		Time.AddBell();
		Add(Scene(Hasse).With(HasseMayor, Marthe, Nora, Rick, Thore));
	}

	private void P3V2C04()
	{
		// Time: "Now, we will postpone taking you to your rooms so that we can eat lunch first."
		SkipToCurrentDay(Lunch);
		// everyone eats lunch at the monastery then the orphans get a tour of it
		Time.AddBell();
	}

	private void P3V2C05()
	{
		// fran/ferdiand tell myne to stop being soft in the monastery's secret room
		Time.AddBell();
		Add(Scene(Temple).With(Brigitte, Damuel, Gil, Fran, Ferdinand, Myne, Nicola));

		// "Ferdinand summoned Benno the second we got back to the temple;"
		var s1 = AddR(Scene(Temple).With(Benno));
		Time.AddBell();
		Return(s1);
	}

	private void P3V2C06()
	{
		// Time: “We will visit again five days from now,” from c05
		// "There were three whole days before my next scheduled visit" from this chapter
		// so the attack on the monastery either occurs 1 day after or 2
		SkipToNextDay(Morning);
		// wilma/myne talk about winter prep
		Add(Scene(Temple).With(Myne, Wilma));
		Time.AddBell();
		// hasse monastery attacked
		Add(Scene(Temple).With(Ferdinand, Myne));

		SkipToNextDay(Morning);
		// hasse mayor send a board
		Add(Scene(Temple).With(Monika, Myne));
		// myne shows it to ferdi
		Time.AddBell();
		Add(Scene(Temple).With(Ferdinand, Fran, Myne));
	}

	private void P3V2C07()
	{
		// Time: "before I returned to the monastery the day after tomorrow"
		SkipToNextDay(Morning);
		Add(Scene(Temple).With(Ferdinand, Myne));

		// Time: "time to go to the castle the next day, my exhaustion was weighing me down just as much as the fear was."
		SkipToNextDay(Morning);
		Add(Scene(Temple).With(Gil, Myne));
		Add(Scene(Temple).With(Ferdinand, Myne));
		Time.AddBell();
		var s1 = AddR(Scene(Castle).With(Brigitte, Damuel, Ferdinand, Myne));
		Time.AddBell();
		Add(Scene(Castle).With(Myne, Rihyarda));
		// Time: "a meeting had been arranged for teatime at fifth bell."
		SkipToCurrentDay(FifthBell);
		// wilfried "it's not fair" myne tells him to quit whining
		Add(Scene(Castle).With(Myne, Wilfried));
		// myne shows sylvester lessy and gets sylvester to give hugo back
		Add(Scene(Castle).With(Ferdinand, Myne, Sylvester));
		Time.AddBell();
		Return(s1);
		Add(Scene(ItalianRestaurant).With(Hugo));
	}

	private void P3V2C08()
	{
		// Time: "The day I could visit Hasse’s orphanage had finally arrived."
		SkipToNextDay(Meetings);
		var s1 = AddR(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Fran, Gil, Myne));
		Time.AddBell();
		Return(s1);

		// Time: "The day after visiting the orphans,"
		SkipToNextDay(Lunch);
		var s2 = AddR(Scene(Temple).With(Benno, Lutz, Mark));
		Time.AddBell();
		Return(s2);

		// Time: "That night, I slept well for the first time in days."
		// "spent the days leading up to the opening of the Italian restaurant relaxing,"
		// "I started lunch early on the day of the Italian restaurant opening"
		SkipToNextDay(Lunch);
		var s3 = AddR(Scene(ItalianRestaurant).With(Benno, Brigitte, Damuel, Fran, Freida, Gustav, Myne));
		Time.AddBell();
		Return(s3);

		// Time: "Lutz reported with a grin the next day."
		SkipToNextDay(Lunch);
		Add(Scene(Temple).With(Benno, Lutz, Mark));
	}

	private void P3V2C09()
	{
		Time.AddBell();
		Add(Scene(MerchantCompanies).With(Benno, Lutz, Mark));
		// myne tells ferdi about the plan she came up with benno
		Add(Scene(Temple).With(Ferdinand, Myne));
		Time.AddBell();
	}

	private void P3V2C10()
	{
		// Time: unknown, no meeting was mentioned iirc
		// so several days after last chapter?
		SkipToDaysAhead(3, Meetings);
		// myne reports to sylvester and swaps places with wilfried for a day
		Add(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Karstedt, Myne));
		Time.AddBell();
		// myne has lunch with wilf and talks about what to do
		Add(Scene(Temple).With(Ferdinand, Lamprecht, Wilfried));
		Add(Scene(Castle).With(Moritz, Oswald));
		// Time: "Fifth bell rang and there was still no sign of Rihyarda."
		SkipToCurrentDay(FifthBell);
		// Time: "“Milady, it’s dinner time!” Rihyarda declared"
		SkipToCurrentDay(Dinner);
		// ferdinand tells sylvester to disinherit wilf
		var s1 = AddR(Scene(Castle).With(Ferdinand));
		Time.AddBell();
		Return(s1);

		SkipToNextDay(Morning);
		// myne shows people in the castle karuta/picture books
		// Time: "Shortly after fourth bell, Wilfried and Lamprecht entered the room"
		SkipToCurrentDay(FourthBell);
		Add(Scene(Castle).With(Ferdinand, Lamprecht, Wilfried));
	}

	private void P3V2C11()
	{
		Add(Scene(Castle).With(Eckhart, Justus));
		Time.AddBell();
		Add(Scene(KnightsOrder).With(Eckhart));
		Add(Scene(NoblesQuarter).With(Justus));
	}

	private void P3V2C12()
	{
		Add(Scene(Castle).With(Kantna));
		SkipToCurrentDay(SixthBell);
		// Time: "A little before sixth bell, Rihyarda received an ordonnanz from Ferdinand"
		// he doesn't die, but he never shows up again
		Kill(Kantna);
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
		Add(Scene(KarstedtsHouse).With(Cornelius, Karstedt));
		Add(Scene(KnightsOrder).With(Angelica));

		// Time: "The next day was a normal one; I practiced harspiel as I always did, then went to help Ferdinand."
		SkipToNextDay(Meetings);
		Add(Scene(Temple).With(Ferdinand, Myne));
	}

	private void P3V2C13()
	{
	}
}