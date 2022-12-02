using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day02
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
			int result = ReadInput()
				.Select(s => CalculateScore1(s))
				.Sum();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static int CalculateScore1(string round)
		{
			int result = round switch
			{
				"A X" => 3 + 1,
				"A Y" => 6 + 2,
				"A Z" => 0 + 3,
				"B X" => 0 + 1,
				"B Y" => 3 + 2,
				"B Z" => 6 + 3,
				"C X" => 6 + 1,
				"C Y" => 0 + 2,
				"C Z" => 3 + 3,
				_ => throw new ArgumentException($"Couldn't parse round \"{round}\".")
			};

			return result;
		}

		private static void Part2()
		{
			int result = ReadInput()
				.Select(s => CalculateScore2(s))
				.Sum();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static int CalculateScore2(string round)
		{
			int result = round switch
			{
				"A X" => 0 + 3, //Player 2 should lose, so pick scissors (Z)
				"A Y" => 3 + 1,	//Should end in draw, pick rock (X) as well
				"A Z" => 6 + 2, //Player 2 should win, pick paper (Y)
				"B X" => 0 + 1, //Player 2 should lose, so pick rock (X)
				"B Y" => 3 + 2,	//Should end in draw, pick paper (Y) as well
				"B Z" => 6 + 3,	//Player 2 should win, pick scissors (Z)
				"C X" => 0 + 2, //Player 2 should lose, so pick paper (Y)
				"C Y" => 3 + 3,	//Should end in draw, pick scissors (Z) as well
				"C Z" => 6 + 1,	//Player 2 should win, pick rock (X)
				_ => throw new ArgumentException($"Couldn't parse round \"{round}\".")
			};

			return result;
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
