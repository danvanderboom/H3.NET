using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static H3.Constants;
using static H3.MathExtensions;
using static H3.BaseCellsUtil;

namespace H3.Model
{
    public struct H3Index
    {
        public ulong Value;

        public H3Index(ulong h3value)
        {
            Value = h3value;
        }

        public H3Index(string h3hex)
        {
            Value = 0;

            ulong h3;
            if (ulong.TryParse(h3hex, NumberStyles.HexNumber, null, out h3))
                Value = h3;
        }

        public static implicit operator H3Index(ulong h3value) => new H3Index(h3value);

        public static implicit operator ulong(H3Index h3index) => h3index.Value;

        public override string ToString() => $"index {Value.ToString("X")}, resolution {Resolution}, base cell {BaseCell}{(IsPentagon ? " (Pentagon)" : "")}";

        #region constants and functions for bitwise manipulation of H3Index's.

        /** The number of bits in an H3 index. */
        const int H3_NUM_BITS = 64;

        /** The bit offset of the max resolution digit in an H3 index. */
        const int H3_MAX_OFFSET = 63;

        /** The bit offset of the mode in an H3 index. */
        const int H3_MODE_OFFSET = 59;

        /** The bit offset of the base cell in an H3 index. */
        const int H3_BC_OFFSET = 45;

        /** The bit offset of the resolution in an H3 index. */
        const int H3_RES_OFFSET = 52;

        /** The bit offset of the reserved bits in an H3 index. */
        const int H3_RESERVED_OFFSET = 56;

        /** The number of bits in a single H3 resolution digit. */
        const int H3_PER_DIGIT_OFFSET = 3;

        // Invalid index used to indicate an error from ToH3Index and related functions.
        public const ulong H3_INVALID_INDEX = 0;

        /** 1's in the 4 mode bits, 0's everywhere else. */
        static ulong H3_MODE_MASK = (ulong)15 << H3_MODE_OFFSET;

        /** 0's in the 4 mode bits, 1's everywhere else. */
        static ulong H3_MODE_MASK_NEGATIVE => ~H3_MODE_MASK;

        /** 1's in the 7 base cell bits, 0's everywhere else. */
        static ulong H3_BC_MASK = (ulong)127 << H3_BC_OFFSET;

        /** 0's in the 7 base cell bits, 1's everywhere else. */
        static ulong H3_BC_MASK_NEGATIVE => ~H3_BC_MASK;

        /** 1's in the 4 resolution bits, 0's everywhere else. */
        static ulong H3_RES_MASK = (ulong)15 << H3_RES_OFFSET;

        /** 0's in the 4 resolution bits, 1's everywhere else. */
        static ulong H3_RES_MASK_NEGATIVE => ~H3_RES_MASK;

        /** 1's in the 3 reserved bits, 0's everywhere else. */
        static ulong H3_RESERVED_MASK = (ulong)7 << H3_RESERVED_OFFSET;

        /** 0's in the 3 reserved bits, 1's everywhere else. */
        static ulong H3_RESERVED_MASK_NEGATIVE => ~H3_RESERVED_MASK;

        /** 1's in the 3 bits of res 15 digit bits, 0's everywhere else. */
        static int H3_DIGIT_MASK = 7;

        /** 0's in the 7 base cell bits, 1's everywhere else. */
        static ulong H3_DIGIT_MASK_NEGATIVE => ~H3_DIGIT_MASK_NEGATIVE;

        /** H3 index with mode 0, res 0, base cell 0, and 7 for all index digits. */
        public static ulong H3_INIT = 35184372088831;


        public int Mode
        {
            get => (int)((Value & H3_MODE_MASK) >> H3_MODE_OFFSET);
            set => Value = ((Value & H3_MODE_MASK_NEGATIVE) | ((ulong)value << H3_MODE_OFFSET));
        }

        public int BaseCell
        {
            get => (int)((Value & H3_BC_MASK) >> H3_BC_OFFSET);
            set => Value = (Value & H3_BC_MASK_NEGATIVE) | ((ulong)value << H3_BC_OFFSET);
        }

