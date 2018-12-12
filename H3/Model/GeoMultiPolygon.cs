using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public struct GeoMultiPolygon
    {
        public int numPolygons;
        public IntPtr polygons; // GeoPolygon[]
    }
}
