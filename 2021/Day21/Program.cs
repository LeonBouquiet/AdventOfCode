using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day21
{
    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/21
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


        public static int Part1()
        {
            List<string> textLines = ReadInput();
            int pos1 = Int32.Parse(textLines[0].Split(": ")[1]);
            int pos2 = Int32.Parse(textLines[1].Split(": ")[1]);

            int[] positions = new int[] { pos1, pos2 };
            int[] scores = new int[] { 0, 0 };

            for (int round = 0; round < 1000; round++)
            {
                for (int turn = 0; turn < 2; turn++)
                {
                    int[] dice = RollThreeTimes();
                    positions[turn] = (((positions[turn] + dice.Sum()) - 1) % 10) + 1;
                    scores[turn] += positions[turn];

                    Console.WriteLine($"Player {turn + 1} rolls {dice[0]}+{dice[1]}+{dice[2]} and moves to space {positions[turn]} for a total score of {scores[turn]}.");

                    if(scores[turn] >= 1000)
                    {
                        int losingPlayer = 1 - turn;
                        return scores[losingPlayer] * RollCount;
                    }
                }
            }

            return 0;
        }

        private static int DeterministicDie = 1;
        private static int RollCount = 0;

        private static int[] RollThreeTimes()
        {
            int[] result = new int[3];
            for (int index = 0; index < 3; index++)
            {
                result[index] = DeterministicDie;
                DeterministicDie = ((++DeterministicDie - 1) % 100) + 1;
            }

            RollCount += 3;
            return result;
        }

        public class Universe
        {
            public long ParentOccurences { get; private set; }

            public int Player1Position { get; private set; }
            public int Player1Score { get; private set; }
            public int Player2Position { get; private set; }
            public int Player2Score { get; private set; }
            public int DiceThrows { get; private set; }
            public int Winner { get; private set; }  //1, 2, or 0 for no winner yet.
            public bool IsProcessed { get; set; }

            public Universe(long parentOccurences, int player1Position, int player1Score, int player2Position, int player2Score, int diceThrows, int winner)
            {
                ParentOccurences = parentOccurences;
                Player1Position = player1Position;
                Player1Score = player1Score;
                Player2Position = player2Position;
                Player2Score = player2Score;
                DiceThrows = diceThrows;
                Winner = winner;
                IsProcessed = false;
            }

            public override bool Equals(object obj)
            {
                Universe other = obj as Universe;
                if (other == null)
                    return false;

                return
                    this.Player1Position == other.Player1Position &&
                    this.Player1Score == other.Player1Score &&
                    this.Player2Position == other.Player2Position &&
                    this.Player2Score == other.Player2Score &&
                    this.DiceThrows == other.DiceThrows &&
                    this.Winner == other.Winner;
            }

            public override int GetHashCode()
            {
                return Player1Position * 13 + Player2Position * 119 + Player1Score * 59 + Player2Score * 143;
            }

            public override string ToString()
            {
                return $"P1 Position={Player1Position}, Score={Player1Score} | P2 Position={Player2Position}, Score={Player2Score} | {DiceThrows} throws, {(Winner == 0 ? "no winner" : (Winner + " wins" ))}.";
            }
        }

        // Written as (diceSum, nrOfOccurences), where the total nrOfOccurences is 27 (3x3x3).
        private static List<(int, int)> ThreeRollOutcomes = new List<(int, int)> 
        {
            (3, 1), (4, 1), (5, 1), (4, 1), (5, 1), (6, 1), (5, 1), (6, 1), (7, 1),
            (4, 1), (5, 1), (6, 1), (5, 1), (6, 1), (7, 1), (6, 1), (7, 1), (8, 1),
            (5, 1), (6, 1), (7, 1), (6, 1), (7, 1), (8, 1), (7, 1), (8, 1), (9, 1),
        };

        public static Dictionary<Universe, long> UniverseOccurences = new Dictionary<Universe, long>();

        public static long Part2()
        {
            List<string> textLines = ReadInput();
            int pos1 = Int32.Parse(textLines[0].Split(": ")[1]);
            int pos2 = Int32.Parse(textLines[1].Split(": ")[1]);

            long[] wins = new long[] { 0, 0 };

            UniverseOccurences.Add(new Universe(1, pos1, 0, pos2, 0, 0, 0), 1L);

            for (int round = 0; round < 12; round++)
            {
                List<Universe> knownUniverses = UniverseOccurences.Keys
                    .Where(u => u.IsProcessed == false)
                    .ToList();

                long total = UniverseOccurences
                    .Select(kvp => kvp.Key.ParentOccurences * kvp.Value)
                    .Sum();
                Console.WriteLine($"Round {round}, processing {knownUniverses.Count} universes (from {total} in total)...");

                foreach(Universe universe in knownUniverses)
                {
                    long thisUniverseOccurs = universe.ParentOccurences * UniverseOccurences[universe];
                    UniverseOccurences.Remove(universe);
                    universe.IsProcessed = true;

                    if(universe.Winner != 0)
                    {
                        wins[universe.Winner - 1] += thisUniverseOccurs;
                        continue;
                    }

                    //Player 1:
                    foreach (var player1Outcome in ThreeRollOutcomes)
                    {
                        int player1NewPosition = (((universe.Player1Position + player1Outcome.Item1) - 1) % 10) + 1;
                        int player1NewScore = universe.Player1Score + player1NewPosition;
                        int winner = (player1NewScore >= 21) ? 1 : 0;

                        if (winner == 1)
                        {
                            Universe spawned = new Universe(thisUniverseOccurs, player1NewPosition, player1NewScore, universe.Player2Position, universe.Player2Score, universe.DiceThrows + 3, winner);
                            long occurences = UniverseOccurences.TryGetOrAdd(spawned, 0L);

                            //long factor = player1Outcome.Item2;
                            occurences = occurences + 1;
                            UniverseOccurences[spawned] = occurences;
                        }
                        else
                        {
                            //Player 2:
                            foreach (var player2Outcome in ThreeRollOutcomes)
                            {
                                int player2NewPosition = (((universe.Player2Position + player2Outcome.Item1) - 1) % 10) + 1;
                                int player2NewScore = universe.Player2Score + player2NewPosition;
                                winner = (player2NewScore >= 21) ? 2 : 0;

                                Universe spawned = new Universe(thisUniverseOccurs, player1NewPosition, player1NewScore, player2NewPosition, player2NewScore, universe.DiceThrows + 6, winner);
                                long currentUniverseOccurs = UniverseOccurences.TryGetOrAdd(spawned, 0L);

                                //Because of the compacted form of ThreeRollOutcomes, this spawned universe counts for multiple times.
                                //long factor = player1Outcome.Item2 * player2Outcome.Item2;
                                long occurences = currentUniverseOccurs + 1;
                                Debug.Assert(occurences >= 0, "Overflow");
                                
                                UniverseOccurences[spawned] = occurences;
                            }
                        }
                    }
                }
            }

            //long player1Wins = UniverseOccurences
            //    .Where(kvp => kvp.Key.Winner == 1)
            //    .Select(kvp => kvp.Key.ParentOccurences * kvp.Value)
            //    .Sum();
            //long player2Wins = UniverseOccurences
            //    .Where(kvp => kvp.Key.Winner == 2)
            //    .Select(kvp => kvp.Key.ParentOccurences * kvp.Value)
            //    .Sum();

            return Math.Max(wins[0], wins[1]);
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
        public static TValue TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            TValue result = default(TValue);
            if (dictionary.TryGetValue(key, out result) == true)
                return result;

            dictionary.Add(key, value);
            return value;
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
