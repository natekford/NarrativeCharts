using static NarrativeCharts.Console.Characters;
using static NarrativeCharts.Console.Locations;

namespace NarrativeCharts.Console.P3;

public sealed class P3V1 : BookwormNarrativeChart
{
	public P3V1(BookwormTimeTracker time) : base(time)
	{
	}

	protected override void ProtectedCreate()
	{
		Chapter("Prologue");
		{
			// Prologue starts with Karstedt seeing Sylvester off
			this.AddScene(Scene(Temple).With(Ferdinand, Karstedt, Myne, Sylvester));
			this.AddScene(Scene(KnightsOrder).With(Bezewanst, Bindewald));
			this.AddScene(Scene(Temple).With(Damuel, Delia, Dirk, Egmont, Ella, Gil, Monika, Nicola, Rosina, Wilma, Zahm));
			this.AddScene(Scene(LowerCityWorkshops).With(Deid, Ingo, Johann));
			this.AddScene(Scene(GilbertaCompany).With(Benno, Leon, Lutz, Mark));
			this.AddScene(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
			this.AddScene(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
			this.AddScene(Scene(OthmarCompany).With(Freida, Gustav));
			// Time: Immediately after
			Time.AddHour();
			// Sylvester goes back to AD conf
			this.AddScene(Scene(RoyalAcademy).With(Sylvester));
			// Discussion about Myne
			this.AddScene(Scene(Temple).With(Ferdinand, Karstedt));
			// karstedt probably goes back to his house after the discussion
			Time.AddBell();
			this.AddScene(Scene(KarstedtsHouse).With(Karstedt));

			// Time: Next morning
			Time.GoToNextDay.Morning();
			// Karstedt interrogates these 2
			this.AddScene(Scene(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
			// Time: Dinner
			Time.GoToCurrentDay.Dinner();
			// Discussion about Myne's baptism
			this.AddScene(Scene(KarstedtsHouse).With(Elvira, Ferdinand, Karstedt));

			// Time: The next day
			Time.GoToNextDay.Morning();
			// Karstest keeps interrogating these 2
			this.AddScene(Scene(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
			// Time: Not long after
			Time.AddBell();
			// Myne's health checkup
			Event("Myne's Health Checkup");
			this.AddScene(Scene(Temple).With(Ferdinand, Karstedt, Myne));
			// SS1 of P3V1, no exact timeline but definitely in this 4 day period between kardstedt/ferdi discussion and myne arrive
			// "Mother had said that she had important news and gathered Eckhart and I at the dinner table to discuss it over tea."
			this.AddScene(Scene(KarstedtsHouse).With(Cornelius, Eckhart, Elvira));
		}

		Chapter("Examination Results and the Noble's Quarter");
		{
			// Time: Immediately after
			Time.AddBell();
			// Leaving Ferdinand's office and going back to Myne's room
			this.AddScene(Scene(Temple).With(Damuel, Fran, Monika, Myne, Nicola, Rosina));
			this.AddScene(Scene(KarstedtsHouse).With(Karstedt));

			// Time: Next 3 days
			Time.GoToDaysAhead(3).Morning();
			// People in the temple getting ready for Myne going to the Noble's quarter
			this.AddScene(Scene(Temple).With(Ella, Fran, Gil, Monika, Myne, Nicola, Rosina, Wilma));
			// Cooks learning with Leise at Guildmaster's house (or italian restaurant?)
			this.AddScene(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
			// Time: Immediately after
			Time.AddHour();
			// Leaving the temple through the front entrance and going to the Noble's quarter
			this.AddScene(Scene(Temple).With(Ella, Ferdinand, Karstedt, Myne, Rosina));
			// Time: Immediately after
			Time.AddHour();
			// Arriving at Karstedt's house
			this.AddScene(Scene(KarstedtsHouse).With(Cornelius, Ella, Elvira, Ferdinand, Karstedt, Myne, Rosina));
		}

		// Karstedt and Cornelius commute to the Knight's Order every day so probably add that somehow?

		Chapter("Preparing for the Baptism Ceremony");
		{
			// Time: Ferdinand checks on Myne every 2 days so probably stays like 6 hours each time?
			Time.AddBells(2);
			// Ferdinand going back to the temple
			this.AddScene(Scene(Temple).With(Ferdinand));

			for (var i = 0; i < 2; ++i)
			{
				Time.GoToDaysAhead(2).Meetings();
				this.AddScene(Scene(KarstedtsHouse).With(Ferdinand));
				Time.AddBells(2);
				this.AddScene(Scene(Temple).With(Ferdinand));
			}

			// Time: On a day where Ferdinand isn't there?
			Time.GoToNextDay.Meetings();
			// Gilberta company comes to sell some rinsham to Myne
			this.AddScene(Scene(KarstedtsHouse).With(Benno, Mark));
			Time.AddBell();
			this.AddScene(Scene(GilbertaCompany).With(Benno, Mark));

			// Time: A day after the Gilberta company visits?
			Time.GoToNextDay.Meetings();
			// Ferdinand promises Myne book room key if she memorizes everyone's names before baptism
			this.AddScene(Scene(KarstedtsHouse).With(Ferdinand));

			// Gil + Gilberta company go to some city's orphanage
			// Exact time isn't given but probably after Benno sells Elvira rinsham and definitely before Myne gets inagurated as
			// the High Bishop
			this.AddScene(Scene(Hasse).With(Benno, Gil, Lutz));

			for (var i = 0; i < 3; ++i)
			{
				Time.AddBells(2);
				this.AddScene(Scene(Temple).With(Ferdinand));
				Time.GoToDaysAhead(2).Meetings();
				this.AddScene(Scene(KarstedtsHouse).With(Ferdinand));
			}
			Time.AddBells(2);
			this.AddScene(Scene(Temple).With(Ferdinand));

			// Time: The day before the baptism
			Time.GoToNextDay.Meetings();
			// Eckhart and Lemprecht come back home from the knight's barracks for Myne's baptism
			this.AddScene(Scene(KarstedtsHouse).With(Eckhart, Lamprecht));

			// Gil + Gilberta company come back from some city's orphanage
			// Exact time isn't given but definitely before Myne gets inagurated as the High Bishop
			this.AddScene(Scene(Temple).With(Gil));
			this.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));
		}

		Chapter("A Noble's Baptism Ceremony");
		{
			Time.GoToNextDay.Meetings();
			// Everyone arrives for Myne's baptism (SS would add a bunch of characters)
			Event("Myne's Baptism");
			this.AddScene(Scene(KarstedtsHouse).With(Ferdinand, Florencia, Sylvester, Wilfried));
		}

		Chapter("Adoption");
		{
			// Time: Immediately after the baptism ceremony
			Time.AddBell();
			// Guard knights get introduced to Myne and Wilf makes Myne pass out
			this.AddScene(Scene(KarstedtsHouse).With(Brigitte, Damuel));
			// Time: After Myne wakes up and talks with Ferdinand/Karstedt
			Time.AddBell();
			this.AddScene(Scene(Temple).With(Ferdinand));
			this.AddScene(Scene(Castle).With(Florencia, Sylvester, Wilfried));
		}

		Chapter("Inaguration Ceremony");
		{
			// Time: "Ferdinand had told me to use the day after my baptism ceremony to rest."
			Time.GoToNextDay.Morning();
			// Lamprecht gives Myne a book and goes back the castle
			this.AddScene(Scene(Castle).With(Lamprecht));

			// Time: The next day
			Time.GoToNextDay.Morning();
			// "Karstedt and Cornelius had already headed to the Knight’s Order, so Elvira was the only one to see me off."
			this.AddScene(Scene(KnightsOrder).With(Cornelius, Karstedt));
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Ella, Myne, Rosina));
			// Inaguration ceremony
			Time.AddBell();
			// Egmont gets crushed a bit
			this.AddScene(Scene(Temple).With(Egmont));
		}

		Chapter("Reunited at Last");
		{
			// Time: "As I ate breakfast the next morning, Gil informed me ... meeting with the Gilberta Company later that day ..."
			// "The Gilberta Company was due to arrive at third bell"
			Time.GoToNextDay.Morning();
			// Zahm introduced as Arno's replacement
			this.AddScene(Scene(Temple).With(Benno, Lutz, Mark, Zahm));
		}

		Chapter("How to Make Fluffy Bread");
		{
			Time.AddBell();
			// Leaving after discussing printing/restaurant
			this.AddScene(Scene(GilbertaCompany).With(Benno, Lutz, Mark));
		}

		Chapter("Starbind Ceremony in the Lower City");
		{
			// Time: "As the Starbind Ceremony approached," idk, a couple days later?
			Time.GoToDaysAhead(3).Meetings();
			// "Today was a day when Benno and Lutz were visiting from the Gilberta Company"
			this.AddScene(Scene(Temple).With(Benno, Lutz));
			Time.AddBell();
			// Leaving after discussing starbinding ceremony
			this.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));
			// Time: "And so, the day of the Starbind Ceremony arrived."
			Time.GoToDaysAhead(2).Morning();
			Event("Starbind Ceremony");
			// "I thought about him, Lutz, and the kids who were about to head to the forest."
			this.AddScene(Scene(LowerCityForest).With(Gil, Lutz));
			// "my whole family had come to see me as the High Bishop."
			this.AddScene(Scene(Temple).With(Effa, Gunther, Kamil, Tuuli));
		}

		Chapter("The Archduke's Castle");
		{
			// Time: lunch is between the lower city and noble starbindings
			Time.AddBell();
			this.AddScene(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
			this.AddScene(Scene(Castle).With(Brigitte, Damuel, Ferdinand, Fran, Myne, Rosina));
			// Myne gets introduced to her temp guard knights and some attendents
			this.AddScene(Scene(Castle).With(Angelica, Cornelius, Norbert, Rihyarda));
			// gil and lutz probably return from the forest fairly soon
			this.AddScene(Scene(Temple).With(Gil));
			this.AddScene(Scene(GilbertaCompany).With(Lutz));
			Time.AddBell();
			// kids say good night to aub
			this.AddScene(Scene(Castle).With(Charlotte, Melchior));
		}

		Chapter("Starbind Ceremony in the Noble's Quarter");
		{
			Time.AddHour();
			// myne gets changed into high bishop outfit
			this.AddScene(Scene(Castle).With(Ottilie));
			// karstedt was behind sylvester at the ceremony
			this.AddScene(Scene(Castle).With(Karstedt));
		}

		Chapter("The Archduke and the Italian Restaurant");
		{
			// Time: the next day
			Time.GoToNextDay.Morning();
			// "He sent his attendants back to the temple without him"
			this.AddScene(Scene(Temple).With(Fran, Rosina));
			// Time: "later that afternoon that my fever finally went down."
			Time.GoToNextDay.MarketClose();
			// "He put me onto his highbeast and we returned to the temple accompanied by Damuel and Brigitte, who followed on either side of us."
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

			// Time: tomorrow
			Time.GoToNextDay.Meetings();
			// "Leon will be coming by tomorrow to get the natural yeast"
			this.AddScene(Scene(Temple).With(Leon));
			Time.AddHour();
			this.AddScene(Scene(GilbertaCompany).With(Leon));

			// Time: 3rd bell of the next day
			Time.GoToNextDay.Meetings();
			Event("Italian Restaurant");
			// sylvester arrives in the temple early on the day of the italian restaurant visit
			this.AddScene(Scene(Temple).With(Cornelius, Eckhart, Karstedt, Sylvester));
			Time.AddHour();
			// rosina leaves early b/c she plays music
			this.AddScene(Scene(ItalianRestaurant).With(Rosina));
			Time.AddBell();
			this.AddScene(Scene(ItalianRestaurant).With(Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Fran, Karstedt, Myne, Sylvester, Zahm));
			this.AddScene(Scene(ItalianRestaurant).With(Benno, Freida, Gustav, Leon, Mark));
		}

		Chapter("Making a Monastery");
		{
			// Time: after lunch at the italian restaurant
			Time.AddBell();
			// hasse orphanage designed, fly over with highbeasts
			this.AddScene(Scene(Hasse).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
			Time.AddBell();
			// hasse orphanage is built and they return to the restaurant
			this.AddScene(Scene(ItalianRestaurant).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
			Time.AddHour();
			// ferdi hires todd as a chef to arrive in the tmple in 36 hours from now
			// syl hires hugo for the castle
			this.AddScene(Scene(OthmarCompany).With(Freida, Gustav));
			this.AddScene(Scene(GilbertaCompany).With(Benno, Leon, Mark));
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
			this.AddScene(Scene(KarstedtsHouse).With(Cornelius, Karstedt));
			this.AddScene(Scene(KnightsOrder).With(Eckhart));
			this.AddScene(Scene(Castle).With(Sylvester));
		}

		// Background Event
		{
			// no transition between chapters so no idea the time difference between italian restaurant and donations chapter
			// but hugo is in the castle kitched at that point, and we can probably assume he goes at the same time
			// todd goes to ferdi's temple kitchen at 2nd bell
			Time.GoToDaysAhead(2).Morning();
			this.AddScene(Scene(Temple).With(Todd));
			this.AddScene(Scene(Castle).With(Ella, Hugo));
		}

		Chapter("How to Gather Donations");
		{
			// Time: during the day, has to be around 17 days letter since ella went to the castle for a month and this is the only
			// section that has an unknown timeskip (the accounted for time between now and when ella is back in the temple during
			// elvira/lamp's visit is ~13 days)
			Time.GoToDaysAhead(17).Meetings();
			Event("Gathering Donations");
			// donation tea party with flor, elvira, and myne
			// brigitte mentioned in next segment, so other guard knights probably also there
			this.AddScene(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Elvira, Myne));
			Time.AddBell();
			// elvira probably goes back after the tea party
			this.AddScene(Scene(KarstedtsHouse).With(Elvira));

			// Time: "I had been bedridden for two days since the tea party"
			Time.GoToDaysAhead(2).Meetings();
			// ferdi, elvira, flor visit sick myne
			this.AddScene(Scene(Castle).With(Elvira, Ferdinand));
			Time.AddBell();
			// ferdi leaves after agreeing to play harspiel
			this.AddScene(Scene(Temple).With(Ferdinand));
		}

		Chapter("My First Magic Training Regimen");
		{
			Time.GoToNextDay.Meetings();
			this.AddScene(Scene(Castle).With(Ferdinand));
			Time.AddHour();
			// ferdi and myne go to some place to train magic
			this.AddScene(Scene(KnightsOrder).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
			Time.AddBells(2);
			// myne blows up her feystone and reforms it then they go back to the castle
			this.AddScene(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
			Time.AddHour();
			this.AddScene(Scene(Temple).With(Ferdinand));
		}

		Chapter("Working Toward Wax Stencils");
		{
			// Time: "dinnertime was my one opportunity to talk to them."
			Time.GoToNextDay.Dinner();
			this.AddScene(Scene(Castle).With(Florencia, Lamprecht, Myne, Sylvester, Wilfried));

			Time.GoToNextDay.Meetings();
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Myne));
			this.AddScene(Scene(Temple).With(Benno, Lutz));
			Time.AddBell();
			this.AddScene(Scene(GilbertaCompany).With(Benno));
		}

		Chapter("An Illustration of Ferdinand");
		{
			Time.AddHour();
			// myne has dirk drain his mana with a taue fruit and cuts some trombes
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Delia, Dirk, Fran, Gil, Lutz, Monika, Myne, Rosina, Wilma));
			Time.AddBell();
			this.AddScene(Scene(GilbertaCompany).With(Lutz));
		}

		Chapter("Johann and Zack");
		{
			Time.GoToNextDay.Meetings();
			this.AddScene(Scene(Temple).With(Johann, Lutz, Zack));
			this.AddScene(Scene(LowerCityForest).With(Gil));
			Time.AddBell();
			this.AddScene(Scene(LowerCityWorkshops).With(Johann, Zack));
			this.AddScene(Scene(GilbertaCompany).With(Lutz));
			this.AddScene(Scene(Temple).With(Gil));

			Time.GoToDaysAhead(3).Meetings();
			// johann and zack return with their blueprints for the wax stencil machine
			this.AddScene(Scene(Temple).With(Johann, Lutz, Zack));
			Time.AddBell();
			this.AddScene(Scene(LowerCityWorkshops).With(Johann, Zack));
			this.AddScene(Scene(GilbertaCompany).With(Lutz));

			Time.GoToNextDay.Meetings();
			// tuuli gives myne a hairpin
			this.AddScene(Scene(Temple).With(Lutz, Tuuli));
			Time.AddBell();
			this.AddScene(Scene(MynesHouse).With(Tuuli));
			this.AddScene(Scene(GilbertaCompany).With(Lutz));
		}

		// Background Event
		{
			// Time: roughly a month after the chefs get loaned out
			Time.GoToNextDay.Morning();
			this.AddScene(Scene(Temple).With(Ella));
			this.AddScene(Scene(ItalianRestaurant).With(Hugo, Todd));
		}

		Chapter("Elvira and Lamprecht Attack");
		{
			// Time: "It was the day after I had met with Tuuli."
			Time.GoToNextDay.Meetings();
			this.AddScene(Scene(Temple).With(Lutz));
			Time.AddBell();
			this.AddScene(Scene(GilbertaCompany).With(Lutz));

			// Time: 3 days later
			Time.GoToDaysAhead(2).Meetings();
			// elvira and lemprecht visit the temple to eat some of ella's food
			this.AddScene(Scene(Temple).With(Elvira, Lamprecht));
			Time.AddBell();
			this.AddScene(Scene(KarstedtsHouse).With(Elvira));
			this.AddScene(Scene(Castle).With(Lamprecht));

			// Time: unknown, not the same day
			Time.GoToNextDay.Meetings();
			// lutz tells about johann and zack's progress
			this.AddScene(Scene(Temple).With(Lutz));
		}

		Chapter("Finishing My Highbeast and the Wax Stencils");
		{
			Time.AddBell();
			// myne goes to the castle to make her highbeast
			this.AddScene(Scene(KnightsOrder).With(Brigitte, Damuel, Ferdinand, Myne));
			Time.AddBell();
			Event("Myne Creates Lessy");
			// they go back to the temple after myne creates lessy
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

			// Time: "From there, I spent my days practicing ... "
			// "It was the evening five days before Ferdinand’s concert."
			// 13 days since concert was said to be "in a month"
			// so a 12 day timeskip at minimum
			Time.GoToDaysAhead(12).MarketClose();
			// lutz and smiths visit with a wax machine
			this.AddScene(Scene(Temple).With(Lutz, Johann, Zack));
			Time.AddBell();
			this.AddScene(Scene(LowerCityWorkshops).With(Johann, Zack));
			this.AddScene(Scene(GilbertaCompany).With(Lutz));

			// Time: "“Good morning, Lady Rozemyne,”"
			Time.GoToNextDay.Morning();
			this.AddScene(Scene(Temple).With(Lutz));
			Time.AddBell();
			this.AddScene(Scene(GilbertaCompany).With(Lutz));
		}

		Chapter("The Harspiel Concert");
		{
			// Time: "I returned to the castle the day before the concert."
			Time.GoToDaysAhead(3).Morning();
			// myne talks to ferdi and reminds him about the conert
			Time.AddHour();
			// "We made our way to the castle. Ella and Rosina were in the carriage for attendants, while my two guard knights and I got into the carriage for nobles."
			this.AddScene(Scene(Castle).With(Brigitte, Damuel, Ella, Myne, Rosina));
			// "Elvira and Florencia were already waiting for me in the castle."
			this.AddScene(Scene(Castle).With(Elvira, Florencia));

			// Time: "And so came the day of the concert." meeting time? afternoon time? no clue
			Time.GoToNextDay.MarketClose();
			Event("Harspiel Concert");
			this.AddScene(Scene(Castle).With(Eckhart, Ferdinand, Karstedt));
			Time.AddBell();
			this.AddScene(Scene(Temple).With(Brigitte, Damuel, Ella, Ferdinand, Myne, Rosina));
			this.AddScene(Scene(KarstedtsHouse).With(Elvira, Karstedt));

			// Time: "It was several days after the concert,"
			Time.GoToDaysAhead(3).Meetings();
			// "Ferdinand had summoned me to his lecture room just like in the old days."
			this.AddScene(Scene(Temple).With(Ferdinand, Myne));
		}

		Chapter("Epilogue");
		{
			// Time: some time after myne gets lectured
			// "“Lutz, the customers have all left,”"
			Time.GoToDaysAhead(2).MarketClose();
			this.AddScene(Scene(GilbertaCompany).With(Benno, Leon, Lutz, Mark));
			// "Ingo and his wife—the owners of the carpentry workshop that Rozemyne exclusively gave business to—were currently living in the monastery in Hasse"
			// "Deid, would also be heading there soon"
			this.AddScene(Scene(Hasse).With(Deid, Ingo));

			// Time: "Two days later, Lutz, Benno, and Tuuli went to the orphanage director’s chambers."
			Time.GoToDaysAhead(2).Meetings();
			this.AddScene(Scene(Temple).With(Benno, Lutz, Tuuli));
			Time.AddBell();
			this.AddScene(Scene(MynesHouse).With(Tuuli));
			this.AddScene(Scene(GilbertaCompany).With(Benno, Lutz));
		}

		Chapter("End");
	}
}