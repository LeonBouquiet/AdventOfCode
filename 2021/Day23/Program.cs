using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day23
{
    public struct Position
    {
        public int X;
        public int Y;

        private static (int, int)[] NeighboursDisplacements 
            = new (int, int)[] { ( 0, -1), (-1,  0), ( 0,  1), ( 1,  0) };

        public static readonly Position None = new Position(-1, -1);

        public Position Above => Displace(0, -1);

        public Position Below => Displace(0, 1);

        public IEnumerable<Position> AllNeighbours
        {
            get
            {
                int x = X;
                int y = Y;
                return NeighboursDisplacements.Select(disp => new Position(x + disp.Item1, y + disp.Item2));
            }
        }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        private Position Displace(int deltaX, int deltaY)
        {
            return new Position(X + deltaX, Y + deltaY);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public struct Destination
    {
        public Position Position;
        public int Distance;

        public Destination(Position position, int distance)
        {
            Position = position;
            Distance = distance;
        }

        public override string ToString()
        {
            return $"{Position} (dist={Distance})";
        }
    }

    public class GameState : IComparable<GameState>
    {
        private char[,] _cells = new char[7,14];

        public GameState Parent { get; private set; }

        public int Cost { get; private set; }

        public int Priority { get; private set; }

        public int MinSolutionCost { get; private set; }

        private int _originalHashCode;

        private int _hashCode;

        public bool IsSolution => (CalculateDistanceToSolution() == 0);

        public char this[Position pos]
        {
            get { return _cells[pos.Y, pos.X]; }
            set { _cells[pos.Y, pos.X] = value; }
        }

        public string Display => ToString();

        protected GameState()
        {
            Parent = null;
            Cost = 0;
        }

        protected GameState(GameState parent, Position position, Destination destination)
        {
            Parent = parent;
            Array.Copy(parent._cells, this._cells, parent._cells.Length);

            //Perform the move.
            char pod = this[position];
            this[position] = '.';
            this[destination.Position] = Char.ToLower(pod);

            Cost = parent.Cost + GetCostFactor(pod) * destination.Distance;
            DetermineHashCode();
            DeterminePriorityAndMinSolutionCost();
        }

        private void DetermineHashCode()
        {
            int positions = GetPodPositions().Aggregate(0, (acc, pos) => acc * 13 * (Char.ToUpperInvariant(this[pos]) - 'A' + 1) + pos.GetHashCode());
            _hashCode = positions;
        }

        public override int GetHashCode()
        {
            _originalHashCode = base.GetHashCode();
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            GameState other = obj as GameState;
            if (other == null || this._hashCode != other._hashCode)
                return false;

            return this.ValidPositions.All(pos => this[pos] == other[pos]);
        }

        public int CompareTo(GameState other)
        {
            return Compare(this, other);
        }

        public IEnumerable<Position> ValidPositions
        {
            get
            {
                for (int y = 1; y < 6; y++) 
                {
                    for (int x = 0; x < 14; x++) 
                    {
                        if (IsValid(_cells[y, x]))
                            yield return new Position(x, y);
                    }
                }
            }
        }

        private static bool IsPod(char c)
        {
            c = Char.ToUpperInvariant(c);
            return (c >= 'A' && c <= 'D');
        }

        private static bool HasPodMoved(char c)
        {
            //Pods in their initial position are in caps ('A'), once moved they are lowercase ('a')
            return (c >= 'a' && c <= 'd');
        }

        private static bool IsValid(char c)
        {
            return (c == '.' || IsPod(c));
        }

        private static bool IsAvailable(char c)
        {
            return (c == '.');
        }

        private IEnumerable<Position> GetPodPositions()
        {
            return ValidPositions.Where(pos => IsPod(this[pos]));
        }

        private bool IsRoomPosition(Position pos)
        {
            return (pos.X == 3 || pos.X == 5 || pos.X == 7 || pos.X == 9) && (pos.Y >= 2 && pos.Y <= 5);
        }

        private Position GetLowestAvailableRoomPosition(Position pos)
        {
            Position candidate = Position.None;
            for (int row = 5; row >= 0; row--)
            {
                candidate = new Position(pos.X, row);
                if (IsAvailable(this[candidate]))
                    return candidate;
            }

            return candidate;
        }

        private bool IsTargetPositionForPod(Char podChar, Position podPosition)
        {
            int destColumn = 3 + (Char.ToUpperInvariant(podChar) - 'A') * 2;
            return (podPosition.X == destColumn && (podPosition.Y >= 2 || podPosition.Y <= 5));
        }

        private static int GetCostFactor(char podChar)
        {

            return Char.ToUpperInvariant(podChar) switch
            {
                'A' => 1,
                'B' => 10,
                'C' => 100,
                'D' => 1000,
                _ => -1
            };
        }
        
        public bool IsUnsolvable
        {
            get
            {
                foreach (Position podPosition in GetPodPositions().Where(pos => IsRoomPosition(pos)))
                {
                    char podChar = this[podPosition];
                    int destColumn = 3 + (Char.ToUpperInvariant(podChar) - 'A') * 2;

                    //If a pod has already moved to a Room position..
                    if (HasPodMoved(podChar))
                    {
                        //but it's the wrong column
                        if (podPosition.X != destColumn)
                            return true;

                        //Or if it has enclosed other pods below it
                        for(Position test = podPosition.Below; IsRoomPosition(test); test = test.Below)
                        {
                            if (IsAvailable(this[test]) == false && Char.ToLowerInvariant(this[test]) != podChar)
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        private int CalculateDistanceToSolution(int basePenalty = 3, bool includeCostFactor = false)
        {
            int result = 0;
            foreach(Position podPosition in GetPodPositions())
            {
                if (IsTargetPositionForPod(this[podPosition], podPosition))
                {
                    Position lowestAvailable = GetLowestAvailableRoomPosition(podPosition);
                    if (podPosition.Y > lowestAvailable.Y)
                    {
                        //Correctly placed in the lowest available Room position
                        continue;
                    }
                }
                else
                {
                    char podChar = this[podPosition];
                    int destColumn = 3 + (Char.ToUpperInvariant(podChar) - 'A') * 2;

                    int distanceToDest = Math.Abs(podPosition.X - destColumn) + Math.Abs(podPosition.Y - 3);
                    int costFactor = includeCostFactor ? GetCostFactor(podChar) : 1;
                    result += basePenalty + costFactor * distanceToDest;
                }
            }

            return result;
        }

        private void DeterminePriorityAndMinSolutionCost()
        {
            Priority = CalculateDistanceToSolution() * 1000 + Cost;
            MinSolutionCost = CalculateDistanceToSolution(0, true);
        }

        //This is only used by the priorityQueue to determine ordering (and not by the HashSets used to determine duplicates).
        public static int Compare(GameState left, GameState right)
        {
            //Lower is better
            int result = left.Priority - right.Priority;

            //Only return 0 if they're actually the same object, otherwise return some deterministic value.
            if (result == 0 && !Object.ReferenceEquals(left, right))
                result = left._originalHashCode - right._originalHashCode;

            return result;
        }

        public IEnumerable<Destination> GenerateValidPodDestinations(Position podPosition)
        {
            //If the pod has moved (i.e. written in lowercase) and is in a Room, it may not move anymore.
            if(HasPodMoved(this[podPosition]) && IsRoomPosition(podPosition))
                return Enumerable.Empty<Destination>();

            List<Destination> result = new List<Destination>();
            HashSet<Position> explored = new HashSet<Position>();
            Queue<Destination> fringe = new Queue<Destination>();
            fringe.Enqueue(new Destination(podPosition, 0));

            while(fringe.Any())
            {
                Destination candidate = fringe.Dequeue();
                explored.Add(candidate.Position);

                foreach(Position neighbour in candidate.Position.AllNeighbours)
                {
                    if(IsAvailable(this[neighbour]) && explored.Contains(neighbour) == false)
                    {
                        Destination destination = new Destination(neighbour, candidate.Distance + 1);
                        fringe.Enqueue(destination);

                        //If the pod is on its initial position, it may move to the hall or directly to a room.
                        //If the pod has moved (which means it must be in the Hall), it may only move to a Room.
                        if (HasPodMoved(this[podPosition]) == false || IsRoomPosition(neighbour))
                        {
                            //We don't want to stop directly above a room
                            if (!(IsRoomPosition(neighbour) == false && IsRoomPosition(neighbour.Below) == true))
                            {
                                //If we move into a Room, it should only be the correct room
                                if(IsRoomPosition(neighbour) == false || IsTargetPositionForPod(this[podPosition], neighbour))
                                    result.Add(destination);
                            }

                            //We only want the lowest possible Room position, so now that we've found this, remove the
                            //Destination with the Position above this one from the results.
                            Position above = neighbour.Above;
                            if (IsRoomPosition(neighbour) && IsRoomPosition(above))
                                result.RemoveAll(dst => dst.Position.Equals(above));
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<GameState> GenerateChildStates()
        {
            foreach(Position podPosition in GetPodPositions())
            {
                List<Destination> destinations = GenerateValidPodDestinations(podPosition).ToList();
                foreach (Destination dest in destinations)
                    yield return new GameState(this, podPosition, dest);
            }
        }


        public override string ToString()
        {
            StringBuilder sbResult = new StringBuilder();
            sbResult.AppendLine($"*** Cost: {Cost}, Distance to solution {CalculateDistanceToSolution() } ***");
            for (int y= 0; y<7; y++)
            {
                for (int x = 0; x < 14; x++)
                    sbResult.Append((_cells[y, x] != 0) ? _cells[y, x] : ' ');

                sbResult.AppendLine();
            }

            return sbResult.ToString();
        }

        public static GameState Parse(string[] lines)
        {
            GameState result = new GameState();
            for (int y = 0; y < lines.Length; y++)
                for (int x = 0; x < lines[y].Length; x++)
                    result._cells[y, x] = lines[y][x];

            result.DetermineHashCode();
            result.DeterminePriorityAndMinSolutionCost();
            return result;
        }


    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/23
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
            List<string> textLines = new List<string>()  //ReadInput();
            { 
                "#############",
                "#...........#",
                "###D#D#B#A###",
                "  #D#C#B#A#  ",
                "  #D#B#A#C#  ",
                "  #B#C#A#C#  ",
                "  #########  "
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            GameState initialState = GameState.Parse(textLines.ToArray());

            HashSet<GameState> explored = new HashSet<GameState>();
            SortedSet<GameState> priorityQueue = new SortedSet<GameState>();
            priorityQueue.Add(initialState);

            int maxSolutionCost = Int32.MaxValue;
            long solutionDurationInMs = -1;

            int iteration = 0;
            while (priorityQueue.Any())
            {
                if(++iteration % 100 == 0)
                    Console.WriteLine($"Iteration {iteration}, {priorityQueue.Count} GameStates available.");

                GameState current = priorityQueue.Min;
                bool removed = priorityQueue.Remove(current);
                Debug.Assert(removed);

                List<GameState> childStates = current.GenerateChildStates().ToList();
                foreach (GameState childState in childStates)
                {
                    if (childState.IsSolution && childState.Cost < maxSolutionCost)
                    {
                        solutionDurationInMs = stopwatch.ElapsedMilliseconds;

                        Console.WriteLine($"Found solution in {solutionDurationInMs}ms. :");
                        Console.WriteLine(childState);

                        maxSolutionCost = childState.Cost;
                        foreach (GameState gs in priorityQueue.Where(gs => gs.Cost + childState.MinSolutionCost >= maxSolutionCost).ToList())
                            priorityQueue.Remove(gs);
                    }
                    else
                    {
                        if (childState.IsUnsolvable == false && childState.Cost + childState.MinSolutionCost < maxSolutionCost && explored.Contains(childState) == false)
                        {
                            explored.Add(childState);
                            priorityQueue.Add(childState);
                        }
                    }
                }
            }

            Console.WriteLine($"Best solution was found in {solutionDurationInMs}ms, total run took {stopwatch.ElapsedMilliseconds}ms.");
            return maxSolutionCost;
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
