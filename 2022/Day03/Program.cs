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
			int? result = 0;

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			int? result = 0;

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
