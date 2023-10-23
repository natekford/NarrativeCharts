using NarrativeCharts.Models;

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
	public static ImmutableDictionary<Character, Hex> ColorValues { get; }
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
		ColorValues = new Dictionary<Character, string>
		{
			[Myne] = "#39658C",
			[Ferdinand] = "#65B8DA",
			// Myne's Family
			[Tuuli] = "#95BE82",
			[Kamil] = "#415BA5",
			[Gunther] = "#37527F",
			[Effa] = "#A0AA8F",
			// Archducal Family
			[Wilfried] = "#FEEFB4",
			[Sylvester] = "#5366AB",
			[Melchior] = "#3B4C80",
			[Florencia] = "#FBF6E0",
			[Charlotte] = "#D3BD96",
			// Karstedt's Family
			[Lamprecht] = "#D18A71",
			[Karstedt] = "#874730",
			[Elvira] = "#46523C",
			[Eckhart] = "#5F7D3F",
			[Cornelius] = "#A6B455",
			// Myne's Guard Knights/Attendants
			[Rihyarda] = "#E9DEC8",
			[Ottilie] = "#A74D47",
			[Damuel] = "#967B4E",
			[Brigitte] = "#A54254",
			[Angelica] = "#53B3DB",
			// Temple Attendants
			[Zahm] = "#000000",
			[Wilma] = "#DF9139",
			[Rosina] = "#E09C5B",
			[Nicola] = "#F1A14A",
			[Monika] = "#8BBFCD",
			[Gil] = "#F8EECB",
			[Fran] = "#5E546C",
			[Delia] = "#A13F34",
			[Dirk] = "#AD5A48",
			// Gilberta Company
			[Mark] = "#726249",
			[Lutz] = "#F7E3A8",
			[Leon] = "#B26D66",
			[Benno] = "#E8CB9F",
			// Workers
			[Zack] = "#E05F0E",
			[Johann] = "#E4A242",
			[Ingo] = "#C1924A",
			// Cooks
			[Todd] = "#000000",
			[Leise] = "#C87965",
			[Hugo] = "#7E5951",
			[Ella] = "#9A515A",
			// Other Guard Knights/Attendants
			[Norbert] = "#656567",
			// Othmar Company
			[Gustav] = "#B6A6A7",
			[Freida] = "#E6AFB4",
			// Lutz's Family
			[Deid] = "#F3EEB6",
			// Temple Priests
			[Egmont] = "#947D77",
			[Bezewanst] = "#BC9A9F",
			// Hasse
			[Thore] = "#8888BE",
			[Rick] = "#667965",
			[Nora] = "#9797CC",
			[Marthe] = "#5C725D",
			[HasseMayor] = "#000000",
			// Ahrensbach
			[Bindewald] = "#B1C6C1",
		}.ToImmutableDictionary(x => x.Key, x => new Hex(x.Value));
	}
}