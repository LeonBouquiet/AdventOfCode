using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day04
{
    public class Passport
    {
        public Dictionary<string, string> Elements;

        private static string[] RequiredFields = new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };

        public bool AreAllPresent
        {
            get
            {
                return RequiredFields.All(fld => Elements.ContainsKey(fld));
            }
        }

        private static readonly Regex HairColorRegex = new Regex("^#[0-9a-f]{6}$");
        private static readonly Regex EyeColorRegex = new Regex("^(amb|blu|brn|gry|grn|hzl|oth)$");
        private static readonly Regex PassportIdRegex = new Regex("^[0-9]{9}$");
        private static readonly Regex HeightRegex = new Regex("^([0-9]{2,3})(cm|in)$");

        public bool IsValid
        {
            get
            {
                bool isByrValid = IsNumberInRange(Elements["byr"], 4, 1920, 2002);
                bool isIyrValid = IsNumberInRange(Elements["iyr"], 4, 2010, 2020);
                bool isEyrValid = IsNumberInRange(Elements["eyr"], 4, 2020, 2030);
                if ((isByrValid && isIyrValid && isEyrValid) == false)
                    return false;

                bool isHclValid = HairColorRegex.IsMatch(Elements["hcl"]);
                bool isEclValid = EyeColorRegex.IsMatch(Elements["ecl"]);
                bool isPidValid = PassportIdRegex.IsMatch(Elements["pid"]);
                if ((isHclValid && isEclValid && isPidValid) == false)
                    return false;

                Match match = HeightRegex.Match(Elements["hgt"]);
                if (match.Success == false)
                    return false;

                if(match.Groups[2].Value == "cm")
                    return IsNumberInRange(match.Groups[1].Value, 3, 150, 193);
                else
                    return IsNumberInRange(match.Groups[1].Value, 2, 59, 76);
            }
        }

        public Passport(string text)
        {
            string[] elements = text.Trim().Split(' ');
            Elements = elements
                .Select(elt => elt.Trim().Split(':'))
                .ToDictionary(arr => arr[0], arr => arr[1]);
        }

        private static bool IsNumberInRange(string value, int nrOfdigits, int min, int max)
        {
            if (value.Length != nrOfdigits || value.All(c => Char.IsDigit(c)) == false)
                return false;

            int year = Int32.Parse(value);
            return (year >= min && year <= max);
        }
    }


    /// <summary>
    /// 
    /// See https://adventofcode.com/2020/day/4
    /// </summary>
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
            List<Passport> passports = ReadInput();

            int validCount = passports.Count(p => p.AreAllPresent);
            Console.WriteLine($"Result: {validCount}");
        }

        private static void Part2()
        {
            List<Passport> passports = ReadInput();

            int validCount = passports.Count(p => p.AreAllPresent && p.IsValid);
            Console.WriteLine($"Result: {validCount}");
        }

        private static List<Passport> ReadInput()
        {
            List<Passport> result = new List<Passport>();

            Console.WriteLine("Day 04, Provide input, terminate with a '.':");

            string passport = "";
            string line;
            while ((line = Console.ReadLine()) != ".")
            {
                if (line != "")
                    passport += " " + line;
                else
                {
                    result.Add(new Passport(passport));
                    passport = "";
                }
            }

            if(passport != "")
                result.Add(new Passport(passport));

            return result;
        }
    }
}
