using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;

namespace Day21
{
	public class Monkey
	{
		public string Name { get; set; }

		public virtual long Value { get; }

		public Monkey(string name, long value)
		{
			Name = name;
			Value = value;
		}

		public static Monkey Parse(string line)
		{
			string[] parts = line.Split(' ', ':')
				.Where(s => !string.IsNullOrEmpty(s))
				.ToArray();

			if (parts.Length < 4)
				return new Monkey(parts[0], Int32.Parse(parts[1]));
			else
				return new MathMonkey(parts[0], new MonkeyPlaceholder(parts[1]), new MonkeyPlaceholder(parts[3]), parts[2][0]);
		}
	}

	public class MonkeyPlaceholder : Monkey
	{
		public MonkeyPlaceholder(string name): base(name, 0)
		{
		}
	}

	public class MathMonkey: Monkey
	{
		public Monkey Left { get; set; }

		public Monkey Right { get; set; }

		public char Operation { get; set; }

		public MathMonkey(string name, Monkey left, Monkey right, char operation)
			: base(name, 0)
		{
			Left = left;
			Right = right;
			Operation = operation;
		}

		public override long Value 
		{
			get
			{
				return Operation switch
				{
					'+' => Left.Value + Right.Value,
					'-' => Left.Value - Right.Value,
					'*' => Left.Value * Right.Value,
					'/' => Left.Value / Right.Value,
					_ => throw new Exception()
				};
			}
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			List<Monkey> monkeys = InputReader.Read<Program>()
				.Select(line => Monkey.Parse(line))
				.ToList();

			Dictionary<string, Monkey> namedMonkeys = monkeys.ToDictionary(m => m.Name);
			foreach(MathMonkey mm in monkeys.OfType<MathMonkey>())
			{
				mm.Left = namedMonkeys[mm.Left.Name];
				mm.Right = namedMonkeys[mm.Right.Name];
			}

			Part1(namedMonkeys["root"]);
			Part2();
		}



		private static void Part1(Monkey root)
		{
			long result = root.Value;
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