        // resolution of the index (0-15)
        public int Resolution
        {
            get => (int)((Value & H3_RES_MASK) >> H3_RES_OFFSET);
            set => Value = (Value & H3_RES_MASK_NEGATIVE) | ((ulong)value << H3_RES_OFFSET);
        }

        // reserved space - setting to non-zero may produce invalid indexes
        public int ReservedBits
        {
            get => (int)((Value & H3_RESERVED_MASK) >> H3_RESERVED_OFFSET);
            set => Value = (Value & H3_RESERVED_MASK_NEGATIVE) | ((ulong)value << H3_RESERVED_OFFSET);
        }


        // Gets the resolution res integer digit (0-7) of h3.
        public int GetIndexDigit(int res) => (int)((Value >> ((MAX_H3_RES - res) * H3_PER_DIGIT_OFFSET)) & (ulong)H3_DIGIT_MASK);

        // Sets the resolution res digit of h3 to the integer digit (0-7)
        public void SetIndexDigit(int res, int digit)
        {
            Value = (Value & (ulong)~(H3_DIGIT_MASK << ((MAX_H3_RES - res) * H3_PER_DIGIT_OFFSET))) | ((ulong)digit << ((MAX_H3_RES - res) * H3_PER_DIGIT_OFFSET));
        }

        #endregion

