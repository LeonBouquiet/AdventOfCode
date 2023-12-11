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

		public string GetNodeNameByDirection(char direction)
		{
			if (direction == 'L')
				return Left;
			else
				return Right;
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
				//Console.WriteLine($"Step {step + 1}: Current node is {current}, moving {directions[step % directions.Count]}...");

				currentNodeName = current.GetNodeNameByDirection(directions[step % directions.Count]);
				if (currentNodeName == "ZZZ")
					break;
			} 

			Console.WriteLine($"The result of part 1 is: {step + 1}");
		}

		private static void Part2()
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

			List<string> startingNodeNames = nodes
				.Where(n => n.Name.EndsWith('A'))
				.Select(n => n.Name)
				.ToList();

			foreach (string startingNodeName in startingNodeNames)
			{ 
				string currentNodeName = startingNodeName;
				HashSet<Tuple<long, string>> nodeNamesVisited = new HashSet<Tuple<long, string>>();
				List<long> endingNodeSteps = new List<long>();

				int step = 0;
				for (; step < 1000000; step++)
				{
					nodeNamesVisited.Add(new Tuple<long, string>(step % directions.Count, currentNodeName));
					if (currentNodeName.EndsWith('Z'))
					{
						Console.WriteLine($"For starting node {startingNodeName}, found an end node at {step} named {currentNodeName}.");
						endingNodeSteps.Add(step);
					}

					Node current = nodeMap[currentNodeName];
					currentNodeName = current.GetNodeNameByDirection(directions[step % directions.Count]);
				}
			}

			//Take the loop lengths, divide them all by 277 (their Lowest Common Denominator), and use 277 * L1 * L2 * ... L6 as the answer.
			var result = 0;
			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
