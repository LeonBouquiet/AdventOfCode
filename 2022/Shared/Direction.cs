﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
	public enum Direction
	{
		Left   = 0,
		Right  = 1,
		Up     = 2,
		Down   = 3
	}

	public static class DirectionHelper
	{
		public static readonly Direction[] AllDirections = new Direction[]
		{
			Direction.Left, Direction.Right, Direction.Up, Direction.Down
		};

		public static readonly (int, int)[] DisplacementsByDirection = new (int, int)[]
		{
			(-1, 0), (1, 0), (0, -1), (0, 1)
		};

		public static Direction Opposite(Direction dir)
		{
			return (Direction)(((int)dir) ^ 1);
		}
	}
}
