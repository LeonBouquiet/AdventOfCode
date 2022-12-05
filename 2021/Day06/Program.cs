using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day06
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

            ulong result = Part2();
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static int Part1()
        {
            string input = ReadInput();
            List<int> startValues = input
                .Split(',')
                .Select(s => Int32.Parse(s))
                .ToList();

            List<int> fishCountsPerTimer = new List<int>(Enumerable.Repeat(0, 9));
            startValues
                .GroupBy(i => i)
                .ToList()
                .ForEach(grp => fishCountsPerTimer[grp.Key] = grp.Count());

            for(int timerLoopCount = 1; timerLoopCount <= 80; timerLoopCount++)
            {
                int nrOfFishOnZero = fishCountsPerTimer[0];
                fishCountsPerTimer.RemoveAt(0);
                fishCountsPerTimer.Add(0);

                fishCountsPerTimer[6] += nrOfFishOnZero;
                fishCountsPerTimer[8] += nrOfFishOnZero;
            }

            return fishCountsPerTimer.Sum();
        }

        public static ulong Part2()
        {
            string input = ReadInput();
            List<int> startValues = input
                .Split(',')
                .Select(s => Int32.Parse(s))
                .ToList();

            List<long> fishCountsPerTimer = new List<long>(Enumerable.Repeat<long>(0, 9));
            startValues
                .GroupBy(i => i)
                .ToList()
                .ForEach(grp => fishCountsPerTimer[grp.Key] = grp.Count());

            for (int timerLoopCount = 1; timerLoopCount <= 256; timerLoopCount++)
            {
                long nrOfFishOnZero = fishCountsPerTimer[0];
                fishCountsPerTimer.RemoveAt(0);
                fishCountsPerTimer.Add(0);

                fishCountsPerTimer[6] += nrOfFishOnZero;
                fishCountsPerTimer[8] += nrOfFishOnZero;
            }

            ulong result = 0;
            fishCountsPerTimer.ForEach(i => result += (ulong)i);

            return result;
        }

        private static string ReadInput()
        {
            Console.WriteLine($"Provide input, terminate with an enter:");
            string line = Console.ReadLine();

            return line;
        }

    }
}
