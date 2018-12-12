using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static H3.Constants;
using H3.Model;


namespace H3Test
{
    [TestClass]
    public class H3IndexToGeoCoordTests
    {
        void assertExpected(H3Index h1, GeoCoord g1)
        {
            double epsilon = 0.000001 * M_PI_180;

            // convert H3 to lat/lon and verify
            // H3_EXPORT(h3ToGeo)(h1, &g2);
            GeoCoord g2 = h1.ToGeoCoord();

            Assert.IsTrue(GeoCoord.AlmostEqualThreshold(g2, g1, epsilon), "got expected h3ToGeo output");

            // Convert back to H3 to verify
            //int res = H3_EXPORT(h3GetResolution)(h1);
            //H3Index h2 = H3_EXPORT(geoToH3)(&g2, res);
            var h2 = g2.ToH3Index(h1.Resolution);

            Assert.AreEqual(h1, h2, "got expected geoToH3 output");
        }

        [TestMethod]
        public async Task Conversions_H3Index_GeoCoord()
        {
            for (int i = 0; i <= 4; i++)
            {
                var fileName = $"res0{i}ic.txt";
                var path = Path.Combine(Environment.CurrentDirectory, "TestData", fileName);

                using (var sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        var match = Regex.Match(line, @"(?<h3>\w+)\s(?<lat>[-+]?([0-9]*\.[0-9]+|[0-9]+))\s(?<lon>[-+]?([0-9]*\.[0-9]+|[0-9]+))");
                            
                        var h3str = match.Groups["h3"].Value;
                        var lat = double.Parse(match.Groups["lat"].Value);
                        var lon = double.Parse(match.Groups["lon"].Value);

                        //h3 = H3_EXPORT(stringToH3)(h3Str);
                        var h3index = new H3Index(h3str);

                        //setGeoDegs(&coord, latDegs, lonDegs);
                        var coord = new GeoCoord(lat, lon);

                        assertExpected(h3index, coord);
                    }
                }
            }

        }
    }
}
