namespace NarrativeCharts.Bookworm;

// Copied from https://stackoverflow.com/a/66354540/17760972
public class NaturalSortStringComparer : IComparer<string>
{
	private readonly StringComparison _comparison;

	public static NaturalSortStringComparer CurrentCulture { get; } = new NaturalSortStringComparer(StringComparison.CurrentCulture);
	public static NaturalSortStringComparer CurrentCultureIgnoreCase { get; } = new NaturalSortStringComparer(StringComparison.CurrentCultureIgnoreCase);
	public static NaturalSortStringComparer InvariantCulture { get; } = new NaturalSortStringComparer(StringComparison.InvariantCulture);
	public static NaturalSortStringComparer InvariantCultureIgnoreCase { get; } = new NaturalSortStringComparer(StringComparison.InvariantCultureIgnoreCase);
	public static NaturalSortStringComparer Ordinal { get; } = new NaturalSortStringComparer(StringComparison.Ordinal);
	public static NaturalSortStringComparer OrdinalIgnoreCase { get; } = new NaturalSortStringComparer(StringComparison.OrdinalIgnoreCase);

	public NaturalSortStringComparer(StringComparison comparison)
	{
		_comparison = comparison;
	}

	public int Compare(string? x, string? y)
	{
		// Let string.Compare handle the case where x or y is null
		if (x is null || y is null)
		{
			return string.Compare(x, y, _comparison);
		}

		var xSegments = GetSegments(x);
		var ySegments = GetSegments(y);

		while (xSegments.MoveNext() && ySegments.MoveNext())
		{
			var xIsNumber = int.TryParse(xSegments.Current, out var xValue);
			var yIsNumber = int.TryParse(ySegments.Current, out var yValue);

			int cmp;

			// If they're both numbers, compare the value
			if (xIsNumber && yIsNumber)
			{
				cmp = xValue.CompareTo(yValue);
				if (cmp != 0)
				{
					return cmp;
				}
			}
			// If x is a number and y is not, x is "lesser than" y
			else if (xIsNumber)
			{
				return -1;
			}
			// If y is a number and x is not, x is "greater than" y
			else if (yIsNumber)
			{
				return 1;
			}

			// OK, neither are number, compare the segments as text
			cmp = xSegments.Current.CompareTo(ySegments.Current, _comparison);
			if (cmp != 0)
			{
				return cmp;
			}
		}

		// At this point, either all segments are equal, or one string is shorter than the other

		// If x is shorter, it's "lesser than" y
		if (x.Length < y.Length)
		{
			return -1;
		}
		// If x is longer, it's "greater than" y
		if (x.Length > y.Length)
		{
			return 1;
		}

		// If they have the same length, they're equal
		return 0;
	}

	private static StringSegmentEnumerator GetSegments(string s) => new(s);

	private struct StringSegmentEnumerator
	{
		private readonly string _s;
		private int _currentPosition;
		private int _length;
		private int _start;
		public readonly ReadOnlySpan<char> Current => _s.AsSpan(_start, _length);

		public StringSegmentEnumerator(string s)
		{
			_s = s;
			_start = -1;
			_length = 0;
			_currentPosition = 0;
		}

		public bool MoveNext()
		{
			if (_currentPosition >= _s.Length)
			{
				return false;
			}

			var start = _currentPosition;
			var isFirstCharDigit = char.IsDigit(_s[_currentPosition]);

			while (++_currentPosition < _s.Length && char.IsDigit(_s[_currentPosition]) == isFirstCharDigit)
			{
			}

			_start = start;
			_length = _currentPosition - start;
			return true;
		}
	}
}