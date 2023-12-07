using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Transactions;
using Shared;

namespace Day07
{
	public enum Card
	{
		C2 = 2, C3, C4, C5, C6, C7, C8, C9, CT, CJ, CQ, CK, CA
	}

	public enum Strength
	{
		HighCard = 1,
		OnePair,
		TwoPair,
		ThreeOfAKind,
		FullHouse,
		FourOfAKind,
		FiveOfAKind,
	}

	public class Hand: IComparable<Hand>
	{
		public Card[] Cards { get; set; }

		public int Bid { get; set; }

		public Strength Strength { get; set; }

		public Hand(Card[] cards, int bid) 
		{
			Cards = cards;
			Bid = bid;
			Strength = DetermineStrength();
		}

		public static Hand Parse(string line)
		{
			string[] parts = line.Split(' ', StringSplitOptions.TrimEntries);
			return new Hand(
				parts[0].ToCharArray().Select(c => Enum.Parse<Card>("C" + c)).ToArray(),
				Int32.Parse(parts[1]));
		}

		private Strength DetermineStrength()
		{
			var groupedCards = Cards
				.GroupBy(c => c)
				.OrderByDescending(grp => grp.Count())
				.ToArray();

			if (groupedCards[0].Count() == 5)
				return Strength.FiveOfAKind;
			else if (groupedCards[0].Count() == 4)
				return Strength.FourOfAKind;
			else if (groupedCards[0].Count() == 3 && groupedCards[1].Count() == 2)
				return Strength.FullHouse;
			else if (groupedCards[0].Count() == 3)
				return Strength.ThreeOfAKind;
			else if (groupedCards[0].Count() == 2 && groupedCards[1].Count() == 2)
				return Strength.TwoPair;
			else if (groupedCards[0].Count() == 2)
				return Strength.OnePair;
			else
				return Strength.HighCard;
		}

		/// <summary>
		/// Returns the value for (this - other)
		/// </summary>
		public int CompareTo(Hand? other)
		{
			if (other != null)
			{
				if (this.Strength != other.Strength)
					return this.Strength - other.Strength;

				for(int index = 0; index < Cards.Length; index++)
				{
					if (this.Cards[index] != other.Cards[index])
						return this.Cards[index] - other.Cards[index];
				}

				return 0;
			}
			else
				return -1;
		}

		public override string ToString()
		{
			return $"{string.Join(',', Cards)}, Strength={Strength}, Bid={Bid}";
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			Part1();
			Part2();
		}

		private static void Part1()
		{
			List<Hand> hands = InputReader.Read<Program>()
				.Select(line => Hand.Parse(line))
				.ToList();

			hands.Sort();

			long result = 0;
			for (int index = 0; index < hands.Count; index++)
				result += (index + 1) * hands[index].Bid;

			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			var result = InputReader.Read<Program>();

			Console.WriteLine($"The result of part 2 is: {result}");
		}

		private static Card ParseCard(char c)
		{
			return Enum.Parse<Card>("C" + c);
		}

	}
}
