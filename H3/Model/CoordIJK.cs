using System;
using System.Collections.Generic;
using System.Text;
using static H3.Constants;

namespace H3.Model
{
    public class CoordIJK : IEquatable<CoordIJK>
    {
        public int i, j, k;

        public CoordIJK() : this(0, 0, 0) { }

        public CoordIJK(CoordIJK c) : this(c.i, c.j, c.k) { }

        public CoordIJK(Vec2d v) => FromHex2d(v);

        public CoordIJK(CoordIJ ij) : this(ij.i, ij.j, 0) { }

        public CoordIJK(int i, int j, int k)
        {
            this.i = i;
            this.j = j;
            this.k = k;
        }

        /* @brief CoordIJK unit vectors corresponding to the 7 H3 digits. */
        public static CoordIJK[] UNIT_VECS =
        {
            new CoordIJK(0, 0, 0),  // direction 0
            new CoordIJK(0, 0, 1),  // direction 1
            new CoordIJK(0, 1, 0),  // direction 2
            new CoordIJK(0, 1, 1),  // direction 3
            new CoordIJK(1, 0, 0),  // direction 4
            new CoordIJK(1, 0, 1),  // direction 5
            new CoordIJK(1, 1, 0),  // direction 6
        };

        /// <summary>
        /// Find the center point in 2D cartesian coordinates of a hex.
        /// </summary>
        /// <returns>The 2D cartesian coordinates of the hex center point.</returns>
        public Vec2d ToHex2d()
        {
            var v = new Vec2d();

            int iminusk = i - k;
            int jminusk = j - k;

            v.x = iminusk - 0.5 * jminusk;
            v.y = jminusk * M_SQRT3_2;

            return v;
        }

        /// <summary>
        /// Determines the H3 digit corresponding to a unit vector in ijk coordinates.
        /// </summary>
        /// <returns>The H3 digit (0-6) corresponding to the ijk unit vector, or
        /// INVALID_DIGIT on failure.</returns>
        public int UnitIJKToDigit()
        {
            var c = new CoordIJK(this).Normalize();

            int digit = (int)Direction.INVALID_DIGIT;
            for (int i = (int)Direction.CENTER_DIGIT; i < (int)Direction.NUM_DIGITS; i++)
            {
                if (c.Equals(UNIT_VECS[i]))
                {
                    digit = i;
                    break;
                }
            }

            return digit;
        }

        /*
         * Determine the containing hex in ijk+ coordinates for a 2D cartesian
         * coordinate vector (from DGGRID).
         *
         * @param v The 2D cartesian coordinate vector.
         * @param h The ijk+ coordinates of the containing hex.
         */
        public static CoordIJK FromHex2d(Vec2d v)
        {
            var h = new CoordIJK();

            double a1, a2;
            double x1, x2;
            int m1, m2;
            double r1, r2;

            // quantize into the ij system and then normalize
            h.k = 0;

            a1 = Math.Abs(v.x);
            a2 = Math.Abs(v.y);

            // first do a reverse conversion
            x2 = a2 / M_SIN60;
            x1 = a1 + x2 / 2.0;

            // check if we have the center of a hex
            m1 = (int)x1;
            m2 = (int)x2;

            // otherwise round correctly
            r1 = x1 - m1;
            r2 = x2 - m2;

            if (r1 < 0.5)
            {
                if (r1 < 1.0 / 3.0)
                {
                    if (r2 < (1.0 + r1) / 2.0)
                    {
                        h.i = m1;
                        h.j = m2;
                    }
                    else
                    {
                        h.i = m1;
                        h.j = m2 + 1;
                    }
                }
                else
                {
                    if (r2 < (1.0 - r1))
                    {
                        h.j = m2;
                    }
                    else
                    {
                        h.j = m2 + 1;
                    }

                    if ((1.0 - r1) <= r2 && r2 < (2.0 * r1))
                    {
                        h.i = m1 + 1;
                    }
                    else
                    {
                        h.i = m1;
                    }
                }
            }
            else
            {
                if (r1 < 2.0 / 3.0)
                {
                    if (r2 < (1.0 - r1))
                    {
                        h.j = m2;
                    }
                    else
                    {
                        h.j = m2 + 1;
                    }

                    if ((2.0 * r1 - 1.0) < r2 && r2 < (1.0 - r1))
                    {
                        h.i = m1;
                    }
                    else
                    {
                        h.i = m1 + 1;
                    }
                }
                else
                {
                    if (r2 < (r1 / 2.0))
                    {
                        h.i = m1 + 1;
                        h.j = m2;
                    }
                    else
                    {
                        h.i = m1 + 1;
                        h.j = m2 + 1;
                    }
                }
            }

            // now fold across the axes if necessary

            if (v.x < 0.0)
            {
                if ((h.j % 2) == 0)  // even
                {
                    long axisi = h.j / 2;
                    long diff = h.i - axisi;
                    h.i = (int)(h.i - 2.0 * diff);
                }
                else
                {
                    long axisi = (h.j + 1) / 2;
                    long diff = h.i - axisi;
                    h.i = (int)(h.i - (2.0 * diff + 1));
                }
            }

            if (v.y < 0.0)
            {
                h.i = h.i - (2 * h.j + 1) / 2;
                h.j = -1 * h.j;
            }

            h.Normalize();

            return h;
        }

