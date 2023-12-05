using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;
using static System.Collections.Specialized.BitVector32;

namespace Day05
{
	public class Map
	{
		public string Name { get; set; }
		public Dictionary<long, long> SourceToDest { get; set; }

		public long Resolve(long source)
		{
			long result;
			if (SourceToDest.TryGetValue(source, out result))
				return result;
			else
				return source;	//Not defined, map to source value.
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

				Dictionary<long, long> map = new Dictionary<long, long>();
				foreach (string line in section.Skip(1))
				{
					long[] values = line
						.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
						.Select(s => Int64.Parse(s))
						.ToArray();

					long destStart = values[0];
					long sourceStart = values[1];
					long count = values[2];

					//Expand the values while filling the map
					for (long index = 0; index < count; index++)
						map[sourceStart + index] = destStart + index;
				}

				result.Add(new Map() { Name = name, SourceToDest = map });
			}

			return result;
		}

	}
}
