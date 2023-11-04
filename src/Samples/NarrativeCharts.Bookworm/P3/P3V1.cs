using NarrativeCharts.Time;

using static NarrativeCharts.Bookworm.BookwormBell;
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
		/* Daily schedule:
		 * Once breakfast is done, I practice the harspiel until third bell,
		 * then help Ferdinand in his office until lunch.
		 * After lunch, I have meetings with business partners,
		 */

		Event("Prologue");
		P3V1C01();
		Event("Examination Results and the Noble's Quarter");
		P3V1C02();
		// Karstedt and Cornelius commute to the Knight's Order every day
		// no idea if it should be added
		Event("Preparing for the Baptism Ceremony");
		P3V1C03();
		Event("A Noble's Baptism Ceremony");
		P3V1C04();
		Event("Adoption");
		P3V1C05();
		Event("Inaguration Ceremony");
		P3V1C06();
		Event("Reunited at Last");
		P3V1C07();
		Event("How to Make Fluffy Bread");
		P3V1C08();
		Event("Starbind Ceremony in the Lower City");
		P3V1C09();
		Event("The Archduke's Castle");
		P3V1C10();
		Event("Starbind Ceremony in the Noble's Quarter");
		P3V1C11();
		Event("The Archduke and the Italian Restaurant");
		P3V1C12();
		Event("Making a Monastery");
		P3V1C13();
		// Background Event
		{
			// no transition between chapters so no idea the time difference between italian restaurant and donations chapter
			// but hugo is in the castle kitched at that point, and we can probably assume he goes at the same time
			// todd goes to ferdi's temple kitchen at 2nd bell
			SkipToDaysAhead(2, Morning);
			Add(Temple, Todd);
			Add(EhrenfestCastle, Ella, Hugo);
		}
		Event("How to Gather Donations");
		P3V1C14();
		Event("My First Magic Training Regimen");
		P3V1C15();
		Event("Working Toward Wax Stencils");
		P3V1C16();
		Event("An Illustration of Ferdinand");
		P3V1C17();
		Event("Johann and Zack");
		P3V1C18();
		// Background Event
		{
			// Time: roughly a month after the chefs get loaned out
			SkipToNextDay(Morning);
			Add(Temple, Ella);
			// Hugo doesn't come back until v2 (todd isn't mentioned but ferdinand does say chef*s*)
			// but idk, myne only retrieves hugo and todd isn't mentioned once in p3v2
			// "I planned to retrieve Hugo as well. I couldn’t foresee there being any issues with that since it was past the date we had agreed on anyway."
			Add(ItalianRestaurant, /*Hugo, */Todd);
		}
		Event("Elvira and Lamprecht Attack");
		P3V1C19();
		Event("Finishing My Highbeast and the Wax Stencils");
		P3V1C20();
		Event("The Harspiel Concert");
		P3V1C21();
		Event("Epilogue");
		P3V1C22();
		Time.AddHours(24);
		Update();
	}

	private void P3V1C01()
	{
		// Prologue starts with Karstedt seeing Sylvester off
		Add(Temple, Ferdinand, Karstedt, Myne, Sylvester);
		Add(KnightsOrder, Bezewanst, Bindewald);
		Add(Temple, Damuel, Delia, Dirk, Egmont, Ella, Fran, Gil, Monika, Nicola, Rosina, Wilma, Zahm);
		Add(LowerCityWorkshops, Deid, Heidi, Ingo, Johann, Josef);
		Add(MerchantCompanies, Benno, Corinna, Freida, Gustav, Leon, Lutz, Mark, Otto);
		Add(MynesHouse, Effa, Gunther, Kamil, Tuuli);
		Add(ItalianRestaurant, Hugo, Leise, Todd);
		// Time: Immediately after
		Jump();
		// Sylvester goes back to AD conf
		Add(EhrenfestCastle, Sylvester);
		// Low usage location: 1 use in the volume
		//Add(RoyalAcademy, Sylvester);
		// Discussion about Myne
		Add(Temple, Ferdinand, Karstedt);
		// karstedt probably goes back to his house after the discussion
		Jump();
		Add(KarstedtsHouse, Karstedt);

		// Time: Next morning
		SkipToNextDay(Morning);
		// Karstedt interrogates these 2
		Add(KnightsOrder, Bezewanst, Bindewald, Karstedt);
		// Time: Dinner
		SkipToCurrentDay(Dinner);
		// Discussion about Myne's baptism
		Add(KarstedtsHouse, Elvira, Ferdinand, Karstedt);
		Jump();
		Add(Temple, Ferdinand);

		// Time: The next day
		SkipToNextDay(Morning);
		// Karstest keeps interrogating these 2
		Add(KnightsOrder, Bezewanst, Bindewald, Karstedt);
		// Time: Not long after
		Jump();
		// Myne's health checkup
		Add(Temple, Ferdinand, Karstedt, Myne);
		// SS1 of P3V1, no exact timeline but definitely in this 4 day period between kardstedt/ferdi discussion and myne arrive
		// "Mother had said that she had important news and gathered Eckhart and I at the dinner table to discuss it over tea."
		Add(KarstedtsHouse, Cornelius, Eckhart, Elvira);
	}

	private void P3V1C02()
	{
		// Time: Immediately after
		Jump();
		// Leaving Ferdinand's office and going back to Myne's room
		Add(Temple, Damuel, Fran, Monika, Myne, Nicola, Rosina);
		Add(KarstedtsHouse, Karstedt);

		// Time: Next 3 days
		SkipToDaysAhead(3, Morning);
		// People in the temple getting ready for Myne going to the Noble's quarter
		Add(Temple, Ella, Fran, Gil, Monika, Myne, Nicola, Rosina, Wilma);
		// Cooks learning with Leise at Guildmaster's house (or italian restaurant?)
		Add(ItalianRestaurant, Hugo, Leise, Todd);
		// Time: Immediately after
		Jump();
		// Leaving the temple through the front entrance and going to the Noble's quarter
		Add(Temple, Ella, Ferdinand, Karstedt, Myne, Rosina);
		// Time: Immediately after
		Jump();
		// Arriving at Karstedt's house
		Add(KarstedtsHouse, Cornelius, Ella, Elvira, Ferdinand, Karstedt, Myne, Rosina);
	}

	private void P3V1C03()
	{
		// Time: Ferdinand checks on Myne every 2 days so probably stays like 6 hours each time?
		Jump(2);
		// Ferdinand going back to the temple
		Add(Temple, Ferdinand);

		for (var i = 0; i < 2; ++i)
		{
			SkipToDaysAhead(2, Meetings);
			Add(KarstedtsHouse, Ferdinand);
			Jump(2);
			Add(Temple, Ferdinand);
		}

		// Time: On a day where Ferdinand isn't there?
		SkipToNextDay(Meetings);
		// Gilberta company comes to sell some rinsham to Myne
		Add(KarstedtsHouse, Benno, Mark);
		Jump();
		Add(MerchantCompanies, Benno, Mark);

		// Time: A day after the Gilberta company visits?
		SkipToNextDay(Morning);
		// Ferdinand promises Myne book room key if she memorizes everyone's names before baptism
		Add(KarstedtsHouse, Ferdinand);

		// Gil + Gilberta company go to some city's orphanage
		// Exact time isn't given but probably after Benno sells Elvira rinsham and definitely before Myne gets inagurated as
		// the High Bishop
		Add(Hasse, Benno, Gil, Lutz);

		for (var i = 0; i < 3; ++i)
		{
			Jump(2);
			Add(Temple, Ferdinand);
			SkipToDaysAhead(2, Meetings);
			Add(KarstedtsHouse, Ferdinand);
		}
		Jump(2);
		Add(Temple, Ferdinand);

		// Time: The day before the baptism
		SkipToNextDay(Meetings);
		// Eckhart and Lemprecht come back home from the knight's barracks for Myne's baptism
		Add(KarstedtsHouse, Eckhart, Lamprecht);

		// Gil + Gilberta company come back from some city's orphanage
		// Exact time isn't given but definitely before Myne gets inagurated as the High Bishop
		Add(Temple, Gil);
		Add(MerchantCompanies, Benno, Lutz);
	}

	private void P3V1C04()
	{
		SkipToNextDay(Meetings);
		// Everyone arrives for Myne's baptism (SS would add a bunch of characters)
		Add(KarstedtsHouse, Ferdinand, Florencia, Sylvester, Wilfried);
	}

	private void P3V1C05()
	{
		// Time: Immediately after the baptism ceremony
		Jump();
		// Guard knights get introduced to Myne and Wilf makes Myne pass out
		Add(KarstedtsHouse, Brigitte, Damuel);
		// Time: After Myne wakes up and talks with Ferdinand/Karstedt
		Jump();
		Add(Temple, Ferdinand);
		Add(EhrenfestCastle, Florencia, Sylvester, Wilfried);
		// no idea on the exact time bezewanst gets killed, but probably before
		// the inaguration ceremony
		Kill(Bezewanst);
	}

	private void P3V1C06()
	{
		// Time: "Ferdinand had told me to use the day after my baptism ceremony to rest."
		SkipToNextDay(Morning);
		// Lamprecht gives Myne a book and goes back the castle
		Add(EhrenfestCastle, Lamprecht);

		// Time: The next day
		SkipToNextDay(Morning);
		// "Karstedt and Cornelius had already headed to the Knight’s Order, so Elvira was the only one to see me off."
		Add(KnightsOrder, Cornelius, Karstedt);
		Add(Temple, Brigitte, Damuel, Ella, Myne, Rosina);
		// Inaguration ceremony
		Jump();
		// Egmont gets crushed a bit
		Add(Temple, Egmont);
	}

	private void P3V1C07()
	{
		// Time: "As I ate breakfast the next morning, Gil informed me ... meeting with the Gilberta Company later that day ..."
		// "The Gilberta Company was due to arrive at third bell"
		SkipToNextDay(Lunch);
		// Zahm introduced as Arno's replacement
		Add(Temple, Benno, Lutz, Mark, Zahm);
	}

	private void P3V1C08()
	{
		Jump();
		// Leaving after discussing printing/restaurant
		Add(MerchantCompanies, Benno, Lutz, Mark);
		Jump();
	}

	private void P3V1C09()
	{
		// Time: "As the Starbind Ceremony approached," idk, a couple days later?
		SkipToDaysAhead(3, Meetings);
		// "Today was a day when Benno and Lutz were visiting from the Gilberta Company"
		Add(Temple, Benno, Lutz);
		Jump();
		// Leaving after discussing starbinding ceremony
		Add(MerchantCompanies, Benno, Lutz);
		// Time: "And so, the day of the Starbind Ceremony arrived."
		SkipToDaysAhead(2, Morning);
		// "I thought about him, Lutz, and the kids who were about to head to the forest."
		Add(Temple, Lutz);
		// Low usage location: 2 times in the volume
		//Add(LowerCityForest, Gil, Lutz);
		// "my whole family had come to see me as the High Bishop."
		Add(Temple, Effa, Gunther, Kamil, Tuuli);
	}

	private void P3V1C10()
	{
		// Time: lunch is between the lower city and noble starbindings
		Jump();
		Add(MynesHouse, Effa, Gunther, Kamil, Tuuli);
		Add(EhrenfestCastle, Brigitte, Damuel, Ferdinand, Fran, Myne, Rosina);
		// Myne gets introduced to her temp guard knights and some attendents
		Add(EhrenfestCastle, Angelica, Cornelius, Norbert, Rihyarda);
		// gil and lutz probably return from the forest fairly soon
		Add(Temple, Gil);
		Add(MerchantCompanies, Lutz);
		Jump();
		// kids say good night to aub
		Add(EhrenfestCastle, Charlotte, Melchior);
	}

	private void P3V1C11()
	{
		Jump();
		// myne gets changed into high bishop outfit
		// karstedt was behind sylvester at the ceremony
		Add(EhrenfestCastle, Karstedt, Ottilie);
		Jump();
	}

	private void P3V1C12()
	{
		// Time: the next day
		SkipToNextDay(Morning);
		// "He sent his attendants back to the temple without him"
		Add(Temple, Fran, Rosina);
		// Time: "later that afternoon that my fever finally went down."
		SkipToNextDay(MarketClose);
		// "He put me onto his highbeast and we returned to the temple accompanied by Damuel and Brigitte, who followed on either side of us."
		Add(Temple, Brigitte, Damuel, Ferdinand, Myne);
		Add(KnightsOrder, Angelica);

		// Time: tomorrow
		SkipToNextDay(Meetings);
		// "Leon will be coming by tomorrow to get the natural yeast"
		Add(Temple, Leon);
		Jump();
		Add(MerchantCompanies, Leon);

		// Time: 3rd bell of the next day
		SkipToNextDay(Meetings);
		// sylvester arrives in the temple early on the day of the italian restaurant visit
		Add(Temple, Cornelius, Eckhart, Karstedt, Sylvester);
		Jump();
		// rosina leaves early b/c she plays music
		Add(ItalianRestaurant, Rosina);
		Jump();
		Add(ItalianRestaurant, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Fran, Karstedt, Myne, Sylvester, Zahm);
		Add(ItalianRestaurant, Benno, Freida, Gustav, Leon, Mark);
	}

	private void P3V1C13()
	{
		// Time: after lunch at the italian restaurant
		Jump();
		// hasse orphanage designed, fly over with highbeasts
		var s1 = AddR(Hasse, Benno, Brigitte, Cornelius, Damuel, Eckhart, Ferdinand, Gustav, Karstedt, Mark, Myne, Sylvester);
		// it didn't take this long in the book, but the chart looks really bad
		// without this
		Jump();
		Jump();
		// hasse orphanage is built and they return to the restaurant
		Return(s1);
		Jump();
		// ferdi hires todd as a chef to arrive in the tmple in 36 hours from now
		// syl hires hugo for the castle
		Add(MerchantCompanies, Benno, Freida, Gustav, Leon, Mark);
		Add(Temple, Brigitte, Damuel, Ferdinand, Fran, Myne, Rosina, Zahm);
		Add(KarstedtsHouse, Cornelius);
		Add(KnightsOrder, Eckhart);
		Add(EhrenfestCastle, Karstedt, Sylvester);
	}

	private void P3V1C14()
	{
		// Time: during the day, has to be around 17 days letter since ella went to the castle for a month and this is the only
		// section that has an unknown timeskip (the accounted for time between now and when ella is back in the temple during
		// elvira/lamp's visit is ~13 days)
		SkipToDaysAhead(17, Meetings);
		// donation tea party with flor, elvira, and myne
		// brigitte mentioned in next segment, so other guard knights probably also there
		Add(EhrenfestCastle, Angelica, Brigitte, Cornelius, Damuel, Elvira, Myne);
		Jump();
		// elvira probably goes back after the tea party
		Add(KarstedtsHouse, Elvira);

		// Time: "I had been bedridden for two days since the tea party"
		SkipToDaysAhead(2, Meetings);
		// ferdi, elvira, flor visit sick myne
		var s1 = AddR(EhrenfestCastle, Elvira, Ferdinand);
		Jump();
		// ferdi leaves after agreeing to play harspiel
		Return(s1);
	}

	private void P3V1C15()
	{
		SkipToNextDay(Meetings);
		Add(EhrenfestCastle, Ferdinand);
		Jump();
		// ferdi and myne go to some place to train magic
		Add(KnightsOrder, Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne);
		Jump(2);
		// myne blows up her feystone and reforms it then they go back to the castle
		Add(EhrenfestCastle, Angelica, Brigitte, Cornelius, Damuel, Ferdinand, Myne);
		Jump();
		Add(Temple, Ferdinand);
	}

	private void P3V1C16()
	{
		// Time: "dinnertime was my one opportunity to talk to them."
		SkipToNextDay(Dinner);
		Add(EhrenfestCastle, Florencia, Lamprecht, Myne, Sylvester, Wilfried);

		SkipToNextDay(Lunch);
		Add(Temple, Brigitte, Damuel, Myne);
		Add(KnightsOrder, Angelica);
		Add(KarstedtsHouse, Cornelius);
		Add(Temple, Benno, Lutz);
		Jump();
		Add(MerchantCompanies, Benno);
	}

	private void P3V1C17()
	{
		Jump();
		// myne has dirk drain his mana with a taue fruit and cuts some trombes
		Add(Temple, Delia, Dirk);
		Jump();
		Add(MerchantCompanies, Lutz);
	}

	private void P3V1C18()
	{
		SkipToNextDay(Lunch);
		Add(Temple, Johann, Lutz, Zack);
		// Low usage location: 2 times in the volume
		//Add(LowerCityForest, Gil);
		Jump();
		Add(LowerCityWorkshops, Johann, Zack);
		Add(MerchantCompanies, Lutz);
		//Add(Temple, Gil);

		SkipToDaysAhead(3, Lunch);
		// johann and zack return with their blueprints for the wax stencil machine
		var s2 = AddR(Temple, Johann, Lutz, Zack);
		Jump();
		Return(s2);

		SkipToNextDay(Lunch);
		// tuuli gives myne a hairpin
		var s3 = AddR(Temple, Lutz, Tuuli);
		Jump();
		Return(s3);
	}

	private void P3V1C19()
	{
		// Time: "It was the day after I had met with Tuuli."
		SkipToNextDay(Lunch);
		var s1 = AddR(Temple, Lutz);
		Jump();
		Return(s1);

		// Time: 3 days later
		SkipToDaysAhead(2, Meetings);
		// elvira and lemprecht visit the temple to eat some of ella's food
		var s2 = AddR(Temple, Elvira, Lamprecht);
		Jump();
		Return(s2);

		// Time: unknown, not the same day
		SkipToNextDay(Lunch);
		// lutz tells about johann and zack's progress
		Add(Temple, Lutz);
	}

	private void P3V1C20()
	{
		Jump();
		// myne goes to the castle to make her highbeast
		var s1 = AddR(KnightsOrder, Brigitte, Damuel, Ferdinand, Myne);
		Add(MerchantCompanies, Lutz);
		Jump();
		// they go back to the temple after myne creates lessy
		Return(s1);

		// Time: "From there, I spent my days practicing ... "
		// "It was the evening five days before Ferdinand’s concert."
		// 13 days since concert was said to be "in a month"
		// so a 12 day timeskip at minimum
		SkipToDaysAhead(12, MarketClose);
		// lutz and smiths visit with a wax machine
		var s2 = AddR(Temple, Lutz, Johann, Zack);
		Jump();
		Return(s2);

		// Time: "“Good morning, Lady Rozemyne,”"
		SkipToNextDay(Morning);
		var s3 = AddR(Temple, Lutz);
		Jump();
		Return(s3);
	}

	private void P3V1C21()
	{
		// Time: "I returned to the castle the day before the concert."
		SkipToDaysAhead(3, Meetings);
		// "We made our way to the castle. Ella and Rosina were in the carriage for attendants, while my two guard knights and I got into the carriage for nobles."
		// "Elvira and Florencia were already waiting for me in the castle."
		var s1 = AddR(EhrenfestCastle, Brigitte, Damuel, Ella, Elvira, Florencia, Myne, Rosina);

		// Time: "And so came the day of the concert." meeting time? afternoon time? no clue
		SkipToNextDay(MarketClose);
		var s2 = AddR(EhrenfestCastle, Eckhart, Ferdinand, Karstedt);
		Jump();
		Return(s1);
		Return(s2);

		// Time: "It was several days after the concert,"
		SkipToDaysAhead(3, Meetings);
		// "Ferdinand had summoned me to his lecture room just like in the old days."
		Add(Temple, Ferdinand, Myne);
	}

	private void P3V1C22()
	{
		// Time: some time after myne gets lectured
		// "“Lutz, the customers have all left,”"
		SkipToDaysAhead(2, MarketClose);
		Add(MerchantCompanies, Benno, Leon, Lutz, Mark);
		// "Ingo and his wife—the owners of the carpentry workshop that Rozemyne exclusively gave business to—were currently living in the monastery in Hasse"
		// "Deid, would also be heading there soon"
		Add(Hasse, Deid, Ingo);

		// Time: "Two days later, Lutz, Benno, and Tuuli went to the orphanage director’s chambers."
		SkipToDaysAhead(2, Lunch);
		var s1 = AddR(Temple, Benno, Lutz, Tuuli);
		Jump();
		Return(s1);
	}
}