using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day16
{
    public class BitStream
    {
        private string _bits;

        private int _position;

        public int Position => _position;

        public BitStream(string hex)
        {
            StringBuilder sbResult = new StringBuilder();
            for(int index = 0; index < hex.Length; index++)
            {
                int i = Convert.ToInt32(hex.Substring(index, 1), fromBase: 16);
                string s = Convert.ToString(i, toBase: 2);
                string leadingZeroes = "0000".Substring(0, 4 - s.Length);
                sbResult.Append(leadingZeroes + s);
            }

            _bits = sbResult.ToString();
            _position = 0;
        }

        public int ReadNext(int nrOfBits)
        {
            if (nrOfBits > 32)
                throw new ArgumentException("nrOfBits");

            int result = Convert.ToInt32(_bits.Substring(_position, nrOfBits), fromBase: 2);
            _position += nrOfBits;

            return result;
        }
    }

    public enum PacketType
    {
        Sum = 0,
        Product = 1,
        Minimum = 2,
        Maximum = 3,
        Literal = 4,
        GreaterThan = 5,
        LessThan = 6,
        EqualTo = 7
    }

    public abstract class Packet
    {
        public int Version { get; private set; }
        public PacketType PacketType { get; private set; }

        public List<Packet> ChildPackets { get; protected set; }

        public string ChildPacketsDisplay
        {
            get
            {
                return string.Join(", ", ChildPackets.Select(cp => cp.Value.ToString()));
            }
        }

        public IEnumerable<Packet> ThisAndDescendantPackets
        {
            get
            {
                return (new Packet[] { this }).Concat(ChildPackets.SelectMany(cp => cp.ThisAndDescendantPackets));
            }
        }

        public abstract long Value { get; }

        public Packet(int version, PacketType packetType)
        {
            Version = version;
            PacketType = packetType;
            ChildPackets = new List<Packet>();
        }

        public static Packet Read(BitStream bitStream)
        {
            int version = bitStream.ReadNext(3);
            PacketType packetType = (PacketType)bitStream.ReadNext(3);

            switch (packetType)
            {
                case PacketType.Literal:
                    return LiteralPacket.Read(version, packetType, bitStream);
                default:
                    return OperatorPacket.Read(version, packetType, bitStream);
            }
        }
    }

    public class LiteralPacket: Packet
    {
        public long Number { get; private set; }

        public override long Value => Number;

        protected LiteralPacket(int version, PacketType packetType, long number): 
            base(version, packetType)
        {
            Number = number;
        }

        public static LiteralPacket Read(int version, PacketType packetType, BitStream bitStream)
        {
            long number = 0;
            bool continueReading;
            do
            {
                continueReading = (bitStream.ReadNext(1) == 1);
                long nibble = bitStream.ReadNext(4);
                number = (number << 4) + nibble;
            } 
            while (continueReading == true);

            return new LiteralPacket(version, packetType, number);
        }
    }

    public class OperatorPacket: Packet
    {
        protected OperatorPacket(int version, PacketType packetType) :
            base(version, packetType)
        {
            ChildPackets = new List<Packet>();
        }

        public override long Value
        {
            get
            {
                switch (this.PacketType)
                {
                    case PacketType.Sum:
                        return ChildPackets.Sum(cp => cp.Value);
                    case PacketType.Product:
                        return ChildPackets.Aggregate(1L, (acc, cp) => acc *= cp.Value);
                    case PacketType.Minimum:
                        return ChildPackets.Min(cp => cp.Value);
                    case PacketType.Maximum:
                        return ChildPackets.Max(cp => cp.Value);
                    case PacketType.GreaterThan:
                        return (ChildPackets.First().Value > ChildPackets.Last().Value) ? 1 : 0;
                    case PacketType.LessThan:
                        return (ChildPackets.First().Value < ChildPackets.Last().Value) ? 1 : 0;
                    case PacketType.EqualTo:
                        return (ChildPackets.First().Value == ChildPackets.Last().Value) ? 1 : 0;
                    default:
                        return -1;
                }
            }
        }

        public static OperatorPacket Read(int version, PacketType packetType, BitStream bitStream)
        {
            OperatorPacket result = new OperatorPacket(version, packetType);

            int lenghtTypeId = bitStream.ReadNext(1);
            if(lenghtTypeId == 0)
            {
                int bitsRemaining = bitStream.ReadNext(15);

                do
                {
                    int startPosition = bitStream.Position;
                    result.ChildPackets.Add(Packet.Read(bitStream));

                    bitsRemaining -= (bitStream.Position - startPosition);
                } while (bitsRemaining > 0);
            }
            else
            {
                int packetCount = bitStream.ReadNext(11);
                for (int index = 0; index < packetCount; index++)
                    result.ChildPackets.Add(Packet.Read(bitStream));
            }

            return result;
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/16
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
            List<string> textLines = ReadInput();
            BitStream input = new BitStream(textLines.First());
            
            Packet topLevel = Packet.Read(input);
            List<Packet> allPackets = topLevel.ThisAndDescendantPackets.ToList();

            int result = allPackets.Sum(p => p.Version);
            return result;
        }

        public static long Part2()
        {
            List<string> textLines = ReadInput();
            BitStream input = new BitStream(textLines.First());

            Packet topLevel = Packet.Read(input);
            long result = topLevel.Value;

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
}
