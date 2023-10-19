using static NarrativeCharts.Console.Characters;
using static NarrativeCharts.Console.Locations;

namespace NarrativeCharts.Console.P3;

public sealed class P3V2 : BookwormNarrativeChart
{
	public override string Name => nameof(P3V2);

	public P3V2(BookwormTimeTracker time) : base(time)
	{
	}

	protected override void ProtectedCreate()
	{
		Chapter("Prologue");
		{
			// Positions of characters at the end of the previous volume
			Add(Scene(Temple).With(Ferdinand, Myne, Damuel, Delia, Dirk, Egmont, Ella, Gil, Monika, Nicola, Rosina, Wilma, Zahm, Fran, Brigitte));
			Add(Scene(KarstedtsHouse).With(Karstedt, Elvira));
			Add(Scene(Castle).With(Sylvester, Charlotte, Florencia, Melchior, Rihyarda, Cornelius, Eckhart, Lamprecht, Wilfried, Angelica, Norbert, Ottilie));
			Add(Scene(KnightsOrder).With(Bezewanst, Bindewald));
			Add(Scene(Hasse).With(Deid, Ingo));
			Add(Scene(LowerCityWorkshops).With(Johann, Zack));
			Add(Scene(GilbertaCompany).With(Benno, Leon, Lutz, Mark));
			Add(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
			Add(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
			Add(Scene(OthmarCompany).With(Freida, Gustav));

			// Time: After the coming of age ceremony in summer, no clue about what exact day or time of day aside from not night
			Time.GoToDaysAhead(2).Lunch();
			Add(Scene(MynesHouse).With(Effa, Tuuli));
			// Time: "That night, while Gunther was drinking"
			Time.GoToCurrentDay.Bed();
			Add(Scene(MynesHouse).With(Effa, Gunther, Kamil));

			// Time: "It’s due in three days"
			Time.GoToDaysAhead(3).Morning();
			UpdateAndAddBell();
			Add(Scene(Temple).With(Benno, Damuel, Effa, Fran, Lutz, Myne, Tuuli));
			Time.AddBell();
			Add(Scene(MynesHouse).With(Effa, Tuuli));
			Add(Scene(GilbertaCompany).With(Benno, Lutz));
		}

		Chapter("Discussing the Harvest Festival");
		{
			// Time: "There will be a meeting at third bell today." on the day after the autumn coming of age ceremony
			Time.GoToDaysAhead(2).Meetings();
			// fran explains the harvest festival to myne then they go to a meeting with the blue priests
			Add(Scene(Temple).With(Egmont, Ferdinand, Fran, Myne));

			// Time: "Not long after lunch"
			Time.GoToCurrentDay.Lunch();
			Add(Scene(Temple).With(Ferdinand, Myne, Zahm));
			Time.AddHour();
			// lutz and myne talk about gunther being assigned as a guard knight for the hasse carriages
			Add(Scene(Temple).With(Damuel, Gil, Lutz, Myne));
			Time.AddBell();
			// lutz tells gunther that he will be able to see myne
			Add(Scene(MynesHouse).With(Effa, Gunther, Lutz, Tuuli));
			Time.AddHour();
			Add(Scene(GilbertaCompany).With(Lutz));
		}

		Chapter("Hasse's Monastery");
		{
			// Time: probably the morning? no idea how many days later tho
			Time.GoToDaysAhead(2).EarlyMorning();
			UpdateAndAddBell();
			// myne sees gunther, the gray priests, and gilberta company off
			Add(Scene(Temple).With(Benno, Gunther, Lutz, Mark, Myne));
			// Time: the story says it's half a day or something to get to hasse via carriage?
			Time.AddDay();
			Add(Scene(Hasse).With(Benno, Gunther, Lutz, Mark));

			// Time: "I would be heading for Hasse three days from now" said the day that the grays depart
			Time.GoToDaysAhead(2).EarlyMorning();
			UpdateAndAddBell();
			Event("Myne Goes To Hasse");
			Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
			Time.AddHour();
			Add(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Myne));
		}

		Chapter("End");
	}
}