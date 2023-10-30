using static NarrativeCharts.Bookworm.BookwormBell;
using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Bookworm.P3;

public sealed class P3V3 : BookwormNarrativeChart
{
	public P3V3(BookwormTimeTracker time) : base(time)
	{
		Name = nameof(P3V3);
	}

	protected override void ProtectedCreate()
	{
		Event("Prologue");
		P3V3C01();
		Event("Ingo and Improving the Printing Press");
		P3V3C02();
		Event("The Gathering of the Gutenbergs");
		P3V3C03();
		Event("The Start of Winter Socializing");
		P3V3C04();
		Event("The Baptism Ceremony and Our Winter Debut");
		P3V3C05();
		Event("A Class for Kids");
		P3V3C06();
		Event("A Tea Party");
		P3V3C07();
		Event("The Dedication Ritual (Take Two)");
		P3V3C08();
		Event("Gathering the Winter Ingredient");
		P3V3C09();
		Event("Fighting the Schnesturm");
		P3V3C10();
		Event("To the End of Winter");
		P3V3C11();
		Event("Selling the Materials");
		P3V3C12();
		Update();
	}

	private void P3V3C01()
	{
		// starts with myne returning from the castle,
		// but already put that into the end of p3v2
		SkipToNextDay(Lunch);
		Add(Scene(Temple).With(Fritz, Lutz));
		Jump();
		Add(Scene(MerchantCompanies).With(Lutz));

		// fran speaks with ferdi
		Add(Scene(Temple).With(Ferdinand, Fran));
	}

	private void P3V3C02()
	{
		// Time: "once he had finished today’s before-bed report"
		SkipToCurrentDay(Bed);
		Add(Scene(Temple).With(Gil, Myne));

		SkipToNextDay(Lunch);
		var s1 = AddR(Scene(Temple).With(Lutz));
		Jump();
		Return(s1);

		SkipToNextDay(Lunch);
		Add(Scene(Temple).With(Benno, Ingo, Lutz));
	}

	private void P3V3C03()
	{
		Jump();
		Add(Scene(MerchantCompanies).With(Benno, Lutz));
		Add(Scene(LowerCityWorkshops).With(Ingo));

		// Time: "Ingo returned to my chambers a few days later"
		SkipToDaysAhead(3, Lunch);
		var s1 = AddR(Scene(Temple).With(Ingo, Johann, Zack));
		Jump();
		Return(s1);

		// Time: "About ten days later"
		SkipToDaysAhead(10, Lunch);

		// Time: "they once again arrived on the scheduled day"
		// no clue when
		SkipToDaysAhead(2, Lunch);
		var s2 = AddR(Scene(Temple).With(Benno, Ingo, Johann, Lutz, Zack));
		Jump();
		Return(s2);
	}

	private void P3V3C04()
	{
		// Time: "I would be going to my hidden room after fifth bell to receive it"
		SkipToDaysAhead(2, FifthBell);
		var s1 = AddR(Scene(Temple).With(Effa, Lutz, Tuuli));
		Jump();
		Return(s1);

		SkipToDaysAhead(3, Morning);
		// ella leaves the castle in The Dedication Ritual (Take Two)
		Add(Scene(Castle).With(Angelica, Brigitte, Cornelius, Damuel, Ella, Ferdinand, Myne, Rosina));
		Jump();
		Add(Scene(NoblesQuarter).With(Ferdinand));

		// Time: "Three days, you mean"
		SkipToDaysAhead(3, Morning);
		Add(Scene(Castle).With(Ferdinand));
	}

	private void P3V3C05()
	{
		Add(Scene(Castle).With(Eckhart, Elvira, Justus, Karstedt));
		Add(Scene(Castle).With(Philine));
		// Time: "we’ve moved straight onto lunch"
		SkipToCurrentDay(Lunch);
		Add(Scene(Castle).With(Christine));
		Jump();
		// not gonna bother including all of the commuting
	}

