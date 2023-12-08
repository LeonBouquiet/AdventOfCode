using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day08
{
	public class Node
	{
		public string Name { get; set; }
		public string Left { get; set; }
		public string Right { get; set; }

		private static readonly char[] SplitOn = new char[] { ' ', '=', '(', ',', ')' };

		public static Node Parse(string line)
		{
			string[] parts = line
				.Split(SplitOn, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
				.ToArray();

			return new Node() { Name = parts[0], Left = parts[1], Right = parts[2] };
		}

		public override string ToString()
		{
			return $"{Name} = ({Left}, {Right})";
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
			List<string>[] sections = InputReader.Read<Program>().Concat(new string[] { "" })
				.PartitionIntoRangesBy(line => string.IsNullOrWhiteSpace(line), includeDelimiters: false)
				.ToArray();

			List<char> directions = sections[0].First()
				.ToCharArray()
				.ToList();
			List<Node> nodes = sections[1]
				.Select(line => Node.Parse(line))
				.ToList();

			Dictionary<string, Node> nodeMap = nodes
				.ToDictionary(node => node.Name, StringComparer.OrdinalIgnoreCase);

			string currentNodeName = "AAA";

			int step = 0;
			for(; step < 10000000; step++)
			{
				Node current = nodeMap[currentNodeName];
				Console.WriteLine($"Step {step + 1}: Current node is {current}, moving {directions[step % directions.Count]}...");

				if (directions[step % directions.Count] == 'L')
					currentNodeName = current.Left;
				else
					currentNodeName = current.Right;

				if (currentNodeName == "ZZZ")
					break;
			} 

			Console.WriteLine($"The result of part 1 is: {step + 1}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
