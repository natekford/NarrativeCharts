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
		Update();
	}

	private void P3V2C01()
	{
		// Positions of characters at the end of the previous volume
		Add(Scene(Temple).With(Ferdinand, Myne, Damuel, Delia, Dirk, Egmont, Ella, Gil, Monika, Nicola, Rosina, Wilma, Zahm, Fran, Brigitte));
		Add(Scene(KarstedtsHouse).With(Karstedt, Elvira));
		Add(Scene(Castle).With(Sylvester, Charlotte, Florencia, Melchior, Rihyarda, Cornelius, Eckhart, Lamprecht, Wilfried, Angelica, Norbert, Ottilie));
		Add(Scene(KnightsOrder).With(Bindewald));
		Add(Scene(Hasse).With(Deid, Ingo));
		Add(Scene(LowerCityWorkshops).With(Johann, Zack));
		Add(Scene(MerchantCompanies).With(Benno, Freida, Gustav, Leon, Lutz, Mark));
		Add(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
		Add(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));

		// Time: After the coming of age ceremony in summer, no clue about what exact day or time of day aside from not night
		SkipToDaysAhead(2, Lunch);
		Add(Scene(MynesHouse).With(Effa, Tuuli));
		// Time: "That night, while Gunther was drinking"
		SkipToCurrentDay(Bed);
		Add(Scene(MynesHouse).With(Effa, Gunther, Kamil));

		// Time: "It’s due in three days"
		SkipToDaysAhead(3, Meetings);
		Add(Scene(Temple).With(Benno, Damuel, Effa, Fran, Lutz, Myne, Tuuli));
		Time.AddBell();
		Add(Scene(MynesHouse).With(Effa, Tuuli));
		Add(Scene(MerchantCompanies).With(Benno, Lutz));
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
		SkipToDaysAhead(2, Morning);
		// myne sees gunther, the gray priests, and gilberta company off
		Add(Scene(Temple).With(Benno, Gunther, Lutz, Mark, Myne));
		// Time: the story says it's half a day or something to get to hasse via carriage?
		Time.AddDay();
		Add(Scene(Hasse).With(Benno, Gunther, Lutz, Mark));

		// Time: "I would be heading for Hasse three days from now" said the day that the grays depart
		SkipToDaysAhead(2, Morning);
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
		Time.AddBell();
		Add(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Myne));
		Time.AddBell();
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

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
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

		// "Ferdinand summoned Benno the second we got back to the temple;"
		Add(Scene(Temple).With(Benno));
		Time.AddBell();
		Add(Scene(MerchantCompanies).With(Benno));
	}

	private void P3V2C06()
	{
		// Time: “We will visit again five days from now,” from c05
	}
}