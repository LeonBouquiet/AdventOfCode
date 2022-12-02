using System;
using System.Collections.Generic;
using System.Linq;

namespace Day03
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2020/day/3
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
            List<string> entries = ReadInput();

            //int treeCount = 0;
            //int currentX = 0;
            //foreach (string entry in entries)
            //{
            //    treeCount += (entry[currentX] == '#') ? 1 : 0;
            //    currentX = (currentX + 3) % entry.Length;
            //}

            int treeCount = CountTrees(entries, 3, 1);
            Console.WriteLine($"Result: {treeCount}");
        }

        private static void Part2()
        {
            List<string> entries = ReadInput();
            long slope1 = CountTrees(entries, 1, 1);
            long slope2 = CountTrees(entries, 3, 1);
            long slope3 = CountTrees(entries, 5, 1);
            long slope4 = CountTrees(entries, 7, 1);
            long slope5 = CountTrees(entries, 1, 2);
            
            long treeCount = slope1 * slope2 * slope3 * slope4 * slope5;
            Console.WriteLine($"Result: {treeCount}");
        }

        private static int CountTrees(List<string> entries, int displacementX, int displacementY)
        {
            int treeCount = 0;
            int currentX = 0;

            for(int currentY = 0; currentY < entries.Count; currentY++)
            {
                string entry = entries[currentY];
                treeCount += (entry[currentX] == '#') ? 1 : 0;

                currentX = (currentX + displacementX) % entry.Length;
                currentY = currentY + (displacementY - 1);
            }

            return treeCount;
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