	private void P3V3C06()
	{
		// Time: "Rihyarda said after breakfast as she explained"
		SkipToNextDay(Morning);
		Add(Scene(Castle).With(Hartmut));
		// myne passes out after thinking she can't become a librarian
		Jump();

		// Time: "The day after all the students had left for the Academy—Angelica and Cornelius included"
		// idr what the exact schedule is, i think it's 1 year of students per day?
		SkipToDaysAhead(4, Morning);
		Add(Scene(RoyalAcademy).With(Angelica, Christine, Cornelius, Hartmut));
	}

	private void P3V3C07()
	{
		// Time: "Around the time that the kids were getting used to the playroom’s schedule"
		SkipToDaysAhead(3, Morning);
		// myne looks through meeting requests
		// sets up meetings with guards' families
		Add(Scene(Castle).With(Myne, Rihyarda));

		// Time: "A few days later, I was given permission"
		SkipToDaysAhead(3, Meetings);
		Add(Scene(Castle).With(Henrik));
		Jump();
		Add(Scene(NoblesQuarter).With(Henrik));

		// Time: "Two days after my meeting with Henrik"
		SkipToDaysAhead(2, Meetings);
		Add(Scene(Castle).With(Helfried));
		Jump();
		Add(Scene(NoblesQuarter).With(Helfried));

		// Time: "Next arrived the day I was scheduled to meet Angelica’s family"
		SkipToDaysAhead(2, Meetings);
		// they dont have names

		// Time: "Yet more days passed after meeting Angelica’s parents"
		SkipToDaysAhead(4, Meetings);
		// tea party with flor/elvira about the harspiel concert
		// scret ferdinand illustration
		Jump();
	}

	private void P3V3C08()
	{
		SkipToNextDay(Morning);
		// myne tells wilf dedication cere in 3 days

		// Time: "The Dedication Ritual begins three days from now"
		SkipToDaysAhead(3, Morning);
		Add(Scene(Temple).With(Brigitte, Damuel, Ella, Ferdinand, Myne, Rosina));

		// Time: "On the first day of the Dedication Ritual"
		SkipToNextDay(Morning);
		// myne, ferdi, and 2 blues do dedication ritual
		Jump();
	}

	private void P3V3C09()
	{
		// Time: "As Ferdinand had predicted, the Dedication Ritual came to an end three days later"
		SkipToDaysAhead(3, ThirdBell);
		// ferdi tells myne to use the spear
		// myne checks on the orphanage
		// myne/ferdi informed lord of winter appeared
		// temple people go to castle first
		Add(Scene(Castle).With(Brigitte, Damuel, Ella, Ferdinand, Myne, Rosina));
		Jump();
	}

	private void P3V3C10()
	{
		// not explictly stated anywhere that this LOW appeard in Haldenzel
		// eckhart probably also goes, but that's not explicitly stated
		var s1 = AddR(Scene(Haldenzel).With(Brigitte, Damuel, Ferdinand, Karstedt, Myne));
		Jump(2);
		Jump();
		Return(s1);

		// Time: "the next day was a sunny one"
		SkipToNextDay(Morning);
	}

	private void P3V3C11()
	{
		// Time: "increasing number of students coming home showed just how late into the season we were"
		// no exact amount of days given, idk, skip ahead a month
		SkipToDaysAhead(30, Meetings);
		// myne meets with sylv to talk about selling study materials
		Jump();
		// myne announces to the playroom that she's going to sell stuff
		Jump();
	}

	private void P3V3C12()
	{
		// Time: ".. Gil to contact Benno upon my return to the temple,
		// ... he took a letter to the Gilberta Company on the next sunny day.
		// ... [Benno] would be ready to meet with me that same afternoon."
		SkipToNextDay(Lunch);
		Add(Scene(Temple).With(Brigitte, Damuel, Ella, Ferdinand, Myne, Rosina));

		SkipToDaysAhead(2, Lunch);
		Add(Scene(Temple).With(Benno, Lutz, Mark));
		Jump();
		// Time: "Mark left partway through to begin preparations early"
		Add(Scene(MerchantCompanies).With(Mark));
		Jump();
		Add(Scene(MerchantCompanies).With(Benno, Lutz));
	}
}