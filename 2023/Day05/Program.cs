using System;
using System.Collections.Generic;
using System.Linq;
using Shared;

namespace Day05
{
	public class RangeEntry
	{
		public long DestStart { get; set; }
		public long SourceStart { get; set; }
		public long Count { get; set; }

		public bool Includes(long source)
		{
			return (SourceStart <= source) && (source < SourceStart + Count);
		}
	}

	public class Map
	{
		public string Name { get; private set; }

		public List<RangeEntry> RangeEntries { get; private set; }

		public Map(string name, IEnumerable<RangeEntry> rangeEntries)
		{
			Name = name;
			RangeEntries = rangeEntries.OrderBy(ent => ent.SourceStart).ToList();
		}

		public long Resolve(long source)
		{
			foreach(var entry in RangeEntries)
			{
				if (entry.Includes(source))
					return (source - entry.SourceStart) + entry.DestStart;
			}

			return source; //Not defined, map to source.
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static void Part1()
		{
			List<string>[] sections = InputReader.Read<Program>().Concat(new string[] {""}) //Add an empty line so last section is terminated as well.
				.PartitionIntoRangesBy(line => string.IsNullOrWhiteSpace(line), includeDelimiters: false)
				.ToArray();

			long[] seeds = sections[0].First().Substring(7)
				.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
				.Select(s => Int64.Parse(s))
				.ToArray();

			List<Map> maps = ParseMaps(sections.Skip(1));

			List<Tuple<long, long>> seedToLocationPairs = new List<Tuple<long, long>>();
			foreach(long seed in seeds)
			{
				long value = seed;
				foreach (Map map in maps)
					value = map.Resolve(value);

				seedToLocationPairs.Add(new Tuple<long, long>(seed, value));
			}

			var result = seedToLocationPairs.Min(pair => pair.Item2);
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static List<Map> ParseMaps(IEnumerable<List<string>> mapSections)
		{
			List<Map> result = new List<Map>();

			foreach (List<string>? section in mapSections)
			{
				string name = section.First().Replace(" map:", "");
				Console.WriteLine($"Constructing map {name}...");

				List<RangeEntry> rangeEntries = new List<RangeEntry>();
				foreach (string line in section.Skip(1))
				{
					long[] values = line
						.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
						.Select(s => Int64.Parse(s))
						.ToArray();

					rangeEntries.Add(new RangeEntry() { DestStart = values[0], SourceStart = values[1], Count = values[2] });
				}

				result.Add(new Map(name, rangeEntries));
			}

			return result;
		}

	}
}
