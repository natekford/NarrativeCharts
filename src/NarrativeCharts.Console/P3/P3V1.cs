using NarrativeCharts.Models;

using static NarrativeCharts.Console.Characters;
using static NarrativeCharts.Console.Locations;

namespace NarrativeCharts.Console.P3;

public static class P3V1
{
	public static NarrativeChartGenerator Generate(BookwormTimeTracker time)
	{
		var p3v1 = new NarrativeChartGenerator();
		Point Scene(double location) => new(time.CurrentTotalHours, location);

		// Prologue starts with Karstedt seeing Sylvester off
		p3v1.AddScene(Scene(Temple).With(Ferdinand, Karstedt, Myne, Sylvester));
		// Time: Immediately after
		time.AddHour();
		// Sylvester goes back to AD conf
		p3v1.AddScene(Scene(RoyalAcademy).With(Sylvester));
		// Discussion about Myne
		p3v1.AddScene(Scene(Temple).With(Ferdinand, Karstedt));
		// karstedt probably goes back to his house after the discussion
		time.AddBell();
		p3v1.AddScene(Scene(KarstedtsHouse).With(Karstedt));

		// Time: Next morning
		time.GoToNextDay.Morning();
		// Karstedt interrogates these 2
		p3v1.AddScene(Scene(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
		// Time: Dinner
		time.GoToCurrentDay.Dinner();
		// Discussion about Myne's baptism
		p3v1.AddScene(Scene(KarstedtsHouse).With(Elvira, Ferdinand, Karstedt));

		// Time: The next day
		time.GoToNextDay.Morning();
		// Karstest keeps interrogating these 2
		p3v1.AddScene(Scene(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
		// Time: Not long after
		time.AddBell();
		// Myne's health checkup
		p3v1.AddScene(Scene(Temple).With(Ferdinand, Karstedt, Myne));
		// SS1 of P3V1, no exact timeline but definitely in this 4 day period between kardstedt/ferdi discussion and myne arrive
		// "Mother had said that she had important news and gathered Eckhart and I at the dinner table to discuss it over tea."
		p3v1.AddScene(Scene(KarstedtsHouse).With(Cornelius, Eckhart, Elvira));
		// Time: Immediately after
		time.AddBell();
		// Leaving Ferdinand's office and going back to Myne's room
		p3v1.AddScene(Scene(Temple).With(Damuel, Fran, Monika, Myne, Nicola, Rosina));
		p3v1.AddScene(Scene(KarstedtsHouse).With(Karstedt));

		// Time: Next 3 days
		time.GoToDaysAhead(3).Morning();
		// People in the temple getting ready for Myne going to the Noble's quarter
		p3v1.AddScene(Scene(Temple).With(Ella, Fran, Gil, Monika, Myne, Nicola, Rosina, Wilma));
		// Cooks learning with Leise at Guildmaster's house (or italian restaurant?)
		p3v1.AddScene(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
		// Time: Immediately after
		time.AddHour();
		// Leaving the temple and going to the Noble's quarter
		p3v1.AddScene(Scene(TempleFrontEntrance).With(Ella, Ferdinand, Karstedt, Myne, Rosina));
		// Time: Immediately after
		time.AddHour();
		// Arriving at Karstedt's house
		p3v1.AddScene(Scene(KarstedtsHouse).With(Cornelius, Ella, Elvira, Ferdinand, Karstedt, Myne, Rosina));

		// Karstedt and Cornelius commute to the Knight's Order every day so probably add that somehow?

		// Time: Ferdinand checks on Myne every 2 days so probably stays like 6 hours each time?
		time.AddBells(2);
		// Ferdinand going back to the temple
		p3v1.AddScene(Scene(Temple).With(Ferdinand));

		for (var i = 0; i < 2; ++i)
		{
			time.GoToDaysAhead(2).Meetings();
			p3v1.AddScene(Scene(KarstedtsHouse).With(Ferdinand));
			time.AddBells(2);
			p3v1.AddScene(Scene(Temple).With(Ferdinand));
		}

		// Time: On a day where Ferdinand isn't there?
		time.GoToNextDay.Meetings();
		// Gilberta company comes to sell some rinsham to Myne
		p3v1.AddScene(Scene(KarstedtsHouse).With(Benno, Mark));
		time.AddBell();
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Mark));

		// Time: A day after the Gilberta company visits?
		time.GoToNextDay.Meetings();
		// Ferdinand promises Myne book room key if she memorizes everyone's names before baptism
		p3v1.AddScene(Scene(KarstedtsHouse).With(Ferdinand));

		// Gil + Gilberta company go to some city's orphanage
		// Exact time isn't given but probably after Benno sells Elvira rinsham and definitely before Myne gets inagurated as
		// the High Bishop
		p3v1.AddScene(Scene(Hasse).With(Benno, Gil, Lutz));

		for (var i = 0; i < 3; ++i)
		{
			time.AddBells(2);
			p3v1.AddScene(Scene(Temple).With(Ferdinand));
			time.GoToDaysAhead(2).Meetings();
			p3v1.AddScene(Scene(KarstedtsHouse).With(Ferdinand));
		}
		time.AddBells(2);
		p3v1.AddScene(Scene(Temple).With(Ferdinand));

		// Time: The day before the baptism
		time.GoToNextDay.Meetings();
		// Eckhart and Lemprecht come back home from the knight's barracks for Myne's baptism
		p3v1.AddScene(Scene(KarstedtsHouse).With(Eckhart, Lamprecht));

		// Gil + Gilberta company come back from some city's orphanage
		// Exact time isn't given but definitely before Myne gets inagurated as the High Bishop
		p3v1.AddScene(Scene(Temple).With(Gil));
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));

		time.GoToNextDay.Meetings();
		// Everyone arrives for Myne's baptism (SS would add a bunch of characters)
		p3v1.AddScene(Scene(KarstedtsHouse).With(Ferdinand, Florencia, Sylvester, Wilfried));
		// Time: Immediately after the baptism ceremony
		time.AddBell();
		// Guard knights get introduced to Myne and Wilf makes Myne pass out
		p3v1.AddScene(Scene(KarstedtsHouse).With(Brigitte, Damuel));
		// Time: After Myne wakes up and talks with Ferdinand/Karstedt
		time.AddBell();
		p3v1.AddScene(Scene(Temple).With(Ferdinand));
		p3v1.AddScene(Scene(Castle).With(Florencia, Sylvester, Wilfried));

		// Time: "Ferdinand had told me to use the day after my baptism ceremony to rest."
		time.GoToNextDay.Morning();
		// Lamprecht gives Myne a book and goes back the castle
		p3v1.AddScene(Scene(Castle).With(Lamprecht));

		// Time: The next day
		time.GoToNextDay.Morning();
		// "Karstedt and Cornelius had already headed to the Knight’s Order, so Elvira was the only one to see me off."
		p3v1.AddScene(Scene(KnightsOrder).With(Cornelius, Karstedt));
		p3v1.AddScene(Scene(Temple).With(Brigitte, Damuel, Ella, Myne, Rosina));
		// Inaguration ceremony
		time.AddBell();
		// Egmont gets crushed a bit
		p3v1.AddScene(Scene(Temple).With(Egmont));

		// Time: "As I ate breakfast the next morning, Gil informed me ... meeting with the Gilberta Company later that day ..."
		// "The Gilberta Company was due to arrive at third bell"
		time.GoToNextDay.Morning();
		// Zahm introduced as Arno's replacement
		p3v1.AddScene(Scene(Temple).With(Benno, Lutz, Mark, Zahm));
		time.AddBell();
		// Leaving after discussing printing/restaurant
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Lutz, Mark));

