﻿using static NarrativeCharts.Bookworm.BookwormBell;
using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Bookworm.P3;

public sealed class P3V1 : BookwormNarrativeChart
{
	public P3V1(BookwormTimeTracker time) : base(time)
	{
		Name = nameof(P3V1);
	}

	protected override void ProtectedCreate()
	{
		Chapter("Prologue");
		P3V1C01();
		Chapter("Examination Results and the Noble's Quarter");
		P3V1C02();
		// Karstedt and Cornelius commute to the Knight's Order every day
		// no idea if it should be added
		Chapter("Preparing for the Baptism Ceremony");
		P3V1C03();
		Chapter("A Noble's Baptism Ceremony");
		P3V1C04();
		Chapter("Adoption");
		P3V1C05();
		Chapter("Inaguration Ceremony");
		P3V1C06();
		Chapter("Reunited at Last");
		P3V1C07();
		Chapter("How to Make Fluffy Bread");
		P3V1C08();
		Chapter("Starbind Ceremony in the Lower City");
		P3V1C09();
		Chapter("The Archduke's Castle");
		P3V1C10();
		Chapter("Starbind Ceremony in the Noble's Quarter");
		P3V1C11();
		Chapter("The Archduke and the Italian Restaurant");
		P3V1C12();
		Chapter("Making a Monastery");
		P3V1C13();
		// Background Event
		{
			// no transition between chapters so no idea the time difference between italian restaurant and donations chapter
			// but hugo is in the castle kitched at that point, and we can probably assume he goes at the same time
			// todd goes to ferdi's temple kitchen at 2nd bell
			SkipToDaysAhead(2, Morning);
			Add(Scene(Temple).With(Todd));
			Add(Scene(Castle).With(Ella, Hugo));
		}
		Chapter("How to Gather Donations");
		P3V1C14();
		Chapter("My First Magic Training Regimen");
		P3V1C15();
		Chapter("Working Toward Wax Stencils");
		P3V1C16();
		Chapter("An Illustration of Ferdinand");
		P3V1C17();
		Chapter("Johann and Zack");
		P3V1C18();
		// Background Event
		{
			// Time: roughly a month after the chefs get loaned out
			SkipToNextDay(Morning);
			Add(Scene(Temple).With(Ella));
			Add(Scene(ItalianRestaurant).With(Hugo, Todd));
		}
		Chapter("Elvira and Lamprecht Attack");
		P3V1C19();
		Chapter("Finishing My Highbeast and the Wax Stencils");
		P3V1C20();
		Chapter("The Harspiel Concert");
		P3V1C21();
		Chapter("Epilogue");
		P3V1C22();
		Update();
	}

