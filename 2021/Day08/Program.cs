using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day08
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/8
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
            List<Tuple<string[], string[]>> parsed = textLines.Select(l => 
                new Tuple<string[], string[]>(
                    l.Split('|')[0].Trim().Split(' '),
                    l.Split('|')[1].Trim().Split(' ')))
                .ToList();

            int result = parsed
                .SelectMany(tpl => tpl.Item2)
                .Count(s => s.Length == 2 || s.Length == 3 || s.Length == 4 || s.Length == 7);

            return result;
        }

        public static int Part2()
        {
            List<string> textLines = ReadInput();
            List<Tuple<string[], string[]>> parsed = textLines.Select(l =>
                new Tuple<string[], string[]>(
                    l.Split('|')[0].Trim().Split(' '),
                    l.Split('|')[1].Trim().Split(' ')))
                .ToList();

            List<int> numbers = parsed.Select(tpl => Decode(tpl.Item1, tpl.Item2)).ToList();
            int result = numbers.Sum();
            return result;
        }



        private static int Decode(string[] inputs, string[] outputDigits)
        {
            //Count how often each of the letters a-g occur (occurences[0] = letter 'a', occurences[1] = 'b', etc.)
            int[] occurences = new int[7];
            foreach (string input in inputs)
            {
                foreach (char c in input)
                    occurences[(c - 'a')]++;
            }

            //Get the characters used for the input of number "4", we can recognise it because it is the only input that uses 4 segments.
            string fourCharacters = inputs.First(s => s.Length == 4);

            //Based on the number of occurences of the segments, and wether or not they occur in the number 4, we can map
            //each segment position (0-7) to the caracter that must correspond to it.
            Dictionary<int, char> positionToCharMapping = new Dictionary<int, char>();
            int[] indexes = Enumerable.Range(0, 7).ToArray();
            positionToCharMapping.Add(0, (char)('a' + indexes.Single(i => occurences[i] == 8 && fourCharacters.Contains((char)('a' + i)) == false)));
            positionToCharMapping.Add(1, (char)('a' + indexes.Single(i => occurences[i] == 6)));
            positionToCharMapping.Add(2, (char)('a' + indexes.Single(i => occurences[i] == 8 && fourCharacters.Contains((char)('a' + i)) == true)));
            positionToCharMapping.Add(3, (char)('a' + indexes.Single(i => occurences[i] == 7 && fourCharacters.Contains((char)('a' + i)) == true)));
            positionToCharMapping.Add(4, (char)('a' + indexes.Single(i => occurences[i] == 4)));
            positionToCharMapping.Add(5, (char)('a' + indexes.Single(i => occurences[i] == 9)));
            positionToCharMapping.Add(6, (char)('a' + indexes.Single(i => occurences[i] == 7 && fourCharacters.Contains((char)('a' + i)) == false)));

            int[] numbers = outputDigits.Select(s => ConvertOutputDigitToNumber(s, positionToCharMapping)).ToArray();
            return numbers[0] * 1000 + numbers[1] * 100 + numbers[2] * 10 + numbers[3];
        }

        // Segment positions used:
        //    0
        //  1   2
        //    3
        //  4   5
        //    6

        private static Dictionary<string, int> PositionStringToNumber = new Dictionary<string, int>() {
            { "012456", 0 },
            { "25", 1 },
            { "02346", 2 },
            { "02356", 3 },
            { "1235", 4 },
            { "01356", 5 },
            { "013456", 6 },
            { "025", 7 },
            { "0123456", 8 },
            { "012356", 9 }
        };

        private static int ConvertOutputDigitToNumber(string outputDigit, Dictionary<int, char> positionToCharMapping)
        {
            List<int> positions = new List<int>();
            foreach(char c in outputDigit)
                positions.Add(positionToCharMapping.Where(kvp => kvp.Value == c).Single().Key);

            positions.Sort();
            string positionString = string.Join("", positions.Select(i => "" + i));

            int number = PositionStringToNumber[positionString];
            return number;
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
