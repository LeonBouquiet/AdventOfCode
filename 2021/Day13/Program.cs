using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day13
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

        public void FoldAlongX(int foldX, Func<TData, TData, TData> combineFunc)
        {
            int maxX = Math.Min(foldX, Width - foldX - 1);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 1; x <= maxX; x++)
                {
                    _cells[y, foldX - x] = combineFunc(_cells[y, foldX - x], _cells[y, foldX + x]);
                    _cells[y, foldX + x] = default(TData);
                }
            }
        }

        public void FoldAlongY(int foldY, Func<TData, TData, TData> combineFunc)
        {
            int maxY = Math.Min(foldY, Height - foldY - 1);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 1; y <= maxY; y++)
                {
                    _cells[foldY - y, x] = combineFunc(_cells[foldY - y, x], _cells[foldY + y, x]);
                    _cells[foldY + y, x] = default(TData);
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
    /// See https://adventofcode.com/2021/day/13
    /// </summary>
    public class Program
    {
        public static Board<bool> Board;

        public static void Main(string[] args)
        {
            Console.WriteLine($"*** Program for {typeof(Program).FullName.Split('.').First()} ***");

            int result = Part1();
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static readonly Regex InstructionRegex = new Regex(@"fold along (x|y)=(\d+)");

        public static int Part1()
        {
            List<string> textLines = ReadInput(delimiter: ".");
            IEnumerable<string> positionLines = textLines
                .TakeWhile(line => line != "");

            List<Position> positions = positionLines
                .Select(line => line.Trim().Split(','))
                .Select(arr => new Position(Int32.Parse(arr[0]), Int32.Parse(arr[1])))
                .ToList();

            int width = positions.Max(pos => pos.X) + 1;
            int height = positions.Max(pos => pos.Y) + 1;
            Board = new Board<bool>(width, height);
            Board.FormatCell = (b => b ? "#" : ".");
            positions.ForEach(pos => Board[pos] = true);

            IEnumerable<string> instructionLines = textLines
                .SkipWhile(line => line != "")
                .Skip(1);
            List<(char, int)> instructions = instructionLines
                .Select(line => InstructionRegex.Match(line))
                .Select(match => (match.Groups[1].Value[0], Int32.Parse(match.Groups[2].Value)))
                .ToList();

            Func<bool, bool, bool> combineFunc = ((b1, b2) => b1 || b2);
            foreach ((char, int) instruction in instructions)
            {
                switch (instruction.Item1)
                {
                    case 'x':
                        Board.FoldAlongX(instruction.Item2, combineFunc);
                        break;
                    case 'y':
                        Board.FoldAlongY(instruction.Item2, combineFunc);
                        break;
                }
            }

            int result = Board.Positions.Count(pos => Board[pos]);
            return result;
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
