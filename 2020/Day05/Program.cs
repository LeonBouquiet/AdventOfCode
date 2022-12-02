using System;
using System.Collections.Generic;

namespace Day05
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2020/day/5
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Part1();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void Part1()
        {
            List<string> entries = ReadInput();

            int maxSeatId = 0;
            foreach(string entry in entries)
            {
                string binary = entry
                    .Replace('F', '0')
                    .Replace('B', '1')
                    .Replace('L', '0')
                    .Replace('R', '1');

                int seatId = Convert.ToInt32(binary, 2);
                maxSeatId = Math.Max(seatId, maxSeatId);
            }

            Console.WriteLine($"Result: {maxSeatId}");
        }

        private static List<string> ReadInput()
        {
            List<string> result = new List<string>();

            Console.WriteLine("Provide input, terminate with an empty line:");

            string line;
            while ((line = Console.ReadLine()) != string.Empty)
            {
                result.Add(line);
            }

            return result;
        }
    }
}
