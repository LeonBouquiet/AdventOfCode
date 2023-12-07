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
		CX = 1, C2, C3, C4, C5, C6, C7, C8, C9, CT, CJ, CQ, CK, CA
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

		public Hand Clone()
		{
			return new Hand(Cards.ToArray(), Bid) { Strength = this.Strength };
		}

		public static Hand Parse(string line)
		{
			string[] parts = line.Split(' ', StringSplitOptions.TrimEntries);
			return new Hand(
				parts[0].ToCharArray().Select(c => Enum.Parse<Card>("C" + c)).ToArray(),
				Int32.Parse(parts[1]));
		}

		public static Hand ParseWithJokers(string line)
		{
			string[] parts = line.Split(' ', StringSplitOptions.TrimEntries);
			Hand hand = new Hand(
				parts[0].Replace('J', 'X').ToCharArray().Select(c => Enum.Parse<Card>("C" + c)).ToArray(),
				Int32.Parse(parts[1]));   //Joker is represented by an 'X'.

			List<Hand> permutations = hand.GenerateAllPermutationUsingJokers().ToList();
			if (permutations.Count > 0)
			{
				//Use the Strenght of the best permutation, but the Cards with the Jokers.
				permutations.Sort();
				hand.Strength = permutations.Last().Strength;
			}

			return hand;
		}

		private IEnumerable<Hand> GenerateAllPermutationUsingJokers()
		{
			Hand instance = this.Clone();
			for(int cardIndex = 0; cardIndex < Cards.Length; cardIndex++)
			{
				if (instance.Cards[cardIndex] == Card.CX)
					instance.Cards[cardIndex] = Card.C2;
			}

			bool performedIncrement;
			do
			{
				performedIncrement = false;

				for (int cardIndex = Cards.Length - 1; cardIndex >= 0; cardIndex--)
				{
					if (this.Cards[cardIndex] == Card.CX)
					{
						if (instance.Cards[cardIndex] < Card.CA)
						{
							//Current cardIndex can still be incremented.
							instance.Cards[cardIndex] = instance.Cards[cardIndex] + 1;
							if (instance.Cards[cardIndex] == Card.CJ)
								instance.Cards[cardIndex] = Card.CQ;	//Skip the J, doesn't exist here.

							instance.Strength = instance.DetermineStrength();
							performedIncrement = true;

							yield return instance.Clone();
						}
						else
						{
							//Reset current index back to C2 and move on to the next joker position, if any.
							instance.Cards[cardIndex] = Card.C2;
						}
					}
				}
			} while (performedIncrement);
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

				bool includesJokers = this.Cards.Any(c => c == Card.CX) || other.Cards.Any(c => c == Card.CX);
				for (int index = 0; index < Cards.Length; index++)
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
			//Part1();
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
			List<Hand> hands = InputReader.Read<Program>()
				.Select(line => Hand.ParseWithJokers(line))
				.ToList();

			hands.Sort();

			long result = 0;
			for (int index = 0; index < hands.Count; index++)
				result += (index + 1) * hands[index].Bid;

			//False: 248118448 (too low)
			//False: 248497077 (too high)
			Console.WriteLine($"The result of part 2 is: {result}");
		}
	}
}
