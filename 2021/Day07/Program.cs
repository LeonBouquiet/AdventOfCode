using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day07
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/7
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"*** Program for {typeof(Program).FullName.Split('.').First()} ***");

            int result = Part2();
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static int Part1()
        {
            string input = ReadInput();
            List<int> positions = input
                .Split(',')
                .Select(s => Int32.Parse(s))
                .ToList();

            int currentPos = (int)(positions.Sum() / positions.Count);
            Console.WriteLine($"Starting at position {currentPos}...");

            for(int iteration = 0; iteration < 5000; iteration++)
            { 
                int cost = CalculateFuelCost(currentPos, positions);
                int leftCost = CalculateFuelCost(currentPos - 1, positions);
                int rightCost = CalculateFuelCost(currentPos + 1, positions);

                if (cost < leftCost && cost < rightCost)
                {
                    Console.WriteLine($"Found solution after {iteration} iterations on position {currentPos}.");
                    return cost;    //Current position is the cheapest.
                }
                else if (leftCost < cost)
                {
                    currentPos--;
                }
                else if(rightCost < cost)
                {
                    currentPos++;
                }
            }

            return -1;
        }

        private static int CalculateFuelCost(int gatherAt, List<int> positions)
        {
            return positions.Sum(p => Math.Abs(p - gatherAt));
        }

        public static int Part2()
        {
            string input = ReadInput();
            List<int> positions = input
                .Split(',')
                .Select(s => Int32.Parse(s))
                .ToList();

            int currentPos = (int)(positions.Sum() / positions.Count);
            Console.WriteLine($"Starting at position {currentPos}...");

            for (int iteration = 0; iteration < 5000; iteration++)
            {
                int cost = CalculateFuelCostV2(currentPos, positions);
                int leftCost = CalculateFuelCostV2(currentPos - 1, positions);
                int rightCost = CalculateFuelCostV2(currentPos + 1, positions);

                if (cost < leftCost && cost < rightCost)
                {
                    Console.WriteLine($"Found solution after {iteration} iterations on position {currentPos}.");
                    return cost;    //Current position is the cheapest.
                }
                else if (leftCost < cost)
                {
                    currentPos--;
                }
                else if (rightCost < cost)
                {
                    currentPos++;
                }
            }

            return -1;
        }

        private static List<int> CostPerDistance = new List<int>(new int[] { 0 });

        private static int GetDistanceCost(int distance)
        {
            if(distance >= CostPerDistance.Count)
            {
                for(int index = CostPerDistance.Count; index <= distance; index++)
                {
                    int cost = CostPerDistance[index - 1] + index;
                    CostPerDistance.Add(cost);
                }
            }

            return CostPerDistance[distance];
        }

        private static int CalculateFuelCostV2(int gatherAt, List<int> positions)
        {
            return positions.Sum(p => GetDistanceCost(Math.Abs(p - gatherAt)));
        }

        private static string ReadInput()
        {
            Console.WriteLine($"Provide input, terminate with an enter:");
            string line = Console.ReadLine();

            return line;
        }

    }
}
