using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day11
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

        private static (int, int)[] AllNeighboursDisplacements = new (int, int)[]
        {
            ( 0, -1), (-1, -1), (-1,  0), (-1,  1), ( 0,  1), ( 1,  1), ( 1,  0), ( 1, -1),
        };


        public IEnumerable<Position> AllNeighbours
        {
            get
            {
                return AllNeighboursDisplacements.Select(disp => new Position(this.X + disp.Item1, this.Y + disp.Item2));
            }
        }

        public bool IsInBounds
        {
            get
            {
                return X >= 0 && X < Program.Board.Width && Y >= 0 && Y < Program.Board.Height;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public class Board<TData>
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        private TData[,] _cells;

        public TData this[Position pos]
        {
            get { return _cells[pos.Y, pos.X]; }
            set { _cells[pos.Y, pos.X] = value; }
        }

        public IEnumerable<Position> Positions
        {
            get
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                        yield return new Position(x, y);
                }
            }
        }

        public Board(int width, int height, TData initialValue = default(TData))
        {
            Width = width;
            Height = height;
            _cells = new TData[height, width];

            Positions.ToList().ForEach(pos => this[pos] = initialValue);
        }

        public Board(List<IEnumerable<TData>> cellsPerRow)
        {
            Width = cellsPerRow.First().Count();
            Height = cellsPerRow.Count;
            _cells = new TData[Height, Width];

            for (int y = 0; y < Height; y++)
            {
                IEnumerable<TData> row = cellsPerRow[y];
                int x = 0;
                foreach (TData cell in row)
                {
                    _cells[y, x++] = cell;
                }
            }
        }

        public Func<TData, string> FormatCell { get; set; }

        public string Display
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            Func<TData, string> formatCell = FormatCell ?? (c => c.ToString());
            StringBuilder sbResult = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sbResult.Append(formatCell(_cells[y, x]));
                }

                sbResult.AppendLine();
            }

            return sbResult.ToString();
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/XX
    /// </summary>
    public class Program
    {
        public static Board<int> Board;

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
            List<string> textLines = ReadInput();
            var cellsPerRow = textLines
                .Select(line => line.Select(c => Int32.Parse("" + c)))
                .ToList();

            Board = new Board<int>(cellsPerRow);
            Board.FormatCell = (cell => cell == 10 ? "*" : cell.ToString());

            int flashCount = 0;
            for (int step = 1; step <= 100; step++)
            {
                foreach (Position pos in Board.Positions)
                {
                    IncrementPosition(pos);
                }

                Console.WriteLine(Board);

                flashCount += Board.Positions.Count(pos => Board[pos] == 10);
                Board.Positions.Where(pos => Board[pos] == 10).ToList().ForEach(pos => Board[pos] = 0);
            }

            return flashCount;
        }

        public static int Part2()
        {
            List<string> textLines = ReadInput();
            var cellsPerRow = textLines
                .Select(line => line.Select(c => Int32.Parse("" + c)))
                .ToList();

            Board = new Board<int>(cellsPerRow);
            Board.FormatCell = (cell => cell == 10 ? "*" : cell.ToString());

            for (int step = 1; step <= 10000; step++)
            {
                foreach (Position pos in Board.Positions)
                {
                    IncrementPosition(pos);
                }

                Console.WriteLine(Board);

                int flashCount = Board.Positions.Count(pos => Board[pos] == 10);
                if(flashCount == Board.Width * Board.Height)
                {
                    return step;
                }

                Board.Positions.Where(pos => Board[pos] == 10).ToList().ForEach(pos => Board[pos] = 0);
            }

            return -1;
        }

        private static void IncrementPosition(Position pos)
        {
            if(Board[pos] != 10 && ++Board[pos] > 9)
            {
                foreach (Position neighnour in pos.AllNeighbours.Where(nb => nb.IsInBounds))
                    IncrementPosition(neighnour);
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