		// Time: "As the Starbind Ceremony approached," idk, a couple days later?
		time.GoToDaysAhead(3).Meetings();
		// "Today was a day when Benno and Lutz were visiting from the Gilberta Company"
		p3v1.AddScene(Scene(Temple).With(Benno, Lutz));
		time.AddBell();
		// Leaving after discussing starbinding ceremony
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));
		// Time: "And so, the day of the Starbind Ceremony arrived."
		time.GoToDaysAhead(2).Morning();
		// "I thought about him, Lutz, and the kids who were about to head to the forest."
		p3v1.AddScene(Scene(LowerCityForest).With(Gil, Lutz));
		// "my whole family had come to see me as the High Bishop."
		p3v1.AddScene(Scene(Temple).With(Effa, Gunther, Kamil, Tuuli));
		// Time: lunch is between the lower city and noble starbindings
		time.AddBell();
		p3v1.AddScene(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
		p3v1.AddScene(Scene(Castle).With(Brigitte, Damuel, Ferdinand, Fran, Myne, Rosina));
		// Myne gets introduced to her temp guard knights and some attendents
		p3v1.AddScene(Scene(Castle).With(Angelica, Cornelius, Norbert, Rihyarda));
		// gil and lutz probably return from the forest fairly soon
		p3v1.AddScene(Scene(Temple).With(Gil));
		p3v1.AddScene(Scene(GilbertaCompany).With(Lutz));
		time.AddBell();
		// kids say good night to aub
		p3v1.AddScene(Scene(Castle).With(Charlotte, Melchior));
		// myne gets changed into high bishop outfit
		p3v1.AddScene(Scene(Castle).With(Ottilie));
		// karstedt was behind sylvester at the ceremony
		p3v1.AddScene(Scene(Castle).With(Karstedt));

		// Time: the next day
		time.GoToNextDay.Morning();
		// "He sent his attendants back to the temple without him"
		p3v1.AddScene(Scene(Temple).With(Fran, Rosina));
		// Time: "later that afternoon that my fever finally went down."
		time.GoToNextDay.MarketClose();
		// "He put me onto his highbeast and we returned to the temple accompanied by Damuel and Brigitte, who followed on either side of us."
		p3v1.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

		// Time: tomorrow
		time.GoToNextDay.Meetings();
		// "Leon will be coming by tomorrow to get the natural yeast"
		p3v1.AddScene(Scene(Temple).With(Leon));
		time.AddHour();
		p3v1.AddScene(Scene(GilbertaCompany).With(Leon));

		// Time: 3rd bell of the next day
		time.GoToNextDay.Meetings();
		// sylvester arrives in the temple early on the day of the italian restaurant visit
		p3v1.AddScene(Scene(Temple).With(Cornelius, Eckhart, Karstedt, Sylvester));
		time.AddHour();
		// rosina leaves early b/c she plays music
		p3v1.AddScene(Scene(ItalianRestaurant).With(Rosina));
		time.AddBell();
		p3v1.AddScene(Scene(ItalianRestaurant).With(Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Fran, Karstedt, Myne, Sylvester, Zahm));
		p3v1.AddScene(Scene(ItalianRestaurant).With(Benno, Freida, Gustav, Leon, Mark));
		// Time: after lunch at the italian restaurant
		time.AddBell();
		// hasse orphanage designed, fly over with highbeasts
		p3v1.AddScene(Scene(Hasse).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
		time.AddBell();
		// hasse orphanage is built and they return to the restaurant
		p3v1.AddScene(Scene(ItalianRestaurant).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
		time.AddHour();
		// ferdi hires todd as a chef to arrive in the tmple in 36 hours from now
		// syl hires hugo for the castle
		p3v1.AddScene(Scene(OthmarCompany).With(Freida, Gustav));
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Leon, Mark));
		p3v1.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
		p3v1.AddScene(Scene(KarstedtsHouse).With(Cornelius, Karstedt));
		p3v1.AddScene(Scene(KnightsOrder).With(Eckhart));
		p3v1.AddScene(Scene(Castle).With(Sylvester));

		// no transition between chapters so no idea the time difference between italian restaurant and donations chapter
		// but hugo is in the castle kitched at that point, and we can probably assume he goes at the same time
		// todd goes to ferdi's temple kitchen at 2nd bell
		time.GoToDaysAhead(2).Morning();
		p3v1.AddScene(Scene(Temple).With(Todd));
		p3v1.AddScene(Scene(Castle).With(Ella, Hugo));

		// Time: during the day, has to be around 17 days letter since ella went to the castle for a month and this is the only
		// section that has an unknown timeskip (the accounted for time between now and when ella is back in the temple during
		// elvira/lamp's visit is ~13 days)
		time.GoToDaysAhead(17).Meetings();
		// donation tea party with flor, elvira, and myne
		// brigitte mentioned in next segment, so other guard knights probably also there
		p3v1.AddScene(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Elvira, Myne));
		time.AddBell();
		// elvira probably goes back after the tea party
		p3v1.AddScene(Scene(KarstedtsHouse).With(Elvira));

		// Time: "I had been bedridden for two days since the tea party"
		time.GoToDaysAhead(2).Meetings();
		// ferdi, elvira, flor visit sick myne
		p3v1.AddScene(Scene(Castle).With(Elvira, Ferdinand));
		time.AddBell();
		// ferdi leaves after agreeing to play harspiel
		p3v1.AddScene(Scene(Temple).With(Ferdinand));

		time.GoToNextDay.Meetings();
		p3v1.AddScene(Scene(Castle).With(Ferdinand));
		time.AddHour();
		// ferdi and myne go to some place to train magic
		p3v1.AddScene(Scene(KnightsOrder).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
		time.AddBells(2);
		// myne blows up her feystone and reforms it then they go back to the castle
		p3v1.AddScene(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
		time.AddHour();
		p3v1.AddScene(Scene(Temple).With(Ferdinand));

		time.GoToNextDay.Meetings();
		p3v1.AddScene(Scene(Temple).With(Brigitte, Damuel, Myne));
		p3v1.AddScene(Scene(Temple).With(Benno, Lutz));
		time.AddBell();
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));
		// myne has dirk drain his mana with a taue fruit and cuts some trombes
		p3v1.AddScene(Scene(Temple).With(Delia, Dirk));

		time.GoToNextDay.Meetings();
		p3v1.AddScene(Scene(Temple).With(Johann, Lutz, Zack));
		p3v1.AddScene(Scene(LowerCityForest).With(Gil));
		time.AddBell();
		p3v1.AddScene(Scene(LowerCityWorkshops).With(Johann, Zack));
		p3v1.AddScene(Scene(GilbertaCompany).With(Lutz));
		p3v1.AddScene(Scene(Temple).With(Gil));

		time.GoToDaysAhead(3).Meetings();
		// johann and zack return with their blueprints for the wax stencil machine
		p3v1.AddScene(Scene(Temple).With(Johann, Lutz, Zack));
		time.AddBell();
		p3v1.AddScene(Scene(LowerCityWorkshops).With(Johann, Zack));
		p3v1.AddScene(Scene(GilbertaCompany).With(Lutz));

		time.GoToNextDay.Meetings();
		// tuuli gives myne a hairpin
		p3v1.AddScene(Scene(Temple).With(Lutz, Tuuli));
		time.AddBell();
		p3v1.AddScene(Scene(MynesHouse).With(Tuuli));
		p3v1.AddScene(Scene(GilbertaCompany).With(Lutz));

		time.GoToNextDay.Meetings();
		p3v1.AddScene(Scene(Temple).With(Lutz));
		time.AddBell();
		p3v1.AddScene(Scene(GilbertaCompany).With(Lutz));

		// Time: roughly a month after the chefs get loaned out
		time.GoToNextDay.Morning();
		p3v1.AddScene(Scene(Temple).With(Ella));
		p3v1.AddScene(Scene(ItalianRestaurant).With(Hugo, Todd));

		// Time: 3 days later
		time.GoToDaysAhead(2).Meetings();
		// elvira and lemprecht visit the temple to eat some of ella's food
		p3v1.AddScene(Scene(Temple).With(Elvira, Lamprecht));
		time.AddBell();
		p3v1.AddScene(Scene(KarstedtsHouse).With(Elvira));
		p3v1.AddScene(Scene(Castle).With(Lamprecht));

		// Time: unknown, not the same day
		time.GoToNextDay.Meetings();
		// lutz tells about johann and zack's progress
		p3v1.AddScene(Scene(Temple).With(Lutz));
		time.AddBell();
		// myne goes to the castle to make her highbeast
		p3v1.AddScene(Scene(KnightsOrder).With(Brigitte, Damuel, Ferdinand, Myne));
		time.AddBell();
		// they go back to the temple after myne creates lessy
		p3v1.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

		// Time: "From there, I spent my days practicing ... "
		// "It was the evening five days before Ferdinand’s concert."
		// 13 days since concert was said to be "in a month"
		// so a 12 day timeskip at minimum
		time.GoToDaysAhead(12).MarketClose();
		// lutz and smiths visit with a wax machine
		p3v1.AddScene(Scene(Temple).With(Lutz, Johann, Zack));
		time.AddBell();
		p3v1.AddScene(Scene(LowerCityWorkshops).With(Johann, Zack));
		p3v1.AddScene(Scene(GilbertaCompany).With(Lutz));

		// Time: "“Good morning, Lady Rozemyne,”"
		time.GoToNextDay.Morning();
		p3v1.AddScene(Scene(Temple).With(Lutz));
		time.AddBell();
		p3v1.AddScene(Scene(GilbertaCompany).With(Lutz));

		// Time: "I returned to the castle the day before the concert."
		time.GoToDaysAhead(3).Morning();
		// myne talks to ferdi and reminds him about the conert
		time.AddHour();
		// "We made our way to the castle. Ella and Rosina were in the carriage for attendants, while my two guard knights and I got into the carriage for nobles."
		p3v1.AddScene(Scene(Castle).With(Brigitte, Damuel, Ella, Myne, Rosina));
		// "Elvira and Florencia were already waiting for me in the castle."
		p3v1.AddScene(Scene(Castle).With(Elvira, Florencia));

		// Time: "And so came the day of the concert." meeting time? afternoon time? no clue
		time.GoToNextDay.MarketClose();
		p3v1.AddScene(Scene(Castle).With(Eckhart, Ferdinand, Karstedt));
		time.AddBell();
		p3v1.AddScene(Scene(Temple).With(Brigitte, Damuel, Ella, Ferdinand, Myne, Rosina));
		p3v1.AddScene(Scene(KarstedtsHouse).With(Elvira, Karstedt));

		// Time: "It was several days after the concert,"
		time.GoToDaysAhead(3).Meetings();
		// "Ferdinand had summoned me to his lecture room just like in the old days."
		p3v1.AddScene(Scene(Temple).With(Ferdinand, Myne));

		// Time: some time after myne gets lectured
		// "“Lutz, the customers have all left,”"
		time.GoToDaysAhead(2).MarketClose();
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Lutz, Mark));
		// "Ingo and his wife—the owners of the carpentry workshop that Rozemyne exclusively gave business to—were currently living in the monastery in Hasse"
		// "Deid, would also be heading there soon"
		p3v1.AddScene(Scene(Hasse).With(Deid, Ingo));

		// Time: "Two days later, Lutz, Benno, and Tuuli went to the orphanage director’s chambers."
		time.GoToDaysAhead(2).Meetings();
		p3v1.AddScene(Scene(Temple).With(Benno, Lutz, Tuuli));
		time.AddBell();
		p3v1.AddScene(Scene(MynesHouse).With(Tuuli));
		p3v1.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));

		return p3v1;
	}
}