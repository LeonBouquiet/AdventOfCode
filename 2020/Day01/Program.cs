using System;
using System.Collections.Generic;
using System.Linq;

namespace Day01
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2020/day/1
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
            List<int> entries = ReadInput();

            int[] sorted = entries.OrderBy(e => e).ToArray();

            foreach(int entry in sorted)
            {
                int expectedCounterpart = 2020 - entry;
                if(Array.BinarySearch(sorted, expectedCounterpart) >= 0)
                {
                    Console.WriteLine($"Result: {entry} * {expectedCounterpart} = {entry * expectedCounterpart}");
                    break;
                }

            }
        }

        private static void Part2()
        {
            List<int> entries = ReadInput();

            int[] sorted = entries.OrderBy(e => e).ToArray();

            for(int firstIndex = 0; firstIndex < sorted.Length; firstIndex++)
            {
                int first = sorted[firstIndex];
                int remainder = 2020 - first;

                for(int secondIndex = 0; secondIndex < sorted.Length; secondIndex++)
                {
                    if (secondIndex == firstIndex)
                        continue;

                    int second = sorted[secondIndex];
                    if (second > remainder)
                        break;

                    int expectedCounterpart = remainder - second;
                    if (Array.BinarySearch(sorted, expectedCounterpart) >= 0)
                    {
                        Console.WriteLine($"Result: {first} * {second} * {expectedCounterpart} = {first * second * expectedCounterpart}");
                        firstIndex = sorted.Length;
                        break;
                    }

                }
            }
        }
        private static List<int> ReadInput()
        {
            List<int> result = new List<int>();

            Console.WriteLine("Provide input, terminate with an empty line:");

            string line;
            while ((line = Console.ReadLine()) != string.Empty)
            {
                result.Add(Int32.Parse(line));
            }

            return result;
        }
    }
}
