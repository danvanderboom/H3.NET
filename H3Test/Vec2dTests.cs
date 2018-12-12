using System;
using System.Collections.Generic;
using System.Text;
using static H3.Constants;
using H3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H3Test
{
    [TestClass]
    public class Vec2dTests
    {
        [TestMethod]
        public void _v2dMag()
        {
            //Vec2d v = { 3.0, 4.0 };
            var v = new Vec2d(3, 4);

            double expected = 5;
            //double mag = _v2dMag(&v);

            //t_assert(fabs(mag - expected) < DBL_EPSILON, "magnitude as expected");
            Assert.IsTrue(Math.Abs(v.Magnitude - expected) < DBL_EPSILON, "magnitude as expected");
        }

        [TestMethod]
        public void _v2dIntersect()
        {
            //Vec2d p0 = { 2.0, 2.0 };
            //Vec2d p1 = { 6.0, 6.0 };
            //Vec2d p2 = { 0.0, 4.0 };
            //Vec2d p3 = { 10.0, 4.0 };
            //Vec2d intersection = { 0.0, 0.0 };
            var p0 = new Vec2d(2, 2);
            var p1 = new Vec2d(6, 6);
            var p2 = new Vec2d(0, 4);
            var p3 = new Vec2d(10, 4);
            var intersection = new Vec2d(0, 0);

            Vec2d._v2dIntersect(p0, p1, p2, p3, ref intersection);

            double expectedX = 4.0;
            double expectedY = 4.0;

            //t_assert(fabs(intersection.x - expectedX) < DBL_EPSILON, "X coord as expected");
            Assert.IsTrue(Math.Abs(intersection.x - expectedX) < DBL_EPSILON, "X coord as expected");

            //t_assert(fabs(intersection.y - expectedY) < DBL_EPSILON, "Y coord as expected");
            Assert.IsTrue(Math.Abs(intersection.y - expectedY) < DBL_EPSILON, "Y coord as expected");
        }

        [TestMethod]
        public void _v2dEquals()
        {
            //Vec2d v1 = { 3.0, 4.0 };
            //Vec2d v2 = { 3.0, 4.0 };
            //Vec2d v3 = { 3.5, 4.0 };
            //Vec2d v4 = { 3.0, 4.5 };
            var v1 = new Vec2d(3, 4);
            var v2 = new Vec2d(3, 4);
            var v3 = new Vec2d(3.5, 4);
            var v4 = new Vec2d(3, 4.5);

            //t_assert(_v2dEquals(&v1, &v2), "true for equal vectors");
            Assert.IsTrue(v1 == v2, "true for equal vectors");

            //t_assert(!_v2dEquals(&v1, &v3), "false for different x");
            Assert.IsFalse(v1 == v3, "false for different x");

            //t_assert(!_v2dEquals(&v1, &v4), "false for different y");
            Assert.IsFalse(v1 == v4, "false for different y");
        }
    }
}
