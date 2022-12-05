using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day22
{
    public enum Axis
    {
        X = 0, 
        Y = 1,
        Z = 2
    }


    public struct Projection
    {
        public int Start { get; private set; }
        public int End { get; private set; }

        public bool IsEmpty => (End - Start == 0);

        public static readonly Projection None = new Projection(int.MaxValue, int.MaxValue);

        public Projection(int start, int end)
        {
            Start = start;
            End = end;

            Debug.Assert(start <= end , "Projection is degenerate.");
        }

        public static Projection Parse(string text)
        {
            int[] values = text
                .Split("..")
                .Select(s => Int32.Parse(s))
                .OrderBy(i => i)
                .ToArray();

            //We use the end value as "up to but excluding"!
            return new Projection(values[0], values[1] + 1);
        }

        public override string ToString()
        {
            return $"{Start}..{End}";
        }
    }

    public class Cuboid
    {
        public static readonly Axis[] AllAxes = new Axis[] { Axis.X, Axis.Y, Axis.Z };

        public Projection[] AxisProjections { get; private set; }

        public Projection X => this[Axis.X];

        public Projection Y => this[Axis.Y];

        public Projection Z => this[Axis.Z];

        public Projection this[Axis axis] => AxisProjections[(int)axis];

        public long CubeCount => (X.End - X.Start) * (Y.End - Y.Start) * (Z.End - Z.Start);


        public Cuboid(IEnumerable<Projection> axisProjections)
        {
            AxisProjections = axisProjections.ToArray();
            Debug.Assert(AxisProjections.Length == 3);
        }

        public override bool Equals(object obj)
        {
            Cuboid other = obj as Cuboid;
            if (other == null)
                return false;

            return AllAxes.All(axis => this[axis].Equals(other[axis]));
        }

        public override int GetHashCode()
        {
            return AxisProjections
                .Select(prj => prj.GetHashCode())
                .Aggregate(0, (acc, i) => acc * 13891 + i);
        }


        public override string ToString()
        {
            return $"X={X}, Y={Y}, Z={Z}";
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/22
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

        public static long Part2()
        {
            List<string> textLines = ReadInput();

            List<Tuple<bool, Cuboid>> commands = textLines
                .Select(line => ParseCommand(line))
                .ToList();

            int[] distinctXCoords = commands
                .SelectMany(cmd => new int[] { cmd.Item2.X.Start, cmd.Item2.X.End })
                .Distinct()
                .OrderBy(i => i)
                .ToArray();
            int[] distinctYCoords = commands
                .SelectMany(cmd => new int[] { cmd.Item2.Y.Start, cmd.Item2.Y.End })
                .Distinct()
                .OrderBy(i => i)
                .ToArray();
            int[] distinctZCoords = commands
                .SelectMany(cmd => new int[] { cmd.Item2.Z.Start, cmd.Item2.Z.End })
                .Distinct()
                .OrderBy(i => i)
                .ToArray();
            
            bool[,,] grid = new bool[distinctZCoords.Length, distinctYCoords.Length, distinctXCoords.Length];


            int lineIndex = 0;
            foreach (Tuple<bool, Cuboid> command in commands)
            {
                Cuboid c = command.Item2;
                int endZIndex = Array.BinarySearch(distinctZCoords, c.Z.End);
                for (int zIndex = Array.BinarySearch(distinctZCoords, c.Z.Start); zIndex < endZIndex; zIndex++)
                {
                    int endYIndex = Array.BinarySearch(distinctYCoords, c.Y.End);
                    for (int yIndex = Array.BinarySearch(distinctYCoords, c.Y.Start); yIndex < endYIndex; yIndex++)
                    {
                        int endXIndex = Array.BinarySearch(distinctXCoords, c.X.End);
                        for (int xIndex = Array.BinarySearch(distinctXCoords, c.X.Start); xIndex < endXIndex; xIndex++)
                        {
                            grid[zIndex, yIndex, xIndex] = command.Item1;
                        }
                    }
                }

                Console.WriteLine($"Finished line { ++lineIndex}.");
            }

            long cubeTotal = 0;
            for (int xIndex = 0; xIndex < distinctXCoords.Length -1; xIndex++)
            {
                for (int yIndex = 0; yIndex < distinctYCoords.Length -1; yIndex++)
                {
                    for (int zIndex = 0; zIndex < distinctZCoords.Length - 1; zIndex++)
                    {
                        if(grid[zIndex, yIndex, xIndex] == true)
                        {
                            long deltaX = distinctXCoords[xIndex + 1] - distinctXCoords[xIndex];
                            long deltaY = distinctYCoords[yIndex + 1] - distinctYCoords[yIndex];
                            long deltaZ = distinctZCoords[zIndex + 1] - distinctZCoords[zIndex];
                            cubeTotal += (deltaX * deltaY * deltaZ);
                        }
                    }
                }
            }

            long result = cubeTotal;
            return result;
        }

        private static readonly Regex CommandRegex = new Regex(@"(on|off) x=(-?\d+..-?\d+),y=(-?\d+..-?\d+),z=(-?\d+..-?\d+)");

        private static Tuple<bool, Cuboid> ParseCommand(string line)
        {
            Match match = CommandRegex.Match(line);
            Debug.Assert(match.Success);

            bool isOn = (match.Groups[1].Value == "on");
            Projection[] projections = match.Groups.Cast<Group>()
                .Skip(2)
                .Select(grp => grp.Value)
                .Select(s => Projection.Parse(s))
                .ToArray();

            return new Tuple<bool, Cuboid>(isOn, new Cuboid(projections));
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
