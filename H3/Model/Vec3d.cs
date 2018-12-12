using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public struct Vec3d
    {
        double x, y, z;

        public Vec3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Calculate the square of the distance between two 3D coordinates.
        /// </summary>
        /// <param name="v1">The first 3D coordinate.</param>
        /// <param name="v2">The second 3D coordinate.</param>
        /// <returns>The square of the distance between the given points.</returns>
        public static double _pointSquareDist(Vec3d v1, Vec3d v2) =>
            Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2) + Math.Pow(v1.z - v2.z, 2);
    }
}
