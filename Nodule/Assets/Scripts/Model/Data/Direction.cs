using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public static class DirectionExtensions
    {
        private static readonly IDictionary<Direction, Direction> Opposites =
            new Dictionary<Direction, Direction>
            {
                { Direction.Up, Direction.Down },
                { Direction.Down, Direction.Up },
                { Direction.Left, Direction.Right },
                { Direction.Right, Direction.Left },
                { Direction.None, Direction.None }
            };

        private static readonly IDictionary<Direction, Point> Points =
            new Dictionary<Direction, Point>
            {
                { Direction.Up, Point.Up },
                { Direction.Down, Point.Down },
                { Direction.Left, Point.Left },
                { Direction.Right, Point.Right },
                { Direction.None, Point.Zero }
            };

        private static readonly IDictionary<Direction, Vector3> Vectors =
            new Dictionary<Direction, Vector3>
            {
                { Direction.Up, Vector3.up },
                { Direction.Down, Vector3.down },
                { Direction.Left, Vector3.left },
                { Direction.Right, Vector3.right },
                { Direction.None, Vector3.zero }
            };

        private static readonly IDictionary<Direction, Quaternion> Rotations =
            new Dictionary<Direction, Quaternion>
            {
                { Direction.Up, Quaternion.Euler(0, 0, 90) },
                { Direction.Down, Quaternion.Euler(0, 0, 270) },
                { Direction.Left, Quaternion.Euler(0, 0, 180) },
                { Direction.Right, Quaternion.Euler(0, 0, 0) },
                { Direction.None, Quaternion.Euler(0, 0, 0) }
            };

        private static readonly IDictionary<Direction, Direction> RotatedDirections =
            new Dictionary<Direction, Direction>
            {
                { Direction.Up, Direction.Left },
                { Direction.Down, Direction.Right },
                { Direction.Left, Direction.Down },
                { Direction.Right, Direction.Up },
                { Direction.None, Direction.None }
            };

        public static Direction Opposite(this Direction direction)
        {
            return Opposites[direction];
        }

        public static Direction Rotated(this Direction direction, int dir)
        {
            while (true)
            {
                if (dir == 0) return direction;
                dir = dir > 0 ? dir - 1 : dir + 1;
                direction = RotatedDirections[direction];
            }
        }

        public static Point ToPoint(this Direction direction)
        {
            return Points[direction];
        }

        public static Vector3 Vector(this Direction direction)
        {
            return Vectors[direction];
        }

        public static Quaternion Rotation(this Direction direction)
        {
            return Rotations[direction];
        }

        public static bool IsHorizontal(this Direction direction)
        {
            return direction == Direction.Left || direction == Direction.Right;
        }

        public static bool IsVertical(this Direction direction)
        {
            return direction == Direction.Up || direction == Direction.Down;
        }

        public static bool IsDownLeft(this Direction direction)
        {
            return direction == Direction.Down || direction == Direction.Left;
        }
    }

    public static class Directions
    {
        private static readonly List<Direction> DirectionList =
            new List<Direction>
            {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            };

        public static List<Direction> All { get { return new List<Direction>(DirectionList);} }

        public static IEnumerable<Direction> Orthagonal(Direction direction)
        {
            return new List<Direction> { direction.Rotated(1), direction.Rotated(-1) };
        }
    }
}