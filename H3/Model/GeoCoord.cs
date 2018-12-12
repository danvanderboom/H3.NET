using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using static H3.Constants;
using static H3.MathExtensions;

namespace H3.Model
{
    public struct GeoCoord
    {
        /** epsilon of ~0.1mm in degrees */
        const double EPSILON_DEG = .000000001;

        /** epsilon of ~0.1mm in radians */
        const double EPSILON_RAD = (EPSILON_DEG * M_PI_180);

        public double latitude;  // latitude in radians
        public double longitude;  // longitude in radians

        public GeoCoord(GeoCoord g)
        {
            latitude = g.latitude;
            longitude = g.longitude;
        }

        public GeoCoord(double lat, double lon)
        {
            latitude = lat;
            longitude = lon;
        }

        /// <summary>
        /// Encodes a coordinate on the sphere to the FaceIJK address of the containing
        /// cell at the specified resolution.
        /// </summary>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <returns>The FaceIJK of the containing cell at resolution res.</returns>
        public FaceIJK ToFaceIJK(int res)
        {
            var ijk = new FaceIJK();

            // first convert to hex2d
            var v = this.ToHex2d(res, ijk.face);

            // then convert to ijk+
            ijk.coord = CoordIJK.FromHex2d(v);

            return ijk;
        } // _geoToFaceIjk


        //void _geoToFaceIjk(const GeoCoord* g, int res, FaceIJK* h) 
        //{
        //    // first convert to hex2d
        //    Vec2d v;
        //    _geoToHex2d(g, res, &h->face, &v);

        //    // then convert to ijk+
        //    _hex2dToCoordIJK(&v, &h->coord);
        //}




        /// <summary>
        /// Encodes a coordinate on the sphere to the H3 index of the containing cell at
        /// the specified resolution.
        /// </summary>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <returns>The encoded H3Index (or 0 on failure).</returns>
        public H3Index ToH3Index(int res)
        {
            if (res < 0 || res > MAX_H3_RES)
                return H3Index.H3_INVALID_INDEX;

            if (!IsFinite(latitude) || !IsFinite(longitude))
                return H3Index.H3_INVALID_INDEX;

            var fijk = this.ToFaceIJK(res);
            return fijk.ToH3Index(res);
        } // geoToH3

        /// <summary>
        /// Set the components of spherical coordinates in decimal degrees.
        /// </summary>
        /// <param name="latDegs">The desired latitidue in decimal degrees.</param>
        /// <param name="lonDegs">The desired longitude in decimal degrees.</param>
        public void setGeoDegs(double latDegs, double lonDegs)
        {
            _setGeoRads(DegreesToRadians(latDegs), DegreesToRadians(lonDegs));
        }

        /// <summary>
        /// Set the components of spherical coordinates in radians.
        /// </summary>
        /// <param name="latRads">The desired latitidue in decimal radians.</param>
        /// <param name="lonRads">The desired longitude in decimal radians.</param>
        public void _setGeoRads(double latRads, double lonRads)
        {
            latitude = latRads;
            longitude = lonRads;
        }

        /*
         * Determines if the components of two spherical coordinates are within some
         * threshold distance of each other.
         *
         * @param p1 The first spherical coordinates.
         * @param p2 The second spherical coordinates.
         * @param threshold The threshold distance.
         * @return Whether or not the two coordinates are within the threshold distance
         *         of each other.
         */
        public static bool AlmostEqualThreshold(GeoCoord p1, GeoCoord p2, double threshold)
        {
            return Math.Abs(p1.latitude - p2.latitude) < threshold
                && Math.Abs(p1.longitude - p2.longitude) < threshold;
        }


        /*
         * Determines if the components of two spherical coordinates are within our
         * standard epsilon distance of each other.
         *
         * @param p1 The first spherical coordinates.
         * @param p2 The second spherical coordinates.
         * @return Whether or not the two coordinates are within the epsilon distance
         *         of each other.
         */
        public static bool AlmostEqual(GeoCoord p1, GeoCoord p2)
        {
            return AlmostEqualThreshold(p1, p2, EPSILON_RAD);
        }

        

