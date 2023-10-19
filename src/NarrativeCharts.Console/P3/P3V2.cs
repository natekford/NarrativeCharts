using static NarrativeCharts.Console.Characters;
using static NarrativeCharts.Console.Locations;

namespace NarrativeCharts.Console.P3;

public sealed class P3V2 : BookwormNarrativeChart
{
	public P3V2(BookwormTimeTracker time) : base(time)
	{
	}

	protected override void ProtectedCreate()
	{
		Chapter("Prologue");
		{
			// Time: After the coming of age ceremony in summer, no clue about what exact day or time of day aside from not night
			Time.GoToDaysAhead(2).Lunch();
			this.AddScene(Scene(MynesHouse).With(Effa, Tuuli));
			// Time: "That night, while Gunther was drinking"
			Time.GoToCurrentDay.Bed();
			this.AddScene(Scene(MynesHouse).With(Effa, Gunther, Kamil));

			// Time: "It’s due in three days"
			Time.GoToDaysAhead(3).Meetings();
			this.AddScene(Scene(Temple).With(Benno, Damuel, Effa, Fran, Lutz, Myne, Tuuli));
			Time.AddBell();
			this.AddScene(Scene(MynesHouse).With(Effa, Tuuli));
			this.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));
		}

		Chapter("Discussing the Harvest Festival");
		{
			// Time: "There will be a meeting at third bell today." on the day after the autumn coming of age ceremony
			Time.GoToDaysAhead(2).Meetings();
			// fran explains the harvest festival to myne then they go to a meeting with the blue priests
			this.AddScene(Scene(Temple).With(Egmont, Ferdinand, Fran, Myne));

			// Time: "Not long after lunch"
			Time.GoToCurrentDay.Lunch();
			this.AddScene(Scene(Temple).With(Ferdinand, Myne, Zahm));
			Time.AddHour();
			// lutz and myne talk about gunther being assigned as a guard knight for the hasse carriages
			this.AddScene(Scene(Temple).With(Damuel, Gil, Lutz, Myne));
			Time.AddBell();
			// lutz tells gunther that he will be able to see myne
			this.AddScene(Scene(MynesHouse).With(Effa, Gunther, Lutz, Tuuli));
			Time.AddHour();
			this.AddScene(Scene(GilbertaCompany).With(Lutz));
		}

		Chapter("Hasse's Monastery");
		{
			// Time: probably the morning? no idea how many days later tho
			Time.GoToDaysAhead(2).Morning();
			// myne sees gunther, the gray priests, and gilberta company off
			this.AddScene(Scene(Temple).With(Benno, Gunther, Lutz, Mark, Myne));
			// Time: the story says it's half a day or something to get to hasse via carriage?
			Time.AddDay();
			this.AddScene(Scene(Hasse).With(Benno, Gunther, Lutz, Mark));

			// Time: "I would be heading for Hasse three days from now" said the day that the grays depart
			Time.GoToDaysAhead(2).Morning();
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
			Time.AddHour();
			this.AddScene(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Myne));
		}

		Chapter("End");
	}
}