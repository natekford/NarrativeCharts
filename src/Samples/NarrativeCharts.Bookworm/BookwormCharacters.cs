﻿using NarrativeCharts.Models;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormCharacters
{
	private const string UNKNOWN = "#000000";

	public static Character Angelica { get; } = new("Angelica");
	public static Character Benno { get; } = new("Benno");
	public static Character Bezewanst { get; } = new("Bezewanst");
	public static Character Bindewald { get; } = new("Bindewald");
	public static Character Brigitte { get; } = new("Brigitte");
	public static Character Charlotte { get; } = new("Charlotte");
	public static ImmutableDictionary<Character, Hex> Colors { get; }
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
	public static Character Frietack { get; } = new("Frietack");
	public static Character Gil { get; } = new("Gil");
	public static Character Gunther { get; } = new("Gunther");
	public static Character Gustav { get; } = new("Gustav");
	public static Character HasseMayor { get; } = new("Hasse's Mayor");
	public static Character Hugo { get; } = new("Hugo");
	public static Character Ingo { get; } = new("Ingo");
	public static Character Johann { get; } = new("Johann");
	public static Character Justus { get; } = new("Justus");
	public static Character Kamil { get; } = new("Kamil");
	public static Character Kampfer { get; } = new("Kampfer");
	public static Character Kantna { get; } = new("Kantna");
	public static Character Karstedt { get; } = new("Karstedt");
	public static Character Lamprecht { get; } = new("Lamprecht");
	public static Character Leise { get; } = new("Leise");
	public static Character Leon { get; } = new("Leon");
	public static Character Lutz { get; } = new("Lutz");
	public static Character Mark { get; } = new("Mark");
	public static Character Marthe { get; } = new("Marthe");
	public static Character Melchior { get; } = new("Melchior");
	public static Character Monika { get; } = new("Monika");
	public static Character Moritz { get; } = new("Moritz");
	public static Character Myne { get; } = new("Myne");
	public static Character Nadine { get; } = new("Nadine");
	public static Character Nicola { get; } = new("Nicola");
	public static Character Nora { get; } = new("Nora");
	public static Character Norbert { get; } = new("Norbert");
	public static Character Oswald { get; } = new("Oswald");
	public static Character Ottilie { get; } = new("Ottilie");
	public static Character Richt { get; } = new("Richt");
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
		Colors = new Dictionary<Character, string>
		{
			[Angelica] = "#53B3DB",
			[Benno] = "#E8CB9F",
			[Bezewanst] = "#BC9A9F",
			[Bindewald] = "#B1C6C1",
			[Brigitte] = "#A54254",
			[Charlotte] = "#D3BD96",
			[Cornelius] = "#A6B455",
			[Damuel] = "#967B4E",
			[Deid] = "#F3EEB6",
			[Delia] = "#A13F34",
			[Dirk] = "#AD5A48",
			[Eckhart] = "#5F7D3F",
			[Effa] = "#A0AA8F",
			[Egmont] = "#947D77",
			[Ella] = "#9A515A",
			[Elvira] = "#46523C",
			[Ferdinand] = "#65B8DA",
			[Florencia] = "#FBF6E0",
			[Fran] = "#5E546C",
			[Freida] = "#E6AFB4",
			[Frietack] = UNKNOWN,
			[Gil] = "#F8EECB",
			[Gunther] = "#37527F",
			[Gustav] = "#B6A6A7",
			[HasseMayor] = UNKNOWN,
			[Hugo] = "#7E5951",
			[Ingo] = "#C1924A",
			[Johann] = "#E4A242",
			[Justus] = "#A1AACB",
			[Kamil] = "#415BA5",
			[Kampfer] = UNKNOWN,
			[Kantna] = UNKNOWN,
			[Karstedt] = "#874730",
			[Lamprecht] = "#D18A71",
			[Leise] = "#C87965",
			[Leon] = "#B26D66",
			[Lutz] = "#F7E3A8",
			[Mark] = "#726249",
			[Marthe] = "#5C725D",
			[Melchior] = "#3B4C80",
			[Monika] = "#8BBFCD",
			[Moritz] = UNKNOWN,
			[Myne] = "#39658C",
			[Nadine] = UNKNOWN,
			[Nicola] = "#F1A14A",
			[Nora] = "#9797CC",
			[Norbert] = "#656567",
			[Oswald] = "#654321",
			[Ottilie] = "#A74D47",
			[Richt] = UNKNOWN,
			[Rick] = "#667965",
			[Rihyarda] = "#E9DEC8",
			[Rosina] = "#E09C5B",
			[Sylvester] = "#5366AB",
			[Thore] = "#8888BE",
			[Todd] = UNKNOWN,
			[Tuuli] = "#95BE82",
			[Wilfried] = "#FEEFB4",
			[Wilma] = "#DF9139",
			[Zack] = "#E05F0E",
			[Zahm] = UNKNOWN,
		}.ToImmutableDictionary(x => x.Key, x => new Hex(x.Value));
	}
}