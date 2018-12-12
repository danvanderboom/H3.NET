using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    public class LinkedGeoPolygon
    {
        public LinkedGeoLoop First;
        public LinkedGeoLoop last;
        public LinkedGeoPolygon Next;

        /// <summary>
        /// Add a linked polygon to the current polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns>New polygon</returns>
        public LinkedGeoPolygon addNewLinkedPolygon()
        {
            //assert(polygon->next == NULL);
            if (Next != null)
                throw new ArgumentException("expected polygon.next == null");

            //LinkedGeoPolygon* next = calloc(1, sizeof(*next));
            var next = new LinkedGeoPolygon();

            Next = next;

            return next;
        }

        /// <summary>
        /// Add a new linked loop to the current polygon
        /// </summary>
        /// <returns></returns>
        public LinkedGeoLoop addNewLinkedLoop()
        {
            //LinkedGeoLoop* loop = calloc(1, sizeof(*loop));
            var loop = new LinkedGeoLoop();

            //return addLinkedLoop(polygon, loop);
            return addLinkedLoop(loop);
        }

        /// <summary>
        /// Add an existing linked loop to the current polygon
        /// </summary>
        /// <param name="loop"></param>
        /// <returns>New loop</returns>
        public LinkedGeoLoop addLinkedLoop(LinkedGeoLoop loop)
        {
            if (last == null)
            {
                //assert(polygon->first == NULL);
                if (First != null)
                    throw new ArgumentException("expected polygon.first to be null");

                First = loop;
            }
            else
            {
                last.Next = loop;
            }

            last = loop;

            return loop;
        }

        /// <summary>
        /// Count the number of polygons in a linked list
        /// </summary>
        /// <returns>Count</returns>
        public int countLinkedPolygons()
        {
            var polygon = this;

            int count = 0;
            while (polygon != null)
            {
                count++;
                polygon = polygon.Next;
            }

            return count;
        }

        /// <summary>
        /// Count the number of linked loops in a polygon
        /// </summary>
        /// <returns>Count</returns>
        public int countLinkedLoops()
        {
            var polygon = this;

            int count = 0;
            var loop = polygon.First;
            while (loop != null)
            {
                count++;
                loop = loop.Next;
            }

            return count;
        }


    }
}
