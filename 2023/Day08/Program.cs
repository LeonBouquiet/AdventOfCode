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

			List<string> currentNodeNames = nodes
				.Where(n => n.Name.EndsWith('A'))
				.Select(n => n.Name)
				.ToList();

			int step = 0;
			for (; step < 1000000000; step++)
			{
				int endingNodeCount = 0;
				for(int index = 0; index < currentNodeNames.Count; index++)
				{
					Node current = nodeMap[currentNodeNames[index]];
					currentNodeNames[index] = current.GetNodeNameByDirection(directions[step % directions.Count]);

					if (currentNodeNames[index].EndsWith('Z'))
						endingNodeCount++;
				}

				if (endingNodeCount > 2)
					Console.WriteLine($"Step {step + 1}: Reached {endingNodeCount} ending nodes, at positions: ({string.Join(", ", currentNodeNames)}) ");
				if (endingNodeCount == currentNodeNames.Count)
					break;
			}


			var result = 0;
			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
