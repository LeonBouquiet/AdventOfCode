using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day20
{
    public enum Rotation
    {
        None = 0,
        Once = 1,
        Twice = 2,
        ThreeTimes = 3,
        Flip = 4,
        FlipAndOnce = 5,
        FlipAndTwice = 6,
        FlipAndThreeTimes = 7
    }

    public class Tile
    {
        public int Id { get; private set; }
        public string[] Data { get; private set; }

        public Rotation Rotation { get; private set; }

        public int[] Sides { get; private set; }

        public int North => Sides[0];
        public int East => Sides[1];
        public int South => Sides[2];
        public int West => Sides[3];

        public Tile(string[] lines)
        {
            Id = Int32.Parse(lines[0].Replace("Tile ", "").TrimEnd(':'));
            Rotation = Rotation.None;

            Data = lines.Skip(1).ToArray();

            Sides = new int[] {
                ParseAsBinary(Data[0]),
                ParseAsBinary(Data.Select(line => line[^1])),
                ParseAsBinary(Data[^1].Reverse()),
                ParseAsBinary(Data.Select(line => line[0]).Reverse())
            };
        }

        public Tile(Tile source, Rotation rotation)
        {
            Id = source.Id;
            Rotation = rotation;

            int rotations = ((int)rotation) % 4;
            Func<int, int> flipFunction = ((int)rotation >= ((int)Rotation.Flip))
                ? (i => ReverseBits(i))
                : (i => i);

            Sides = new int[] {
                flipFunction(source.Sides[(rotations + 0) % 4]),
                flipFunction(source.Sides[(rotations + 1) % 4]),
                flipFunction(source.Sides[(rotations + 2) % 4]),
                flipFunction(source.Sides[(rotations + 3) % 4])
            };
        }

        private static int ParseAsBinary(IEnumerable<char> chars)
        {
            int result = chars.Aggregate(0, (acc, c) => acc * 2 + (c == '#' ? 1 : 0));
            return result;
        }

        public static int ReverseBits(int binary)
        {
            int result = 0;
            for(int index = 0; index < 10; index++)
            {
                result = result * 2 + (binary & 0x01);
                binary >>= 1;
            }

            return result;
        }

        public Tile GetFlipped()
        {
            //Toggle the '4' bit
            Rotation flippedRotation = (Rotation)(((int)this.Rotation) ^ 4);
            return new Tile(this, flippedRotation);
        }

        public IEnumerable<Tile> GenerateAllRotations(bool includeFlips = true)
        {
            List<Tile> rotatedTiles = Enum.GetValues<Rotation>()
                .Where(rot => includeFlips == true || (((int)rot) & 4) == 0)
                .Select(rot => new Tile(this, rot))
                .ToList();

            return rotatedTiles;
        }

        public override string ToString()
        {
            return $"Id={Id}, Rotation={Rotation}, Sides=(N={North}, E={East}, S={South}, W={West})";
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

            long result = Part2();
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static long Part1()
        {
            List<string> textLines = ReadInput(".");
            List<Tile> tiles = ParseAsTiles(textLines);
            HashSet<int> candidateSides = tiles
                .SelectMany(t => new Tile[] { t, t.GetFlipped() })
                .SelectMany(t => t.Sides)
                .Select(i => Tile.ReverseBits(i))
                .ToHashSet();

            var groupedByNrOfSidesMatched = tiles
                .Select(t => new { 
                    Tile = t, 
                    CandidateSides = tiles
                        .Where(t2 => t2 != t)
                        .SelectMany(t2 => t2.GenerateAllRotations())
                        .SelectMany(r => r.Sides)
                        .ToHashSet() })
                .GroupBy(pair => pair.Tile.Sides.Count(i => pair.CandidateSides.Contains(i)), pair => pair.Tile.Id)
                .ToList();

            var cornerTileIds = groupedByNrOfSidesMatched
                .Where(grp => grp.Key == 2)
                .SelectMany(grp => grp)
                .ToList();
            long result = cornerTileIds
                .Select(i => (long)i)
                .Aggregate(1L, (acc, l) => acc *= l);
            return result;
        }

        public static long Part2()
        {
            List<string> textLines = ReadInput(".");
            List<Tile> tiles = ParseAsTiles(textLines);

            SideToTileMap = tiles
                .SelectMany(t => new Tile[] { t, t.GetFlipped() })
                .SelectMany(t => t.Sides.Select(i => new { Side = i, Tile = t }))
                .GroupBy(pair => pair.Side)
                .ToDictionary(grp => grp.Key, grp => grp.Select(pair => pair.Tile).ToArray());

            //Because we're not using backtracing, this algorithm only works if we can directly find the correct
            //neighbour of every Tile - that is, every internal Side must have exactly one other matching Side.
            //In this case, this means there are 4 * 12 = 48 border Sides to the image which shouldn't have a match,
            //and 11 * 12 + 12 * 11 = 264 internal Sides that have exactly one other matching Side.
            //Let's verify this - note that the SideToTileMap stores all Tile sides twice, so we expect double values here.
            Debug.Assert(SideToTileMap.Values.Count(v => v.Length == 1) == 48 * 2);
            Debug.Assert(SideToTileMap.Values.Count(v => v.Length == 2) == 264 * 2);

            TilesGroupedByNrOfSidesMatched = tiles
                .Select(t => new
                {
                    Tile = t,
                    CandidateSides = tiles
                        .Where(t2 => t2 != t)
                        .SelectMany(t2 => new Tile[] { t2, t2.GetFlipped() })
                        .SelectMany(r => r.Sides)
                        .ToHashSet()
                })
                .GroupBy(
                    keySelector: pair => pair.Tile.Sides.Count(i => pair.CandidateSides.Contains(i)),
                    elementSelector: pair => pair.Tile)
                .ToDictionary(grp => grp.Key, grp => grp.ToArray());

            Tile[,] image = ComposeImage(tiles);

            long result = 0;
            return result;
        }

        private static Dictionary<int, Tile[]> SideToTileMap;

        private static Dictionary<int, Tile[]> TilesGroupedByNrOfSidesMatched;

        private static Tile[,] ComposeImage(List<Tile> tiles)
        {
            //Take the first corner tile and orient it so that we can use it as the top-left piece (i.e. with the North
            //and West sides having no matching tile other that itself).
            Tile topLeftTile = TilesGroupedByNrOfSidesMatched[2]
                .Skip(2).First()
                .GenerateAllRotations(includeFlips: false)
                .Where(t => SideToTileMap[t.North].All(t2 => t2.Id == t.Id) && SideToTileMap[t.West].All(t2 => t2.Id == t.Id))
                .Single();

            Tile[,] image = new Tile[12, 12];
            image[0, 0] = topLeftTile;

            for(int y = 0; y < 12; y++)
            {
                for(int x = 0; x < 12; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    Tile tile = FindMatchingTile(image, x, y);
                    image[y, x] = tile;
                }
            }

            return image;
        }

        private static Tile FindMatchingTile(Tile[,] image, int x, int y)
        {
            Tile tileToTheWest = (x > 0) ? image[y, x - 1] : null;
            Tile tileToTheNorth = (y > 0) ? image[y - 1, x] : null;

            //Either the West or North side should have a known neighbouring tile.
            if(tileToTheWest != null)
            {
                //Except for the tileToTheWest, there has to be exactly one other tile with this Side.
                int sideRequirement = Tile.ReverseBits(tileToTheWest.East);
                Tile tile = SideToTileMap[sideRequirement]
                    .Where(t => t.Id != tileToTheWest.Id)
                    .Single();

                //Now get it rotated in the correct orientation - note that this could produce 2 results if the bits in
                //the Side are like a palindrome.
                List<Tile> rotated = tile
                    .GenerateAllRotations()
                    .Where(t => t.West == sideRequirement)
                    .ToList();

                return rotated.Single();
            }
            else
            {
                //Same story, only with tileToTheNorth.
                int sideRequirement = Tile.ReverseBits(tileToTheNorth.South);
                Tile tile = SideToTileMap[sideRequirement]
                    .Where(t => t.Id != tileToTheNorth.Id)
                    .Single();

                List<Tile> rotated = tile
                    .GenerateAllRotations()
                    .Where(t => t.North == sideRequirement)
                    .ToList();

                return rotated.Single();
            }
        }

        private static List<Tile> ParseAsTiles(IEnumerable<string> textLines)
        {
            List<Tile> tiles = new List<Tile>();
            List<string> currentBlock = new List<string>();

            foreach (string line in textLines)
            {
                if (line != string.Empty)
                {
                    currentBlock.Add(line);
                }
                else
                {
                    tiles.Add(new Tile(currentBlock.ToArray()));
                    currentBlock.Clear();
                }
            }

            if (currentBlock.Any())
                tiles.Add(new Tile(currentBlock.ToArray()));

            return tiles;
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

    public static class Extensions
    {
        public static TValue TryGetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultIfNotFound = default(TValue))
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            TValue found = default(TValue);
            if (dictionary.TryGetValue(key, out found))
                return found;
            else
                return defaultIfNotFound;
        }

        public static IEnumerable<(TElement, TElement)> SubsequentPairs<TElement>(this IEnumerable<TElement> elements)
        {
            if (elements.Skip(1).Any())
            {
                TElement previous = elements.First();
                foreach (TElement current in elements.Skip(1))
                {
                    yield return (previous, current);
                    previous = current;
                }
            }
        }
    }
}