	private void P3V1C01()
	{
		// Prologue starts with Karstedt seeing Sylvester off
		Add(Scene(Temple).With(Ferdinand, Karstedt, Myne, Sylvester));
		Add(Scene(KnightsOrder).With(Bezewanst, Bindewald));
		Add(Scene(Temple).With(Damuel, Delia, Dirk, Egmont, Ella, Fran, Gil, Monika, Nicola, Rosina, Wilma, Zahm));
		Add(Scene(LowerCityWorkshops).With(Deid, Ingo, Johann));
		Add(Scene(GilbertaCompany).With(Benno, Leon, Lutz, Mark));
		Add(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
		Add(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
		Add(Scene(OthmarCompany).With(Freida, Gustav));
		Add(Scene(KarstedtsHouse).With(Cornelius, Eckhart, Elvira, Lamprecht));
		// Characters we don't know yet
		// There are more, but this makes sylvester's line look less lonely
		// and we know these characters are at the castle from the start
		// Angelica/Ottilie aren't known to always be at the castle tho
		Add(Scene(Castle).With(Charlotte, Florencia, Melchior, Norbert, Rihyarda));
		// Time: Immediately after
		Time.AddBell();
		// Sylvester goes back to AD conf
		Add(Scene(Castle).With(Sylvester));
		// Low usage location: 1 use in the volume
		//Add(Scene(RoyalAcademy).With(Sylvester));
		// Discussion about Myne
		Add(Scene(Temple).With(Ferdinand, Karstedt));
		// karstedt probably goes back to his house after the discussion
		Time.AddBell();
		Add(Scene(KarstedtsHouse).With(Karstedt));

		// Time: Next morning
		SkipToNextDay(Morning);
		// Karstedt interrogates these 2
		Add(Scene(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
		// Time: Dinner
		SkipToCurrentDay(Dinner);
		// Discussion about Myne's baptism
		Add(Scene(KarstedtsHouse).With(Elvira, Ferdinand, Karstedt));
		Time.AddBell();
		Add(Scene(Temple).With(Ferdinand));

		// Time: The next day
		SkipToNextDay(Morning);
		// Karstest keeps interrogating these 2
		Add(Scene(KnightsOrder).With(Bezewanst, Bindewald, Karstedt));
		// Time: Not long after
		Time.AddBell();
		// Myne's health checkup
		Add(Scene(Temple).With(Ferdinand, Karstedt, Myne));
		// SS1 of P3V1, no exact timeline but definitely in this 4 day period between kardstedt/ferdi discussion and myne arrive
		// "Mother had said that she had important news and gathered Eckhart and I at the dinner table to discuss it over tea."
		Add(Scene(KarstedtsHouse).With(Cornelius, Eckhart, Elvira));
	}

	private void P3V1C02()
	{
		// Time: Immediately after
		Time.AddBell();
		// Leaving Ferdinand's office and going back to Myne's room
		Add(Scene(Temple).With(Damuel, Fran, Monika, Myne, Nicola, Rosina));
		Add(Scene(KarstedtsHouse).With(Karstedt));

		// Time: Next 3 days
		SkipToDaysAhead(3, Morning);
		// People in the temple getting ready for Myne going to the Noble's quarter
		Add(Scene(Temple).With(Ella, Fran, Gil, Monika, Myne, Nicola, Rosina, Wilma));
		// Cooks learning with Leise at Guildmaster's house (or italian restaurant?)
		Add(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
		// Time: Immediately after
		Time.AddBell();
		// Leaving the temple through the front entrance and going to the Noble's quarter
		Add(Scene(Temple).With(Ella, Ferdinand, Karstedt, Myne, Rosina));
		// Time: Immediately after
		Time.AddBell();
		// Arriving at Karstedt's house
		Add(Scene(KarstedtsHouse).With(Cornelius, Ella, Elvira, Ferdinand, Karstedt, Myne, Rosina));
	}

	private void P3V1C03()
	{
		// Time: Ferdinand checks on Myne every 2 days so probably stays like 6 hours each time?
		Time.AddBells(2);
		// Ferdinand going back to the temple
		Add(Scene(Temple).With(Ferdinand));

		for (var i = 0; i < 2; ++i)
		{
			SkipToDaysAhead(2, Meetings);
			Add(Scene(KarstedtsHouse).With(Ferdinand));
			Time.AddBells(2);
			Add(Scene(Temple).With(Ferdinand));
		}

		// Time: On a day where Ferdinand isn't there?
		SkipToNextDay(Meetings);
		// Gilberta company comes to sell some rinsham to Myne
		Add(Scene(KarstedtsHouse).With(Benno, Mark));
		Time.AddBell();
		Add(Scene(GilbertaCompany).With(Benno, Mark));

		// Time: A day after the Gilberta company visits?
		SkipToNextDay(Morning);
		// Ferdinand promises Myne book room key if she memorizes everyone's names before baptism
		Add(Scene(KarstedtsHouse).With(Ferdinand));

		// Gil + Gilberta company go to some city's orphanage
		// Exact time isn't given but probably after Benno sells Elvira rinsham and definitely before Myne gets inagurated as
		// the High Bishop
		Add(Scene(Hasse).With(Benno, Gil, Lutz));

		for (var i = 0; i < 3; ++i)
		{
			Time.AddBells(2);
			Add(Scene(Temple).With(Ferdinand));
			SkipToDaysAhead(2, Meetings);
			Add(Scene(KarstedtsHouse).With(Ferdinand));
		}
		Time.AddBells(2);
		Add(Scene(Temple).With(Ferdinand));

		// Time: The day before the baptism
		SkipToNextDay(Meetings);
		// Eckhart and Lemprecht come back home from the knight's barracks for Myne's baptism
		Add(Scene(KarstedtsHouse).With(Eckhart, Lamprecht));

		// Gil + Gilberta company come back from some city's orphanage
		// Exact time isn't given but definitely before Myne gets inagurated as the High Bishop
		Add(Scene(Temple).With(Gil));
		Add(Scene(GilbertaCompany).With(Benno, Lutz));
	}

	private void P3V1C04()
	{
		SkipToNextDay(Meetings);
		// Everyone arrives for Myne's baptism (SS would add a bunch of characters)
		Add(Scene(KarstedtsHouse).With(Ferdinand, Florencia, Sylvester, Wilfried));
	}

	private void P3V1C05()
	{
		// Time: Immediately after the baptism ceremony
		Time.AddBell();
		// Guard knights get introduced to Myne and Wilf makes Myne pass out
		Add(Scene(KarstedtsHouse).With(Brigitte, Damuel));
		// Time: After Myne wakes up and talks with Ferdinand/Karstedt
		Time.AddBell();
		Add(Scene(Temple).With(Ferdinand));
		Add(Scene(Castle).With(Florencia, Sylvester, Wilfried));
		// no idea on the exact time bezewanst gets killed, but probably before
		// the inaguration ceremony
		Kill(Bezewanst);
	}

	private void P3V1C06()
	{
		// Time: "Ferdinand had told me to use the day after my baptism ceremony to rest."
		SkipToNextDay(Morning);
		// Lamprecht gives Myne a book and goes back the castle
		Add(Scene(Castle).With(Lamprecht));

		// Time: The next day
		SkipToNextDay(Morning);
		// "Karstedt and Cornelius had already headed to the Knight’s Order, so Elvira was the only one to see me off."
		Add(Scene(KnightsOrder).With(Cornelius, Karstedt));
		Add(Scene(Temple).With(Brigitte, Damuel, Ella, Myne, Rosina));
		// Inaguration ceremony
		Time.AddBell();
		// Egmont gets crushed a bit
		Add(Scene(Temple).With(Egmont));
	}

	private void P3V1C07()
	{
		// Time: "As I ate breakfast the next morning, Gil informed me ... meeting with the Gilberta Company later that day ..."
		// "The Gilberta Company was due to arrive at third bell"
		SkipToNextDay(Morning);
		// Zahm introduced as Arno's replacement
		Add(Scene(Temple).With(Benno, Lutz, Mark, Zahm));
	}

	private void P3V1C08()
	{
		Time.AddBell();
		// Leaving after discussing printing/restaurant
		Add(Scene(GilbertaCompany).With(Benno, Lutz, Mark));
		Time.AddBell();
	}

	private void P3V1C09()
	{
		// Time: "As the Starbind Ceremony approached," idk, a couple days later?
		SkipToDaysAhead(3, Meetings);
		// "Today was a day when Benno and Lutz were visiting from the Gilberta Company"
		Add(Scene(Temple).With(Benno, Lutz));
		Time.AddBell();
		// Leaving after discussing starbinding ceremony
		Add(Scene(GilbertaCompany).With(Benno, Lutz));
		// Time: "And so, the day of the Starbind Ceremony arrived."
		SkipToDaysAhead(2, Morning);
		// "I thought about him, Lutz, and the kids who were about to head to the forest."
		Add(Scene(Temple).With(Lutz));
		// Low usage location: 2 times in the volume
		//Add(Scene(LowerCityForest).With(Gil, Lutz));
		// "my whole family had come to see me as the High Bishop."
		Add(Scene(Temple).With(Effa, Gunther, Kamil, Tuuli));
	}

	private void P3V1C10()
	{
		// Time: lunch is between the lower city and noble starbindings
		Time.AddBell();
		Add(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
		Add(Scene(Castle).With(Brigitte, Damuel, Ferdinand, Fran, Myne, Rosina));
		// Myne gets introduced to her temp guard knights and some attendents
		Add(Scene(Castle).With(Angelica, Cornelius, Norbert, Rihyarda));
		// gil and lutz probably return from the forest fairly soon
		Add(Scene(Temple).With(Gil));
		Add(Scene(GilbertaCompany).With(Lutz));
		Time.AddBell();
		// kids say good night to aub
		Add(Scene(Castle).With(Charlotte, Melchior));
	}

	private void P3V1C11()
	{
		Time.AddBell();
		// myne gets changed into high bishop outfit
		// karstedt was behind sylvester at the ceremony
		Add(Scene(Castle).With(Karstedt, Ottilie));
		Time.AddBell();
	}

	private void P3V1C12()
	{
		// Time: the next day
		SkipToNextDay(Morning);
		// "He sent his attendants back to the temple without him"
		Add(Scene(Temple).With(Fran, Rosina));
		// Time: "later that afternoon that my fever finally went down."
		SkipToNextDay(MarketClose);
		// "He put me onto his highbeast and we returned to the temple accompanied by Damuel and Brigitte, who followed on either side of us."
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));
		Add(Scene(KnightsOrder).With(Angelica));

		// Time: tomorrow
		SkipToNextDay(Meetings);
		// "Leon will be coming by tomorrow to get the natural yeast"
		Add(Scene(Temple).With(Leon));
		Time.AddBell();
		Add(Scene(GilbertaCompany).With(Leon));

		// Time: 3rd bell of the next day
		SkipToNextDay(Meetings);
		// sylvester arrives in the temple early on the day of the italian restaurant visit
		Add(Scene(Temple).With(Cornelius, Eckhart, Karstedt, Sylvester));
		Time.AddBell();
		// rosina leaves early b/c she plays music
		Add(Scene(ItalianRestaurant).With(Rosina));
		Time.AddBell();
		Add(Scene(ItalianRestaurant).With(Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Fran, Karstedt, Myne, Sylvester, Zahm));
		Add(Scene(ItalianRestaurant).With(Benno, Freida, Gustav, Leon, Mark));
	}

	private void P3V1C13()
	{
		// Time: after lunch at the italian restaurant
		Time.AddBell();
		// hasse orphanage designed, fly over with highbeasts
		Add(Scene(Hasse).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
		// it didn't take this long in the book, but the chart looks really bad
		// without this
		Time.AddBells(2);
		Update();
		Time.AddBells(2);
		// hasse orphanage is built and they return to the restaurant
		Add(Scene(ItalianRestaurant).With(Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester));
		Time.AddBell();
		// ferdi hires todd as a chef to arrive in the tmple in 36 hours from now
		// syl hires hugo for the castle
		Add(Scene(OthmarCompany).With(Freida, Gustav));
		Add(Scene(GilbertaCompany).With(Benno, Leon, Mark));
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Fran, Myne, Rosina, Zahm));
		Add(Scene(KarstedtsHouse).With(Cornelius, Karstedt));
		Add(Scene(KnightsOrder).With(Eckhart));
		Add(Scene(Castle).With(Sylvester));
	}

	private void P3V1C14()
	{
		// Time: during the day, has to be around 17 days letter since ella went to the castle for a month and this is the only
		// section that has an unknown timeskip (the accounted for time between now and when ella is back in the temple during
		// elvira/lamp's visit is ~13 days)
		SkipToDaysAhead(17, Meetings);
		// donation tea party with flor, elvira, and myne
		// brigitte mentioned in next segment, so other guard knights probably also there
		Add(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Elvira, Myne));
		Time.AddBell();
		// elvira probably goes back after the tea party
		Add(Scene(KarstedtsHouse).With(Elvira));

		// Time: "I had been bedridden for two days since the tea party"
		SkipToDaysAhead(2, Meetings);
		// ferdi, elvira, flor visit sick myne
		Add(Scene(Castle).With(Elvira, Ferdinand));
		Time.AddBell();
		// ferdi leaves after agreeing to play harspiel
		Add(Scene(Temple).With(Ferdinand));
		Add(Scene(KarstedtsHouse).With(Elvira));
	}

	private void P3V1C15()
	{
		SkipToNextDay(Meetings);
		Add(Scene(Castle).With(Ferdinand));
		Time.AddBell();
		// ferdi and myne go to some place to train magic
		Add(Scene(KnightsOrder).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
		Time.AddBells(2);
		// myne blows up her feystone and reforms it then they go back to the castle
		Add(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne));
		Time.AddBell();
		Add(Scene(Temple).With(Ferdinand));
	}

	private void P3V1C16()
	{
		// Time: "dinnertime was my one opportunity to talk to them."
		SkipToNextDay(Dinner);
		Add(Scene(Castle).With(Florencia, Lamprecht, Myne, Sylvester, Wilfried));

		SkipToNextDay(Meetings);
		Add(Scene(Temple).With(Brigitte, Damuel, Myne));
		Add(Scene(KnightsOrder).With(Angelica));
		Add(Scene(Temple).With(Benno, Lutz));
		Time.AddBell();
		Add(Scene(GilbertaCompany).With(Benno));
	}

	private void P3V1C17()
	{
		Time.AddBell();
		// myne has dirk drain his mana with a taue fruit and cuts some trombes
		Add(Scene(Temple).With(Delia, Dirk));
		Time.AddBell();
		Add(Scene(GilbertaCompany).With(Lutz));
	}

	private void P3V1C18()
	{
		SkipToNextDay(Meetings);
		Add(Scene(Temple).With(Johann, Lutz, Zack));
		// Low usage location: 2 times in the volume
		//Add(Scene(LowerCityForest).With(Gil));
		Time.AddBell();
		Add(Scene(LowerCityWorkshops).With(Johann, Zack));
		Add(Scene(GilbertaCompany).With(Lutz));
		Add(Scene(Temple).With(Gil));

		SkipToDaysAhead(3, Meetings);
		// johann and zack return with their blueprints for the wax stencil machine
		Add(Scene(Temple).With(Johann, Lutz, Zack));
		Time.AddBell();
		Add(Scene(LowerCityWorkshops).With(Johann, Zack));
		Add(Scene(GilbertaCompany).With(Lutz));

		SkipToNextDay(Meetings);
		// tuuli gives myne a hairpin
		Add(Scene(Temple).With(Lutz, Tuuli));
		Time.AddBell();
		Add(Scene(MynesHouse).With(Tuuli));
		Add(Scene(GilbertaCompany).With(Lutz));
	}

	private void P3V1C19()
	{
		// Time: "It was the day after I had met with Tuuli."
		SkipToNextDay(Meetings);
		Add(Scene(Temple).With(Lutz));
		Time.AddBell();
		Add(Scene(GilbertaCompany).With(Lutz));

		// Time: 3 days later
		SkipToDaysAhead(2, Meetings);
		// elvira and lemprecht visit the temple to eat some of ella's food
		Add(Scene(Temple).With(Elvira, Lamprecht));
		Time.AddBell();
		Add(Scene(KarstedtsHouse).With(Elvira));
		Add(Scene(Castle).With(Lamprecht));

		// Time: unknown, not the same day
		SkipToNextDay(Meetings);
		// lutz tells about johann and zack's progress
		Add(Scene(Temple).With(Lutz));
	}

	private void P3V1C20()
	{
		Time.AddBell();
		// myne goes to the castle to make her highbeast
		Add(Scene(KnightsOrder).With(Brigitte, Damuel, Ferdinand, Myne));
		Add(Scene(GilbertaCompany).With(Lutz));
		Time.AddBell();
		// they go back to the temple after myne creates lessy
		Add(Scene(Temple).With(Brigitte, Damuel, Ferdinand, Myne));

		// Time: "From there, I spent my days practicing ... "
		// "It was the evening five days before Ferdinand’s concert."
		// 13 days since concert was said to be "in a month"
		// so a 12 day timeskip at minimum
		SkipToDaysAhead(12, MarketClose);
		// lutz and smiths visit with a wax machine
		Add(Scene(Temple).With(Lutz, Johann, Zack));
		Time.AddBell();
		Add(Scene(LowerCityWorkshops).With(Johann, Zack));
		Add(Scene(GilbertaCompany).With(Lutz));

		// Time: "“Good morning, Lady Rozemyne,”"
		SkipToNextDay(Morning);
		Add(Scene(Temple).With(Lutz));
		Time.AddBell();
		Add(Scene(GilbertaCompany).With(Lutz));
	}

	private void P3V1C21()
	{
		// Time: "I returned to the castle the day before the concert."
		SkipToDaysAhead(3, Meetings);
		// "We made our way to the castle. Ella and Rosina were in the carriage for attendants, while my two guard knights and I got into the carriage for nobles."
		Add(Scene(Castle).With(Brigitte, Damuel, Ella, Myne, Rosina));
		// "Elvira and Florencia were already waiting for me in the castle."
		Add(Scene(Castle).With(Elvira, Florencia));

		// Time: "And so came the day of the concert." meeting time? afternoon time? no clue
		SkipToNextDay(MarketClose);
		Add(Scene(Castle).With(Eckhart, Ferdinand, Karstedt));
		Time.AddBell();
		Add(Scene(Temple).With(Brigitte, Damuel, Ella, Ferdinand, Myne, Rosina));
		Add(Scene(KarstedtsHouse).With(Elvira, Karstedt));
		Add(Scene(KnightsOrder).With(Eckhart));

		// Time: "It was several days after the concert,"
		SkipToDaysAhead(3, Meetings);
		// "Ferdinand had summoned me to his lecture room just like in the old days."
		Add(Scene(Temple).With(Ferdinand, Myne));
	}

	private void P3V1C22()
	{
		// Time: some time after myne gets lectured
		// "“Lutz, the customers have all left,”"
		SkipToDaysAhead(2, MarketClose);
		Add(Scene(GilbertaCompany).With(Benno, Leon, Lutz, Mark));
		// "Ingo and his wife—the owners of the carpentry workshop that Rozemyne exclusively gave business to—were currently living in the monastery in Hasse"
		// "Deid, would also be heading there soon"
		Add(Scene(Hasse).With(Deid, Ingo));

		// Time: "Two days later, Lutz, Benno, and Tuuli went to the orphanage director’s chambers."
		SkipToDaysAhead(2, Meetings);
		Add(Scene(Temple).With(Benno, Lutz, Tuuli));
		Time.AddBell();
		Add(Scene(MynesHouse).With(Tuuli));
		Add(Scene(GilbertaCompany).With(Benno, Lutz));
	}
}