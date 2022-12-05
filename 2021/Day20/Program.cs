using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day20
{
    public class Position : IEquatable<Position>
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position other)
        {
            if (other == null)
                return false;

            return (this.X == other.X && this.Y == other.Y);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Position);
        }

        public override int GetHashCode()
        {
            return Y * 1137 + X;
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

        public void Resize(int width, int height, int offsetX, int offsetY)
        {
            TData[,] newCells = new TData[height, width];
            if (width >= Width && height >= Height)
            {
                foreach (Position sourcePos in this.Positions)
                {
                    newCells[sourcePos.Y + offsetY, sourcePos.X + offsetX] = _cells[sourcePos.Y, sourcePos.X];
                }

                _cells = newCells;
                Width = width;
                Height = height;
            }
            else
            {
                TData[,] oldCells = _cells;
                _cells = newCells;
                Width = width;
                Height = height;

                foreach (Position sourcePos in this.Positions)
                {
                    _cells[sourcePos.Y, sourcePos.X] = oldCells[sourcePos.Y + offsetY, sourcePos.X + offsetX];
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

    public class Image: Board<int>
    {
        public Image(int width, int height, int initialValue = 0)
            : base(width, height, initialValue)
        {
        }

        public Image(List<IEnumerable<int>> cellsPerRow): base(cellsPerRow)
        {
        }


        public int GetThreeByThreeValue(Position position)
        {
            int result = position.AllNeighbours
                .Select(pos => pos.IsInBounds(this) ? this[pos] : 0)
                .Aggregate(0, (acc, i) => (acc << 1) + i);

            return result;
        }

        public void SmoothBorders()
        {
            for (int x = 0; x < Width; x++)
            {
                _cells[0, x] = _cells[1, 1];
                _cells[Height - 1, x] = _cells[Height - 2, 1];
            }

            for(int y = 0; y < Height; y++)
            {
                _cells[y, 0] = _cells[1, 1];
                _cells[y, Width - 1] = _cells[1, Height - 2];
            }
        }
    }


    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/20
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

        public static int Part2()
        {
            List<string> textLines = ReadInput(".");

            Stopwatch stopwatch = Stopwatch.StartNew();

            List<int> imageEnhancement = textLines
                .First()
                .Select(c => c == '#' ? 1 : 0)
                .ToList();

            Image source = new Image(textLines
                .Skip(2)
                .Select(line => line.Select(c => c == '#' ? 1 : 0))
                .ToList());
            source.FormatCell = (i => i == 1 ? "#" : ".");

            int maxSteps = 50;
            source.Resize(source.Width + 2 * maxSteps + 10, source.Height + 2 * maxSteps + 10, maxSteps + 5, maxSteps + 5);
            
            for (int step = 1; step <= maxSteps; step++)
            {
                Image target = new Image(source.Width, source.Height);
                target.FormatCell = source.FormatCell;

                foreach (Position position in source.Positions)
                {
                    int threeByThree = source.GetThreeByThreeValue(position);
                    int value = imageEnhancement[threeByThree];
                    target[position] = value;
                }

                target.SmoothBorders();
                source = target;
            }

            int result = source.Positions.Count(pos => source[pos] == 1);
            Console.WriteLine($"Elapsed time is {stopwatch.ElapsedMilliseconds}ms.");
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
