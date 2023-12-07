using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day06
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static void Part1()
		{
			List<int[]> timesAndDistances = InputReader.Read<Program>()
				.Select(line => line
					.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
					.Skip(1)
					.Select(s => Int32.Parse(s))
					.ToArray())
				.ToList();

			int result = 1;
			for(int raceIndex = 0; raceIndex < timesAndDistances[0].Length; raceIndex++)
			{
				int time = timesAndDistances[0][raceIndex];
				int recordDistance = timesAndDistances[1][raceIndex];

				int winCount = 0;
				for (int holdButtonTime = 0; holdButtonTime < time; holdButtonTime++)
				{
					int distance = holdButtonTime * (time - holdButtonTime);
					if (distance > recordDistance)
						winCount++;
				}

				result *= winCount;
			}

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			List<long> timeAndDistance = InputReader.Read<Program>()
				.Select(line => line
				    .Replace(" ", "")
					.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
					.Skip(1)
					.Select(s => Int64.Parse(s))
					.First())
				.ToList();

			long time = timeAndDistance[0];
			long recordDistance = timeAndDistance[1];

			long winCount = 0;
			for (long holdButtonTime = 0; holdButtonTime < time; holdButtonTime++)
			{
				long distance = holdButtonTime * (time - holdButtonTime);
				if (distance > recordDistance)
					winCount++;
			}

			Console.WriteLine($"The result of part 2 is: {winCount}");
		}

	}
}
