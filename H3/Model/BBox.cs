using System;
using System.Collections.Generic;
using System.Text;
using H3.Model;
using static H3.Constants;
using static H3.MathExtensions;

namespace H3.Model
{
    public struct BBox
    {
        public double north;  // north latitude
        public double south;  // south latitude
        public double east;   // east longitude
        public double west;   // west longitude

        public BBox(double north, double south, double east, double west)
        {
            this.north = north;
            this.south = south;
            this.east = east;
            this.west = west;
        }

        /// <summary>
        /// Whether the given bounding box crosses the antimeridian.
        /// </summary>
        public bool bboxIsTransmeridian() => east < west;

        /// <summary>
        /// Get the center of a bounding box
        /// </summary>
        /// <returns>Center coordinate</returns>
        public GeoCoord bboxCenter()
        {
            var center = new GeoCoord();

            center.latitude = (north + south) / 2.0;

            // If the bbox crosses the antimeridian, shift east 360 degrees
            double newEast = bboxIsTransmeridian() ? east + M_2PI : east;
            center.longitude = ConstrainLongitude((newEast + west) / 2.0);

            return center;
        }

        /// <summary>
        /// Whether the bounding box contains a given point
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>Whether the point is contained</returns>
        public bool bboxContains(GeoCoord point) =>
            point.latitude >= south && point.latitude <= north && bboxIsTransmeridian()
                // transmeridian case
                ? (point.longitude >= west || point.longitude <= east)
                // standard case
                : (point.longitude >= west && point.longitude <= east);

        /// <summary>
        /// Whether two bounding boxes are strictly equal.
        /// </summary>
        /// <param name="b1">Bounding box 1</param>
        /// <param name="b2">Bounding box 2</param>
        /// <returns>Whether the boxes are equal</returns>
        public static bool bboxEquals(BBox b1, BBox b2) =>
            b1.north == b2.north && b1.south == b2.south && b1.east == b2.east && b1.west == b2.west;

        /// <summary>
        /// Returns the radius of a given hexagon in km.
        /// </summary>
        /// <param name="h3Index">The index of the hexagon</param>
        /// <returns>The radius of the hexagon in km</returns>
        public static double _hexRadiusKm(H3Index h3Index)
        {
            // There is probably a cheaper way to determine the radius of a
            // hexagon, but this way is conceptually simple
            GeoCoord h3Center = h3Index.ToGeoCoord();
            GeoBoundary h3Boundary = h3Index.ToGeoBoundary();

            // TODO: double check this logic
            return GeoCoord._geoDistKm(h3Center, h3Boundary.verts[0]);
        }

        /// <summary>
        /// Get the radius of the bbox in hexagons - i.e. the radius of a k-ring centered
        /// on the bbox center and covering the entire bbox.
        /// </summary>
        /// <param name="bbox">Bounding box to measure</param>
        /// <param name="res">Resolution of hexagons to use in measurement</param>
        /// <returns>Radius in hexagons</returns>
        public int bboxHexRadius(BBox bbox, int res)
        {
            var center = new GeoCoord();

            // Determine the center of the bounding box
            center = bboxCenter();

            // Use a vertex on the side closest to the equator, to ensure the longest
            // radius in cases with significant distortion. East/west is arbitrary.
            double lat = Math.Abs(bbox.north) > Math.Abs(bbox.south) ? bbox.south : bbox.north;
            var vertex = new GeoCoord(lat, bbox.east);

            // Determine the length of the bounding box "radius" to then use
            // as a circle on the earth that the k-rings must be greater than
            double bboxRadiusKm = GeoCoord._geoDistKm(center, vertex);

            // Determine the radius of the center hexagon
            double centerHexRadiusKm = _hexRadiusKm(center.ToH3Index(res));

            // The closest point along a hexagon drawn through the center points
            // of a k-ring aggregation is exactly 1.5 radii of the hexagon. For
            // any orientation of the GeoJSON encased in a circle defined by the
            // bounding box radius and center, it is guaranteed to fit in this k-ring
            // Rounded *up* to guarantee containment
            return (int)Math.Ceiling(bboxRadiusKm / (1.5 * centerHexRadiusKm));
        }
    }
}
