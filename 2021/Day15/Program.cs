using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day15
{
    public class Position: IEquatable<Position>
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

        private static (int, int)[] AllNeighboursDisplacements = new (int, int)[]
        {
            ( 0, -1), (-1, -1), (-1,  0), (-1,  1), ( 0,  1), ( 1,  1), ( 1,  0), ( 1, -1),
        };


        public IEnumerable<Position> AllNeighbours
        {
            get
            {
                return AllNeighboursDisplacements
                    .Select(disp => new Position(this.X + disp.Item1, this.Y + disp.Item2));
            }
        }

        public IEnumerable<Position> DirectNeighbours
        {
            get
            {
                return AllNeighboursDisplacements
                    .Where(disp => disp.Item1 == 0 || disp.Item2 == 0)
                    .Select(disp => new Position(this.X + disp.Item1, this.Y + disp.Item2));
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

        public void Copy(Board<TData> source, Position targetPos, Func<TData, TData> transform)
        {
            foreach(Position sourcePos in source.Positions)
            {
                this[new Position(targetPos.X + sourcePos.X, targetPos.Y + sourcePos.Y)] = transform(source[sourcePos]);
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

    public class PathNode: IComparable<PathNode>
    {
        public PathNode Parent { get; private set; }

        public Position Position { get; private set; }

        public int DistanceFromStart { get; private set; }

        public int DistanceToGoalEstimate { get; private set; }

        public PathNode(PathNode parent, Position position, int distanceFromStart, int distanceToGoalEstimate)
        {
            Parent = parent;
            Position = position;
            DistanceFromStart = distanceFromStart;
            DistanceToGoalEstimate = distanceToGoalEstimate;
        }

        public int CompareTo(PathNode other)
        {
            int diff = (this.DistanceFromStart + this.DistanceToGoalEstimate) - (other.DistanceFromStart + other.DistanceToGoalEstimate);
            if (diff == 0)
                diff = this.GetHashCode() - other.GetHashCode();

            return diff;
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

            //PathNode n1 = new PathNode(null, new Position(0, 0), 0, 3);
            //PathNode n2 = new PathNode(null, new Position(0, 0), 0, 3);
            //bool b = n2.Equals(n1);

            //SortedSet<PathNode> ss = new SortedSet<PathNode>();
            //b = ss.Add(n1);
            //b = ss.Add(n2);

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
            Board.FormatCell = (cell => cell.ToString());

            int result = FindPath();
            return result;
        }

        public static int Part2()
        {
            List<string> textLines = ReadInput();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var cellsPerRow = textLines
                .Select(line => line.Select(c => Int32.Parse("" + c)))
                .ToList();

            Board<int> tile = new Board<int>(cellsPerRow);
            
            Board = new Board<int>(tile.Width * 5, tile.Height * 5);
            Board.FormatCell = (cell => cell.ToString());

            int[,] increments = new int[,]
            {
                { 0, 1, 2, 3, 4 },
                { 1, 2, 3, 4, 5 },
                { 2, 3, 4, 5, 6 },
                { 3, 4, 5, 6, 7 },
                { 4, 5, 6, 7, 8 },
            };

            

            for(int y = 0; y < 5; y++)
            {
                for(int x = 0; x < 5; x++)
                {
                    Board.Copy(tile, new Position(x * tile.Width, y * tile.Height), i => (((i - 1) + increments[y, x]) % 9) + 1);
                }
            }

            int result = FindPath();

            stopwatch.Stop();
            Console.WriteLine($"Duration: {stopwatch.ElapsedMilliseconds}ms");

            return result;
        }

        private static int FindPath()
        {
            PathNode startNode = new PathNode(null, new Position(0, 0), 0, Board.Width + Board.Height - 2);
            Position goalPosition = new Position(Board.Width - 1, Board.Height - 1);

            HashSet<Position> reached = new HashSet<Position>();
            SortedSet<PathNode> frontier = new SortedSet<PathNode>();
            frontier.Add(startNode);

            while (frontier.Any())
            {
                PathNode current = frontier.Min;
                frontier.Remove(current);

                if (current.Position.Equals(goalPosition))
                {
                    List<PathNode> path = new List<PathNode>();

                    for (var node = current; node != null; node = node.Parent)
                        path.Add(node);

                    path.Reverse();

                    return current.DistanceFromStart;
                }

                if (reached.Contains(current.Position) == false)
                {
                    reached.Add(current.Position);

                    foreach (Position neighbour in current.Position.DirectNeighbours.Where(pos => pos.IsInBounds(Board)))
                    {
                        if (reached.Contains(neighbour) == false)
                        {
                            PathNode node = new PathNode(
                                parent: current,
                                position: neighbour,
                                distanceFromStart: current.DistanceFromStart + Board[neighbour],
                                distanceToGoalEstimate: (Board.Width - neighbour.X - 1) + (Board.Height - neighbour.Y - 1));

                            frontier.Add(node);
                        }
                    }
                }
            }

            return -1;
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
