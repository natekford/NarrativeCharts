using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Bookworm;

public static class BookwormCharacters
{
	public static Character Angelica { get; } = new("Angelica", "#53B3DB");
	public static Character Benno { get; } = new("Benno", "#E8CB9F");
	public static Character Bezewanst { get; } = new("Bezewanst", "#BC9A9F");
	public static Character Bindewald { get; } = new("Bindewald", "#B1C6C1");
	public static Character Brigitte { get; } = new("Brigitte", "#A54254");
	public static Character Charlotte { get; } = new("Charlotte", "#D3BD96");
	public static Character Cornelius { get; } = new("Cornelius", "#A6B455");
	public static Character Damuel { get; } = new("Damuel", "#967B4E");
	public static Character Deid { get; } = new("Deid", "#F3EEB6");
	public static Character Delia { get; } = new("Delia", "#A13F34");
	public static ImmutableDictionary<string, Character> Dictionary { get; }
	public static Character Dirk { get; } = new("Dirk", "#AD5A48");
	public static Character Eckhart { get; } = new("Eckhart", "#5F7D3F");
	public static Character Effa { get; } = new("Effa", "#A0AA8F");
	public static Character Egmont { get; } = new("Egmont", "#947D77");
	public static Character Ella { get; } = new("Ella", "#9A515A");
	public static Character Elvira { get; } = new("Elvira", "#46523C");
	public static Character Ferdinand { get; } = new("Ferdinand", "#65B8DA");
	public static Character Florencia { get; } = new("Florencia", "#FBF6E0");
	public static Character Fran { get; } = new("Fran", "#5E546C");
	public static Character Freida { get; } = new("Freida", "#E6AFB4");
	public static Character Gil { get; } = new("Gil", "#F8EECB");
	public static Character Gunther { get; } = new("Gunther", "#37527F");
	public static Character Gustav { get; } = new("Gustav", "#B6A6A7");
	public static Character Hugo { get; } = new("Hugo", "#7E5951");
	public static Character Ingo { get; } = new("Ingo", "#C1924A");
	public static Character Johann { get; } = new("Johann", "#E4A242");
	public static Character Kamil { get; } = new("Kamil", "#415BA5");
	public static Character Karstedt { get; } = new("Karstedt", "#874730");
	public static Character Lamprecht { get; } = new("Lamprecht", "#D18A71");
	public static Character Leise { get; } = new("Leise", "#C87965");
	public static Character Leon { get; } = new("Leon", "#B26D66");
	public static Character Lutz { get; } = new("Lutz", "#F7E3A8");
	public static Character Mark { get; } = new("Mark", "#726249");
	public static Character Melchior { get; } = new("Melchior", "#3B4C80");
	public static Character Monika { get; } = new("Monika", "#8BBFCD");
	public static Character Myne { get; } = new("Myne", "#39658C");
	public static Character Nicola { get; } = new("Nicola", "#F1A14A");
	public static Character Norbert { get; } = new("Norbert", "#656567");
	public static Character Ottilie { get; } = new("Ottilie", "#A74D47");
	public static Character Rihyarda { get; } = new("Rihyarda", "#E9DEC8");
	public static Character Rosina { get; } = new("Rosina", "#E09C5B");
	public static Character Sylvester { get; } = new("Sylvester", "#5366AB");
	public static Character Todd { get; } = new("Todd", "#000000");
	public static Character Tuuli { get; } = new("Tuuli", "#95BE82");
	public static Character Wilfried { get; } = new("Wilfried", "#FEEFB4");
	public static Character Wilma { get; } = new("Wilma", "#DF9139");
	public static Character Zack { get; } = new("Zack", "#E05F0E");
	public static Character Zahm { get; } = new("Zahm", "#000000");

	static BookwormCharacters()
	{
		Dictionary = typeof(BookwormCharacters)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(Character))
			.Select(x => (Character)x.GetValue(null)!)
			.ToImmutableDictionary(x => x.Name, x => x);
	}
}

public readonly record struct Character(string Name, string Color);