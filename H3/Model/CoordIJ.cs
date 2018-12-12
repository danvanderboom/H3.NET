using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public struct CoordIJ
    {
        public int i;
        public int j;

        public CoordIJ(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        /// <summary>
        /// Transforms coordinates from the IJK+ coordinate system to the IJ coordinate system.
        /// </summary>
        /// <param name="ijk">The input IJK+ coordinates</param>
        public CoordIJ(CoordIJK ijk)
        {
            i = ijk.i - ijk.k;
            j = ijk.j - ijk.k;
        }
    }
}
