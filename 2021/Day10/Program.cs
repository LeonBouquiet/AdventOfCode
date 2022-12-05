using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day10
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/10
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

        public static List<(char, char, int, int)> Delimiters = new List<(char, char, int, int)>() 
        { 
            ('(', ')', 3    , 1),
            ('[', ']', 57   , 2),
            ('{', '}', 1197,  3),
            ('<', '>', 25137, 4),
        };

        public static bool IsOpener(char c)
        {
            return Delimiters.Where(d => d.Item1 == c).Any();
        }

        public static char GetCloserFor(char opener)
        {
            return Delimiters.First(d => d.Item1 == opener).Item2;
        }

        public static int GetSyntaxErrorScoreFor(char closer)
        {
            return Delimiters.First(d => d.Item2 == closer).Item3;
        }

        public static int GetClosingScoreFor(char opener)
        {
            return Delimiters.First(d => d.Item1 == opener).Item4;
        }

        public static int Part1()
        {
            List<string> textLines = ReadInput();

            int result = textLines.Select(s => GetCorruptionScore(s)).Sum();
            return result;
        }

        private static int GetCorruptionScore(string line)
        {
            Stack<char> openers = new Stack<char>();
            foreach(char c in line)
            {
                if(IsOpener(c) == true)
                {
                    openers.Push(c);
                }
                else
                {
                    if (c == GetCloserFor(openers.Peek()))
                        openers.Pop();
                    else
                    {
                        Console.WriteLine($"Expected {GetCloserFor(openers.Peek())}, but found {c} instead.");
                        return GetSyntaxErrorScoreFor(closer: c);
                    }
                }
            }

            return 0;
        }

        public static long Part2()
        {
            List<string> textLines = ReadInput();

            List<long> closingScores = textLines
                .Where(s => GetCorruptionScore(s) == 0)
                .Select(s => (long)GetClosingScore(s))
                .ToList();

            closingScores.Sort();
            long result = closingScores[closingScores.Count / 2];

            return result;
        }

        private static long GetClosingScore(string line)
        {
            Stack<char> openers = new Stack<char>();
            foreach (char c in line)
            {
                if (IsOpener(c) == true)
                {
                    openers.Push(c);
                }
                else
                {
                    if (c == GetCloserFor(openers.Peek()))
                        openers.Pop();
                    else
                        throw new InvalidOperationException();
                }
            }

            long score = 0;
            while (openers.Any())
            {
                score = score * 5 + GetClosingScoreFor(openers.Pop());
            }

            return score;
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
