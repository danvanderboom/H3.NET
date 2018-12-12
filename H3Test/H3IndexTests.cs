using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H3.Model;
using static H3.Constants;

namespace H3Test
{
    [TestClass]
    public class H3IndexTests
    {
        [TestMethod]
        public void geoToH3ExtremeCoordinates()
        {
            // Check that none of these cause crashes.
            //GeoCoord g = { 0, 1E45 };
            //H3_EXPORT(geoToH3)(&g, 14);
            var g = new GeoCoord(0, 1E45);
            g.ToH3Index(14);

            //GeoCoord g2 = { 1E46, 1E45 };
            //H3_EXPORT(geoToH3)(&g2, 15);
            var g2 = new GeoCoord(1E46, 1E45);
            g2.ToH3Index(15);

            //GeoCoord g4;
            //setGeoDegs(&g4, 2, -3E39);
            //H3_EXPORT(geoToH3)(&g4, 0);
            var g4 = new GeoCoord(2, -3E39);
            g4.ToH3Index(0);
        }

        [TestMethod]
        public void faceIjkToH3ExtremeCoordinates()
        {
            //FaceIJK fijk0I = { 0, { 3, 0, 0 } };
            //t_assert(_faceIjkToH3(&fijk0I, 0) == 0, "i out of bounds at res 0");
            var fijk0I = new FaceIJK(0, new CoordIJK(3, 0, 0));
            Assert.IsTrue(0 == (ulong)fijk0I.ToH3Index(0), "i out of bounds at res 0");

            //FaceIJK fijk0J = { 1, { 0, 4, 0 } };
            //t_assert(_faceIjkToH3(&fijk0J, 0) == 0, "j out of bounds at res 0");
            var fijk0J = new FaceIJK(1, new CoordIJK(0, 4, 0));
            Assert.IsTrue(0 == fijk0J.ToH3Index(0), "i out of bounds at res 0");

            //FaceIJK fijk0K = { 2, { 2, 0, 5 } };
            //t_assert(_faceIjkToH3(&fijk0K, 0) == 0, "k out of bounds at res 0");
            var fijk0K = new FaceIJK(2, new CoordIJK(2, 0, 5));
            Assert.IsTrue(0 == fijk0K.ToH3Index(0), "i out of bounds at res 0");


            //FaceIJK fijk1I = { 3, { 6, 0, 0 } };
            //t_assert(_faceIjkToH3(&fijk1I, 1) == 0, "i out of bounds at res 1");
            var fijk1I = new FaceIJK(3, new CoordIJK(6, 0, 0));
            Assert.IsTrue(0 == fijk1I.ToH3Index(1), "i out of bounds at res 0");

            //FaceIJK fijk1J = { 4, { 0, 7, 1 } };
            //t_assert(_faceIjkToH3(&fijk1J, 1) == 0, "j out of bounds at res 1");
            var fijk1J = new FaceIJK(4, new CoordIJK(0, 7, 1));
            Assert.IsTrue(0 == fijk1J.ToH3Index(1), "i out of bounds at res 0");

            //FaceIJK fijk1K = { 5, { 2, 0, 8 } };
            //t_assert(_faceIjkToH3(&fijk1K, 1) == 0, "k out of bounds at res 1");
            var fijk1K = new FaceIJK(5, new CoordIJK(2, 0, 8));
            Assert.IsTrue(0 == fijk1K.ToH3Index(1), "i out of bounds at res 0");


            //FaceIJK fijk2I = { 6, { 18, 0, 0 } };
            //t_assert(_faceIjkToH3(&fijk2I, 2) == 0, "i out of bounds at res 2");
            var fijk2I = new FaceIJK(6, new CoordIJK(18, 0, 0));
            Assert.IsTrue(0 == fijk2I.ToH3Index(2), "i out of bounds at res 0");

            //FaceIJK fijk2J = { 7, { 0, 19, 1 } };
            //t_assert(_faceIjkToH3(&fijk2J, 2) == 0, "j out of bounds at res 2");
            var fijk2J = new FaceIJK(7, new CoordIJK(0, 19, 1));
            Assert.IsTrue(0 == fijk2J.ToH3Index(2), "i out of bounds at res 0");

            //FaceIJK fijk2K = { 8, { 2, 0, 20 } };
            //t_assert(_faceIjkToH3(&fijk2K, 2) == 0, "k out of bounds at res 2");
            var fijk2K = new FaceIJK(8, new CoordIJK(2, 0, 20));
            Assert.IsTrue(0 == fijk2K.ToH3Index(2), "i out of bounds at res 0");
        }

