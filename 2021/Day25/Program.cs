using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day25
{
    public struct Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IEnumerable<Position> AllNeighbours
        {
            get
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                        yield return new Position(this.X + x, this.Y + y);
                }
            }
        }

        public bool IsInBounds<TData>(Board<TData> board)
        {
            return X >= 0 && X < board.Width && Y >= 0 && Y < board.Height;
        }

        public Position DisplaceWithWrap<TData>(Board<TData> board, int deltaX, int deltaY)
        {
            int x = (X + deltaX + board.Width) % board.Width;
            int y = (Y + deltaY + board.Height) % board.Height;
            return new Position(x, y);
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

        protected TData[,] _cells;

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

        protected Board(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new TData[height, width];
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

    public class Seafloor : Board<char>
    {
        public Seafloor(int width, int height, char initialValue = '.')
            : base(width, height, initialValue)
        {
            FormatCell = (c => "" + c);
        }

        public Seafloor(List<IEnumerable<char>> cellsPerRow)
            : base(cellsPerRow)
        {
        }

        public IEnumerable<Position> GetAllCucumberPositionsOf(char c)
        {
            return Positions.Where(pos => this[pos] == c);
        }

        public Seafloor Copy(params char[] include)
        {
            Seafloor result = new Seafloor(Width, Height);
            result.FormatCell = this.FormatCell;

            char c = include.Any() ? include.First() : 'X';
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (_cells[y, x] == c)
                        result._cells[y, x] = c;
                    else
                        result._cells[y, x] = '.';
                }
            }

            foreach (char c2 in include.Skip(1))
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (_cells[y, x] == c2)
                            result._cells[y, x] = c2;
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/25
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"*** Program for {typeof(Program).FullName.Split('.').First()} ***");

            int result = Part1();
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static int Part1()
        {
            List<string> textLines = ReadInput("");

            Stopwatch stopwatch = Stopwatch.StartNew();
            Seafloor originalFloor = new Seafloor(textLines
                .Select(line => line.Cast<char>())
                .ToList());

            for (int step = 1; step <= 1000; step++)
            {
                Seafloor nextFloor = originalFloor.Copy();
                int cucumbersMoved = 0;

                foreach (Position pos in originalFloor.GetAllCucumberPositionsOf('>'))
                {
                    Position nextPosition = pos.DisplaceWithWrap(originalFloor, 1, 0);
                    if (originalFloor[nextPosition] == '.')
                        cucumbersMoved++;
                    else
                        nextPosition = pos;

                    nextFloor[nextPosition] = '>';
                }

                foreach (Position pos in originalFloor.GetAllCucumberPositionsOf('v'))
                {
                    Position nextPosition = pos.DisplaceWithWrap(originalFloor, 0, 1);
                    if (originalFloor[nextPosition] != 'v' && nextFloor[nextPosition] != '>')
                        cucumbersMoved++;
                    else
                        nextPosition = pos;

                    nextFloor[nextPosition] = 'v';
                }

                if(cucumbersMoved == 0)
                {
                    Console.WriteLine($"No cucumbers moved after {step} steps:");
                    Console.WriteLine(nextFloor);

                    Console.WriteLine($"Elapsed time is {stopwatch.ElapsedMilliseconds}ms.");
                    return step;
                }

                originalFloor = nextFloor;
            }

            return 0;
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
