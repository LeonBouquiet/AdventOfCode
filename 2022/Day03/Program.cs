using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day03
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();

			Console.WriteLine("Press enter to exit...");
			Console.ReadLine();
		}

		private static void Part1()
		{
			var result = ReadInput()
				.Select(line => new { First = line.Substring(0, line.Length / 2), Second = line.Substring(line.Length / 2) })
				.Select(pair => pair.First.Intersect(pair.Second).Single())
				.Select(c => (c < 'a') ? (c - 'A' + 27) : (c - 'a' + 1))
				.Sum();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = ReadInput()
				.PartitionIntoRangesOfN(3)
				.Select(range => (range[0].Intersect(range[1])).Intersect(range[2]).Single())
				.Select(c => (c < 'a') ? (c - 'A' + 27) : (c - 'a' + 1))
				.Sum();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static IEnumerable<string> ReadInput()
		{
			using (Stream? stream = typeof(Program).Assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.Input.txt"))
			{
				using (StreamReader sr = new StreamReader(stream!))
				{
					string? line;
					do
					{
						line = sr.ReadLine();
						if (line != null)
							yield return line;
					}
					while (line != null);
				}
			}
		}
	}
}