        /// <summary>
        /// Normalizes ijk coordinates by setting the components to the smallest possible values. 
        /// </summary>
        public CoordIJK Normalize()
        {
            var c = new CoordIJK(this);

            // remove any negative values
            if (c.i < 0)
            {
                c.j -= c.i;
                c.k -= c.i;
                c.i = 0;
            }

            if (c.j < 0)
            {
                c.i -= c.j;
                c.k -= c.j;
                c.j = 0;
            }

            if (c.k < 0)
            {
                c.i -= c.k;
                c.j -= c.k;
                c.k = 0;
            }

            // remove the min value if needed
            int min = c.i;

            if (c.j < min)
                min = c.j;

            if (c.k < min)
                min = c.k;

            if (min > 0)
            {
                c.i -= min;
                c.j -= min;
                c.k -= min;
            }

            return c;
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the indexing parent of a cell in a counter-clockwise aperture 7 grid.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _upAp7()
        {
            var ijk = new CoordIJK(this);

            // convert to CoordIJ
            int i = ijk.i - ijk.k;
            int j = ijk.j - ijk.k;

            ijk.i = (int)Math.Round((3 * i - j) / 7.0);
            ijk.j = (int)Math.Round((i + 2 * j) / 7.0);
            ijk.k = 0;
            ijk = ijk.Normalize();

            return ijk;
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the indexing parent of a cell in a clockwise aperture 7 grid.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _upAp7r()
        {
            var ijk = new CoordIJK(this);

            // convert to CoordIJ
            int i = ijk.i - ijk.k;
            int j = ijk.j - ijk.k;

            ijk.i = (int)Math.Round((2 * i + j) / 7.0);
            ijk.j = (int)Math.Round((3 * j - i) / 7.0);
            ijk.k = 0;

            ijk = ijk.Normalize();

            return ijk;
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex centered on the indicated
        /// hex at the next finer aperture 7 counter-clockwise resolution.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _downAp7()
        {
            var ijk = new CoordIJK(this);

            // res r unit vectors in res r+1
            var iVec = new CoordIJK(3, 0, 1);
            var jVec = new CoordIJK(1, 3, 0);
            var kVec = new CoordIJK(0, 1, 3);

            iVec *= ijk.i;
            jVec *= ijk.j;
            kVec *= ijk.k;

            // TODO: setting the same variable twice here??
            ijk = (iVec + jVec + kVec).Normalize();

            return ijk;
        }

        /// <summary>Find the normalized ijk coordinates of the hex centered on the indicated
        /// hex at the next finer aperture 7 clockwise resolution.Works in place.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _downAp7r()
        {
            var ijk = new CoordIJK(this);

            // res r unit vectors in res r+1
            var iVec = new CoordIJK(3, 1, 0) * ijk.i;
            var jVec = new CoordIJK(0, 3, 1) * ijk.j;
            var kVec = new CoordIJK(1, 0, 3) * ijk.k;

            ijk = (iVec + jVec + kVec).Normalize();

            return ijk;
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex in the specified digit
        /// direction from the specified ijk coordinates.
        /// </summary>
        /// <param name="direction">The digit direction from the original ijk coordinates.</param>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK Neighbor(Direction direction)
        {
            var ijk = new CoordIJK(this);

            if (direction > Direction.CENTER_DIGIT && direction < Direction.NUM_DIGITS)
                ijk = (ijk + UNIT_VECS[(int)direction]).Normalize();

            return ijk;
        }

        /// <summary>
        /// Rotates ijk coordinates 60 degrees counter-clockwise.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _ijkRotate60ccw()
        {
            var ijk = new CoordIJK(this);

            // unit vector rotations
            var iVec = new CoordIJK(1, 1, 0) * ijk.i;
            var jVec = new CoordIJK(0, 1, 1) * ijk.j;
            var kVec = new CoordIJK(1, 0, 1) * ijk.k;

            ijk = (iVec + jVec + kVec).Normalize();

            return ijk;
        }

        /// <summary>
        /// Rotates ijk coordinates 60 degrees clockwise.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _ijkRotate60cw()
        {
            var ijk = new CoordIJK(this);

            // unit vector rotations
            var iVec = new CoordIJK(1, 0, 1) * ijk.i;
            var jVec = new CoordIJK(1, 1, 0) * ijk.j;
            var kVec = new CoordIJK(0, 1, 1) * ijk.k;

            ijk = (iVec + jVec + kVec).Normalize();

            return ijk;
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex centered on the indicated
        /// hex at the next finer aperture 3 counter-clockwise resolution.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _downAp3()
        {
            var ijk = new CoordIJK(this);

            // res r unit vectors in res r+1
            var iVec = new CoordIJK(2, 0, 1) * ijk.i;
            var jVec = new CoordIJK(1, 2, 0) * ijk.j;
            var kVec = new CoordIJK(0, 1, 2) * ijk.k;

            ijk = (iVec + jVec + kVec).Normalize();

            return ijk;
        }

        /// <summary>
        /// Find the normalized ijk coordinates of the hex centered on the indicated
        /// hex at the next finer aperture 3 clockwise resolution.Works in place.
        /// </summary>
        /// <returns>The ijk coordinates.</returns>
        public CoordIJK _downAp3r()
        {
            var ijk = new CoordIJK(this);

            // res r unit vectors in res r+1
            var iVec = new CoordIJK(2, 1, 0) * ijk.i;
            var jVec = new CoordIJK(0, 2, 1) * ijk.j;
            var kVec = new CoordIJK(1, 0, 2) * ijk.k;

            ijk = (iVec + jVec + kVec).Normalize();

            return ijk;
        }

        /// <summary>
        /// Finds the distance between the two coordinates.
        /// </summary>
        /// <param name="c1">The first set of ijk coordinates.</param>
        /// <param name="c2">The second set of ijk coordinates.</param>
        /// <returns></returns>
        public static int Distance(CoordIJK c1, CoordIJK c2)
        {
            var diff = (c1 - c2).Normalize();
            var absDiff = new CoordIJK(Math.Abs(diff.i), Math.Abs(diff.j), Math.Abs(diff.k));
            return Math.Max(absDiff.i, Math.Max(absDiff.j, absDiff.k));
        }

        public bool Equals(CoordIJK other) => (other.i == i && other.j == j && other.k == k);

        public static CoordIJK operator +(CoordIJK a, CoordIJK b) => new CoordIJK(a.i + b.i, a.j + b.j, a.k + b.k);

        public static CoordIJK operator -(CoordIJK a, CoordIJK b) => new CoordIJK(a.i - b.i, a.j - b.j, a.k - b.k);

        public static CoordIJK operator *(CoordIJK a, int factor) => new CoordIJK(a.i * factor, a.j *= factor, a.k *= factor);







        // TODO: move the methods below somewhere else, since they're not specific to CoordIJK

        /*
         * Rotates indexing digit 60 degrees counter-clockwise. Returns result.
         *
         * @param digit Indexing digit (between 1 and 6 inclusive)
         */
        public static Direction _rotate60ccw(Direction digit)
        {
            switch (digit)
            {
                case Direction.K_AXES_DIGIT:
                    return Direction.IK_AXES_DIGIT;
                case Direction.IK_AXES_DIGIT:
                    return Direction.I_AXES_DIGIT;
                case Direction.I_AXES_DIGIT:
                    return Direction.IJ_AXES_DIGIT;
                case Direction.IJ_AXES_DIGIT:
                    return Direction.J_AXES_DIGIT;
                case Direction.J_AXES_DIGIT:
                    return Direction.JK_AXES_DIGIT;
                case Direction.JK_AXES_DIGIT:
                    return Direction.K_AXES_DIGIT;
                default:
                    return digit;
            }
        }

        /*
         * Rotates indexing digit 60 degrees clockwise. Returns result.
         *
         * @param digit Indexing digit (between 1 and 6 inclusive)
         */
        public static Direction _rotate60cw(Direction digit)
        {
            switch (digit)
            {
                case Direction.K_AXES_DIGIT:
                    return Direction.JK_AXES_DIGIT;
                case Direction.JK_AXES_DIGIT:
                    return Direction.J_AXES_DIGIT;
                case Direction.J_AXES_DIGIT:
                    return Direction.IJ_AXES_DIGIT;
                case Direction.IJ_AXES_DIGIT:
                    return Direction.I_AXES_DIGIT;
                case Direction.I_AXES_DIGIT:
                    return Direction.IK_AXES_DIGIT;
                case Direction.IK_AXES_DIGIT:
                    return Direction.K_AXES_DIGIT;
                default:
                    return digit;
            }
        }
    }
}
