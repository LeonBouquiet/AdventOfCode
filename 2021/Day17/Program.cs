using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day17
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public class Box
    {
        public Point TopLeft { get; private set; }

        public Point BottomRight { get; private set; }

        public Box(Point topLeft, Point bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        public bool IsInside(Point p)
        {
            return (p.X >= TopLeft.X && p.X <= BottomRight.X && p.Y >= TopLeft.Y && p.Y <= BottomRight.Y);
        }

        public override string ToString()
        {
            return $"{TopLeft} - {BottomRight}";
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/17
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

        private static readonly Regex InputRegex = new Regex(@"target area: x=(-?\d+)..(-?\d+), y=(-?\d+)..(-?\d+)");

        private static readonly Regex VectorRegex = new Regex(@"(-?\d+), *(-?\d+)");

        public static int Part1()
        {
            //List<string> textLines = ReadInput();
            Match match = InputRegex.Match("target area: x=257..286, y=-101..-57");
            int[] values = match.Groups.Cast<Group>()
                .Skip(1)
                .Select(grp => Int32.Parse(grp.Value))
                .ToArray();

            Box targetBox = new Box(
                new Point((values[0] < values[1] ? values[0] : values[1]), (values[2] < values[3] ? values[2] : values[3])),
                new Point((values[0] >= values[1] ? values[0] : values[1]), (values[2] >= values[3] ? values[2] : values[3])));

            Console.WriteLine($"Working with Box {targetBox}, specify initial velocity:");

            (Status, Point) result = (Status.Hit, new Point(0, 0));
            do
            {
                string vector = Console.ReadLine();
                if (vector == "")
                    break;

                match = VectorRegex.Match(vector);
                if (match.Success)
                {
                    result = CalculateTrajectory(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value), targetBox);
                    Console.WriteLine($"Status: {result.Item1}: {result.Item2}");
                }
                else
                {
                    Console.WriteLine("???");
                }
            } while (true);

            return (result.Item1 == Status.Hit) ? result.Item2.Y : -1;
        }

        public static long Part2()
        {
            Match match = InputRegex.Match("target area: x=257..286, y=-101..-57");
            int[] values = match.Groups.Cast<Group>()
                .Skip(1)
                .Select(grp => Int32.Parse(grp.Value))
                .ToArray();

            Box targetBox = new Box(
                new Point((values[0] < values[1] ? values[0] : values[1]), (values[2] < values[3] ? values[2] : values[3])),
                new Point((values[0] >= values[1] ? values[0] : values[1]), (values[2] >= values[3] ? values[2] : values[3])));

            long hitCount = 0;
            for (int deltaX = 0; deltaX <= 286; deltaX++) 
            {
                for (int deltaY = -101; deltaY <= 500; deltaY++)
                {
                    (Status, Point) result = CalculateTrajectory(deltaX, deltaY, targetBox);
                    if (result.Item1 == Status.Hit)
                        hitCount++;
                }
            }

            return hitCount;
        }

        public enum Status
        {
            TooClose,
            TooFar,
            TooFast,
            Hit
        }

        private static (Status, Point) CalculateTrajectory(int deltaX, int deltaY, Box targetBox)
        {
            Point current = new Point(0, 0);
            Point highestPoint = current;

            do
            {
                current = new Point(current.X + deltaX, current.Y + deltaY);
                highestPoint = current.Y > highestPoint.Y ? current : highestPoint;

                if (targetBox.IsInside(current))
                    return (Status.Hit, highestPoint);

                if (deltaX > 0)
                    deltaX--;
                deltaY--;
            }
            while (current.Y >= targetBox.TopLeft.Y);

            if (current.X < targetBox.TopLeft.X)
                return (Status.TooClose, highestPoint);
            else if (current.X > targetBox.BottomRight.X)
                return (Status.TooFar, highestPoint);
            else
                return (Status.TooFast, highestPoint);
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
