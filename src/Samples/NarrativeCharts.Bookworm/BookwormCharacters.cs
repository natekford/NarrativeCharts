using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormCharacters
{
	public static Character Angelica { get; } = new("Angelica");
	public static Character Benno { get; } = new("Benno");
	public static Character Bezewanst { get; } = new("Bezewanst");
	public static Character Bindewald { get; } = new("Bindewald");
	public static Character Brigitte { get; } = new("Brigitte");
	public static Character Charlotte { get; } = new("Charlotte");
	public static ImmutableDictionary<string, Color> ColorValues { get; }
	public static Character Cornelius { get; } = new("Cornelius");
	public static Character Damuel { get; } = new("Damuel");
	public static Character Deid { get; } = new("Deid");
	public static Character Delia { get; } = new("Delia");
	public static Character Dirk { get; } = new("Dirk");
	public static Character Eckhart { get; } = new("Eckhart");
	public static Character Effa { get; } = new("Effa");
	public static Character Egmont { get; } = new("Egmont");
	public static Character Ella { get; } = new("Ella");
	public static Character Elvira { get; } = new("Elvira");
	public static Character Ferdinand { get; } = new("Ferdinand");
	public static Character Florencia { get; } = new("Florencia");
	public static Character Fran { get; } = new("Fran");
	public static Character Freida { get; } = new("Freida");
	public static Character Gil { get; } = new("Gil");
	public static Character Gunther { get; } = new("Gunther");
	public static Character Gustav { get; } = new("Gustav");
	public static Character HasseMayor { get; } = new("Hasse's Mayor");
	public static Character Hugo { get; } = new("Hugo");
	public static Character Ingo { get; } = new("Ingo");
	public static Character Johann { get; } = new("Johann");
	public static Character Kamil { get; } = new("Kamil");
	public static Character Karstedt { get; } = new("Karstedt");
	public static Character Lamprecht { get; } = new("Lamprecht");
	public static Character Leise { get; } = new("Leise");
	public static Character Leon { get; } = new("Leon");
	public static Character Lutz { get; } = new("Lutz");
	public static Character Mark { get; } = new("Mark");
	public static Character Marthe { get; } = new("Marthe");
	public static Character Melchior { get; } = new("Melchior");
	public static Character Monika { get; } = new("Monika");
	public static Character Myne { get; } = new("Myne");
	public static Character Nicola { get; } = new("Nicola");
	public static Character Nora { get; } = new("Nora");
	public static Character Norbert { get; } = new("Norbert");
	public static Character Ottilie { get; } = new("Ottilie");
	public static Character Rick { get; } = new("Rick");
	public static Character Rihyarda { get; } = new("Rihyarda");
	public static Character Rosina { get; } = new("Rosina");
	public static Character Sylvester { get; } = new("Sylvester");
	public static Character Thore { get; } = new("Thore");
	public static Character Todd { get; } = new("Todd");
	public static Character Tuuli { get; } = new("Tuuli");
	public static Character Wilfried { get; } = new("Wilfried");
	public static Character Wilma { get; } = new("Wilma");
	public static Character Zack { get; } = new("Zack");
	public static Character Zahm { get; } = new("Zahm");

	static BookwormCharacters()
	{
		ColorValues = new Dictionary<string, string>
		{
			[Myne.Name] = "#39658C",
			[Ferdinand.Name] = "#65B8DA",
			// Myne's Family
			[Tuuli.Name] = "#95BE82",
			[Kamil.Name] = "#415BA5",
			[Gunther.Name] = "#37527F",
			[Effa.Name] = "#A0AA8F",
			// Archducal Family
			[Wilfried.Name] = "#FEEFB4",
			[Sylvester.Name] = "#5366AB",
			[Melchior.Name] = "#3B4C80",
			[Florencia.Name] = "#FBF6E0",
			[Charlotte.Name] = "#D3BD96",
			// Karstedt's Family
			[Lamprecht.Name] = "#D18A71",
			[Karstedt.Name] = "#874730",
			[Elvira.Name] = "#46523C",
			[Eckhart.Name] = "#5F7D3F",
			[Cornelius.Name] = "#A6B455",
			// Myne's Guard Knights/Attendants
			[Rihyarda.Name] = "#E9DEC8",
			[Ottilie.Name] = "#A74D47",
			[Damuel.Name] = "#967B4E",
			[Brigitte.Name] = "#A54254",
			[Angelica.Name] = "#53B3DB",
			// Temple Attendants
			[Zahm.Name] = "#000000",
			[Wilma.Name] = "#DF9139",
			[Rosina.Name] = "#E09C5B",
			[Nicola.Name] = "#F1A14A",
			[Monika.Name] = "#8BBFCD",
			[Gil.Name] = "#F8EECB",
			[Fran.Name] = "#5E546C",
			[Delia.Name] = "#A13F34",
			[Dirk.Name] = "#AD5A48",
			// Gilberta Company
			[Mark.Name] = "#726249",
			[Lutz.Name] = "#F7E3A8",
			[Leon.Name] = "#B26D66",
			[Benno.Name] = "#E8CB9F",
			// Workers
			[Zack.Name] = "#E05F0E",
			[Johann.Name] = "#E4A242",
			[Ingo.Name] = "#C1924A",
			// Cooks
			[Todd.Name] = "#000000",
			[Leise.Name] = "#C87965",
			[Hugo.Name] = "#7E5951",
			[Ella.Name] = "#9A515A",
			// Other Guard Knights/Attendants
			[Norbert.Name] = "#656567",
			// Othmar Company
			[Gustav.Name] = "#B6A6A7",
			[Freida.Name] = "#E6AFB4",
			// Lutz's Family
			[Deid.Name] = "#F3EEB6",
			// Temple Priests
			[Egmont.Name] = "#947D77",
			[Bezewanst.Name] = "#BC9A9F",
			// Hasse
			[Thore.Name] = "#8888BE",
			[Rick.Name] = "#667965",
			[Nora.Name] = "#9797CC",
			[Marthe.Name] = "#5C725D",
			[HasseMayor.Name] = "#000000",
			// Ahrensbach
			[Bindewald.Name] = "#B1C6C1",
		}.ToImmutableDictionary(x => x.Key, x => new Color(x.Value));
	}
}

public readonly record struct Character(string Name);

public readonly record struct Color(string Hex);