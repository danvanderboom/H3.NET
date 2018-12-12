using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    // base cell at a given ijk and required rotations into its system
    public class BaseCellOrient
    {
        public int baseCell;  // base cell number
        public int ccwRot60;  // number of ccw 60 degree rotations relative to current face

        public BaseCellOrient(int baseCell, int ccwRot60)
        {
            this.baseCell = baseCell;
            this.ccwRot60 = ccwRot60;
        }

        public static implicit operator BaseCellOrient((int baseCell, int ccwRot60) tuple)
        {
            return new BaseCellOrient(tuple.baseCell, tuple.ccwRot60);
        }

        public static implicit operator (int, int)(BaseCellOrient orient)
        {
            return (orient.baseCell, orient.ccwRot60);
        }
    }
}
