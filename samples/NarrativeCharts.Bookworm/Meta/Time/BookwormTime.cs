﻿using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormTime
{
	// Royal Academy schedule: (probably wont follow this)
	// Second bell apparently marked the start of breakfast
	// second-and-a-half bell then marking the start of morning classes.
	// Third bell signaled a change in subject, as did third-and-a-half bell
	// fourth bell we would return to our dormitories for lunch.
	// Afternoon classes would begin at fourth-and-a-half bell
	// dinner at sixth bell.
	// Seventh bell was curfew

	// source: https://w.atwiki.jp/booklove/pages/195.html#footnote_body_2
	public static ImmutableArray<int> Lengths { get; } = new[]
	{
		4,
		3,
		3,
		2,
		3,
		2,
		3,
		4,
	}.ToImmutableArray();
}