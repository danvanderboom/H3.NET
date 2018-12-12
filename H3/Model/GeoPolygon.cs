using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public struct GeoPolygon
    {
        public GeoFence geofence; // exterior boundary of the polygon
        public int numHoles; // number of elements in the array pointed to by holes
        public IntPtr holes; // GeoCoord[] - interior boundaries (holes) in the polygon
    }
}
