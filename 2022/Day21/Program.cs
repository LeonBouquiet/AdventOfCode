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
		public static bool DontEvaluateHuman = false;

		public Monkey Parent = null!;

		public string Name { get; set; }

		private long _value;

		public virtual long Value 
		{ 
			get 
			{ 
				if (DontEvaluateHuman == false || Name != "humn") 
					return _value; 
				else 
					throw new Exception(); 
			}
			set { _value = value; }
		}

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

		public override string ToString() => $"{Name}: {Value}";
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

		public long ShouldProduce { get; set; }

		public MathMonkey(string name, Monkey left, Monkey right, char operation)
			: base(name, 0)
		{
			Left = left;
			Left.Parent = this;
			Right = right;
			Right.Parent = this;
			Operation = operation;
		}

		public override string ToString() => $"({Name}: {Left.Value} {Operation} {Right.Value})";

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
					'=' => (Left.Value == Right.Value) ? 0 : -1,
					_ => throw new Exception()
				};
			}
		}

		public char IsInLeftOrRightSubtree(Monkey monkey)
		{
			for(Monkey current = monkey; current != null; current = current.Parent)
			{
				if (current == Left)
					return 'L';
				else if (current == Right)
					return 'R';
			}

			return ' ';
		}

		public long CalculateLeftSideValue()
		{
			return Operation switch
			{
				'+' => ShouldProduce - Right.Value,  //Left + Right = ShouldProduce,  bijv 8 + 2 = 10
				'-' => ShouldProduce + Right.Value,  //Left - Right = ShouldProduce,  bijv. 8 - 2 = 6
				'*' => ShouldProduce / Right.Value,  //Left * Right = ShouldProduce,  bijv 8 * 2 = 16
				'/' => ShouldProduce * Right.Value,  //Left / Right = ShouldProduce,  bijv 8 / 2 = 4
				'=' => Right.Value,
				_ => throw new Exception()
			};
		}

		public long CalculateRightSideValue()
		{
			return Operation switch
			{
				'+' => ShouldProduce - Left.Value,     //Left + Right = ShouldProduce,  bijv. 8 + 2 = 10
				'-' => Left.Value - ShouldProduce,     //Left - Right = ShouldProduce,  bijv. 8 - 2 = 6
				'*' => ShouldProduce / Left.Value,     //Left * Right = ShouldProduce,  bijv 8 * 2 = 16
				'/' => Left.Value / ShouldProduce,     //Left / Right = ShouldProduce,  bijv 8 / 2 = 4
				'=' => Left.Value,
				_ => throw new Exception()
			};
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
				if (mm.Left.Parent == null)
					mm.Left.Parent = mm;
				else
					throw new Exception();

				mm.Right = namedMonkeys[mm.Right.Name];
				if (mm.Right.Parent == null)
					mm.Right.Parent = mm;
				else
					throw new Exception();
			}

			Part1(namedMonkeys["root"]);
			Part2(namedMonkeys["root"] as MathMonkey, namedMonkeys["humn"]);
		}



		private static void Part1(Monkey root)
		{
			long result = root.Value;
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2(MathMonkey root, Monkey human)
		{
			root.Operation = '=';
			Monkey.DontEvaluateHuman = true;

			MathMonkey current = root;
			do
			{
				char side = current.IsInLeftOrRightSubtree(human);
				if(side == 'L')
				{
					if(!(current.Left is MathMonkey))
					{
						long result = current.CalculateLeftSideValue();
						Console.WriteLine($"Human value is {result}.");
						return;
					}

					MathMonkey child = (MathMonkey)current.Left;
					child.ShouldProduce = current.CalculateLeftSideValue();

					current = child;
				}
				else if(side == 'R')
				{
					if (!(current.Right is MathMonkey))
					{
						long result = current.CalculateRightSideValue();
						Console.WriteLine($"Human value is {result}.");
						return;
					}

					MathMonkey child = (MathMonkey)current.Right;
					child.ShouldProduce = current.CalculateRightSideValue();

					current = child;
				}

			} while (true);

		}

	}
}
