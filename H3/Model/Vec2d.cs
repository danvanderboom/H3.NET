using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public struct Vec2d : IEquatable<Vec2d>
    {
        public double x, y;

        public Vec2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /*
         * Calculates the magnitude of a 2D cartesian vector.
         * @param v The 2D cartesian vector.
         * @return The magnitude of the vector.
         */
        public static double _v2dMag(Vec2d v) => Math.Sqrt(v.x * v.x + v.y * v.y);

        public double Magnitude => Math.Sqrt(x * x + y * y);

        /*
         * Finds the intersection between two lines. Assumes that the lines intersect
         * and that the intersection is not at an endpoint of either line.
         * @param p0 The first endpoint of the first line.
         * @param p1 The second endpoint of the first line.
         * @param p2 The first endpoint of the second line.
         * @param p3 The second endpoint of the second line.
         * @param inter The intersection point.
         */
        public static void _v2dIntersect(Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3, ref Vec2d inter)
        {
            Vec2d s1, s2;
            s1.x = p1.x - p0.x;
            s1.y = p1.y - p0.y;
            s2.x = p3.x - p2.x;
            s2.y = p3.y - p2.y;

            var t = (s2.x * (p0.y - p2.y) - s2.y * (p0.x - p2.x)) / (-s2.x * s1.y + s1.x * s2.y);

            inter.x = p0.x + (t * s1.x);
            inter.y = p0.y + (t * s1.y);
        }

        /*
         * Whether two 2D vectors are equal. Does not consider possible false
         * negatives due to floating-point errors.
         * @param v1 First vector to compare
         * @param v2 Second vector to compare
         * @return Whether the vectors are equal
         */
        public static bool _v2dEquals(Vec2d v1, Vec2d v2) => v1.x == v2.x && v1.y == v2.y;

        public bool Equals(Vec2d other) => x == other.x && y == other.y;

        public override bool Equals(object o) => Equals((Vec2d)o);

        public override int GetHashCode() => $"{x},{y}".GetHashCode();

        public static bool operator ==(Vec2d lhs, Vec2d rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vec2d lhs, Vec2d rhs) => !(lhs == rhs);
    }
}
