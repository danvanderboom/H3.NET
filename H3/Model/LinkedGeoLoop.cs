using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public class LinkedGeoLoop
    {
        public LinkedGeoCoord First;
        public LinkedGeoCoord Last;
        public LinkedGeoLoop Next;

        /*
         * Add a new linked coordinate to the current loop
         * @param  loop   Loop to add coordinate to
         * @param  vertex Coordinate to add
         * @return        Pointer to the coordinate
         */
        public LinkedGeoCoord addLinkedCoord(GeoCoord vertex)
        {
            //LinkedGeoCoord* coord = malloc(sizeof(*coord));
            //*coord = (LinkedGeoCoord){.vertex = *vertex, .next = NULL};
            var coord = new LinkedGeoCoord { Vertex = vertex, Next = null };

            //LinkedGeoCoord* last = loop->last;
            if (Last == null)
            {
                //assert(loop->first == NULL);
                if (First != null)
                    throw new ArgumentException("expected loop.first == null");

                First = coord;
            }
            else
            {
                Last.Next = coord;
            }

            Last = coord;

            return coord;
        }

        /// <summary>
        /// Count the number of coordinates in a loop
        /// </summary>
        /// <returns>Count</returns>
        public int countLinkedCoords()
        {
            //LinkedGeoCoord* coord = loop->first;
            var coord = First;

            int count = 0;
            while (coord != null)
            {
                count++;
                coord = coord.Next;
            }

            return count;
        }

        ///// <summary>
        ///// Count the number of polygons containing a given loop.
        ///// </summary>
        ///// <param name="loop">Loop to count containers for</param>
        ///// <param name="polygons">Polygons to test</param>
        ///// <param name="bboxes">Bounding boxes for polygons, used in point-in-poly check</param>
        ///// <param name="polygonCount">Number of polygons in the test array</param>
        ///// <returns>Number of polygons containing the loop</returns>
        //public int countContainers(LinkedGeoPolygon[] polygons, BBox[] bboxes, int polygonCount)
        //{
        //    var loop = this;

        //    int containerCount = 0;
        //    for (int i = 0; i < polygonCount; i++)
        //        if (loop != polygons[i].First && pointInsideLinkedGeoLoop(polygons[i].First, bboxes[i], loop.First.Vertex))
        //            containerCount++;

        //    return containerCount;
        //}


    }
}