        /// <summary>
        /// Returns whether or not an H3 index is valid.
        /// </summary>
        public bool IsValid()
        {
            if (Mode != H3_HEXAGON_MODE)
                return false;

            int baseCell = BaseCell;
            if (baseCell < 0 || baseCell >= NUM_BASE_CELLS)
                return false;

            if (Resolution < 0 || Resolution > MAX_H3_RES)
                return false;

            for (int r = 1; r <= Resolution; r++)
            {
                var digit = (Direction)GetIndexDigit(r);
                if ((int)digit < (int)Direction.CENTER_DIGIT || (int)digit >= (int)Direction.NUM_DIGITS)
                    return false;
            }

            for (int r = Resolution + 1; r <= MAX_H3_RES; r++)
            {
                var digit = (Direction)GetIndexDigit(r);
                if ((int)digit != (int)Direction.INVALID_DIGIT)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes an H3 index.
        /// </summary>
        /// <param name="hp">The H3 index to initialize.</param>
        /// <param name="res">The H3 resolution to initialize the index to.</param>
        /// <param name="baseCell">The H3 base cell to initialize the index to.</param>
        /// <param name="initDigit">The H3 digit (0-7) to initialize all of the index digits to.</param>
        public void SetH3Index(int res, int baseCell, int initDigit)
        {
            Value = H3_INIT;
            Mode = H3_HEXAGON_MODE;
            Resolution = res;
            BaseCell = baseCell;

            for (int r = 1; r <= res; r++)
                SetIndexDigit(r, initDigit);
        }
       
        /// <summary>
        /// Produces the parent index for a given H3 index. 
        /// </summary>
        /// <param name="child">H3Index to find parent of</param>
        /// <param name="parentRes">The resolution to switch to (parent, grandparent, etc)</param>
        /// <returns>H3Index of the parent, or 0 if you actually asked for a child</returns>
        public static H3Index ToParentH3Index(H3Index child, int parentRes)
        {
            int childRes = child.Resolution;

            if (parentRes > childRes)
                return H3_INVALID_INDEX;
            else if (parentRes == childRes)
                return child;
            else if (parentRes < 0 || parentRes > MAX_H3_RES)
                return H3_INVALID_INDEX;

            var parentIndex = new H3Index(child);
            parentIndex.Resolution = parentRes;

            for (int i = parentRes + 1; i <= childRes; i++)
                parentIndex.SetIndexDigit(i, H3_DIGIT_MASK);

            return parentIndex;
        }

        /// <summary>
        /// maxH3ToChildrenSize returns the maximum number of children possible for a
        /// given child level.
        /// </summary>
        /// <param name="childRes">The resolution of the child level you're interested in</param>
        /// <returns>Count of maximum number of children (equal for hexagons, less for
        /// pentagons</returns>
        public int maxH3ToChildrenSize(int childRes)
        {
            int parentRes = Resolution;
            if (parentRes > childRes)
                return 0;

            return ipow(7, childRes - parentRes);
        }

        /// <summary>
        /// MakeDirectChild takes an index and immediately returns the immediate child
        /// index based on the specified cell number.Bit operations only, could generate
        /// invalid indexes if not careful(deleted cell under a pentagon).
        /// </summary>
        /// <param name="h">H3Index to find the direct child of</param>
        /// <param name="cellNumber">int id of the direct child (0-6)</param>
        /// <returns>The new H3Index for the child</returns>
        public static H3Index MakeDirectChild(H3Index h, int cellNumber)
        {
            int childRes = h.Resolution + 1;

            h.Resolution = childRes;

            var childH = new H3Index(h);
            childH.SetIndexDigit(childRes, cellNumber);

            return childH;
        }

        /// <summary>
        /// Generates all of the children from the given hexagon id at the specified 
        /// resolution storing them into the provided memory pointer.
        /// It's assumed that maxH3ToChildrenSize was used to determine the allocation.
        /// </summary>
        /// <param name="h3index">H3Index to find the children of</param>
        /// <param name="childRes">The child level to produce</param>
        /// <returns>List of H3 indexes for all of the chilren</returns>
        public List<H3Index> ToChildren(int childRes)
        {
            var children = new List<H3Index>();

            int parentRes = Resolution;

            if (parentRes > childRes)
                return children;

            if (parentRes == childRes)
            {
                children.Add(this);
                return children;
            }

            int bufferSize = maxH3ToChildrenSize(childRes);
            int bufferChildStep = bufferSize / 7;

            var child = new H3Index(Value);

            for (int i = 0; i < 7; i++)
            {
                if (IsPentagon && i == (int)Direction.K_AXES_DIGIT)
                {
                    var nextChild = new H3Index(child + (ulong)bufferChildStep);

                    while (child < nextChild)
                    {
                        children.Add(new H3Index(H3_INVALID_INDEX));
                        child = new H3Index(child + 1);
                    }
                }
                else
                {
                    children.AddRange(MakeDirectChild(this, i).ToChildren(childRes));
                    child = new H3Index(child + (ulong)bufferChildStep);
                }
            }

            return children;
        }

        /// <summary>
        /// Rotate an H3Index 60 degrees counter-clockwise about a pentagonal center.
        /// </summary>
        /// <returns>The H3Index.</returns>
        public H3Index _h3RotatePent60ccw()
        {
            // rotate in place; skips any leading 1 digits (k-axis)

            var foundFirstNonZeroDigit = false;
            for (int r = 1, res = Resolution; r <= res; r++)
            {
                // rotate this digit
                SetIndexDigit(r, (int)CoordIJK._rotate60ccw((Direction)GetIndexDigit(r)));

                // look for the first non-zero digit so we
                // can adjust for deleted k-axes sequence
                // if necessary
                if (!foundFirstNonZeroDigit && GetIndexDigit(r) != 0)
                {
                    foundFirstNonZeroDigit = true;

                    // adjust for deleted k-axes sequence
                    if (_h3LeadingNonZeroDigit() == (int)Direction.K_AXES_DIGIT)
                        _h3Rotate60ccw();
                }
            }
            return Value;
        }

        /// <summary>
        /// Rotate an H3Index 60 degrees clockwise about a pentagonal center.
        /// </summary>
        /// <returns>The H3Index.</returns>
        public H3Index _h3RotatePent60cw()
        {
            // rotate in place; skips any leading 1 digits (k-axis)

            var foundFirstNonZeroDigit = false;
            for (int r = 1, res = Resolution; r <= res; r++)
            {
                // rotate this digit
                SetIndexDigit(r, (int)CoordIJK._rotate60cw((Direction)GetIndexDigit(r)));

                // look for the first non-zero digit so we
                // can adjust for deleted k-axes sequence
                // if necessary
                if (!foundFirstNonZeroDigit && GetIndexDigit(r) != 0)
                {
                    foundFirstNonZeroDigit = true;

                    // adjust for deleted k-axes sequence
                    if (_h3LeadingNonZeroDigit() == (int)Direction.K_AXES_DIGIT)
                        Value = _h3Rotate60cw();
                }
            }
            return Value;
        }

        /// <summary>
        /// Rotate an H3Index 60 degrees counter-clockwise.
        /// </summary>
        /// <returns>The H3Index.</returns>
        public H3Index _h3Rotate60ccw()
        {
            var h = new H3Index(this);

            for (int r = 1, res = h.Resolution; r <= res; r++)
            {
                var oldDigit = (Direction)h.GetIndexDigit(r);
                h.SetIndexDigit(r, (int)CoordIJK._rotate60ccw(oldDigit));
            }

            return h;
        }

        /// <summary>
        /// Rotate an H3Index 60 degrees clockwise.
        /// </summary>
        /// <returns>The H3Index.</returns>
        public H3Index _h3Rotate60cw()
        {
            for (int r = 1, res = Resolution; r <= res; r++)
                SetIndexDigit(r, (int)CoordIJK._rotate60cw((Direction)GetIndexDigit(r)));

            return Value;
        }

        /// <summary>
        /// Rotate an H3Index 60 degrees clockwise about a pentagonal center.
        /// </summary>
        /// <param name="h"></param>
        /// <returns>The H3Index.</returns>
        public static H3Index _h3RotatePent60cw(H3Index h)
        {
            // rotate in place; skips any leading 1 digits (k-axis)

            var foundFirstNonZeroDigit = false;
            for (int r = 1, res = h.Resolution; r <= res; r++)
            {
                // rotate this digit
                h.SetIndexDigit(r, (int)CoordIJK._rotate60cw((Direction)h.GetIndexDigit(r)));

                // look for the first non-zero digit so we
                // can adjust for deleted k-axes sequence
                // if necessary
                if (!foundFirstNonZeroDigit && h.GetIndexDigit(r) != 0)
                {
                    foundFirstNonZeroDigit = true;

                    // adjust for deleted k-axes sequence
                    if ((Direction)h._h3LeadingNonZeroDigit() == Direction.K_AXES_DIGIT)
                        h = h._h3Rotate60cw();
                }
            }
            return h;
        }

        /// <summary>
        /// Convert an H3Index to the FaceIJK address on a specified icosahedral face.
        /// </summary>
        /// <param name="h">The H3Index.</param>
        /// <param name="fijk">fijk The FaceIJK address, initialized with the desired face
        /// and normalized base cell coordinates.</param>
        /// <returns>Returns 1 if the possibility of overage exists, otherwise 0.</returns>
        public bool _h3ToFaceIjkWithInitializedFijk(ulong h, FaceIJK fijk)
        {
            CoordIJK ijk = fijk.coord;
            int res = Resolution;

            // center base cell hierarchy is entirely on this face
            var possibleOverage = true;
            if (!_isBaseCellPentagon(BaseCell) && (res == 0 || (fijk.coord.i == 0 && fijk.coord.j == 0 && fijk.coord.k == 0)))
                possibleOverage = false;

            for (int r = 1; r <= res; r++)
            {
                if (isResClassIII(r))
                    ijk = ijk._downAp7(); // Class III == rotate ccw
                else
                    ijk = ijk._downAp7r(); // Class II == rotate cw

                ijk = ijk.Neighbor((Direction)GetIndexDigit(r));
            }

            return possibleOverage;
        }

        /// <summary>
        /// Convert an H3Index to a FaceIJK address.
        /// </summary>
        /// <returns>The corresponding FaceIJK address.</returns>
        public FaceIJK ToFaceIjk()
        {
            var fijk = new FaceIJK();

            int baseCell = BaseCell;
            // adjust for the pentagonal missing sequence; all of sub-sequence 5 needs
            // to be adjusted (and some of sub-sequence 4 below)
            if (_isBaseCellPentagon(baseCell) && _h3LeadingNonZeroDigit() == 5)
                Value = _h3Rotate60cw();

            // start with the "home" face and ijk+ coordinates for the base cell of c
            fijk = baseCellData[baseCell].homeFijk;
            if (!_h3ToFaceIjkWithInitializedFijk(Value, fijk))
                return fijk;  // no overage is possible; h lies on this face

            // if we're here we have the potential for an "overage"; i.e., it is
            // possible that c lies on an adjacent face

            CoordIJK origIJK = fijk.coord;

            // if we're in Class III, drop into the next finer Class II grid
            int res = Resolution;
            if (isResClassIII(res))
            {
                // Class III
                fijk.coord = fijk.coord._downAp7r();
                res++;
            }

            // adjust for overage if needed
            // a pentagon base cell with a leading 4 digit requires special handling
            var pentLeading4 = (_isBaseCellPentagon(baseCell) && _h3LeadingNonZeroDigit() == 4);
            if (fijk.AdjustOverageClassII(res, pentLeading4, isSubstrate: false) > 0)
            {
                // if the base cell is a pentagon we have the potential for secondary
                // overages
                if (_isBaseCellPentagon(baseCell))
                    while (true)
                        if (fijk.AdjustOverageClassII(res, pentLeading4: false, isSubstrate: false) == 0)
                            break;

                if (res != Resolution)
                    fijk.coord = fijk.coord._upAp7r();
            }
            else if (res != Resolution)
            {
                fijk.coord = origIJK;
            }

            return fijk;
        }

        /// <summary>
        /// Encodes a coordinate on the sphere to the H3 index of the containing cell at
        /// the specified resolution.
        /// 
        /// Returns 0 on invalid input.
        /// </summary>
        /// <param name="g">The spherical coordinates to encode.</param>
        /// <param name="res">The desired H3 resolution for the encoding.</param>
        /// <returns>The encoded H3Index (or 0 on failure).</returns>
        public static H3Index FromGeoCoord(GeoCoord g, int res)
        {
            if (res < 0 || res > MAX_H3_RES)
                return H3_INVALID_INDEX;

            if (!IsFinite(g.latitude) || !IsFinite(g.longitude))
                return H3_INVALID_INDEX;

            var fijk = g.ToFaceIJK(res);
            return fijk.ToH3Index(res);
        }

        /// <summary>
        /// Takes an H3Index and determines if it is actually a pentagon.
        /// Returns 1 if it is a pentagon, otherwise 0.
        /// </summary>
        public bool IsPentagon => _isBaseCellPentagon(BaseCell) && _h3LeadingNonZeroDigit() == 0;

        /// <summary>
        /// Returns the highest resolution non-zero digit in an H3Index.
        /// </summary>
        /// <returns>The highest resolution non-zero digit in the H3Index.</returns>
        public int _h3LeadingNonZeroDigit()
        {
            for (int res = 1; res <= Resolution; res++)
                if (GetIndexDigit(res) != 0)
                    return GetIndexDigit(res);

            // if we're here it's all 0's
            return (int)Direction.CENTER_DIGIT;
        }

        /// <summary>
        /// Convert an H3Index to a FaceIJK address.
        /// </summary>
        /// <returns>The corresponding FaceIJK address.</returns>
        public FaceIJK ToFaceIJK()
        {
            var fijk = new FaceIJK();

            int baseCell = BaseCell;

            // adjust for the pentagonal missing sequence; all of sub-sequence 5 needs
            // to be adjusted (and some of sub-sequence 4 below)
            if (_isBaseCellPentagon(baseCell) && (int)_h3LeadingNonZeroDigit() == 5)
                _h3Rotate60cw();

            // start with the "home" face and ijk+ coordinates for the base cell of c
            fijk = baseCellData[baseCell].homeFijk;
            if (!_h3ToFaceIjkWithInitializedFijk(this, fijk))
                return fijk;  // no overage is possible; h lies on this face

            // if we're here we have the potential for an "overage"; i.e., it is
            // possible that c lies on an adjacent face

            CoordIJK origIJK = fijk.coord;

            // if we're in Class III, drop into the next finer Class II grid
            int res = Resolution;
            if (isResClassIII(res))
            {
                // Class III
                fijk.coord = fijk.coord._downAp7r();
                res++;
            }

            // adjust for overage if needed
            // a pentagon base cell with a leading 4 digit requires special handling
            var pentLeading4 = (_isBaseCellPentagon(baseCell) && _h3LeadingNonZeroDigit() == 4);
            if (fijk.AdjustOverageClassII(res, pentLeading4, isSubstrate: false) > 0)
            {
                // if the base cell is a pentagon we have the potential for secondary
                // overages
                if (_isBaseCellPentagon(baseCell))
                    while (true)
                        if (fijk.AdjustOverageClassII(res, pentLeading4: false, isSubstrate: false) == 0)
                            break;

                if (res != Resolution)
                    fijk.coord = fijk.coord._upAp7r();
            }
            else if (res != Resolution)
            {
                fijk.coord = origIJK;
            }

            return fijk;
        }

        /// <summary>
        /// Rotate an H3Index 60 degrees counter-clockwise about a pentagonal center.
        /// </summary>
        /// <param name="h">The H3Index.</param>
        public static H3Index _h3RotatePent60ccw(H3Index h)
        {
            // rotate in place; skips any leading 1 digits (k-axis)

            var foundFirstNonZeroDigit = false;
            for (int r = 1, res = h.Resolution; r <= res; r++)
            {
                // rotate this digit
                Direction dir = CoordIJK._rotate60ccw((Direction)h.GetIndexDigit(r));
                h.SetIndexDigit(r, (int)dir);

                // look for the first non-zero digit so we
                // can adjust for deleted k-axes sequence
                // if necessary
                if (!foundFirstNonZeroDigit && h.GetIndexDigit(r) != 0)
                {
                    foundFirstNonZeroDigit = true;

                    // adjust for deleted k-axes sequence
                    if ((Direction)h._h3LeadingNonZeroDigit() == Direction.K_AXES_DIGIT)
                        h = h._h3Rotate60ccw();
                }
            }
            return h;
        }

        /// <summary>
        /// Determines the cell boundary in spherical coordinates for an H3 index.
        /// </summary>
        /// <returns>The boundary of the H3 cell in spherical coordinates.</returns>
        public GeoBoundary ToGeoBoundary() => this.ToFaceIJK().ToGeoBoundary(this.Resolution, this.IsPentagon);

        /// <summary>
        /// Determines the spherical coordinates of the center point of an H3 index.
        /// </summary>
        /// <param name="h3"></param>
        /// <returns>The spherical coordinates of the H3 cell center.</returns>
        public GeoCoord ToGeoCoord() => this.ToFaceIJK().ToGeoCoord(Resolution);

        /// <summary>
        /// Returns whether or not a resolution is a Class III grid. Note that odd
        /// resolutions are Class III and even resolutions are Class II.
        /// </summary>
        /// <param name="res">The H3 resolution.</param>
        /// <returns>1 if the resolution is a Class III grid, and 0 if the resolution is
        /// a Class II grid.</returns>
        public static bool isResClassIII(int res) => res % 2 > 0;


        /*
         * h3IsResClassIII takes a hexagon ID and determines if it is in a
         * Class III resolution (rotated versus the icosahedron and subject
         * to shape distortion adding extra points on icosahedron edges, making
         * them not true hexagons).
         * @param h The H3Index to check.
         * @return Returns 1 if the hexagon is class III, otherwise 0.
         */
        public bool IsResClassIII() => this.Resolution % 2 == 0;
}
}
