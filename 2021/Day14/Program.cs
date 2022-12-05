using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day14
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/14
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
            string template = textLines.First();

            Dictionary<string, char> insertionRules = textLines
                .Skip(2)
                .Select(line => line.Trim().Replace(" -> ", "-"))
                .Select(line => line.Split("-"))
                .ToDictionary(arr => arr[0], arr => arr[1][0]);

            for (int step = 1; step <= 10; step++)
            {
                for (int index = 0; index < template.Length - 1; index++)
                {
                    string key = template.Substring(index, 2);
                    char toInsert = ' ';
                    if (insertionRules.TryGetValue(key, out toInsert))
                    {
                        template = template.Insert(index + 1, "" + toInsert);
                        index++;
                    }
                }

                Console.WriteLine($"Result after step {step}: {template}");
            }

            var characterCounts = template
                .GroupBy(c => c)
                .Select(grp => new { Count = grp.LongCount(), Key = grp.Key })
                .OrderByDescending(a => a.Count)
                .ToList();

            long result = characterCounts.First().Count - characterCounts.Last().Count;
            return result;
        }

        public static long Part2()
        {
            List<string> textLines = ReadInput(".");
            DateTime start = DateTime.Now;

            string template = textLines.First();
            Dictionary<string, char> insertionRules = textLines
                .Skip(2)
                .Select(line => line.Trim().Replace(" -> ", "-"))
                .Select(line => line.Split("-"))
                .ToDictionary(arr => arr[0], arr => arr[1][0]);

            Dictionary<string, long> sourcePairs = new Dictionary<string, long>();
            Dictionary<char, long> characterOccurences = new Dictionary<char, long>();

            characterOccurences[template[0]] = characterOccurences.TryGetValueOrDefault(template[0], 0) + 1;
            for (int index = 0; index < template.Length - 1; index++)
            {
                characterOccurences[template[index + 1]] = characterOccurences.TryGetValueOrDefault(template[index + 1], 0) + 1;

                string key = template.Substring(index, 2);
                sourcePairs[key] = sourcePairs.TryGetValueOrDefault(key, 0) + 1;
            }

            for (int step = 1; step <= 40; step++)
            {
                Dictionary<string, long> targetPairs = new Dictionary<string, long>();

                string[] occuringKeys = sourcePairs
                    .Where(kvp => kvp.Value > 0)
                    .Select(kvp => kvp.Key)
                    .ToArray();

                foreach(string key in occuringKeys)
                {
                    char toInsert = insertionRules.TryGetValueOrDefault(key, '/');
                    if(toInsert != '/')
                    {
                        long currentKeyCount = sourcePairs[key];

                        string key1 = key[0] + ("" + toInsert);
                        targetPairs[key1] = targetPairs.TryGetValueOrDefault(key1, 0L) + currentKeyCount;

                        string key2 = ("" + toInsert) + key[1];
                        targetPairs[key2] = targetPairs.TryGetValueOrDefault(key2, 0L) + currentKeyCount;

                        characterOccurences[toInsert] = characterOccurences.TryGetValueOrDefault(toInsert, 0) + currentKeyCount;
                    }
                }

                sourcePairs = targetPairs;

                Console.WriteLine($"Template length after step {step}: {characterOccurences.Sum(kvp => kvp.Value)}");
            }

            var characterCounts = characterOccurences
                .OrderByDescending(kvp => kvp.Value)
                .ToList();

            long result = characterCounts.First().Value - characterCounts.Last().Value;

            TimeSpan duration = DateTime.Now - start;
            Console.WriteLine($"Duration in ms: {duration.TotalMilliseconds}");

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
