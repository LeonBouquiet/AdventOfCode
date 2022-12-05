using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day12
{
    public class Vertex
    {
        public string Id { get; private set; }

        private List<Vertex> _edgesTo = new List<Vertex>();

        public IEnumerable<Vertex> EdgesTo
        {
            get { return _edgesTo; }
        }

        public bool IsSmall
        {
            get { return Char.IsLower(Id[0]); }
        }

        public Vertex(string id)
        {
            Id = id;
        }

        public void AddEdgeTo(Vertex vertex)
        {
            _edgesTo.Add(vertex);
        }

        public override bool Equals(object obj)
        {
            Vertex other = obj as Vertex;
            if (other != null)
                return this.Id == other.Id;

            return false;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 1139;
        }

        public override string ToString()
        {
            return Id;
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/12
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

        private static Stack<Vertex> _currentPath = new Stack<Vertex>();

        private static HashSet<Vertex> _visitedSmallVertices = new HashSet<Vertex>();

        private static List<Vertex[]> _foundPaths = new List<Vertex[]>();

        public static int Part1()
        {
            HashSet<Vertex> vertices = new HashSet<Vertex>();
            
            List<string> textLines = ReadInput();
            foreach(string[] pair in textLines.Select(line => line.Trim().Split('-')))
            {
                Vertex first = vertices.TryGetOrAdd(new Vertex(pair[0]));
                Vertex second = vertices.TryGetOrAdd(new Vertex(pair[1]));

                first.AddEdgeTo(second);
                second.AddEdgeTo(first);
            }

            Vertex startVertex = vertices.Single(v => v.Id == "start");
            ExplorePaths(startVertex);

            return _foundPaths.Count;
        }

        private static void ExplorePaths(Vertex current)
        {
            _currentPath.Push(current);

            if (current.IsSmall)
                _visitedSmallVertices.Add(current);

            if (current.Id == "end")
            {
                _foundPaths.Add(_currentPath.Reverse().ToArray());
                Console.WriteLine(string.Join("-", _currentPath.Reverse()));
            }
            else
            {
                foreach (Vertex reachable in current.EdgesTo)
                {
                    if (reachable.IsSmall == false || _visitedSmallVertices.Contains(reachable) == false)
                        ExplorePaths(reachable);
                }
            }

            if (current.IsSmall)
                _visitedSmallVertices.Remove(current);

            _currentPath.Pop();
        }

        private static Dictionary<Vertex, int> _visitedSmallVertices2 = new Dictionary<Vertex, int>();

        public static int Part2()
        {
            HashSet<Vertex> vertices = new HashSet<Vertex>();

            List<string> textLines = ReadInput();
            foreach (string[] pair in textLines.Select(line => line.Trim().Split('-')))
            {
                Vertex first = vertices.TryGetOrAdd(new Vertex(pair[0]));
                Vertex second = vertices.TryGetOrAdd(new Vertex(pair[1]));

                first.AddEdgeTo(second);
                second.AddEdgeTo(first);
            }

            Vertex startVertex = vertices.Single(v => v.Id == "start");
            ExplorePaths2(startVertex);

            return _foundPaths.Count;
        }

        private static void ExplorePaths2(Vertex current)
        {
            _currentPath.Push(current);

            if (current.IsSmall)
                _visitedSmallVertices2[current] = _visitedSmallVertices2.TryGetValueOrDefault(current, 0) + 1;

            if (current.Id == "end")
            {
                _foundPaths.Add(_currentPath.Reverse().ToArray());
                Console.WriteLine(string.Join("-", _currentPath.Reverse()));
            }
            else
            {
                foreach (Vertex reachable in current.EdgesTo)
                {
                    if (reachable.IsSmall == false || IsVisitAllowed(reachable))
                        ExplorePaths2(reachable);
                }
            }

            if (current.IsSmall)
                _visitedSmallVertices2[current] = _visitedSmallVertices2.TryGetValueOrDefault(current, 0) - 1;

            _currentPath.Pop();
        }

        public static bool IsVisitAllowed(Vertex target)
        {
            int visitCount = _visitedSmallVertices2.TryGetValueOrDefault(target, 0) + 1;
            bool hasVisitedTwice = _visitedSmallVertices2.Any(kvp => kvp.Value == 2);
            int maxVisits = (target.Id == "start" || target.Id == "end" || hasVisitedTwice) ? 1 : 2;

            return visitCount <= maxVisits;
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
        public static TElement TryGetOrAdd<TElement>(this HashSet<TElement> hashSet, TElement element)
        {
            TElement result = default(TElement);
            if (hashSet.TryGetValue(element, out result) == true)
                return result;

            hashSet.Add(element);
            return element;
        }

        public static TValue TryGetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue ifNotFound = default(TValue))
        {
            TValue result;
            if (dictionary.TryGetValue(key, out result))
                return result;

            return ifNotFound;
        }
    }
}