        [TestMethod]
        public void h3IsValidAtResolution()
        {
            for (int i = 0; i <= MAX_H3_RES; i++)
            {
                //GeoCoord geoCoord = { 0, 0 };
                var geoCoord = new GeoCoord(0, 0);

                //H3Index h3 = H3_EXPORT(geoToH3)(&geoCoord, i);
                var h3 = geoCoord.ToH3Index(i);

                //char failureMessage[BUFF_SIZE];
                //sprintf(failureMessage, "h3IsValid failed on resolution %d", i);
                //t_assert(H3_EXPORT(h3IsValid)(h3), failureMessage);
                Assert.IsTrue(h3.IsValid(), $"h3IsValid failed on resolution {i}");
            }
        }

        [TestMethod]
        public void h3IsValidDigits()
        {
            //GeoCoord geoCoord = { 0, 0 };
            var geoCoord = new GeoCoord(0, 0);

            //H3Index h3 = H3_EXPORT(geoToH3)(&geoCoord, 1);
            var h3 = geoCoord.ToH3Index(1);

            //// Set a bit for an unused digit to something else.
            //h3 ^= 1;
            h3 ^= 1;

            //t_assert(!H3_EXPORT(h3IsValid)(h3), "h3IsValid failed on invalid unused digits");
            Assert.IsFalse(h3.IsValid(), "h3IsValid failed on invalid unused digits");
        }

        [TestMethod]
        public void h3IsValidBaseCell()
        {
            for (int i = 0; i < NUM_BASE_CELLS; i++)
            {
                //H3Index h = H3_INIT;
                var h = new H3Index(H3Index.H3_INIT);

                //H3_SET_MODE(h, H3_HEXAGON_MODE);
                h.Mode = H3_HEXAGON_MODE;

                //H3_SET_BASE_CELL(h, i);
                h.BaseCell = i;

                //char failureMessage[BUFF_SIZE];
                //sprintf(failureMessage, "h3IsValid failed on base cell %d", i);
                //t_assert(H3_EXPORT(h3IsValid)(h), failureMessage);
                Assert.IsTrue(h.IsValid(), $"h3IsValid failed on base cell {i}");

                //t_assert(H3_EXPORT(h3GetBaseCell)(h) == i, "failed to recover base cell");
                Assert.IsTrue(h.BaseCell == i, "failed to recover base cell");
            }
        }

        [TestMethod]
        public void h3IsValidBaseCellInvalid()
        {
            //H3Index hWrongBaseCell = H3_INIT;
            var hWrongBaseCell = new H3Index(H3Index.H3_INIT);

            //H3_SET_MODE(hWrongBaseCell, H3_HEXAGON_MODE);
            hWrongBaseCell.Mode = H3_HEXAGON_MODE;

            //H3_SET_BASE_CELL(hWrongBaseCell, NUM_BASE_CELLS);
            hWrongBaseCell.BaseCell = NUM_BASE_CELLS;

            //t_assert(!H3_EXPORT(h3IsValid)(hWrongBaseCell), "h3IsValid failed on invalid base cell");
            Assert.IsFalse(hWrongBaseCell.IsValid(), "h3IsValid failed on invalid base cell");
        }

        [TestMethod]
        public void h3IsValidWithMode()
        {
            for (int i = 0; i <= 0xf; i++)
            {
                //H3Index h = H3_INIT;
                var h = new H3Index(H3Index.H3_INIT);

                //H3_SET_MODE(h, i);
                h.Mode = i;

                //char failureMessage[BUFF_SIZE];
                //sprintf(failureMessage, "h3IsValid failed on mode %d", i);
                //t_assert(!H3_EXPORT(h3IsValid)(h) || i == 1, failureMessage);
                Assert.IsTrue(!h.IsValid() || i == 1, $"h3IsValid failed on mode {i}");
            }
        }

        [TestMethod]
        public void h3BadDigitInvalid()
        {
            //H3Index h = H3_INIT;
            var h = new H3Index(H3Index.H3_INIT);

            //H3_SET_MODE(h, H3_HEXAGON_MODE);
            h.Mode = H3_HEXAGON_MODE;

            //H3_SET_RESOLUTION(h, 1);
            h.Resolution = 1;

            //t_assert(!H3_EXPORT(h3IsValid)(h), "h3IsValid failed on too large digit");
            Assert.IsFalse(h.IsValid(), "h3IsValid failed on too large digit");

        }

