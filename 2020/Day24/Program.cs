using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day24
{
    public enum WindDirection
    {
        E, SE, SW, W, NW, NE
    }

    public class Position
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static WindDirection[] AllDirections = 
            new WindDirection[] { WindDirection.E, WindDirection.SE, WindDirection.SW, WindDirection.W, WindDirection.NW, WindDirection.NE };

        public IEnumerable<Position> NeighbouringPositions
        {
            get
            {
                foreach(WindDirection wd in AllDirections)
                    yield return this.Displace(wd);
            }
        }

        public Position Displace(WindDirection windDirection)
        {
            int hx = Math.Abs(this.Y % 2);
            switch (windDirection)
            {
                case WindDirection.E:
                    return new Position(this.X + 1, this.Y);
                case WindDirection.SE:
                    return new Position(this.X + hx, this.Y + 1);
                case WindDirection.SW:
                    return new Position(this.X - (1 - hx), this.Y + 1);
                case WindDirection.W:
                    return new Position(this.X - 1, this.Y);
                case WindDirection.NW:
                    return new Position(this.X - (1 - hx), this.Y - 1);
                case WindDirection.NE:
                    return new Position(this.X + hx, this.Y - 1);
                default:
                    throw new ArgumentException();
            }
        }

        public override bool Equals(object obj)
        {
            Position other = obj as Position;
            if (other == null)
                return false;

            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X * 1137 + Y;
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

        public static int Part1()
        {
            List<string> textLines = ReadInput();

            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;
            foreach (string textLine in textLines)
            {
                Position position = new Position(0, 0);
                Tokenize(textLine).ForEach(wd => position = position.Displace(wd));

                minX = Math.Min(minX, position.X);
                minY = Math.Min(minY, position.Y);
                maxX = Math.Max(maxX, position.X);
                maxY = Math.Max(maxY, position.Y);
            }

            int displacementX = minX;
            int displacementY = minY;
            int[,] tiles = new int[maxY - minY + 1, maxX - minX + 1];

            foreach (string textLine in textLines)
            {
                Position position = new Position(0, 0);
                foreach (WindDirection wd in Tokenize(textLine))
                { 
                    position = position.Displace(wd);
                }

                tiles[position.Y - displacementY, position.X - displacementX]++;
            }

            int result = tiles.Cast<int>().Count(i => (i % 2) == 1);
            return result;
        }

        public static int Part2()
        {
            List<string> textLines = ReadInput();

            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;
            foreach (string textLine in textLines)
            {
                Position position = new Position(0, 0);
                Tokenize(textLine).ForEach(wd => position = position.Displace(wd));

                minX = Math.Min(minX, position.X);
                minY = Math.Min(minY, position.Y);
                maxX = Math.Max(maxX, position.X);
                maxY = Math.Max(maxY, position.Y);
            }

            minX -= 100;
            minY -= 100;
            maxX += 100;
            maxY += 100;

            //Create tiles grid 
            int displacementX = minX;
            int displacementY = minY;
            bool[,] tiles = new bool[maxY - minY + 1, maxX - minX + 1];

            List<Position> possiblyBlackTiles = new List<Position>();

            //Paint initial tiles
            foreach (string textLine in textLines)
            {
                Position position = new Position(0, 0);
                foreach (WindDirection wd in Tokenize(textLine))
                {
                    position = position.Displace(wd);
                }

                tiles[position.Y - displacementY, position.X - displacementX] = !tiles[position.Y - displacementY, position.X - displacementX];
                possiblyBlackTiles.Add(position);
            }

            List<Position> blackTiles = possiblyBlackTiles
                .Where(pos => tiles[pos.Y - displacementY, pos.X - displacementX] == true)
                .ToList();

            Console.WriteLine($"Day 0: we start with {blackTiles.Count} black tiles.");

            for (int day = 1; day <= 100; day++)
            {
                //Determine which black tiles should be flipped to white because they have 0 or more than 2 black tiles next to it.
                List<Position> flipToWhite = blackTiles
                    .Where(pos => 
                    { 
                        int blackCount = pos.NeighbouringPositions.Count(nb => tiles[nb.Y - displacementY, nb.X - displacementX] == true);
                        return blackCount == 0 || blackCount > 2;
                    })
                    .ToList();

                //Next, get the white positions close to the black ones, and determine which should be flipped to black because it has 2 black tiles next to it.
                List<Position> whiteNeighbours = blackTiles
                    .SelectMany(pos => pos.NeighbouringPositions)
                    .Distinct()
                    .Where(pos => tiles[pos.Y - displacementY, pos.X - displacementX] == false)
                    .ToList();
                List<Position> flipToBlack = whiteNeighbours
                    .Where(pos => pos.NeighbouringPositions.Count(nb => tiles[nb.Y - displacementY, nb.X - displacementX] == true) == 2)
                    .ToList();

                List<Position> overlap = flipToWhite.Intersect(flipToBlack).ToList();
                if (overlap.Any())
                    throw new ArgumentException("Overlap");

                //Now perform the flips
                flipToWhite.ForEach(pos => tiles[pos.Y - displacementY, pos.X - displacementX] = false);
                flipToBlack.ForEach(pos => tiles[pos.Y - displacementY, pos.X - displacementX] = true);

                //We now have a new list of blackTiles.
                blackTiles = blackTiles
                    .Where(pos => tiles[pos.Y - displacementY, pos.X - displacementX] == true)
                    .Concat(flipToBlack)
                    .ToList();

                Console.WriteLine($"On Day {day}: {blackTiles.Count} black tiles.");
            }

            int result = tiles.Cast<bool>().Count(b => b == true);
            return result;
        }

        private static readonly Regex WindDirectionRegex = new Regex("(e|se|sw|w|nw|ne)");

        private static List<WindDirection> Tokenize(string textLine)
        {
            List<WindDirection> result = new List<WindDirection>();

            int currentPos = 0;
            do
            {
                Match match = WindDirectionRegex.Match(textLine, currentPos);
                if (match.Success == false)
                    throw new ArgumentException($"Regex didn't match on position {currentPos}.");

                result.Add(Enum.Parse<WindDirection>(match.Groups[1].Value, ignoreCase: true));

                currentPos += match.Groups[1].Value.Length;
            }
            while (currentPos < textLine.Length);

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
