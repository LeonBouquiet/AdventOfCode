using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day09
{
    public class Position
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            Position other = obj as Position;
            if (other == null)
                return false;

            return (this.X == other.X && this.Y == other.Y);
        }

        public override int GetHashCode()
        {
            return Y * 1137 + X;
        }

        public static Position[] NeighbourDisplacements = new Position[]
        {
            new Position(-1, 0), new Position(0, -1), new Position(0, 1), new Position(1, 0)
        };

        public IEnumerable<Position> Neighbours
        {
            get
            {
                return NeighbourDisplacements.Select(nb => new Position(this.X + nb.X, this.Y + nb.Y));
            }
        }

        public bool IsInBounds
        {
            get
            {
                return X >= 0 && X < Program.Width && Y >= 0 && Y < Program.Height;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }


    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/
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

        public static int Width;
        public static int Height;
        public static int[,] Board;


        public static int Part1()
        {
            List<string> textLines = ReadInput();

            Width = textLines[0].Length;
            Height = textLines.Count;
            Board = new int[Height, Width];

            for(int y = 0; y < Height; y++)
            {
                string textLine = textLines[y];
                for(int x = 0; x < Width; x++)
                {
                    Board[y, x] = Int32.Parse("" + textLine[x]);
                }
            }

            List<Position> lowPoints = new List<Position>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Position position = new Position(x, y);
                    int lowerNeighbourCount = position.Neighbours
                        .Where(nb => nb.IsInBounds)
                        .Count(nb => (Board[y, x] >= Board[nb.Y, nb.X]));

                    if (lowerNeighbourCount == 0)
                        lowPoints.Add(position);
                }
            }

            List<int> riskLevels = lowPoints
                .Select(pos => Board[pos.Y, pos.X] + 1)
                .ToList();

            int result = riskLevels.Sum();
            return result;
        }

        public static int Part2()
        {
            List<string> textLines = ReadInput();

            Width = textLines[0].Length;
            Height = textLines.Count;
            Board = new int[Height, Width];

            for (int y = 0; y < Height; y++)
            {
                string textLine = textLines[y];
                for (int x = 0; x < Width; x++)
                {
                    Board[y, x] = Int32.Parse("" + textLine[x]);
                }
            }

            List<Position> lowPoints = new List<Position>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Position position = new Position(x, y);
                    int lowerNeighbourCount = position.Neighbours
                        .Where(nb => nb.IsInBounds)
                        .Count(nb => (Board[y, x] >= Board[nb.Y, nb.X]));

                    if (lowerNeighbourCount == 0)
                        lowPoints.Add(position);
                }
            }

            List<int> basinSizes = new List<int>();
            foreach(Position lowPoint in lowPoints)
            {
                Dictionary<Position, Position> explored = new Dictionary<Position, Position>();
                FloodFill(lowPoint, explored);

                basinSizes.Add(explored.Count);
            }

            basinSizes.Sort();
            basinSizes.Reverse();

            int result = basinSizes[0] * basinSizes[1] * basinSizes[2];
            return result;
        }

        private static void FloodFill(Position position, Dictionary<Position, Position> explored)
        {
            explored.Add(position, position);

            foreach(Position neighbour in position.Neighbours)
            {
                if (neighbour.IsInBounds == false || explored.ContainsKey(neighbour))
                    continue;

                if (Board[neighbour.Y, neighbour.X] != 9)
                    FloodFill(neighbour, explored);
            }
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
