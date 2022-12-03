using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day03
{
	public static class Extensions
	{
		public static IEnumerable<List<TElement>> PartitionIntoRangesOfN<TElement>(this IEnumerable<TElement> elements, int n)
		{
			List<TElement> range = new List<TElement>();
			foreach(TElement elt in elements)
			{
				range.Add(elt);

				if (range.Count == n) 
				{ 
					yield return range;
					range = new List<TElement>();
				}
			}
		}
	}
}
