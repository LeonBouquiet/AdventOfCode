using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day01
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//Part1();
			Part2();

			Console.WriteLine("Press enter to exit...");
			Console.ReadLine();
		}

		private static void Part1()
		{
			var result = InputReader.Read<Program>()
				.Select(line => line.ToCharArray().Where(c => Char.IsDigit(c)).ToArray())
				.Select(arr => (arr.First() - '0') * 10 + (arr.Last() - '0'))
				.Sum();

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>()
				.Select(line => ExtractAllDigits(line).ToArray())
				.Select(arr => (arr.First() * 10 + arr.Last()))
				.Sum();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static IEnumerable<int> ExtractAllDigits(string line)
		{
			for(int index = 0; index < line.Length; index++)
			{
				int? digit = TryExtractDigitAt(line, index);
				if (digit != null)
					yield return digit.Value;
			}

		}

		private static string[] DigitNames = new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

		private static int? TryExtractDigitAt(string line, int pos)
		{
			if (Char.IsDigit(line[pos]))
				return line[pos] - '0';

			for(int digitNr = 0; digitNr <= 9; digitNr++)
			{
				if (pos + DigitNames[digitNr].Length <= line.Length && line.Substring(pos, DigitNames[digitNr].Length) == DigitNames[digitNr])
					return digitNr;
			}

			return null;
		}
	}
}