        [TestMethod]
        public void h3ToString()
        {
            //const size_t bufSz = 17;
            //char buf[17] = { 0 };
            //H3_EXPORT(h3ToString)(0x1234, buf, bufSz - 1);
            var h = new H3Index(0x1234);
            var hstr = h.Value.ToString("X");

            //// Buffer should be unmodified because the size was too small
            //t_assert(buf[0] == 0, "h3ToString failed on buffer too small");
            //H3_EXPORT(h3ToString)(0xcafe, buf, bufSz);
            var c = new H3Index(0xcafe);
            var cstr = c.Value.ToString("X");

            //t_assert(strcmp(buf, "cafe") == 0, "h3ToString failed to produce base 16 results");
            Assert.IsTrue(cstr.Contains("CAFE"), "h3ToString failed to produce base 16 results");

            //H3_EXPORT(h3ToString)(0xffffffffffffffff, buf, bufSz);
            var big = new H3Index(0xffffffffffffffff);
            var bigstr = big.Value.ToString("x");

            //t_assert(strcmp(buf, "ffffffffffffffff") == 0, "h3ToString failed on large input");
            Assert.IsTrue(bigstr.Contains("ffffffffffffffff"), "h3ToString failed on large input");
        }

        [TestMethod]
        public void stringToH3()
        {
            //t_assert(H3_EXPORT(stringToH3)("") == 0, "got an index from nothing");
            var h1 = new H3Index("");
            Assert.IsTrue(h1 == 0, "got an index from nothing");

            //t_assert(H3_EXPORT(stringToH3)("**") == 0, "got an index from junk");
            var h2 = new H3Index("**");
            Assert.IsTrue(h1 == 0, "got an index from junk");

            //t_assert(H3_EXPORT(stringToH3)("ffffffffffffffff") == 0xffffffffffffffff, "failed on large input");
            var h3 = new H3Index("ffffffffffffffff");
            Assert.IsTrue(h1 == 0, "failed on large input");
        }

        [TestMethod]
        public void setH3Index()
        {
            //H3Index h;
            var h = new H3Index();

            //setH3Index(&h, 5, 12, 1);
            h.SetH3Index(5, 12, 1);

            //t_assert(H3_GET_RESOLUTION(h) == 5, "resolution as expected");
            Assert.AreSame(5, h.Resolution, "resolution as expected");

            //t_assert(H3_GET_BASE_CELL(h) == 12, "base cell as expected");
            Assert.AreSame(12, h.BaseCell, "base cell as expected");

            //t_assert(H3_GET_MODE(h) == H3_HEXAGON_MODE, "mode as expected");
            Assert.AreSame(H3_HEXAGON_MODE, h.Mode, "mode as expected");

            //for (int i = 1; i <= 5; i++)
            //    t_assert(H3_GET_INDEX_DIGIT(h, i) == 1, "digit as expected");
            for (int i = 1; i <= 5; i++)
                Assert.AreSame(1, h.GetIndexDigit(i), "digit as expected");

            //for (int i = 6; i <= MAX_H3_RES; i++)
            //    t_assert(H3_GET_INDEX_DIGIT(h, i) == 7, "blanked digit as expected");
            for (int i = 6; i <= MAX_H3_RES; i++)
                Assert.AreSame(7, h.GetIndexDigit(i), "blanked digit as expected");

            //t_assert(h == 0x85184927fffffffL, "index matches expected");
            Assert.AreSame(0x85184927fffffff, h.Value, "blanked digit as expected");
        }

        [TestMethod]
        public void h3IsResClassIII()
        {
            //GeoCoord coord = { 0, 0 };
            var coord = new GeoCoord(0, 0);

            for (int i = 0; i <= MAX_H3_RES; i++)
            {
                //H3Index h = H3_EXPORT(geoToH3)(&coord, i);
                var h = coord.ToH3Index(i);

                //t_assert(H3_EXPORT(h3IsResClassIII)(h) == isResClassIII(i), "matches existing definition");
                Assert.IsTrue(h.IsResClassIII() == H3Index.isResClassIII(i), "matches existing definition");
            }
        }
    }
}
