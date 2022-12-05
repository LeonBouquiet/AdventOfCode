using System;
using System.Collections.Generic;

namespace Day01
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/1
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Part2();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void Part1()
        {
            List<int> measurements = ReadInput();

            int nrOfIncreases = 0;
            for (int index = 1; index < measurements.Count; index++)
            {
                if (measurements[index] > measurements[index - 1])
                    nrOfIncreases++;
            }

            Console.WriteLine("Result:");
            Console.WriteLine(nrOfIncreases);
        }

        private static void Part2()
        {
            List<int> measurements = ReadInput();

            int previousSum = Int32.MaxValue;
            int nrOfIncreases = 0;
            for (int index = 2; index < measurements.Count; index++)
            {
                int windowAverage = measurements[index - 2] + measurements[index - 1] + measurements[index];
                if (windowAverage > previousSum)
                    nrOfIncreases++;

                previousSum = windowAverage;
            }

            Console.WriteLine("Result:");
            Console.WriteLine(nrOfIncreases);
        }

        private static List<int> ReadInput()
        {
            List<int> lines = new List<int>();

            Console.WriteLine("Provide input, terminate with an empty line:");

            string line;
            while ((line = Console.ReadLine()) != string.Empty)
            {
                lines.Add(Int32.Parse(line));
            }

            return lines;
        }
    }
}