        /*
         * Find the great circle distance in radians between two spherical coordinates.
         *
         * @param p1 The first spherical coordinates.
         * @param p2 The second spherical coordinates.
         * @return The great circle distance in radians between p1 and p2.
         */
        public static double _geoDistRads(GeoCoord p1, GeoCoord p2)
        {
            // use spherical triangle with p1 at A, p2 at B, and north pole at C
            double bigC = Math.Abs(p2.longitude - p1.longitude);
            if (bigC > M_PI)  // assume we want the complement
            {
                // note that in this case they can't both be negative
                double lon1 = p1.longitude;
                if (lon1 < 0.0)
                    lon1 += 2.0 * M_PI;

                double lon2 = p2.longitude;
                if (lon2 < 0.0)
                    lon2 += 2.0 * M_PI;

                bigC = Math.Abs(lon2 - lon1);
            }

            double b = M_PI_2 - p1.latitude;
            double a = M_PI_2 - p2.latitude;

            // use law of cosines to find c
            double cosc = Math.Cos(a) * Math.Cos(b) + Math.Sin(a) * Math.Sin(b) * Math.Cos(bigC);

            if (cosc > 1.0)
                cosc = 1.0;

            if (cosc < -1.0)
                cosc = -1.0;

            return Math.Acos(cosc);
        }

        /*
         * Find the great circle distance in kilometers between two spherical
         * coordinates.
         *
         * @param p1 The first spherical coordinates.
         * @param p2 The second spherical coordinates.
         * @return The distance in kilometers between p1 and p2.
         */
        public static double _geoDistKm(GeoCoord p1, GeoCoord p2) => EARTH_RADIUS_KM * _geoDistRads(p1, p2);

        /*
         * Determines the azimuth to p2 from p1 in radians.
         *
         * @param p1 The first spherical coordinates.
         * @param p2 The second spherical coordinates.
         * @return The azimuth in radians from p1 to p2.
         */
        public static double _geoAzimuthRads(GeoCoord p1, GeoCoord p2)
        {
            return Math.Atan2(Math.Cos(p2.latitude) * Math.Sin(p2.longitude - p1.longitude),
                 Math.Cos(p1.latitude) * Math.Sin(p2.latitude) - Math.Sin(p1.latitude) * Math.Cos(p2.latitude) * Math.Cos(p2.longitude - p1.longitude));
        }

        /// <summary>
        /// Computes the point on the sphere a specified azimuth and distance from another point.
        /// </summary>
        /// <param name="p1">The first spherical coordinates.</param>
        /// <param name="az">The desired azimuth from p1.</param>
        /// <param name="distance">The desired distance from p1, must be non-negative.</param>
        /// <returns>The spherical coordinates at the desired azimuth and distance from p1.</returns>
        public static GeoCoord _geoAzDistanceRads(GeoCoord p1, double az, double distance)
        {
            if (distance < EPSILON)
                return new GeoCoord(p1);

            var p2 = new GeoCoord();

            double sinlat, sinlon, coslon;

            az = PositiveAngleRadians(az);

            // check for due north/south azimuth
            if (az < EPSILON || Math.Abs(az - M_PI) < EPSILON)
            {
                if (az < EPSILON)  // due north
                    p2.latitude = p1.latitude + distance;
                else  // due south
                    p2.latitude = p1.latitude - distance;

                if (Math.Abs(p2.latitude - M_PI_2) < EPSILON)  // north pole
                {
                    p2.latitude = M_PI_2;
                    p2.longitude = 0.0;
                }
                else if (Math.Abs(p2.latitude + M_PI_2) < EPSILON)  // south pole
                {
                    p2.latitude = -M_PI_2;
                    p2.longitude = 0.0;
                }
                else
                    p2.longitude = ConstrainLongitude(p1.longitude);
            }
            else  // not due north or south
            {
                sinlat = Math.Sin(p1.latitude) * Math.Cos(distance) +
                         Math.Cos(p1.latitude) * Math.Sin(distance) * Math.Cos(az);

                if (sinlat > 1.0) sinlat = 1.0;
                if (sinlat < -1.0) sinlat = -1.0;
                p2.latitude = Math.Asin(sinlat);

                if (Math.Abs(p2.latitude - M_PI_2) < EPSILON)  // north pole
                {
                    p2.latitude = M_PI_2;
                    p2.longitude = 0.0;
                }
                else if (Math.Abs(p2.latitude + M_PI_2) < EPSILON)  // south pole
                {
                    p2.latitude = -M_PI_2;
                    p2.longitude = 0.0;
                }
                else
                {
                    sinlon = Math.Sin(az) * Math.Sin(distance) / Math.Cos(p2.latitude);
                    coslon = (Math.Cos(distance) - Math.Sin(p1.latitude) * Math.Sin(p2.latitude)) / Math.Cos(p1.latitude) / Math.Cos(p2.latitude);
                    if (sinlon > 1.0) sinlon = 1.0;
                    if (sinlon < -1.0) sinlon = -1.0;
                    if (coslon > 1.0) sinlon = 1.0;
                    if (coslon < -1.0) sinlon = -1.0;
                    p2.longitude = ConstrainLongitude(p1.longitude + Math.Atan2(sinlon, coslon));
                }
            }

            return p2;
        }

