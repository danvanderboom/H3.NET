using System;
using System.Collections.Generic;
using System.Text;
using static H3.Constants;

namespace H3.Model
{
    public struct GeoBoundary
    {
        public int numVerts; //  number of vertices
        public GeoCoord[] verts; //  vertices in ccw order
    }
}
