using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day01
{
	public static class Extensions
	{
		public static IEnumerable<List<TElement>> PartitionIntoRangesBy<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> isDelimiter, bool includeDelimiters)
		{
			List<TElement> range = new List<TElement>();
			foreach(TElement elt in elements)
			{
				if(isDelimiter(elt) == false)
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
	}
}
