using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Core.Data
{
    public partial struct Point
    {
        public readonly int x;
        public readonly int y;
        //public readonly int z;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
            //this.z = z;
        }

        public Point(Point point)
        {
            this.x = point.x;
            this.y = point.y;
        }

        public Point Sign { get { return new Point(Math.Sign(x), Math.Sign(y));} }
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
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            unchecked { return (x * 397) ^ y; }
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }

        public static Point Zero { get { return new Point(0, 0); } }
        public static Point One { get { return new Point(1, 1); } }

        public static Point Up { get { return new Point(0, 1); } }
        public static Point Down { get { return new Point(0, -1); } }
        public static Point Left { get { return new Point(-1, 0); } }
        public static Point Right { get { return new Point(1, 0); } }
    }

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

        public bool IsDirection { get { return x == 0 || y == 0; } }

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
            var xMax = points.Max(point => point.x);
            var yMax = points.Max(point => point.y);
            var xMin = points.Min(point => point.x);
            var yMin = points.Min(point => point.y);

            return new Point(xMax - xMin, yMax - yMin);
        }

        public static Point Next(Point pos, int length, Direction direction)
        {
            return pos + direction.ToPoint()*length;
        }

        public static Point Abs(Point point)
        {
            return new Point(Math.Abs(point.x), Math.Abs(point.y));
        }

        public static Point Round(Vector2 pos)
        {
            return new Point((int)Math.Round(pos.x), (int)Math.Round(pos.y));
        }

        public static implicit operator Vector3(Point v)
        {
            return new Vector3(v.x, v.y);
        }

        public static implicit operator Vector2(Point v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.x - p2.x, p1.y - p2.y);
        }

        public static Point operator *(Point p1, int f1)
        {
            return new Point(p1.x * f1, p1.y * f1);
        }

        public static Point operator *(int f1, Point p1)
        {
            return new Point(p1.x * f1, p1.y * f1);
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
}
