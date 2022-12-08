using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
	public class Grid<TData>
	{
		public int Width { get; private set; }

		public int Height { get; private set; }

		private TData[,] _cells;

		public TData this[Location loc]
		{
			get { return _cells[loc.Y, loc.X]; }
			set { _cells[loc.Y, loc.X] = value; }
		}

		public TData this[int x, int y]
		{
			get { return _cells[y, x]; }
			set { _cells[y, x] = value; }
		}

		public IEnumerable<Location> Locations
		{
			get
			{
				for (int y = 0; y < Height; y++)
				{
					for (int x = 0; x < Width; x++)
						yield return new Location(x, y);
				}
			}
		}
		public Func<TData, string> FormatCell { get; set; } = (c => (c != null) ? c.ToString()! : " ");

		public string Display
		{
			get { return ToString(); }
		}

		public Grid(int width, int height, TData initialValue = default(TData))
		{
			Width = width;
			Height = height;

			_cells = new TData[height, width];

			Locations.ToList().ForEach(pos => this[pos] = initialValue);
		}

		public Grid(List<IEnumerable<TData>> cellsPerRow)
		{
			Width = cellsPerRow.First().Count();
			Height = cellsPerRow.Count;
			_cells = new TData[Height, Width];

			for (int y = 0; y < Height; y++)
			{
				IEnumerable<TData> row = cellsPerRow[y];
				int x = 0;
				foreach (TData cell in row)
				{
					_cells[y, x++] = cell;
				}
			}
		}

		public bool IsInBounds(Location loc)
		{
			return loc.X >= 0 && loc.X < Width && loc.Y >= 0 && loc.Y < Height;
		}

		public override string ToString()
		{
			Func<TData, string> formatCell = FormatCell ?? (c => (c != null) ? c.ToString()! : " ");
			StringBuilder sbResult = new StringBuilder();

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					sbResult.Append(formatCell(_cells[y, x]));
				}

				sbResult.AppendLine();
			}

			return sbResult.ToString();
		}
	}
}
