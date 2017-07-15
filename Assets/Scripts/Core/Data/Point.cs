using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Data
{
    /// <summary>
    /// A Point represents a position in 2D grid space. Useful for identifying nodes and arcs 
    /// in the game board grid.
    /// </summary>
    public partial struct Point
    {
        public readonly int X;
        public readonly int Y;
        //public readonly int z;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
            //this.z = z;
        }

        public Point(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }

        public Point Sign { get { return new Point(Math.Sign(X), Math.Sign(Y));} }
        public Point Next(int length, Direction direction)
        {
            return this + direction.ToPoint() * length;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point) obj);
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked { return (X * 397) ^ Y; }
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

        public static Point Zero { get { return new Point(0, 0); } }
        public static Point One { get { return new Point(1, 1); } }

        public static Point Up { get { return new Point(0, 1); } }
        public static Point Down { get { return new Point(0, -1); } }
        public static Point Left { get { return new Point(-1, 0); } }
        public static Point Right { get { return new Point(1, 0); } }
    }

    #region Extensions
    public partial struct Point
    {
        public static readonly Dictionary<Point, Direction> Directions =
            new Dictionary<Point, Direction>
        {
                { Down, Direction.Down },
                { Up, Direction.Up },
                { Right, Direction.Right },
                { Left, Direction.Left },
                { Zero, Direction.None }
        };

        public bool IsDirection { get { return X == 0 || Y == 0; } }

        public Direction ToDirection
        {
            get
            {
                Direction direction;
                return Directions.TryGetValue(this.Sign, out direction) ? direction : Direction.None;
            }
        }

        public static Point Boundary(ICollection<Point> points)
        {
            if (!points.Any()) return Zero;

            // Take the min and max of all (x,y) coordinates
            var xMax = points.Max(point => point.X);
            var yMax = points.Max(point => point.Y);
            var xMin = points.Min(point => point.X);
            var yMin = points.Min(point => point.Y);

            return new Point(xMax - xMin, yMax - yMin);
        }

        public static Point Next(Point pos, int length, Direction direction)
        {
            return pos + direction.ToPoint()*length;
        }

        public static Point Abs(Point point)
        {
            return new Point(Math.Abs(point.X), Math.Abs(point.Y));
        }

        public static Point Round(Vector2 pos)
        {
            return new Point((int)Math.Round(pos.x), (int)Math.Round(pos.y));
        }

        public static implicit operator Vector3(Point v)
        {
            return new Vector3(v.X, v.Y);
        }

        public static implicit operator Vector2(Point v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point p1, int f1)
        {
            return new Point(p1.X * f1, p1.Y * f1);
        }

        public static Point operator *(int f1, Point p1)
        {
            return new Point(p1.X * f1, p1.Y * f1);
        }
    }

    //public static class PointExtension
    //{
    //    public static Direction ToDirection(this Vector3 vector)
    //    {
    //        Direction direction;
    //        return Point.Directions.TryGetValue(GetSign(vector), out direction) ? direction : Direction.None;
    //    }

    //    private static Point GetSign(Vector3 vector)
    //    {
    //        return new Point(Math.Sign(vector.x), Math.Sign(vector.y));
    //    }
    //}

    #endregion Extensions
}
