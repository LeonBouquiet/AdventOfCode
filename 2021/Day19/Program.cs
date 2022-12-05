using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day19
{
    public class Point3D
    {
        public int X => Coords[0];
        public int Y => Coords[1];
        public int Z => Coords[2];

        public int[] Coords { get; private set; }

        public Point3D(int x, int y, int z)
        {
            Coords = new int[] { x, y, z };
        }

        public override bool Equals(object obj)
        {
            Point3D other = obj as Point3D;
            if (other == null)
                return false;

            return (this.X == other.X && this.Y == other.Y && this.Z == other.Z);
        }

        public override int GetHashCode()
        {
            return X + Y * 1013 + Z * 1013029;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public int ManhattanDistanceTo(Point3D other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public double DistanceTo(Point3D other)
        {
            return Math.Pow((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z), 1 / 3d);
        }
    }

    public class Translation: Point3D
    {
        public static readonly Translation None = new Translation(0, 0, 0);

        public Translation(int x, int y, int z):
            base(x, y, z)
        {
        }

        public Translation Clone()
        {
            return new Translation(X, Y, Z);
        }

        public Point3D Translate(Point3D p)
        {
            return new Point3D(p.X + this.X, p.Y + this.Y, p.Z + this.Z);
        }

    }

    public class Scanner
    {
        public int Id { get; private set; }

        public string RotationDescription { get; private set; }

        public Translation Translation { get; set; }

        public List<Point3D> Beacons { get; private set; }

        public List<int> Fingerprint { get; set; }

        public Scanner(int id)
        {
            Id = id;
            Beacons = new List<Point3D>();
        }

        public List<int> CalculateFingerprint()
        {
            //When sorted on X, Y and Z coords, calculate all distances between each consecutive pair of points.
            //These distances are not affected by translations or orientations, and should be relatively characteristic:
            //another Scanner that shares 12 points with this one will have a lot of the same distances.
            List<int> result = new List<int>();
            result.AddRange(Beacons
                .OrderBy(p => p.X).ThenBy(p => p.Y)
                .SubsequentPairs()
                .Select(pair => (int)(pair.Item1.DistanceTo(pair.Item2) * 100)));
            result.AddRange(Beacons
                .OrderBy(p => p.Y).ThenBy(p => p.Z)
                .SubsequentPairs()
                .Select(pair => (int)(pair.Item1.DistanceTo(pair.Item2) * 100)));
            result.AddRange(Beacons
                .OrderBy(p => p.Z).ThenBy(p => p.X)
                .SubsequentPairs()
                .Select(pair => (int)(pair.Item1.DistanceTo(pair.Item2) * 100)));

            result.Sort();
            return result;
        }

        public static List<Scanner> SortOnMatchingFingerprint(Scanner target, IEnumerable<Scanner> others)
        {
            var inbetween = others.Select(other => new Tuple<int, Scanner>(target.Fingerprint.Intersect(other.Fingerprint).Count(), other))
                .OrderByDescending(pair => pair.Item1)
                .ToList();
            List<Scanner> result = inbetween
                .Select(pair => pair.Item2)
                .ToList();

            return result;
        }

        public Scanner TryToFindScannerWithNPointsOverlap(IEnumerable<Scanner> others, int nrOfPoints)
        {
            //When prioritized, the first match is always the right one!
            foreach(Scanner other in others.Take(1))
            {
                Scanner result = TryToFindTransformationWithNPointsOverlap(other, nrOfPoints);
                if (result != null)
                    return result;
            }

            return null;
        }

        public Scanner TryToFindTransformationWithNPointsOverlap(Scanner other, int nrOfPoints)
        {
            foreach(Rotation rotation in Rotation.AllRotations)
            {
                Scanner rotatedOther = other.Rotate(rotation);
                Scanner result = TryToFindTranslationWithNPointsOverlap(rotatedOther, nrOfPoints);
                if (result != null)
                    return result;
            }

            return null;
        }

        public Scanner TryToFindTranslationWithNPointsOverlap(Scanner other, int nrOfPoints)
        {
            foreach(Point3D candidate in this.Beacons.Skip(nrOfPoints - 1))
            {
                foreach(Point3D otherCandidate in other.Beacons.Skip(nrOfPoints - 1))
                {
                    Translation translation = new Translation(candidate.X - otherCandidate.X, candidate.Y - otherCandidate.Y, candidate.Z - otherCandidate.Z);
                    Scanner translatedOther = other.Translate(translation);
                    if (this.HasNPointsInCommonWith(translatedOther, nrOfPoints))
                        return translatedOther;
                }
            }

            return null;
        }

        public bool HasNPointsInCommonWith(Scanner other, int nrOfPoints)
        {
            HashSet<Point3D> otherPoints = other.Beacons.ToHashSet();
            int pointsInCommon = 0;

            for (int index = 0; index < this.Beacons.Count; index++)
            {
                if (otherPoints.Contains(this.Beacons[index]))
                    pointsInCommon++;

                int pointsToEvaluate = this.Beacons.Count - index;
                if (nrOfPoints - pointsInCommon > pointsToEvaluate)
                    break;
            }

            return (pointsInCommon >= nrOfPoints);
        }

        public Scanner Rotate(Rotation rotation)
        {
            Scanner result = new Scanner(this.Id);
            result.RotationDescription = rotation.Description;
            result.Translation = this.Translation.Clone();
            result.Fingerprint = this.Fingerprint;
            result.Beacons = this.Beacons
                .Select(p => rotation.Rotate(p))
                .ToList();

            return result;
        }

        public Scanner Translate(Translation translation)
        {
            Scanner result = new Scanner(this.Id);
            result.RotationDescription = this.RotationDescription;
            result.Translation = translation.Clone();
            result.Fingerprint = this.Fingerprint;
            result.Beacons = this.Beacons
                .Select(p => translation.Translate(p))
                .ToList();

            return result;
        }


        public override string ToString()
        {
            return $"Scanner {Id} ({Beacons.Count} beacons)";
        }
    }

    public class Rotation
    {
        public static readonly List<Rotation> AllRotations =
                ("( X, Y,-Z)|( X, Z,-Y)|( X,-Y,-Z)|( X,-Z, Y)|( Y, X,-Z)|( Y, Z, X)|( Y,-X, Z)|( Y,-Z,-X)|( Z, X, Y)|( Z, Y,-X)|( Z,-X,-Y)|( Z,-Y, X)|" +
                 "(-X, Y,-Z)|(-X, Z, Y)|(-X,-Y, Z)|(-X,-Z,-Y)|(-Y, X, Z)|(-Y, Z,-X)|(-Y,-X,-Z)|(-Y,-Z, X)|(-Z, X,-Y)|(-Z, Y, X)|(-Z,-X, Y)|(-Z,-Y,-X)")
            .Split('|')
            .Select(desc => new Rotation(desc))
            .ToList();

        public int[] AxisMapping;

        public int[] Signs;

        // E.g. "(-Z, Y, X)"
        public string Description;

        public Rotation(string description)
        {
            Description = description;

            AxisMapping = new int[3];
            AxisMapping[0] = (description[2] - 'X');
            AxisMapping[1] = (description[5] - 'X');
            AxisMapping[2] = (description[8] - 'X');
            Signs = new int[3];
            Signs[0] = (description[1] == '-' ? -1 : 1);
            Signs[1] = (description[4] == '-' ? -1 : 1);
            Signs[2] = (description[7] == '-' ? -1 : 1);
        }

        public Point3D Rotate(Point3D p)
        {
            Point3D result = new Point3D(Signs[AxisMapping[0]] * p.Coords[AxisMapping[0]], Signs[AxisMapping[1]] * p.Coords[AxisMapping[1]], Signs[AxisMapping[2]] * p.Coords[AxisMapping[2]]);
            return result;
        }

        public override string ToString()
        {
            return Description;
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/19
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
            List<string> textLines = ReadInput(".");
            
            int cursor = 0;
            List<Scanner> scanners = new List<Scanner>();
            while (cursor < textLines.Count)
                scanners.Add(Parse(textLines, ref cursor));

            //foreach (Scanner knownScanner in scanners)
            //{
            //    Scanner other = knownScanner.TryToFindScannerWithNPointsOverlap(scanners.Where(sc => sc.Id != knownScanner.Id), 12);
            //    if (other != null)
            //        Console.WriteLine($"Matched scanner {knownScanner.Id} to {other.Id} with translation {other.TranslationDescription} (under rotation {other.RotationDescription}).");
            //    else
            //        Console.WriteLine($"Couldn't match {knownScanner.Id}.");
            //}

            //Scanner knownScanner = scanners[2];
            //Scanner other = knownScanner.TryToFindScannerWithNPointsOverlap(scanners.Where(sc => sc.Id != knownScanner.Id), 12);


            List<Scanner> scannersWithKnownPositionAndOrientation = new List<Scanner>();
            scannersWithKnownPositionAndOrientation.Add(scanners[0]);
            scanners.RemoveAt(0);

            HashSet<Point3D> distinctPoints = new HashSet<Point3D>(scannersWithKnownPositionAndOrientation.SelectMany(sc => sc.Beacons));
            for (int iteration = 0; iteration < scannersWithKnownPositionAndOrientation.Count; iteration++)
            {
                List<Scanner> matchesThisIteration = new List<Scanner>();

                Scanner knownScanner = scannersWithKnownPositionAndOrientation[iteration];
                Scanner other = null;
                do
                {
                    other = knownScanner.TryToFindScannerWithNPointsOverlap(scanners, 12);
                    if (other != null)
                    {
                        other.Beacons
                            .Where(p => distinctPoints.Contains(p) == false)
                            .ToList()
                            .ForEach(p => distinctPoints.Add(p));

                        matchesThisIteration.Add(other);
                        scanners.RemoveAll(sc => sc.Id == other.Id);

                        Console.WriteLine($"Matched scanner {knownScanner.Id} to {other.Id} with translation {other.Translation} (under rotation {other.RotationDescription}).");
                    }
                }
                while (other != null);

                scannersWithKnownPositionAndOrientation.AddRange(matchesThisIteration);
            }

            //var transformation = scanners[0].TryToFindTransformationWithNPointsOverlap(scanners[1], 12);

            return distinctPoints.Count;
        }

        public static int Part2()
        {
            List<string> textLines = ReadInput(".");
            Stopwatch stopwatch = Stopwatch.StartNew();

            int cursor = 0;
            List<Scanner> scanners = new List<Scanner>();
            while (cursor < textLines.Count)
                scanners.Add(Parse(textLines, ref cursor));

            List<Scanner> scannersWithKnownPositionAndOrientation = new List<Scanner>();
            scannersWithKnownPositionAndOrientation.Add(scanners[0]);
            scanners.RemoveAt(0);

            for (int iteration = 0; iteration < scannersWithKnownPositionAndOrientation.Count; iteration++)
            {
                List<Scanner> matchesThisIteration = new List<Scanner>();

                Scanner knownScanner = scannersWithKnownPositionAndOrientation[iteration];
                List<Scanner> prioritized = Scanner.SortOnMatchingFingerprint(knownScanner, scanners);

                Scanner other = null;
                do
                {
                    other = knownScanner.TryToFindScannerWithNPointsOverlap(prioritized, 12);
                    if (other != null)
                    {
                        matchesThisIteration.Add(other);
                        scanners.RemoveAll(sc => sc.Id == other.Id);
                        prioritized.RemoveAll(sc => sc.Id == other.Id);

                        Console.WriteLine($"Matched scanner {knownScanner.Id} to {other.Id} with translation {other.Translation} (under rotation {other.RotationDescription}).");
                    }
                }
                while (other != null);

                scannersWithKnownPositionAndOrientation.AddRange(matchesThisIteration);
            }

            int maxManhattanDistance = 0;
            foreach (Scanner outer in scannersWithKnownPositionAndOrientation)
            {
                foreach(Scanner inner in scannersWithKnownPositionAndOrientation.Where(sc => sc.Id != outer.Id))
                {
                    maxManhattanDistance = Math.Max(maxManhattanDistance, outer.Translation.ManhattanDistanceTo(inner.Translation));
                }
            }

            Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds}ms.");
            return maxManhattanDistance;
        }

        private static readonly Regex ScannerHeader = new Regex(@"--- scanner (\d+) ---");

        public static Scanner Parse(List<string> lines, ref int cursor)
        {
            Match match = ScannerHeader.Match(lines[cursor++]);
            Debug.Assert(match.Success);
            Scanner result = new Scanner(Int32.Parse(match.Groups[1].Value));
            result.Translation = Translation.None;

            for(; cursor < lines.Count; cursor++)
            {
                if (lines[cursor] == "")
                    break;

                int[] coords = lines[cursor]
                    .Split(",")
                    .Select(s => Int32.Parse(s))
                    .ToArray();

                result.Beacons.Add(new Point3D(coords[0], coords[1], coords[2]));
            }

            result.Fingerprint = result.CalculateFingerprint();

            cursor++;
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

    public static class Extension
    {
        /// </summary>
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
