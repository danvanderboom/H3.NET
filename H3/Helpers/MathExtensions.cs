using System;
using System.Collections.Generic;
using System.Text;
using static H3.Constants;

namespace H3
{
    public static class MathExtensions
    {
        public const double M_SQRT7 = 2.6457513110645905905016157536392604257102;

        /// <summary>
        /// Efficient integer exponentiation.
        /// </summary>
        /// <param name="Base">The integer base</param>
        /// <param name="exp">The exponent</param>
        /// <returns>The exponentiated value</returns>
        public static int ipow(int Base, int exp)
        {
            int result = 1;
            while (exp != 0)
            {
                if ((exp & 1) != 0)
                    result *= Base;

                exp >>= 1;
                Base *= Base;
            }

            return result;
        }

        public static bool IsFinite(double d) => !double.IsInfinity(d) && !double.IsNaN(d);

        /// <summary>
        /// Convert from radians to decimal degrees.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>The corresponding decimal degrees.</returns>
        public static double RadiansToDegrees(double radians) => radians * M_180_PI;

        /// <summary>
        /// Convert from decimal degrees to radians.
        /// </summary>
        /// <param name="degrees">The decimal degrees.</param>
        /// <returns>The corresponding radians.</returns>
        public static double DegreesToRadians(double degrees) => degrees * M_PI_180;

        /// <summary>
        /// Ensures latitudes are in the proper bounds.
        /// </summary>
        /// <param name="lng">The origin latitude value</param>
        /// <returns>The corrected latitude value</returns>
        public static double ConstrainLatitude(double lat)
        {
            while (lat > M_PI_2)
                lat = lat - M_PI;

            return lat;
        }

        /// <summary>
        /// Ensures longitudes are in the proper bounds.
        /// </summary>
        /// <param name="lng">The origin longitude value</param>
        /// <returns>The corrected longitude value</returns>
        public static double ConstrainLongitude(double lng)
        {
            while (lng > M_PI)
                lng = lng - (2 * M_PI);

            while (lng < -M_PI)
                lng = lng + (2 * M_PI);

            return lng;
        }

        /// <summary>
        /// Normalizes radians to a value between 0.0 and two PI.
        /// </summary>
        /// <param name="rads"></param>
        /// <returns></returns>
        public static double PositiveAngleRadians(double rads)
        {
            double tmp = ((rads < 0.0) ? rads + M_2PI : rads);
            if (rads >= M_2PI)
                tmp -= M_2PI;
            return tmp;
        }
    }
}
