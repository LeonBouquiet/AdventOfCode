using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day03
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/3
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
            List<string> lines = ReadInput();

            int[] ones = new int[lines.First().Length];
            Array.Fill(ones, 0);

            foreach(string line in lines)
            {
                for(int index = 0; index < line.Length; index++)
                {
                    if (line[index] == '1')
                        ones[index]++;
                }
            }

            string gammaRateText = "";
            string epsilonRateText = "";
            for (int index = 0; index < ones.Length; index++)
            {
                gammaRateText += (ones[index] > lines.Count / 2) ? '1' : '0';
                epsilonRateText += (ones[index] > lines.Count / 2) ? '0' : '1';
            }

            int gammaRate = Convert.ToInt32(gammaRateText, 2);
            int epsilonRate = Convert.ToInt32(epsilonRateText, 2);
            int result = gammaRate * epsilonRate;

            Console.WriteLine($"Result: {result}");
        }

        private static void Part2()
        {
            List<string> lines = ReadInput();
            int numberLength = lines.First().Length;

            List<string> oxygenCandidates = lines.ToList();
            for (int index = 0; index < numberLength; index++)
            {
                int ones = oxygenCandidates.Count(line => line[index] == '1');
                char mostCommonValue = (ones >= (oxygenCandidates.Count / 2.0f)) ? '1' : '0';

                oxygenCandidates.RemoveAll(can => can[index] != mostCommonValue);
                if (oxygenCandidates.Count <= 1)
                    break;
            }

            int oxygen = Convert.ToInt32(oxygenCandidates.Single(), 2);

            List<string> scrubberCandidates = lines.ToList();
            for (int index = 0; index < numberLength; index++)
            {
                int ones = scrubberCandidates.Count(line => line[index] == '1');
                char leastCommonValue = (ones >= (scrubberCandidates.Count / 2.0f)) ? '0' : '1';

                scrubberCandidates.RemoveAll(can => can[index] != leastCommonValue);
                if (scrubberCandidates.Count <= 1)
                    break;
            }

            int scrubber = Convert.ToInt32(scrubberCandidates.Single(), 2);

            int result = oxygen * scrubber;
            Console.WriteLine($"Result: {result}");
        }

        private static List<string> ReadInput()
        {
            List<string> lines = new List<string>();

            Console.WriteLine("Provide input, terminate with an empty line:");

            string line;
            while ((line = Console.ReadLine()) != string.Empty)
            {
                lines.Add(line);
            }

            return lines;
        }
    }
}
