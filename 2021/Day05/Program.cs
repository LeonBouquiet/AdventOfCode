using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day05
{
    public class Point
    {
        public int X { get; protected set; }

        public int Y { get; protected set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point Displace(int deltaX, int deltaY)
        {
            return new Point(this.X + deltaX, this.Y + deltaY);
        }

        public override string ToString()
        {
            return string.Format("({0,3}, {1,3})", X, Y);
        }
    }

    public class Line
    {
        public Point Start { get; private set; }

        public Point End { get; private set; }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        private static Regex LineRegex = new Regex(@"(\d+),(\d+) -> (\d+),(\d+)");

        public Line(string text)
        {
            Match match = LineRegex.Match(text);
            if (match.Success == false)
                throw new ArgumentException("text");

            Start = new Point(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
            End = new Point(Int32.Parse(match.Groups[3].Value), Int32.Parse(match.Groups[4].Value));
        }

        public bool IsAxisAligned
        {
            get { return (Start.X == End.X) || (Start.Y == End.Y); }
        }

        public IEnumerable<Point> Points
        {
            get
            {
                int deltaX = End.X - Start.X;
                int deltaY = End.Y - Start.Y;
                int steps = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
                deltaX = deltaX / steps;
                deltaY = deltaY / steps;

                Point current = Start;
                for(int step  = 0; step <= steps; step++)
                {
                    yield return current;
                    current = current.Displace(deltaX, deltaY);
                }
            }
        }

        public override string ToString()
        {
            return $"{Start} -> {End}";
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/5
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
            List<string> linesText = ReadInput();

            List<Line> lines = linesText
                .Select(s => new Line(s))
                .Where(l => l.IsAxisAligned)
                .ToList();

            int width = Math.Max(lines.Max(l => l.Start.X), lines.Max(l => l.End.X)) + 1;
            int height = Math.Max(lines.Max(l => l.Start.Y), lines.Max(l => l.End.Y)) + 1;
            int[,] board = new int[height, width];

            foreach(Line line in lines)
            {
                foreach (Point p in line.Points)
                    board[p.Y, p.X]++;
            }

            int nrOfValuesLargerThanOne = board.Cast<int>().Count(i => i > 1);
            return nrOfValuesLargerThanOne;
        }

        public static int Part2()
        {
            List<string> linesText = ReadInput();

            List<Line> lines = linesText
                .Select(s => new Line(s))
                .ToList();

            int width = Math.Max(lines.Max(l => l.Start.X), lines.Max(l => l.End.X)) + 1;
            int height = Math.Max(lines.Max(l => l.Start.Y), lines.Max(l => l.End.Y)) + 1;
            int[,] board = new int[height, width];

            foreach (Line line in lines)
            {
                foreach (Point p in line.Points)
                    board[p.Y, p.X]++;
            }

            int nrOfValuesLargerThanOne = board.Cast<int>().Count(i => i > 1);
            return nrOfValuesLargerThanOne;
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