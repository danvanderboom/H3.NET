using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public struct GeoFence
    {
        public int numVerts;
        public IntPtr verts; // GeoCoord[]
    }
}
