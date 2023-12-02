namespace Shared
{
	public static class Extensions
	{
		public static IEnumerable<List<TElement>> PartitionIntoRangesBy<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> isDelimiter, bool includeDelimiters)
		{
			List<TElement> range = new List<TElement>();
			foreach (TElement elt in elements)
			{
				if (isDelimiter(elt) == false)
				{
					range.Add(elt);
				}
				else
				{
					if (includeDelimiters)
						range.Add(elt);

					yield return range;

					range = new List<TElement>();
				}
			}
		}

		public static IEnumerable<List<TElement>> PartitionIntoRangesOfN<TElement>(this IEnumerable<TElement> elements, int n)
		{
			List<TElement> range = new List<TElement>();
			foreach (TElement elt in elements)
			{
				range.Add(elt);

				if (range.Count == n)
				{
					yield return range;
					range = new List<TElement>();
				}
			}
		}

		public static IEnumerable<TResult> LaggingWindow<TElement, TResult>(this IEnumerable<TElement> elements, int size, Func<int, IEnumerable<TElement>, TResult> projection)
		{
			int index = 0;
			List<TElement> currentWindow = new List<TElement>();
			foreach(TElement elt in elements)
			{
				if (currentWindow.Count == size)
					currentWindow.RemoveAt(0);

				currentWindow.Add(elt);
				yield return projection(index++, currentWindow);
			}
		}
	}
}
