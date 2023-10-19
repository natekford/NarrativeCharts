using NarrativeCharts.Models;

using static NarrativeCharts.Console.Characters;
using static NarrativeCharts.Console.Locations;

namespace NarrativeCharts.Console.P3;

public static class P3V2
{
	public static NarrativeChart Generate(BookwormTimeTracker time)
	{
		var p3v2 = new NarrativeChart();
		Point Scene(int location) => new(time.CurrentTotalHours, location);

		// Chapter: Prologue
		{
			// Time: After the coming of age ceremony in summer, no clue about what exact day or time of day aside from not night
			time.GoToDaysAhead(2).Lunch();
			p3v2.AddScene(Scene(MynesHouse).With(Effa, Tuuli));
			// Time: "That night, while Gunther was drinking"
			time.GoToCurrentDay.Bed();
			p3v2.AddScene(Scene(MynesHouse).With(Effa, Gunther, Kamil));

			// Time: "It’s due in three days"
			time.GoToDaysAhead(3).Meetings();
			p3v2.AddScene(Scene(Temple).With(Benno, Damuel, Effa, Fran, Lutz, Myne, Tuuli));
			time.AddBell();
			p3v2.AddScene(Scene(MynesHouse).With(Effa, Tuuli));
			p3v2.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));
		}

		// Chapter: Discussing the Harvest Festival
		{
			// Time: "There will be a meeting at third bell today." on the day after the autumn coming of age ceremony
			time.GoToDaysAhead(2).Meetings();
			// fran explains the harvest festival to myne then they go to a meeting with the blue priests
			p3v2.AddScene(Scene(Temple).With(Egmont, Ferdinand, Fran, Myne));

			// Time: "Not long after lunch"
			time.GoToCurrentDay.Lunch();
			p3v2.AddScene(Scene(Temple).With(Ferdinand, Myne, Zahm));
			time.AddHour();
			// lutz and myne talk about gunther being assigned as a guard knight for the hasse carriages
			p3v2.AddScene(Scene(Temple).With(Damuel, Gil, Lutz, Myne));
			time.AddBell();
			// lutz tells gunther that he will be able to see myne
			p3v2.AddScene(Scene(MynesHouse).With(Effa, Gunther, Lutz, Tuuli));
			time.AddHour();
			p3v2.AddScene(Scene(GilbertaCompany).With(Lutz));
		}

		// Chatper: Hasse's Monastery
		{
			// Time: probably the morning? no idea how many days later tho
			time.GoToDaysAhead(2).Morning();
			// myne sees gunther, the gray priests, and gilberta company off
			p3v2.AddScene(Scene(Temple).With(Benno, Gunther, Lutz, Mark, Myne));
			// Time: the story says it's half a day or something to get to hasse via carriage?
			time.AddDay();
			p3v2.AddScene(Scene(Hasse).With(Benno, Gunther, Lutz, Mark));

			// Time: "I would be heading for Hasse three days from now" said the day that the grays depart
			time.GoToDaysAhead(2).Morning();
			p3v2.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
			time.AddHour();
			p3v2.AddScene(Scene(Hasse).With(Brigitte, Damuel, Ferdinand, Myne));
		}

		return p3v2;
	}
}