using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day04
{
    public class Board
    {
        private int[,] _numbers = new int[5, 5];

        private int[] _remainingInEachRow = new int[] { 5, 5, 5, 5, 5 };

        private int[] _remainingInEachColumn = new int[] { 5, 5, 5, 5, 5 };

        public bool HasWon { get; private set; } = false;

        public Board(string[] lines)
        {
            if (lines == null || lines.Length != 5)
                throw new ArgumentException("lines");

            for (int y = 0; y < 5; y++)
            {
                int[] rowNrs = lines[y].Trim()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Int32.Parse(s.Trim()))
                    .ToArray();

                if (rowNrs.Length != 5)
                    throw new ArgumentException($"Row {y} has {rowNrs.Length} elements.");
                for (int x = 0; x < 5; x++)
                    _numbers[y, x] = rowNrs[x];
            }
        }

        public bool Mark(int number)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (_numbers[y, x] == number)
                    {
                        _numbers[y, x] = -1;
                        _remainingInEachRow[y]--;
                        _remainingInEachColumn[x]--;

                        if (_remainingInEachRow[y] == 0 || _remainingInEachColumn[x] == 0)
                            HasWon = true;
                    }

                }
            }

            return HasWon;
        }

        public bool HasWon2
        {
            get
            {
                for (int y = 0; y < 5; y++)
                {
                    int remainingInThisRow = 5;

                    for (int x = 0; x < 5; x++)
                    {
                        if (_numbers[y, x] == -1)
                            remainingInThisRow--;
                    }

                    if (remainingInThisRow == 0)
                        return true;
                }

                for (int x = 0; x < 5; x++)
                {
                    int remainingInThisColumn = 5;

                    for (int y = 0; y < 5; y++)
                    {
                        if (_numbers[y, x] == -1)
                            remainingInThisColumn--;
                    }

                    if (remainingInThisColumn == 0)
                        return true;
                }

                return false;
            }
        }

        public IEnumerable<int> UnmarkedNumbers
        {
            get
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        if (_numbers[y, x] != -1)
                            yield return _numbers[y, x];
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sbResult = new StringBuilder();
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    sbResult.AppendFormat("{0,3} ", _numbers[y, x]);
                }

                sbResult.AppendLine();
            }

            return sbResult.ToString();
        }
    }


    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/4
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            int result = Part1();
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        public static int Part1()
        {
            Tuple<int[], List<Board>> input = ReadInput();
            int[] numbers = input.Item1;
            List<Board> boards = input.Item2;

            for (int numberIndex = 0; numberIndex < numbers.Length; numberIndex++)
            {
                foreach (Board board in boards)
                {
                    board.Mark(numbers[numberIndex]);
                }

                Board winner = boards.FirstOrDefault(b => b.HasWon || b.HasWon2);
                if (winner != null)
                {
                    int unmarkedSum = winner.UnmarkedNumbers.Sum();
                    int result = unmarkedSum * numbers[numberIndex];

                    return result;
                }
            }

            return -1;
        }

        public static int Part2()
        {
            Tuple<int[], List<Board>> input = ReadInput();
            int[] numbers = input.Item1;
            List<Board> boards = input.Item2;

            for (int numberIndex = 0; numberIndex < numbers.Length; numberIndex++)
            {
                foreach (Board board in boards)
                {
                    board.Mark(numbers[numberIndex]);
                }

                if(boards.Count > 1)
                {
                    boards.RemoveAll(b => b.HasWon2);
                }
                else
                {
                    if (boards.First().HasWon2)
                    {
                        int unmarkedSum = boards.First().UnmarkedNumbers.Sum();
                        int result = unmarkedSum * numbers[numberIndex];

                        return result;
                    }
                }
            }

            return -1;
        }

        private static Tuple<int[], List<Board>> ReadInput()
        {
            Console.WriteLine("Provide input, terminate with a '.':");
            int[] numbers = Console.ReadLine()
                .Split(",")
                .Select(s => s.Trim())
                .Where(s => string.IsNullOrEmpty(s) == false)
                .Select(s => Int32.Parse(s))
                .ToArray();

            List<Board> boards = new List<Board>();
            List<string> boardLines = new List<string>();

            string line;
            do
            {
                line = Console.ReadLine();
                if (line == string.Empty || line == ".")
                {
                    if (boardLines.Any())
                    {
                        Board board = new Board(boardLines.ToArray());
                        boards.Add(board);

                        boardLines.Clear();
                    }
                }
                else
                {
                    boardLines.Add(line);
                }
            }
            while (line != ".");

            return new Tuple<int[], List<Board>>(numbers, boards);
        }
    }
}
