using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day02
{
    public class Displacement
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private static readonly Regex DisplacementRegex = new Regex(@"(forward|up|down) (\d+)");

        public Displacement(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Displacement(string text)
        {
            Match match = DisplacementRegex.Match(text);
            int value = Int32.Parse(match.Groups[2].Value);

            switch (match.Groups[1].Value)
            {
                case "forward":
                    X = value; break;
                case "up":
                    Y = -value; break;
                case "down":
                    Y = value; break;
            }
        }

        public void Add(Displacement other)
        {
            X = X + other.X;
            Y = Y + other.Y;
        }
    }

    public class DisplacementV2
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public int Aim { get; private set; }

        private static readonly Regex DisplacementRegex = new Regex(@"(forward|up|down) (\d+)");

        public DisplacementV2(int x, int y, int aim)
        {
            X = x;
            Y = y;
            Aim = aim;
        }

        public DisplacementV2(string text)
        {
            Match match = DisplacementRegex.Match(text);
            int value = Int32.Parse(match.Groups[2].Value);

            switch (match.Groups[1].Value)
            {
                case "forward":
                    X = value; break;
                case "up":
                    Y = -value; break;
                case "down":
                    Y = value; break;
            }
        }

        public void Add(DisplacementV2 other)
        {
            if (other.Y != 0)
            {
                Aim += other.Y;
            }
            else
            {
                X += other.X;
                Y += Aim * other.X;
            }
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/2
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Part2();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        //private static void Part1()
        //{
        //    List<Displacement> displacements = ReadInput();

        //    Displacement position = new Displacement(0, 0);
        //    displacements.ForEach(d => position.Add(d));

        //    int result = position.X * position.Y;
        //    Console.WriteLine($"Result: {result}");
        //}

        private static void Part2()
        {
            List<DisplacementV2> displacements = ReadInput();

            DisplacementV2 position = new DisplacementV2(0, 0, 0);
            displacements.ForEach(d => position.Add(d));

            int result = position.X * position.Y;
            Console.WriteLine($"Result: {result}");
        }

        private static List<DisplacementV2> ReadInput()
        {
            List<DisplacementV2> displacements = new List<DisplacementV2>();

            Console.WriteLine("Provide input, terminate with an empty line:");

            string line;
            while ((line = Console.ReadLine()) != string.Empty)
            {
                displacements.Add(new DisplacementV2(line));
            }

            return displacements;
        }
    }

}
