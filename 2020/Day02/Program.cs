using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day02
{
    public class PolicyWithPassword
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public char Character { get; set; }
        public string Password { get; set; }

        private static readonly Regex PolicyRegex = new Regex(@"^(\d+)-(\d+) (\w): (.+)$");

        public bool IsAccordingToPolicy
        {
            get
            {
                int count = Password.Count(c => c == Character);
                return (count >= Minimum && count <= Maximum);
            }
        }

        public bool IsAccordingToPolicyV2
        {
            get
            {
                bool isMinimumMatch = (Password[Minimum - 1] == Character);
                bool isMaximumMatch = (Password[Maximum - 1] == Character);
                return isMinimumMatch != isMaximumMatch;
            }
        }

        public static PolicyWithPassword Parse(string text)
        {
            Match match = PolicyRegex.Match(text);
            return new PolicyWithPassword()
            {
                Minimum = Int32.Parse(match.Groups[1].Value),
                Maximum = Int32.Parse(match.Groups[2].Value),
                Character = match.Groups[3].Value[0],
                Password = match.Groups[4].Value
            };
        }

    }


    public class Program
    {
        public static void Main(string[] args)
        {
            Part2();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void Part1()
        {
            List<PolicyWithPassword> entries = ReadInput();
            int validCount = entries.Count(ent => ent.IsAccordingToPolicy);

            Console.WriteLine($"Result: {validCount}");
        }

        private static void Part2()
        {
            List<PolicyWithPassword> entries = ReadInput();
            int validCount = entries.Count(ent => ent.IsAccordingToPolicyV2);

            Console.WriteLine($"Result: {validCount}");
        }

        private static List<PolicyWithPassword> ReadInput()
        {
            List<PolicyWithPassword> result = new List<PolicyWithPassword>();

            Console.WriteLine("Provide input, terminate with an empty line:");


            string line;
            while ((line = Console.ReadLine()) != string.Empty)
            {
                result.Add(PolicyWithPassword.Parse(line));
            }

            return result;
        }
    }
}
