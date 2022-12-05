using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day18
{
    public class Node
    {
        public Node Parent { get; private set; }

        public Node LeftChild { get; private set; }

        public Node RightChild { get; private set; }

        public int? Number { get; private set; }

        public bool IsRegular => (Number != null);

        public Node Root
        {
            get
            {
                Node current = this;
                while (current.Parent != null)
                    current = current.Parent;

                return current;
            }
        }

        public int Depth => (Parent == null) ? 0 : Parent.Depth + 1;

        public long Magnitude => IsRegular ? Number.Value : (3 * LeftChild.Magnitude) + (2 * RightChild.Magnitude);

        public IEnumerable<Node> InOrderSubtree
        {
            get
            {
                if (this.IsRegular)
                    return new Node[] { this };
                else
                    return LeftChild.InOrderSubtree.Concat(new Node[] { this }).Concat(RightChild.InOrderSubtree);
            }
        }

        public Node(Node parent)
        {
            Parent = parent;
        }

        public Node Clone(Node clonedParent)
        {
            Node result = new Node(clonedParent);
            if (IsRegular)
            {
                result.Number = this.Number;
            }
            else
            {
                result.LeftChild = this.LeftChild.Clone(result);
                result.RightChild = this.RightChild.Clone(result);
            }

            return result;
        }

        public static Node Parse(string text)
        {
            int cursor = 0;
            return Parse(null, text, ref cursor);
        }

        public static Node Parse(Node parent, string text, ref int cursor)
        {
            Node result = new Node(parent);
            if(text[cursor] == '[')
            {
                cursor++;
                result.LeftChild = Parse(result, text, ref cursor);
                Debug.Assert(text[cursor] == ',');
                cursor++;
                result.RightChild = Parse(result, text, ref cursor);
                Debug.Assert(text[cursor] == ']');
            }
            else
            {
                result.Number = Int32.Parse("" + text[cursor]);
            }

            cursor++;
            return result;
        }

        public void Explode()
        {
            Debug.Assert(LeftChild != null && LeftChild.IsRegular);
            Debug.Assert(RightChild != null && RightChild.IsRegular);

            List<Node> nodesInOrder = this.Root.InOrderSubtree.ToList();
            int targetIndex = nodesInOrder.IndexOf(this);

            for (int leftIndex = targetIndex - 2; leftIndex >= 0; leftIndex--)
            {
                if (nodesInOrder[leftIndex].IsRegular)
                {
                    nodesInOrder[leftIndex].Number += this.LeftChild.Number;
                    break;
                }
            }

            for (int rightIndex = targetIndex + 2; rightIndex < nodesInOrder.Count; rightIndex++)
            {
                if (nodesInOrder[rightIndex].IsRegular)
                {
                    nodesInOrder[rightIndex].Number += this.RightChild.Number;
                    break;
                }
            }

            LeftChild = null;
            RightChild = null;
            Number = 0;
        }

        public void Split()
        {
            Debug.Assert(IsRegular);

            LeftChild = new Node(this);
            LeftChild.Number = (int)Math.Floor(Number.Value / 2.0f);
            RightChild = new Node(this);
            RightChild.Number = (int)Math.Ceiling(Number.Value / 2.0f);
            Number = null;
        }

        public static Node AddAndReduce(Node left, Node right)
        {
            Node result = Node.Add(left, right);
            result.Reduce();

            return result;
        }

        public static Node Add(Node left, Node right)
        {
            Node result = new Node(null);
            result.LeftChild = left.Clone(result);
            result.LeftChild.Parent = result;
            result.RightChild = right.Clone(result);
            result.RightChild.Parent = result;

            return result;
        }

        public void Reduce()
        {
            while (true)
            {
                Node atDepthFour = this.InOrderSubtree.FirstOrDefault(n => n.Depth >= 4 && n.LeftChild?.Number != null && n.RightChild?.Number != null);
                if (atDepthFour != null)
                {
                    atDepthFour.Explode();
                }
                else
                {
                    Node tenOrGreater = this.InOrderSubtree.FirstOrDefault(n => n.IsRegular && n.Number >= 10);
                    if (tenOrGreater != null)
                        tenOrGreater.Split();
                    else
                        break;
                }
            }
        }

        public override string ToString()
        {
            if (IsRegular)
                return $"{Number}";
            else
                return $"[{LeftChild},{RightChild}]";
        }
    }

    /// <summary>
    /// 
    /// See https://adventofcode.com/2021/day/18
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
            List<Node> nodes = textLines
                .Select(line => Node.Parse(line))
                .ToList();

            Node result = nodes[0];
            foreach(Node node in nodes.Skip(1))
                result = Node.AddAndReduce(result, node);

            Console.WriteLine(result);
            return result.Magnitude;
        }

        public static long Part2()
        {
            List<string> textLines = ReadInput();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Node> nodes = textLines
                .Select(line => Node.Parse(line))
                .ToList();

            long maxMagnitude = 0;
            string maxDescription = "";
            foreach(Node left in nodes)
            {
                foreach(Node right in nodes.Where(n => n.Equals(left) == false))
                {
                    Node sum = Node.AddAndReduce(left, right);
                    long magnitude = sum.Magnitude;
                    if(magnitude > maxMagnitude)
                    {
                        maxMagnitude = magnitude;
                        maxDescription = $"{left} + {right}";
                    }
                }
            }

            Console.WriteLine(maxDescription);
            stopwatch.Stop();
            Console.WriteLine($"Duration {stopwatch.ElapsedMilliseconds}ms.");

            return maxMagnitude;
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
