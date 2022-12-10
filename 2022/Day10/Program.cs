using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Day10
{
	public class Instruction
	{
		public string? OpCode { get; set; }
		public int Operand { get; set; }
		public int EffectiveFromCycle { get; set; }
		public int RegisterX { get; set; }

		public static Instruction Parse(string line)
		{
			string[] parts = line.Split(' ');
			int operand = (parts.Length > 1) ? Int32.Parse(parts[1]) : 0;
			return new Instruction() { OpCode = parts[0], Operand = operand };
		}

		public override string ToString()
		{
			return $"{OpCode} {(OpCode != "noop" ? Operand : " ")} (X={RegisterX}, Effective from cycle {EffectiveFromCycle})";
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
			var instructions = InputReader.Read<Program>()
				.Select(line => Instruction.Parse(line))
				.ToList();

			int x = 1;
			int cycle = 1;
			int nextSignalStrengthCycle = 20;
			int aggregatedSignalStrength = 0;
			foreach(Instruction ins in instructions)
			{
				if (ins.OpCode == "addx")
					x += ins.Operand;

				int increment = ((ins.OpCode == "noop") ? 1 : 2);
				if (cycle + increment >= nextSignalStrengthCycle)
				{
					int signalStrength = (nextSignalStrengthCycle * x);
					aggregatedSignalStrength += signalStrength;
					nextSignalStrengthCycle += 40;
				}

				cycle += increment;
				ins.RegisterX = x;
				ins.EffectiveFromCycle = cycle;
			}

			Console.WriteLine($"The result of part 1 is: {aggregatedSignalStrength}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
