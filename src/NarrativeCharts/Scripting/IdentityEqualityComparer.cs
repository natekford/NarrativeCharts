using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NarrativeCharts.Scripting;

// copied from https://stackoverflow.com/a/37128063
internal sealed class IdentityEqualityComparer<T> : IEqualityComparer<T> where T : class
{
	public bool Equals(T? x, T? y)
		=> ReferenceEquals(x, y);

	public int GetHashCode([DisallowNull] T obj)
		=> RuntimeHelpers.GetHashCode(obj);
}