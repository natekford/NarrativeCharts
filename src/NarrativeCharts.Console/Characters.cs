using ImageMagick;

using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Console;

public static class Characters
{
	public static Character Angelica { get; } = new("Angelica", MagickColors.Cyan);
	public static Character Benno { get; } = new("Benno", MagickColors.Beige);
	public static Character Bezewanst { get; } = new("Bezewanst", MagickColors.Silver);
	public static Character Bindewald { get; } = new("Bindewald", MagickColors.Gray);
	public static Character Brigitte { get; } = new("Brigitte", MagickColors.DarkRed);
	public static ImmutableDictionary<string, Character> CharacterDictionary { get; }
	public static Character Charlotte { get; } = new("Charlotte", MagickColors.Silver);
	public static Character Cornelius { get; } = new("Cornelius", MagickColors.LimeGreen);
	public static Character Damuel { get; } = new("Damuel", MagickColors.Brown);
	public static Character Deid { get; } = new("Deid", MagickColors.Yellow);
	public static Character Delia { get; } = new("Delia", MagickColors.Red);
	public static Character Dirk { get; } = new("Dirk", MagickColors.RosyBrown);
	public static Character Eckhart { get; } = new("Eckhart", MagickColors.DarkGreen);
	public static Character Effa { get; } = new("Effa", MagickColors.LightGreen);
	public static Character Egmont { get; } = new("Egmont", MagickColors.Brown);
	public static Character Ella { get; } = new("Ella", MagickColors.RosyBrown);
	public static Character Elvira { get; } = new("Elvira", MagickColors.DarkOliveGreen);
	public static Character Ferdinand { get; } = new("Ferdinand", MagickColors.LightBlue);
	public static Character Florencia { get; } = new("Florencia", MagickColors.LightYellow);
	public static Character Fran { get; } = new("Fran", MagickColors.Purple);
	public static Character Freida { get; } = new("Freida", MagickColors.Pink);
	public static Character Gil { get; } = new("Gil", MagickColors.LightGoldenrodYellow);
	public static Character Gunther { get; } = new("Gunther", MagickColors.Blue);
	public static Character Gustav { get; } = new("Gustav", MagickColors.Silver);
	public static Character Hugo { get; } = new("Hugo", MagickColors.SaddleBrown);
	public static Character Ingo { get; } = new("Ingo", MagickColors.RosyBrown);
	public static Character Johann { get; } = new("Johann", MagickColors.Orange);
	public static Character Kamil { get; } = new("Kamil", MagickColors.MidnightBlue);
	public static Character Karstedt { get; } = new("Kardstedt", MagickColors.IndianRed);
	public static Character Lamprecht { get; } = new("Lamprecht", MagickColors.RosyBrown);
	public static Character Leise { get; } = new("Leise", MagickColors.Green);
	public static Character Leon { get; } = new("Leon", MagickColors.SandyBrown);
	public static Character Lutz { get; } = new("Lutz", MagickColors.Yellow);
	public static Character Mark { get; } = new("Mark", MagickColors.Brown);
	public static Character Melchior { get; } = new("Melchior", MagickColors.MediumPurple);
	public static Character Monika { get; } = new("Monika", MagickColors.DarkSeaGreen);
	public static Character Myne { get; } = new("Myne", MagickColors.MidnightBlue);
	public static Character Nicola { get; } = new("Nicola", MagickColors.OrangeRed);
	public static Character Norbert { get; } = new("Norbert", MagickColors.LightGray);
	public static Character Ottilie { get; } = new("Ottilie", MagickColors.DarkOrange);
	public static Character Rihyarda { get; } = new("Rihyarda", MagickColors.DimGray);
	public static Character Rosina { get; } = new("Rosina", MagickColors.SandyBrown);
	public static Character Sylvester { get; } = new("Sylvester", MagickColors.MediumPurple);
	public static Character Todd { get; } = new("Todd", MagickColors.Black);
	public static Character Tuuli { get; } = new("Tuuli", MagickColors.LightSeaGreen);
	public static Character Wilfried { get; } = new("Wilfried", MagickColors.LightYellow);
	public static Character Wilma { get; } = new("Wilma", MagickColors.Orange);
	public static Character Zack { get; } = new("Zack", MagickColors.SlateGray);
	public static Character Zahm { get; } = new("Zahm", MagickColors.Black);

	static Characters()
	{
		CharacterDictionary = typeof(Characters)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(Character))
			.Select(x => (Character)x.GetValue(null)!)
			.ToImmutableDictionary(x => x.Name, x => x);
	}
}

public readonly record struct Character(string Name, MagickColor Color);