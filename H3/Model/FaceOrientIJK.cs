using H3.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public class FaceOrientIJK
    {
        public int face;            ///< face number
        public CoordIJK translate;  ///< res 0 translation relative to primary face
        public int ccwRot60;  ///< number of 60 degree ccw rotations relative to primary face

        public FaceOrientIJK(int face, CoordIJK translate, int ccwRot60)
        {
            this.face = face;
            this.translate = translate;
            this.ccwRot60 = ccwRot60;
        }
    }
}
