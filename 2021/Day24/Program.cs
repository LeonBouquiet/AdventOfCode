using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day24
{
    public enum OpCode
    {
        Inp,
        Add,
        Mul,
        Div,
        Mod,
        Eql
    }

    public enum Register
    {
        W = 0, X, Y, Z
    }

    public record Instruction
    {
        public OpCode OpCode;

        public Register Operand1;

        public Register? Operand2;

        public int? Operand2Value;

        public Instruction(OpCode opCode, Register operand1, Register? operand2, int? operand2Value)
        {
            OpCode = opCode;
            Operand1 = operand1;
            Operand2 = operand2;
            Operand2Value = operand2Value;
        }

        public static Instruction Parse(string line)
        {
            string[] parts = line.Split(' ');
            OpCode opCode = Enum.Parse<OpCode>(parts[0], ignoreCase: true);
            Register operand1 = Enum.Parse<Register>(parts[1], ignoreCase: true);

            Register? operand2 = null;
            int? operand2Value = null;

            if (parts.Length > 2)
            {
                int parsed = 0;
                if (Int32.TryParse(parts[2], out parsed))
                    operand2Value = parsed;
                else
                    operand2 = Enum.Parse<Register>(parts[2], ignoreCase: true);
            }

            return new Instruction(opCode, operand1, operand2, operand2Value);
        }

        public override string ToString()
        {
            string operand2 = (Operand2 != null) ? Operand2.ToString() : (Operand2Value != null ? Operand2Value.ToString() : "");
            return $"{OpCode} {Operand1} {operand2}";
        }
    }

    public class Processor
    {
        private long[] _registers;

        public long W => _registers[0];
        public long X => _registers[1];
        public long Y => _registers[2];
        public long Z => _registers[3];

        public List<Instruction> Program;
        public int InstructionPtr;

        public string Input;
        public int Cursor;
        public bool RequireXEqlWWherePossible;

        public Processor(List<Instruction> program)
        {
            Program = program;
        }

        private void Reset(string input)
        {
            _registers = new long[4];
            InstructionPtr = 0;
            Input = input;
            Cursor = 0;
        }

        public bool Execute(Instruction ins)
        {
            switch (ins.OpCode)
            {
                case OpCode.Inp:
                    ExecuteReadFromInput(ins.Operand1);
                    break;
                case OpCode.Add:
                    ExecuteReadWrite(ins.Operand1, ins.Operand2, ins.Operand2Value, (left, right) => left + right);
                    break;
                case OpCode.Mul:
                    ExecuteReadWrite(ins.Operand1, ins.Operand2, ins.Operand2Value, (left, right) => left * right);
                    break;
                case OpCode.Div:
                    ExecuteReadWrite(ins.Operand1, ins.Operand2, ins.Operand2Value, (left, right) => left / right);
                    break;
                case OpCode.Mod:
                    ExecuteReadWrite(ins.Operand1, ins.Operand2, ins.Operand2Value, (left, right) => left % right);
                    break;
                case OpCode.Eql:
                    ExecuteReadWrite(ins.Operand1, ins.Operand2, ins.Operand2Value, (left, right) => left == right ? 1 : 0);
                    break;
            }

            if (RequireXEqlWWherePossible && ins.ToString() == "Eql X W")
            {
                Instruction previousIns = Program[InstructionPtr - 2];
                if (previousIns.OpCode == OpCode.Add && previousIns.Operand2Value < 10)
                {
                    //It's possible that X now matches W (= the input digit), stop processing now if this is not the case.
                    if (X != 1)
                        return false;
                }
            }

            return true;
        }

        private void ExecuteReadFromInput(Register operand1)
        {
            if (Cursor >= Input.Length)
                throw new InvalidOperationException("No more input available.");

            _registers[(int)operand1] = Input[Cursor++] - '0';
        }

        private void ExecuteReadWrite(Register operand1, Register? operand2, int? operand2Value, Func<long, long, long> operation)
        {
            long op1 = _registers[(int)operand1];
            long op2 = (operand2 != null) ? _registers[(int)operand2] : operand2Value.Value;
            long result = operation(op1, op2);
            _registers[(int)operand1] = result;
        }

        public long? Run(string input, int? uptoLineNr = null)
        {
            Reset(input);

            //Console.WriteLine($"Running program with input {input}");

            foreach (Instruction instruction in Program)
            {
                InstructionPtr++;
                bool shouldContinue = Execute(instruction);

                if (shouldContinue == false)
                    return null;

                if (InstructionPtr == uptoLineNr)
                    break;
            }

            return Z;
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/24
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"*** Program for {typeof(Program).FullName.Split('.').First()} ***");

            Part1();
            //Console.WriteLine($"Result: {result}");

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void Part1()
        {
            List<Instruction> instructions = TextLines
                .Select(line => Instruction.Parse(line))
                .ToList();

            Processor processor = new Processor(instructions);
            processor.RequireXEqlWWherePossible = true;

            List<string> results = MonadRange(99999, 11111)
                .Select(input => new { Input = input, Result = processor.Run(input, uptoLineNr: 5 * 18 - 1) })
                .Where(ir => ir.Result != null)
                .Select(ir => ir.Input)
                .SelectMany(input => MonadRange(999, 111)
                    .Select(input2 => new { Input = input + input2, Result = processor.Run(input + input2, uptoLineNr: 8 * 18 - 1) })
                    .Where(ir => ir.Result != null)
                    .Select(ir => ir.Input)
                    .SelectMany(input2 => MonadRange(99, 11)
                        .Select(input3 => new { Input = input2 + input3, Result = processor.Run(input2 + input3, uptoLineNr: 10 * 18 - 1) })
                        .Where(ir => ir.Result != null)
                        .Select(ir => ir.Input)
                        .SelectMany(input3 => MonadRange(99, 11)
                            .Select(input4 => new { Input = input3 + input4, Result = processor.Run(input3 + input4, uptoLineNr: 12 * 18 - 1) })
                            .Where(ir => ir.Result != null)
                            .Select(ir => ir.Input)
                            .SelectMany(input4 => MonadRange(99, 11)
                                .Select(input5 => new { Input = input4 + input5, Result = processor.Run(input4 + input5) })
                                .Where(ir => ir.Result == 0)
                                .Select(ir => ir.Input)))))
                .Take(1).ToList();

            string result = results.First();
            Console.WriteLine($"Result: {result}");
        }

        private static IEnumerable<string> MonadRange(long from, long upToIncluding)
        {
            if(from <= upToIncluding)
            {
                for(long index = from; index <= upToIncluding; index++)
                {
                    string result = index.ToString();
                    if (result.Contains('0') == false)
                        yield return result;
                }
            }
            else
            {
                for (long index = from; index >= upToIncluding; index--)
                {
                    string result = index.ToString();
                    if (result.Contains('0') == false)
                        yield return result;
                }
            }
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

        private static string[] TextLines = new string[]
        {
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 1",
            "add x 12",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 6",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 1",
            "add x 11",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 12",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 1",
            "add x 10",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 5",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 1",
            "add x 10",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 10",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 26",
            "add x -16",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 7",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 1",
            "add x 14",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 0",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 1",
            "add x 12",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 4",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 26",
            "add x -4",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 12",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 1",
            "add x 15",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 14",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 26",
            "add x -7",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 13",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 26",
            "add x -8",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 10",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 26",
            "add x -4",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 11",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 26",
            "add x -15",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 9",
            "mul y x",
            "add z y",
            "inp w",
            "mul x 0",
            "add x z",
            "mod x 26",
            "div z 26",
            "add x -8",
            "eql x w",
            "eql x 0",
            "mul y 0",
            "add y 25",
            "mul y x",
            "add y 1",
            "mul z y",
            "mul y 0",
            "add y w",
            "add y 9",
            "mul y x",
            "add z y"
        };

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

    }
}
