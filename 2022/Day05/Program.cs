using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;

namespace Day05
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static List<Stack<Char>> Stacks;

		private static void Part1()
		{
			Process(ApplyMove1);

			string result = string.Join("", Stacks.Select(s => s.Count > 0 ? s.Peek() : ' '));
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			Process(ApplyMove2);

			string result = string.Join("", Stacks.Select(s => s.Count > 0 ? s.Peek() : ' '));
			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static void Process(Action<int, int, int> applyMoveAction)
		{
			var lines = InputReader.Read<Program>().ToList();
			string[] stackLines = lines
				.TakeWhile(s => string.IsNullOrWhiteSpace(s) == false)
				.ToArray();

			//Create the stacks of crates - index 0 corresponds to column 1.
			Stacks = new List<Stack<char>>();
			for (int index = 0; index < 9; index++)
				Stacks.Add(new Stack<char>());

			stackLines
				.Reverse()
				.Skip(1)        //Skip the lines with the column numbers
				.ToList()
				.ForEach(s => ParseStackLine(s));

			string[] moveLines = lines
				.Skip(stackLines.Length + 1)
				.ToArray();

			Regex moveLineRegex = new Regex(@"move (\d+) from (\d+) to (\d+)", RegexOptions.Compiled);
			foreach (string moveLine in moveLines)
			{
				Match match = moveLineRegex.Match(moveLine);
				int count = Int32.Parse(match.Groups[1].Value);
				int sourceIndex = Int32.Parse(match.Groups[2].Value);
				int targetIndex = Int32.Parse(match.Groups[3].Value);

				applyMoveAction(count, sourceIndex, targetIndex);
			}
		}

		private static void ParseStackLine(string line)
		{
			for(int index = 0; index < 9; index++)
			{
				int charPos = index * 4 + 1;
				if(line.Length > charPos)
				{
					char crate = line[charPos];
					if (crate != ' ')
						Stacks[index].Push(crate);
				}
			}
		}

		private static void ApplyMove1(int count, int sourceIndex, int targetIndex)
		{
			for(int i = 0; i < count; i++)
			{
				char crate = Stacks[sourceIndex - 1].Pop();
				Stacks[targetIndex - 1].Push(crate);
			}
		}

		private static void ApplyMove2(int count, int sourceIndex, int targetIndex)
		{
			List<char> cratesToMove = new List<char>();
			for (int i = 0; i < count; i++)
				cratesToMove.Add(Stacks[sourceIndex - 1].Pop());

			foreach(char crate in cratesToMove.AsEnumerable().Reverse())
				Stacks[targetIndex - 1].Push(crate);
		}

	}
}
