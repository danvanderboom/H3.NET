using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    // information on a single base cell
    public class BaseCellData
    {
        public FaceIJK homeFijk;                   // "home" face and normalized ijk coordinates on that face
        public bool isPentagon;                    // is this base cell a pentagon?
        public int[] cwOffsetPent = new int[2];    // if a pentagon, what are its two clockwise offset faces?

        public BaseCellData(FaceIJK homeFijk, bool isPentagon, int[] cwOffsetPent)
        {
            this.homeFijk = homeFijk;
            this.isPentagon = isPentagon;
            this.cwOffsetPent = cwOffsetPent;
        }

        // (face, (coord), isPentagon, (cwOffsetPent))
        public static implicit operator BaseCellData((int, (int, int, int), bool, (int, int)) tuple) =>
            new BaseCellData(
                new FaceIJK(tuple.Item1, new Model.CoordIJK(tuple.Item2.Item1, tuple.Item2.Item2, tuple.Item2.Item3)), 
                tuple.Item3, 
                new int[] { tuple.Item4.Item1, tuple.Item4.Item2 });

        // (face, (coord), isPentagon, (cwOffsetPent))
        //public static implicit operator (int, (int, int, int), bool, (int, int)) (BaseCellData data) => 
        //    (
        //        data.homeFijk.face, 
        //        (data.homeFijk.coord.i, data.homeFijk.coord.j, data.homeFijk.coord.k), 
        //        data.isPentagon, 
        //        (data.cwOffsetPent[0], data.cwOffsetPent[1])
        //    );
    }
}


