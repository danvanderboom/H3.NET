using H3.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static H3.Constants;

namespace H3Test
{
    [TestClass]
    public class Vec3dTests
    {
        [TestMethod]
        public void _pointSquareDist()
        {
            //Vec3d v1 = { 0, 0, 0 };
            //Vec3d v2 = { 1, 0, 0 };
            //Vec3d v3 = { 0, 1, 1 };
            //Vec3d v4 = { 1, 1, 1 };
            //Vec3d v5 = { 1, 1, 2 };
            var v1 = new Vec3d(0, 0, 0);
            var v2 = new Vec3d(1, 0, 0);
            var v3 = new Vec3d(0, 1, 1);
            var v4 = new Vec3d(1, 1, 1);
            var v5 = new Vec3d(1, 1, 2);

            //t_assert(fabs(_pointSquareDist(&v1, &v1)) < DBL_EPSILON, "distance to self is 0");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(v1, v1)) < DBL_EPSILON, "distance to self is 0");

            //t_assert(fabs(_pointSquareDist(&v1, &v2) - 1) < DBL_EPSILON, "distance to <1,0,0> is 1");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(v1, v2) - 1) < DBL_EPSILON, "distance to <1,0,0> is 1");

            //t_assert(fabs(_pointSquareDist(&v1, &v3) - 2) < DBL_EPSILON, "distance to <0,1,1> is 2");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(v1, v3) - 2) < DBL_EPSILON, "distance to <0,1,1> is 2");

            //t_assert(fabs(_pointSquareDist(&v1, &v4) - 3) < DBL_EPSILON, "distance to <1,1,1> is 3");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(v1, v4) - 3) < DBL_EPSILON, "distance to <1,1,1> is 3");

            //t_assert(fabs(_pointSquareDist(&v1, &v5) - 6) < DBL_EPSILON, "distance to <1,1,2> is 6");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(v1, v5) - 6) < DBL_EPSILON, "distance to <1,1,2> is 6");
        }

        [TestMethod]
        public void _geoToVec3d()
        {
            //Vec3d origin = { 0 };
            var origin = new Vec3d(0, 0, 0);

            //GeoCoord c1 = { 0, 0 };
            var c1 = new GeoCoord(0, 0);

            //Vec3d p1;
            //_geoToVec3d(&c1, &p1);
            var p1 = c1.ToVec3d();

            //t_assert(fabs(_pointSquareDist(&origin, &p1) - 1) < EPSILON_RAD, "Geo point is on the unit sphere");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(origin, p1) - 1) < EPSILON_RAD, "Geo point is on the unit sphere");

            //GeoCoord c2 = { M_PI_2, 0 };
            var c2 = new GeoCoord(M_PI_2, 0);

            //Vec3d p2;
            //_geoToVec3d(&c2, &p2);
            var p2 = c2.ToVec3d();

            //t_assert(fabs(_pointSquareDist(&p1, &p2) - 2) < EPSILON_RAD, "Geo point is on another axis");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(p1, p2) - 2) < EPSILON_RAD, "Geo point is on another axis");

            //GeoCoord c3 = { M_PI, 0 };
            var c3 = new GeoCoord(M_PI, 0);

            //Vec3d p3;
            //_geoToVec3d(&c3, &p3);
            var p3 = c3.ToVec3d();

            //t_assert(fabs(_pointSquareDist(&p1, &p3) - 4) < EPSILON_RAD, "Geo point is the other side of the sphere");
            Assert.IsTrue(Math.Abs(Vec3d._pointSquareDist(p1, p3) - 4) < EPSILON_RAD, "Geo point is the other side of the sphere");
        }
    }
}
