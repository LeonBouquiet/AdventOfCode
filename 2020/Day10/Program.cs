using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day10
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"*** Program for {typeof(Program).FullName.Split('.').First()} ***");

            long result = Part2();
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static int Part1()
        {
            List<string> textLines = ReadInput();

            List<int> sortedAdapers = textLines
                .Select(s => Int32.Parse(s))
                .OrderBy(i => i)
                .ToList();

            sortedAdapers.Insert(0, 0);
            sortedAdapers.Add(sortedAdapers.Last() + 3);

            List<int> differences = Enumerable.Range(0, sortedAdapers.Count - 1)
                .Select(i => sortedAdapers[i + 1] - sortedAdapers[i])
                .ToList();

            int result = differences.Count(i => i == 1) * differences.Count(i => i == 3);
            return result;
        }

        private static List<int> SortedAdapers;

        public static long Part2()
        {
            List<string> textLines = ReadInput();

            SortedAdapers = textLines
                .Select(s => Int32.Parse(s))
                .OrderBy(i => i)
                .ToList();

            SortedAdapers.Insert(0, 0);
            SortedAdapers.Add(SortedAdapers.Last() + 3);

            long result = CountConfigurations(0);
            return result;
        }

        private static Dictionary<int, long> CachedCountsPerIndex = new Dictionary<int, long>();

        private static long CountConfigurations(int currentIndex)
        {
            if (currentIndex == SortedAdapers.Count - 1)
                return 1;

            long pathCount = 0;
            if (CachedCountsPerIndex.TryGetValue(currentIndex, out pathCount) == false)
            {
                for (int skip = 1; skip <= 3; skip++)
                {
                    if (currentIndex + skip < SortedAdapers.Count && SortedAdapers[currentIndex + skip] - SortedAdapers[currentIndex] <= 3)
                    {
                        pathCount += CountConfigurations(currentIndex + skip);
                    }
                }

                CachedCountsPerIndex[currentIndex] = pathCount;
            }

            return pathCount;
        }


        private static List<string> ReadInput(string delimiter = "")
        {
            List<string> lines = new List<string>();

            Console.WriteLine($"Provide input, terminate with { (delimiter != "" ? delimiter : "an empty line") }:");

            string line;
            while ((line = Console.ReadLine()) != delimiter)
            {
                lines.Add(line);
            }

            return lines;
        }

    }
}