        /// <summary>
        /// Encodes a coordinate on the sphere to the corresponding icosahedral face and
        /// containing 2D hex coordinates relative to that face center.
        /// </summary>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <param name="face">The icosahedral face containing the spherical coordinates.</param>
        /// <returns>The 2D hex coordinates of the cell containing the point.</returns>
        public Vec2d ToHex2d(int res, int face)
        {
            var v = new Vec2d();

            var v3d = this.ToVec3d();

            // determine the icosahedron face
            face = 0;
            double sqd = Vec3d._pointSquareDist(FaceIJK.faceCenterPoint[0], v3d);
            for (int f = 1; f < NUM_ICOSA_FACES; f++)
            {
                double sqdT = Vec3d._pointSquareDist(FaceIJK.faceCenterPoint[f], v3d);
                if (sqdT < sqd)
                {
                    face = f;
                    sqd = sqdT;
                }
            }

            // cos(r) = 1 - 2 * sin^2(r/2) = 1 - 2 * (sqd / 4) = 1 - sqd/2
            double r = Math.Acos(1 - sqd / 2);

            if (r < EPSILON)
            {
                v.x = v.y = 0.0;
                return v;
            }

            // now have face and r, now find CCW theta from CII i-axis
            double theta = PositiveAngleRadians(FaceIJK.faceAxesAzRadsCII[face][0] - PositiveAngleRadians(GeoCoord._geoAzimuthRads(FaceIJK.faceCenterGeo[face], this)));

            // adjust theta for Class III (odd resolutions)
            if (H3Index.isResClassIII(res))
                theta = PositiveAngleRadians(theta - M_AP7_ROT_RADS);

            // perform gnomonic scaling of r
            r = Math.Tan(r);

            // scale for current resolution length u
            r /= RES0_U_GNOMONIC;
            for (int i = 0; i < res; i++)
                r *= M_SQRT7;

            // we now have (r, theta) in hex2d with theta ccw from x-axes

            // convert to local x,y
            v.x = r * Math.Cos(theta);
            v.y = r * Math.Sin(theta);

            return v;
        }

