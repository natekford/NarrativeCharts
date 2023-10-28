﻿using static NarrativeCharts.Bookworm.BookwormBell;
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
		Event("Prologue");
		P3V2C01();
		Event("Discussing the Harvest Festival");
		P3V2C02();
		// Background event
		{
			// Ingo and Deid probably come back from Hasse since the priests are
			// able to move it, and Lutz/Myne never mention them being at the
			// monastery when they go there
			Add(Scene(LowerCityWorkshops).With(Deid, Ingo));
		}
		Event("Hasse's Monastery");
		P3V2C03();
		Event("The New Orphans");
		P3V2C04();
		Event("The Orphan's Treatment and Investigating the City");
		P3V2C05();
		Event("The Monastery's Barrier");
		P3V2C06();
		Event("A New Task and Winter Preparations");
		P3V2C07();
		Event("Opening the Italian Restaurant");
		P3V2C08();
		Event("Discussing How to Improve Hasse");
		P3V2C09();
		Event("Switching Places");
		P3V2C10();
		Event("Preparing for the Harvest Festival");
		P3V2C11();
		Event("Hasse's Contract");
		P3V2C12();
		Event("Starting Merchant Activities");
		P3V2C13();
		Event("Hasse's Harvest Festival");
		P3V2C14();
		Event("The Harvest Festival");
		P3V2C15();
		Event("The Night of Schutzaria");
		P3V2C16();
		Event("Aftermath");
		P3V2C17();
		Event("My Winter Preparations");
		P3V2C18();
		Event("Epilogue");
		P3V2C19();
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
		AddUnit();
		Return(s1);
	}

	private void P3V2C02()
	{
		// Time: "There will be a meeting at third bell today." on the day after the autumn coming of age ceremony
		SkipToDaysAhead(2, Meetings);
		// fran explains the harvest festival to myne then they go to a meeting with the blue priests
		Add(Scene(Temple).With(Egmont, Ferdinand, Fran, Myne));
		// Time: "Not long after lunch"
		AddUnit();
		Add(Scene(Temple).With(Ferdinand, Myne, Zahm));
		AddUnit();
		// lutz and myne talk about gunther being assigned as a guard knight for the hasse carriages
		Add(Scene(Temple).With(Damuel, Gil, Lutz, Myne));
		AddUnit();
		// lutz tells gunther that he will be able to see myne
		Add(Scene(MynesHouse).With(Effa, Gunther, Lutz, Tuuli));
		AddUnit();
		Add(Scene(MerchantCompanies).With(Lutz));
	}

	private void P3V2C03()
	{
		// Time: probably the morning? no idea how many days later tho
		SkipToDaysAhead(2, Lunch);
		// myne sees gunther, the gray priests, and gilberta company off
		Add(Scene(Temple).With(Benno, Gunther, Lutz, Mark, Myne));
		// Time: the story says it's half a day or something to get to hasse via carriage?
		AddUnit();
		Add(Scene(Hasse).With(Benno, Gunther, Lutz, Mark));

		// Time: "I would be heading for Hasse three days from now" said the day that the grays depart
		SkipToDaysAhead(2, Morning);
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
		AddUnit();
		var s1 = AddR(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Myne));
		AddUnit();
		Return(s1);

		// Time: unknown, some time before myne comes back to hasse
		SkipToDaysAhead(2, Meetings);
		Add(Scene(MerchantCompanies).With(Benno, Mark, Lutz));
		Add(Scene(MynesHouse).With(Gunther));

		// Time: "The three days passed before I knew it"
		SkipToNextDay(Morning);
		Add(Scene(Hasse).With(Brigitte, Damuel, Gil, Fran, Ferdinand, Myne, Nicola));
		AddUnit();
		Add(Scene(Hasse).With(HasseMayor, Marthe, Nora, Rick, Thore));
	}

	private void P3V2C04()
	{
		// Time: "Now, we will postpone taking you to your rooms so that we can eat lunch first."
		SkipToCurrentDay(Lunch);
		// everyone eats lunch at the monastery then the orphans get a tour of it
		AddUnit();
	}

	private void P3V2C05()
	{
		// fran/ferdiand tell myne to stop being soft in the monastery's secret room
		AddUnit();
		Add(Scene(Temple).With(Brigitte, Damuel, Gil, Fran, Ferdinand, Myne, Nicola));

		// "Ferdinand summoned Benno the second we got back to the temple;"
		var s1 = AddR(Scene(Temple).With(Benno));
		AddUnit();
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
		AddUnit();
		// hasse monastery attacked
		Add(Scene(Temple).With(Ferdinand, Myne));

		SkipToNextDay(Morning);
		// hasse mayor send a board
		Add(Scene(Temple).With(Monika, Myne));
		// myne shows it to ferdi
		AddUnit();
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
		AddUnit();
		var s1 = AddR(Scene(Castle).With(Brigitte, Damuel, Ferdinand, Myne));
		AddUnit();
		Add(Scene(Castle).With(Myne, Rihyarda));
		// Time: "a meeting had been arranged for teatime at fifth bell."
		SkipToCurrentDay(FifthBell);
		// wilfried "it's not fair" myne tells him to quit whining
		Add(Scene(Castle).With(Myne, Wilfried));
		// myne shows sylvester lessy and gets sylvester to give hugo back
		Add(Scene(Castle).With(Ferdinand, Myne, Sylvester));
		AddUnit();
		Return(s1);
		Add(Scene(ItalianRestaurant).With(Hugo));
	}

	private void P3V2C08()
	{
		// Time: "The day I could visit Hasse’s orphanage had finally arrived."
		SkipToNextDay(Meetings);
		var s1 = AddR(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Fran, Gil, Myne));
		AddUnit();
		Return(s1);

		// Time: "The day after visiting the orphans,"
		SkipToNextDay(Lunch);
		var s2 = AddR(Scene(Temple).With(Benno, Lutz, Mark));
		AddUnit();
		Return(s2);

		// Time: "That night, I slept well for the first time in days."
		// "spent the days leading up to the opening of the Italian restaurant relaxing,"
		// "I started lunch early on the day of the Italian restaurant opening"
		SkipToNextDay(Lunch);
		var s3 = AddR(Scene(ItalianRestaurant).With(Benno, Brigitte, Damuel, Fran, Freida, Gustav, Myne));
		AddUnit();
		Return(s3);

		// Time: "Lutz reported with a grin the next day."
		SkipToNextDay(Lunch);
		Add(Scene(Temple).With(Benno, Lutz, Mark));
	}

	private void P3V2C09()
	{
		AddUnit();
		Add(Scene(MerchantCompanies).With(Benno, Lutz, Mark));
		// myne tells ferdi about the plan she came up with benno
		Add(Scene(Temple).With(Ferdinand, Myne));
		AddUnit();
	}

	private void P3V2C10()
	{
		// Time: unknown, no meeting was mentioned iirc
		// so several days after last chapter?
		SkipToDaysAhead(3, Meetings);
		// myne reports to sylvester and swaps places with wilfried for a day
		Add(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Karstedt, Myne));
		AddUnit();
		// myne has lunch with wilf and talks about what to do
		// brig/damuel go with wilf (from wilf's SS)
		// "Lamprecht and Damuel will accompany you as guards"
		// "Rozemyne’s female knight who had entered with Ferdinand saluted and stepped aside."
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Lamprecht, Wilfried));
		Add(Scene(Castle).With(Moritz, Oswald));
		// Time: "Fifth bell rang and there was still no sign of Rihyarda."
		SkipToCurrentDay(FifthBell);
		// Time: "“Milady, it’s dinner time!” Rihyarda declared"
		SkipToCurrentDay(Dinner);
		// ferdinand tells sylvester to disinherit wilf
		var s1 = AddR(Scene(Castle).With(Ferdinand));
		AddUnit();
		Return(s1);

		SkipToNextDay(Morning);
		// myne shows people in the castle karuta/picture books
		// Time: "Shortly after fourth bell, Wilfried and Lamprecht entered the room"
		SkipToCurrentDay(FourthBell);
		Add(Scene(Castle).With(Brigitte, Damuel, Ferdinand, Lamprecht, Wilfried));
	}

	private void P3V2C11()
	{
		Add(Scene(Castle).With(Eckhart, Justus));
		AddUnit();
		Add(Scene(KnightsOrder).With(Eckhart));
		// "I’ve been doing scholar work in the castle ever since Lord Ferdinand entered the temple"
		Add(Scene(Castle).With(Justus));
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
		AddUnit();
		// myne updates gilberta company on hasse
		var s1 = AddR(Scene(Temple).With(Benno, Lutz, Mark));
		AddUnit();
		Return(s1);

		// Time: "Two days had passed since Mark was given permission to spread rumors."
		// "Wine at noon?"
		SkipToDaysAhead(2, Lunch);
		var s2 = AddR(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Fran, Myne));
		Add(Scene(Hasse).With(Richt));
		AddUnit();
		Return(s2);
	}

	private void P3V2C14()
	{
		// Time: unknown, "On the morning of the Harvest Festival"
		SkipToDaysAhead(5, Morning);
		// via highbeast
		Add(Scene(Hasse).With(Brigitte, Damuel, Eckhart, Ferdinand, Fran, Justus, Myne));
		// via carriage
		Freeze(Ella, Monika, Nicola, Rosina);
		Freeze(Benno, Gil, Gunther, Lutz, Mark);
		AddUnit(2);
		Add(Scene(Hasse).With(Ella, Monika, Nicola, Rosina));
		Add(Scene(Hasse).With(Benno, Gil, Gunther, Lutz, Mark));
		// Time: "Fifth bell rang just a second ago"
		SkipToCurrentDay(FifthBell);
		// myne discusses hasse with gil/lutz
		AddUnit();
	}

	private void P3V2C15()
	{
		// Time: "When dawn broke"
		SkipToNextDay(Morning);
		// monastery being closed for the winter
		AddUnit(2);
		Add(Scene(Temple).With(Gil, Marthe, Nora, Rick, Thore));
		Add(Scene(MynesHouse).With(Gunther));
		Add(Scene(MerchantCompanies).With(Lutz));
		// via highbeast
		Add(Scene(HarvestFestivalTowns).With(Brigitte, Damuel, Eckhart, Ferdinand, Fran, Justus, Myne));
		// via carriage
		Freeze(Ella, Monika, Nicola, Rosina);
		AddUnit();
		// benno/mark spread rumours before leaving
		Add(Scene(MerchantCompanies).With(Benno, Mark));
		// via carriage
		Add(Scene(HarvestFestivalTowns).With(Ella, Monika, Nicola, Rosina));

		// Time: "Justus began his work as a tax official first thing in the morning."
		SkipToNextDay(Morning);
		// justus sends taxes back to the castle
		// Time: "Or so I thought until the third day."
		SkipToDaysAhead(2, Morning);
	}

	private void P3V2C16()
	{
		// Time: "Just as I was really getting exhausted from the quick succession of festivals, we arrived at Dorvan"
		// could be immediately after the end of the previous chapter, who knows?
		SkipToNextDay(Lunch);

		// They're at some town named Dorvan, but it doesn't need a location tbh
		// Time: "After informing Dorvan’s mayor that we would be staying for a few days after the Harvest Festival"
		// "“Tonight is the Night of Schutzaria.” Justus explained as we ate breakfast together."
		SkipToDaysAhead(3, Morning);
		// Time: "would be going to look for a ruelle tree after lunch"
		SkipToCurrentDay(Lunch);
		var s1 = AddR(Scene(RuelleTree).With(Damuel, Eckhart, Justus));
		AddUnit();
		Return(s1);
		SkipToCurrentDay(Bed);
		// gathering ruelles, myne's gets eaten
		Add(Scene(RuelleTree).With(Brigitte, Damuel, Eckhart, Justus, Myne));
	}

	private void P3V2C17()
	{
		AddUnit();
		// ferdi is contacted, he tells myne to contain the goltze
		Add(Scene(RuelleTree).With(Ferdinand));
		AddUnit();
		Add(Scene(HarvestFestivalTowns).With(Ferdinand));
		Add(Scene(HarvestFestivalTowns).With(Brigitte, Damuel, Eckhart, Justus, Myne));
		// Time: "I ended up bedridden"
		SkipToDaysAhead(2, Meetings);
	}

	private void P3V2C18()
	{
		Add(Scene(Temple).With(Brigitte, Damuel, Ella, Ferdinand, Fran, Monika, Myne, Nicola, Rosina));
		Add(Scene(KnightsOrder).With(Eckhart));
		Add(Scene(Castle).With(Justus));

		// Time: "And so began a series of meetings with blue priests that continued day after day."
		SkipToDaysAhead(2, Meetings);
		Add(Scene(Temple).With(Kampfer, Frietack));

		SkipToDaysAhead(2, Meetings);
		var s1 = AddR(Scene(Castle).With(Brigitte, Damuel, Ferdinand, Myne));
		AddUnit();
		Return(s1);
	}

	private void P3V2C19()
	{
		// Time: "Brigitte thanked her for her concern, then got into bed."
		SkipToCurrentDay(Bed);
		Add(Scene(KnightsOrder).With(Brigitte, Nadine));

		// karstedt talks to her about stuff, but adding him in makes the graph
		// look very messy at that point
		SkipToNextDay(Tea);
		Add(Scene(KarstedtsHouse).With(Brigitte, Nadine));
		AddUnit();
		Add(Scene(KnightsOrder).With(Nadine));
		// brigitte probably goes back to the knights order but
		// i haven't been bothering with any commuting points
		Add(Scene(Temple).With(Brigitte));

		// Time: "That night, Brigitte sent an ordonnanz to her brother, Giebe Illgner."
		AddUnit();
	}
}