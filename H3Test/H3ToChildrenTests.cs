using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H3.Model;

namespace H3Test
{
    [TestClass]
    public class H3ToChildrenTests
    {
        const int PADDED_COUNT = 10;

        GeoCoord sf;
        H3Index sfHex8;

        [TestInitialize]
        public void Setup()
        {
            sf = new GeoCoord(0.659966917655, 2 * 3.14159 - 2.1364398519396);
            sfHex8 = sf.ToH3Index(8);
        }

        static void verifyCountAndUniqueness(List<H3Index> children, int paddedCount, int expectedCount)
        {
            int numFound = 0;
            for (int i = 0; i < paddedCount; i++)
            {
                ulong currIndex = children[i];
                if (currIndex == 0)
                    continue;

                numFound++;
                // verify uniqueness
                int indexSeen = 0;
                for (int j = i + 1; j < paddedCount; j++)
                    if (children[j] == currIndex)
                        indexSeen++;

                Assert.AreEqual(0, indexSeen, "index was seen only once");
            }

            Assert.AreEqual(expectedCount, numFound, "got expected number of children");
        }

        [TestMethod]
        public void oneResStep()
        {
            int expectedCount = 7;
            int paddedCount = 10;

            //H3Index sfHex9s[PADDED_COUNT] = { 0 };
            var sfHex9s = sfHex8.ToChildren(9);

            //H3_EXPORT(h3ToGeo)(sfHex8, &center);
            GeoCoord center = sfHex8.ToGeoCoord();

            //H3Index sfHex9_0 = H3_EXPORT(geoToH3)(&center, 9);
            var sfHex9_0 = center.ToH3Index(9);

            int numFound = 0;

            // Find the center
            for (int i = 0; i < paddedCount; i++)
                if (sfHex9_0 == sfHex9s[i])
                    numFound++;

            Assert.AreEqual(1, numFound, "found the center hex");

            // Get the neighbor hexagons by averaging the center point and outer
            // points then query for those independently

            //H3_EXPORT(h3ToGeoBoundary)(sfHex8, &outside);
            GeoBoundary outside = sfHex8.ToGeoBoundary();

            for (int i = 0; i < outside.numVerts; i++)
            {
                var avg = new GeoCoord
                {
                    latitude = (outside.verts[i].latitude + center.latitude) / 2,
                    longitude = (outside.verts[i].longitude + center.longitude) / 2
                };

                // H3_EXPORT(geoToH3)(&avg, 9);
                H3Index avgHex9 = avg.ToH3Index(9);
                for (int j = 0; j < expectedCount; j++)
                    if (avgHex9 == sfHex9s[j])
                        numFound++;
            }

            Assert.AreEqual(expectedCount, numFound, "found all expected children");
        }

        [TestMethod]
        public void multipleResSteps()
        {
            // Lots of children. Will just confirm number and uniqueness
            int expectedCount = 49;
            int paddedCount = 60;

            //H3Index* children = calloc(paddedCount, sizeof(H3Index));
            //H3_EXPORT(h3ToChildren)(sfHex8, 10, children);
            var children = sfHex8.ToChildren(10);

            verifyCountAndUniqueness(children, paddedCount, expectedCount);
        }

        [TestMethod]
        public void sameRes()
        {
            int expectedCount = 1;
            int paddedCount = 7;

            //H3Index* children = calloc(paddedCount, sizeof(H3Index));
            //H3_EXPORT(h3ToChildren)(sfHex8, 8, children);
            var children = sfHex8.ToChildren(8);

            verifyCountAndUniqueness(children, paddedCount, expectedCount);
        }

        [TestMethod]
        public void childResTooHigh()
        {
            int expectedCount = 0;
            int paddedCount = 7;

            //H3Index* children = calloc(paddedCount, sizeof(H3Index));
            //H3_EXPORT(h3ToChildren)(sfHex8, 7, children);
            var children = sfHex8.ToChildren(7);

            verifyCountAndUniqueness(children, paddedCount, expectedCount);
        }

        [TestMethod]
        public void pentagonChildren()
        {
            var pentagon = new H3Index();
            pentagon.SetH3Index(1, 4, 0);

            int expectedCount = (5 * 7) + 6;
            int paddedCount = pentagon.maxH3ToChildrenSize(3);

            //H3Index* children = calloc(paddedCount, sizeof(H3Index));
            //H3_EXPORT(h3ToChildren)(sfHex8, 10, children);
            //H3_EXPORT(h3ToChildren)(pentagon, 3, children);
            var children = new List<H3Index>();
            children.AddRange(sfHex8.ToChildren(10));
            children.AddRange(pentagon.ToChildren(3));

            verifyCountAndUniqueness(children, paddedCount, expectedCount);
        }
    }
}
