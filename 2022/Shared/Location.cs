using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{

	public class Location
	{
		public int X { get; private set; }
		public int Y { get; private set; }

		public Location(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override bool Equals(object? obj)
		{
			Location? other = obj as Location;
			if (other == null)
				return false;

			return (this.X == other.X && this.Y == other.Y);
		}

		public override int GetHashCode()
		{
			return Y * 1137 + X;
		}

		private static (int, int)[] CoreNeighboursDisplacements = new (int, int)[]
		{
			( 0, -1), (-1,  0), ( 0,  1), ( 1,  0),
		};


		public IEnumerable<Location> CoreNeighbours
		{
			get
			{
				return CoreNeighboursDisplacements.Select(disp => new Location(this.X + disp.Item1, this.Y + disp.Item2));
			}
		}

		private static (int, int)[] AllNeighboursDisplacements = new (int, int)[]
		{
			( 0, -1), (-1, -1), (-1,  0), (-1,  1), ( 0,  1), ( 1,  1), ( 1,  0), ( 1, -1),
		};


		public IEnumerable<Location> AllNeighbours
		{
			get
			{
				return AllNeighboursDisplacements.Select(disp => new Location(this.X + disp.Item1, this.Y + disp.Item2));
			}
		}

		public Location Displace(Direction dir)
		{
			(int, int) displacement = DirectionHelper.DisplacementsByDirection[(int)dir];
			return new Location(X + displacement.Item1, Y + displacement.Item2);
		}

		public Location Displace(int deltaX, int deltaY)
		{
			return new Location(X + deltaX, Y + deltaY);
		}

		public int ManhattanDistanceTo(Location other)
		{
			return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
		}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}