        /// <summary>
        /// Determines the center point in spherical coordinates of a cell given by 2D
        /// hex coordinates on a particular icosahedral face.
        /// </summary>
        /// <param name="v">The 2D hex coordinates of the cell.</param>
        /// <param name="face">The icosahedral face upon which the 2D hex coordinate system is centered.</param>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <param name="isSubstrate">Indicates whether or not this grid is actually a substrate
        /// grid relative to the specified resolution.</param>
        /// <returns>The spherical coordinates of the cell center point.</returns>
        public static GeoCoord FromHex2d(Vec2d v, int face, int res, bool isSubstrate)
        {
            var g = new GeoCoord();

            // calculate (r, theta) in hex2d
            double r = Vec2d._v2dMag(v);

            if (r < EPSILON)
                return FaceIJK.faceCenterGeo[face];

            double theta = Math.Atan2(v.y, v.x);

            // scale for current resolution length u
            for (int i = 0; i < res; i++)
                r /= M_SQRT7;

            // scale accordingly if this is a substrate grid
            if (isSubstrate)
            {
                r /= 3.0;
                if (H3Index.isResClassIII(res)) r /= M_SQRT7;
            }

            r *= RES0_U_GNOMONIC;

            // perform inverse gnomonic scaling of r
            r = Math.Atan(r);

            // adjust theta for Class III
            // if a substrate grid, then it's already been adjusted for Class III
            if (!isSubstrate && H3Index.isResClassIII(res))
                theta = PositiveAngleRadians(theta + M_AP7_ROT_RADS);

            // find theta as an azimuth
            theta = PositiveAngleRadians(FaceIJK.faceAxesAzRadsCII[face][0] - theta);

            // now find the point at (r,theta) from the face center
            g = GeoCoord._geoAzDistanceRads(FaceIJK.faceCenterGeo[face], theta, r);

            return g;
        }

        /// <summary>
        /// Calculate the 3D coordinate on unit sphere from the latitude and longitude.
        /// </summary>
        /// <param name="geo">The latitude and longitude of the point.</param>
        /// <param name="v">The 3D coordinate of the point.</param>
        public Vec3d ToVec3d() => new Vec3d(
            x: Math.Cos(longitude) * Math.Cos(latitude),
            y: Math.Sin(longitude) * Math.Cos(latitude),
            z: Math.Sin(latitude));

        #region Reference Data

        /*
         * The following functions provide meta information about the H3 hexagons at
         * each zoom level. Since there are only 16 total levels, these are current
         * handled with hardwired static values, but it may be worthwhile to put these
         * static values into another file that can be autogenerated by source code in
         * the future.
         */

        public static double hexAreaKm2(int res)
        {
            double[] areas =
            {
                4250546.848, 607220.9782, 86745.85403, 12392.26486,
                1770.323552, 252.9033645, 36.1290521,  5.1612932,
                0.7373276,   0.1053325,   0.0150475,   0.0021496,
                0.0003071,   0.0000439,   0.0000063,   0.0000009
            };
            return areas[res];
        }

        public static double hexAreaM2(int res)
        {
            double[] areas =
            {
                4.25055E+12, 6.07221E+11, 86745854035, 12392264862,
                1770323552,  252903364.5, 36129052.1,  5161293.2,
                737327.6,    105332.5,    15047.5,     2149.6,
                307.1,       43.9,        6.3,         0.9
            };
            return areas[res];
        }

        public static double edgeLengthKm(int res)
        {
            double[] lens =
            {
                1107.712591, 418.6760055, 158.2446558, 59.81085794,
                22.6063794,  8.544408276, 3.229482772, 1.220629759,
                0.461354684, 0.174375668, 0.065907807, 0.024910561,
                0.009415526, 0.003559893, 0.001348575, 0.000509713
            };
            return lens[res];
        }

        public static double edgeLengthM(int res)
        {
            double[] lens =
            {
                1107712.591, 418676.0055, 158244.6558, 59810.85794,
                22606.3794,  8544.408276, 3229.482772, 1220.629759,
                461.3546837, 174.3756681, 65.90780749, 24.9105614,
                9.415526211, 3.559893033, 1.348574562, 0.509713273
            };
            return lens[res];
        }

        /** @brief Number of unique valid H3Indexes at given resolution. */
        public static long numHexagons(int res)
        {
            long[] nums =
            {
                122,
                842,
                5882,
                41162,
                288122,
                2016842,
                14117882,
                98825162,
                691776122,
                4842432842,
                33897029882,
                237279209162,
                1660954464122,
                11626681248842,
                81386768741882,
                569707381193162
            };
            return nums[res];
        }

        #endregion
    }
}
