using static NarrativeCharts.Console.Characters;
using static NarrativeCharts.Console.Locations;
using NarrativeCharts.Models;

namespace NarrativeCharts.Console;

public static class Program
{
	private static void Main()
	{
		var generator = new NarrativeChartGenerator();
		var time = new BookwormTimeTracker();
		Point Point(double location) => new(time.CurrentTotalHours, location);

		// Prologue starts with Karstedt seeing Sylvester off
		generator.AddScene(Point(Temple).With(Ferdinand, Karstedt, Myne, Sylvester));
		// Time: Immediately after
		time.AddHour();
		// Sylvester goes back to AD conf
		generator.AddScene(Point(RoyalAcademy).With(Sylvester));
		// Discussion about Myne
		generator.AddScene(Point(Temple).With(Ferdinand, Karstedt));
		// karstedt probably goes back to his house after the discussion
		time.AddBell();
		generator.AddScene(Point(KarstedtsHouse).With(Karstedt));

		// Time: Next morning
		time.GoToNextDay.Morning();
		// Karstedt interrogates these 2
		generator.AddScene(Point(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
		// Time: Dinner
		time.GoToCurrentDay.Dinner();
		// Discussion about Myne's baptism
		generator.AddScene(Point(KarstedtsHouse).With(Elvira, Ferdinand, Karstedt));

		// Time: The next day
		time.GoToNextDay.Morning();
		// Karstest keeps interrogating these 2
		generator.AddScene(Point(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
		// Time: Not long after
		time.AddBell();
		// Myne's health checkup
		generator.AddScene(Point(Temple).With(Ferdinand, Karstedt, Myne));
		// Time: Immediately after
		time.AddBell();
		// Leaving Ferdinand's office and going back to Myne's room
		generator.AddScene(Point(Temple).With(Damuel, Fran, Monika, Myne, Nicola, Rosina));

		// Time: Next 3 days
		time.GoToDaysAhead(3).Morning();
		// People in the temple getting ready for Myne going to the Noble's quarter
		generator.AddScene(Point(Temple).With(Ella, Fran, Gil, Monika, Myne, Nicola, Rosina, Wilma));
		// Cooks learning with Leise at Guildmaster's house (or italian restaurant?)
		generator.AddScene(Point(ItalianRestaurant).With(Hugo, Leise, Todd));
		// Time: Immediately after
		time.AddHour();
		// Leaving the temple and going to the Noble's quarter
		generator.AddScene(Point(TempleFrontEntrance).With(Ella, Ferdinand, Karstedt, Myne, Rosina));
		// Time: Immediately after
		time.AddHour();
		// Arriving at Karstedt's house
		generator.AddScene(Point(KarstedtsHouse).With(Cornelius, Ella, Elvira, Ferdinand, Karstedt, Myne, Rosina));

		// Karstedt and Cornelius commute to the Knight's Order every day so probably add that somehow?

		// Time: Ferdinand checks on Myne every 2 days so probably stays like 6 hours each time?
		time.AddBells(2);
		// Ferdinand going back to the temple
		generator.AddScene(Point(Temple).With(Ferdinand));

		for (var i = 0; i < 2; ++i)
		{
			time.GoToDaysAhead(2).Meetings();
			generator.AddScene(Point(KarstedtsHouse).With(Ferdinand));
			time.AddBells(2);
			generator.AddScene(Point(Temple).With(Ferdinand));
		}

		// Time: On a day where Ferdinand isn't there?
		time.GoToNextDay.Meetings();
		// Gilberta company comes to sell some rinsham to Myne
		generator.AddScene(Point(KarstedtsHouse).With(Benno, Mark));
		time.AddBell();
		generator.AddScene(Point(GilbertaCompany).With(Benno, Mark));

		// Time: A day after the Gilberta company visits?
		time.GoToNextDay.Meetings();
		// Ferdinand promises Myne book room key if she memorizes everyone's names before baptism
		generator.AddScene(Point(KarstedtsHouse).With(Ferdinand));

		// Gil + Gilberta company go to some city's orphanage
		// Exact time isn't given but probably after Benno sells Elvira rinsham and definitely before Myne gets inagurated as
		// the High Bishop
		generator.AddScene(Point(Hasse).With(Benno, Gil, Lutz));

		for (var i = 0; i < 3; ++i)
		{
			time.AddBells(2);
			generator.AddScene(Point(Temple).With(Ferdinand));
			time.GoToDaysAhead(2).Meetings();
			generator.AddScene(Point(KarstedtsHouse).With(Ferdinand));
		}
		time.AddBells(2);
		generator.AddScene(Point(Temple).With(Ferdinand));

		// Time: The day before the baptism
		time.GoToNextDay.Meetings();
		// Eckhart and Lemprecht come back home from the knight's barracks for Myne's baptism
		generator.AddScene(Point(KarstedtsHouse).With(Eckhart, Lamprecht));

		// Gil + Gilberta company come back from some city's orphanage
		// Exact time isn't given but definitely before Myne gets inagurated as the High Bishop
		generator.AddScene(Point(Temple).With(Gil));
		generator.AddScene(Point(GilbertaCompany).With(Benno, Lutz));

		time.GoToNextDay.Meetings();
		// Everyone arrives for Myne's baptism (SS would add a bunch of characters)
		generator.AddScene(Point(KarstedtsHouse).With(Ferdinand, Florencia, Sylvester, Wilfried));
		// Time: Immediately after the baptism ceremony
		time.AddBell();
		// Guard knights get introduced to Myne and Wilf makes Myne pass out
		generator.AddScene(Point(KarstedtsHouse).With(Brigitte, Damuel));
		// Time: After Myne wakes up and talks with Ferdinand/Karstedt
		time.AddBell();
		generator.AddScene(Point(Temple).With(Ferdinand));
		generator.AddScene(Point(Castle).With(Florencia, Sylvester, Wilfried));

		// Time: "Ferdinand had told me to use the day after my baptism ceremony to rest."
		time.GoToNextDay.Morning();
		// Lamprecht gives Myne a book and goes back the castle
		generator.AddScene(Point(Castle).With(Lamprecht));

		// Time: The next day
		time.GoToNextDay.Morning();
		// "Karstedt and Cornelius had already headed to the Knight’s Order, so Elvira was the only one to see me off."
		generator.AddScene(Point(KnightsOrder).With(Cornelius, Karstedt));
		generator.AddScene(Point(Temple).With(Brigitte, Damuel, Ella, Myne, Rosina));
		// Inaguration ceremony
		time.AddBell();
		// Egmont gets crushed a bit
		generator.AddScene(Point(Temple).With(Egmont));

		// Time: "As I ate breakfast the next morning, Gil informed me ... meeting with the Gilberta Company later that day ..."
		// "The Gilberta Company was due to arrive at third bell"
		time.GoToNextDay.Morning();
		// Zahm introduced as Arno's replacement
		generator.AddScene(Point(Temple).With(Benno, Lutz, Mark, Zahm));
		time.AddBell();
		// Leaving after discussing printing/restaurant
		generator.AddScene(Point(GilbertaCompany).With(Benno, Lutz, Mark));

		// Time: "As the Starbind Ceremony approached," idk, a couple days later?
		time.GoToDaysAhead(3).Meetings();
		// "Today was a day when Benno and Lutz were visiting from the Gilberta Company"
		generator.AddScene(Point(Temple).With(Benno, Lutz));
		time.AddBell();
		// Leaving after discussing starbinding ceremony
		generator.AddScene(Point(GilbertaCompany).With(Benno, Lutz));
		// Time: "And so, the day of the Starbind Ceremony arrived."
		time.GoToDaysAhead(2).Morning();
		// "I thought about him, Lutz, and the kids who were about to head to the forest."
		generator.AddScene(Point(LowerCityForest).With(Gil, Lutz));
		// "my whole family had come to see me as the High Bishop."
		generator.AddScene(Point(Temple).With(Effa, Gunther, Kamil, Tuuli));
		// Time: lunch is between the lower city and noble starbindings
		time.AddBell();
		generator.AddScene(Point(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
		generator.AddScene(Point(Castle).With(Brigitte, Damuel, Ferdinand, Fran, Myne, Rosina));
		// Myne gets introduced to her temp guard knights and some attendents
		generator.AddScene(Point(Castle).With(Angelica, Cornelius, Norbert, Rihyarda));
		// gil and lutz probably return from the forest fairly soon
		generator.AddScene(Point(Temple).With(Gil));
		generator.AddScene(Point(GilbertaCompany).With(Lutz));
		time.AddBell();
		// kids say good night to aub
		generator.AddScene(Point(Castle).With(Charlotte, Melchior));
		// myne gets changed into high bishop outfit
		generator.AddScene(Point(Castle).With(Ottilie));
		// karstedt was behind sylvester at the ceremony
		generator.AddScene(Point(Castle).With(Karstedt));

		// Time: the next day
		time.GoToNextDay.Morning();
		// "He sent his attendants back to the temple without him"
		generator.AddScene(Point(Temple).With(Fran, Rosina));
		// Time: "later that afternoon that my fever finally went down."
		time.GoToNextDay.MarketClose();
		// "He put me onto his highbeast and we returned to the temple accompanied by Damuel and Brigitte, who followed on either side of us."
		generator.AddScene(Point(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

		// Time: tomorrow
		time.GoToNextDay.Meetings();
		// "Leon will be coming by tomorrow to get the natural yeast"
		generator.AddScene(Point(Temple).With(Leon));
		time.AddHour();
		generator.AddScene(Point(GilbertaCompany).With(Leon));

		// Time: 3rd bell of the next day
		time.GoToNextDay.Meetings();
		// sylvester arrives in the temple early on the day of the italian restaurant visit
		generator.AddScene(Point(Temple).With(Cornelius, Eckhart, Karstedt, Sylvester));
		time.AddHour();
		// rosina leaves early b/c she plays music
		generator.AddScene(Point(ItalianRestaurant).With(Rosina));
		time.AddBell();
		generator.AddScene(Point(ItalianRestaurant).With(Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Fran, Karstedt, Myne, Sylvester, Zahm));
		generator.AddScene(Point(ItalianRestaurant).With(Benno, Freida, Gustav, Leon, Mark));
		// Time: after lunch at the italian restaurant
		time.AddBell();
		// hasse orphanage designed, fly over with highbeasts
		generator.AddScene(Point(Hasse).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
		time.AddBell();
		// hasse orphanage is built and they return to the restaurant
		generator.AddScene(Point(ItalianRestaurant).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
		time.AddHour();
		// ferdi hires todd as a chef to arrive in the tmple in 36 hours from now
		// syl hires hugo for the castle
		generator.AddScene(Point(OthmarCompany).With(Freida, Gustav));
		generator.AddScene(Point(GilbertaCompany).With(Benno, Leon, Mark));
		generator.AddScene(Point(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
		generator.AddScene(Point(KarstedtsHouse).With(Cornelius, Karstedt));
		generator.AddScene(Point(KnightsOrder).With(Eckhart));
		generator.AddScene(Point(Castle).With(Sylvester));

		// no transition between chapters so no idea the time difference between italian restaurant and donations chapter
		// but hugo is in the castle kitched at that point, and we can probably assume he goes at the same time
		// todd goes to ferdi's temple kitchen at 2nd bell
		time.GoToDaysAhead(2).Morning();
		generator.AddScene(Point(Temple).With(Todd));
		generator.AddScene(Point(Castle).With(Hugo));

		// Time: during the day, idk, several days after the chefs arrive?
		time.GoToDaysAhead(2).Meetings();
		// donation tea party with flor, elvira, and myne
		// brigitte mentioned in next segment, so other guard knights probably also there
		generator.AddScene(Point(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Elvira, Myne));
		time.AddBell();
		// elvira probably goes back after the tea party
		generator.AddScene(Point(KarstedtsHouse).With(Elvira));

		// Time: "I had been bedridden for two days since the tea party"
		time.GoToDaysAhead(2).Meetings();
		// ferdi, elvira, flor visit sick myne
		generator.AddScene(Point(Castle).With(Elvira, Ferdinand));
		time.AddBell();
		// ferdi leaves after agreeing to play harspiel
		generator.AddScene(Point(Temple).With(Ferdinand));

		time.GoToNextDay.Meetings();
		generator.AddScene(Point(Castle).With(Ferdinand));
		time.AddHour();
		// ferdi and myne go to some place to train magic
		generator.AddScene(Point(KnightsOrder).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
		time.AddBells(2);
		// myne blows up her feystone and reforms it then they go back to the castle
		generator.AddScene(Point(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
		time.AddHour();
		generator.AddScene(Point(Temple).With(Ferdinand));

		time.GoToNextDay.Meetings();
		generator.AddScene(Point(Temple).With(Brigitte, Damuel, Myne));
		generator.AddScene(Point(Temple).With(Benno, Lutz));
		time.AddBell();
		generator.AddScene(Point(GilbertaCompany).With(Benno, Lutz));
		// myne has dirk drain his mana with a taue fruit and cuts some trombes
		generator.AddScene(Point(Temple).With(Delia, Dirk));

		time.GoToNextDay.Meetings();
		generator.AddScene(Point(Temple).With(Johann, Lutz, Zack));
		generator.AddScene(Point(LowerCityForest).With(Gil));
		time.AddBell();
		generator.AddScene(Point(LowerCityWorkshops).With(Johann, Zack));
		generator.AddScene(Point(GilbertaCompany).With(Lutz));
		generator.AddScene(Point(Temple).With(Gil));

		time.GoToDaysAhead(3).Meetings();
		// johann and zack return with their blueprints for the wax stencil machine
		generator.AddScene(Point(Temple).With(Johann, Lutz, Zack));
		time.AddBell();
		generator.AddScene(Point(LowerCityWorkshops).With(Johann, Zack));
		generator.AddScene(Point(GilbertaCompany).With(Lutz));

		time.GoToNextDay.Meetings();
		// tuuli gives myne a hairpin
		generator.AddScene(Point(Temple).With(Lutz, Tuuli));
		time.AddBell();
		generator.AddScene(Point(MynesHouse).With(Tuuli));
		generator.AddScene(Point(GilbertaCompany).With(Lutz));

		time.GoToNextDay.Meetings();
		generator.AddScene(Point(Temple).With(Lutz));
		time.AddBell();
		generator.AddScene(Point(GilbertaCompany).With(Lutz));

		// Time: 3 days later
		time.GoToDaysAhead(3).Meetings();
		// elvira and lemprecht visit the temple to eat some of ella's food
		generator.AddScene(Point(Temple).With(Elvira, Lamprecht));
		time.AddBell();
		generator.AddScene(Point(KarstedtsHouse).With(Elvira));
		generator.AddScene(Point(Castle).With(Lamprecht));

		// Time: unknown, not the same day
		time.GoToNextDay.Meetings();
		// lutz tells about johann and zack's progress
		generator.AddScene(Point(Temple).With(Lutz));
		time.AddBell();
		// myne goes to the castle to make her highbeast
		generator.AddScene(Point(KnightsOrder).With(Brigitte, Damuel, Ferdinand, Myne));
		time.AddBell();
		// they go back to the temple after myne creates lessy
		generator.AddScene(Point(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

		// Time: "From there, I spent my days practicing ... "
		// "It was the evening five days before Ferdinand’s concert."
		// 13 days since concert was said to be "in a month"
		// so a 12 day timeskip at minimum
		time.GoToDaysAhead(12).Meetings();
		// lutz and smiths visit with a wax machine
		generator.AddScene(Point(Temple).With(Lutz, Johann, Zack));
		time.AddBell();

		System.Console.WriteLine(time.CurrentTotalHours);
		System.Console.ReadLine();
	}
}