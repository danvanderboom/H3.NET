using System;
using System.Collections.Generic;
using System.Text;
using static H3.Constants;
using static H3.BaseCellsUtil;

namespace H3.Model
{
    public struct FaceIJK
    {
        /// <summary>
        /// face number
        /// </summary>
        public int face;

        /// <summary>
        /// ijk coordinates on that face
        /// </summary>
        public CoordIJK coord;

        //public FaceIJK() : this(0, new CoordIJK(0, 0, 0)) { }

        public FaceIJK(int face, CoordIJK coord)
        {
            this.face = face;
            this.coord = coord;
        }

        public FaceIJK(FaceIJK prototype)
        {
            face = prototype.face;
            coord = new CoordIJK(prototype.coord.i, prototype.coord.j, prototype.coord.k);
        }

        /// <summary>
        /// Determines the center point in spherical coordinates of a cell given by
        /// a FaceIJK address at a specified resolution.
        /// </summary>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <returns>The spherical coordinates of the cell center point.</returns>
        public GeoCoord ToGeoCoord(int res) => GeoCoord.FromHex2d(coord.ToHex2d(), face, res, false);

        /// <summary>
        /// Generates the cell boundary in spherical coordinates for a cell given by a
        /// FaceIJK address at a specified resolution.
        /// </summary>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <param name="isPentagon">Whether or not the cell is a pentagon.</param>
        /// <returns>The spherical coordinates of the cell boundary.</returns>
        public GeoBoundary ToGeoBoundary(int res, bool isPentagon)
        {
            if (isPentagon)
                return PentagonToGeoBoundary(res);

            var g = new GeoBoundary();

            // the vertexes of an origin-centered cell in a Class II resolution on a
            // substrate grid with aperture sequence 33r. The aperture 3 gets us the
            // vertices, and the 3r gets us back to Class II.
            // vertices listed ccw from the i-axes
            var vertsCII = new CoordIJK[NUM_HEX_VERTS]
            {
                new CoordIJK(2, 1, 0),  // 0
                new CoordIJK(1, 2, 0),  // 1
                new CoordIJK(0, 2, 1),  // 2
                new CoordIJK(0, 1, 2),  // 3
                new CoordIJK(1, 0, 2),  // 4
                new CoordIJK(2, 0, 1)   // 5
            };

            // the vertexes of an origin-centered cell in a Class III resolution on a
            // substrate grid with aperture sequence 33r7r. The aperture 3 gets us the
            // vertices, and the 3r7r gets us to Class II.
            // vertices listed ccw from the i-axes
            var vertsCIII = new CoordIJK[NUM_HEX_VERTS]
            {
                new CoordIJK(5, 4, 0),  // 0
                new CoordIJK(1, 5, 0),  // 1
                new CoordIJK(0, 5, 4),  // 2
                new CoordIJK(0, 1, 5),  // 3
                new CoordIJK(4, 0, 5),  // 4
                new CoordIJK(5, 0, 1)   // 5
            };

            // get the correct set of substrate vertices for this resolution
            var verts = H3Index.isResClassIII(res) ? vertsCIII : vertsCII;

            // adjust the center point to be in an aperture 33r substrate grid
            // these should be composed for speed
            FaceIJK centerIJK = this;
            centerIJK.coord = centerIJK.coord._downAp3()._downAp3r();

            // if res is Class III we need to add a cw aperture 7 to get to
            // icosahedral Class II
            int adjRes = res;
            if (H3Index.isResClassIII(res))
            {
                centerIJK.coord = centerIJK.coord._downAp7r();
                adjRes++;
            }

            // The center point is now in the same substrate grid as the origin
            // cell vertices. Add the center point substate coordinates
            // to each vertex to translate the vertices to that cell.
            var fijkVerts = new FaceIJK[NUM_HEX_VERTS];
            for (int v = 0; v < NUM_HEX_VERTS; v++)
            {
                fijkVerts[v].face = centerIJK.face;
                fijkVerts[v].coord = (centerIJK.coord + verts[v]).Normalize();
            }

            // convert each vertex to lat/lon
            // adjust the face of each vertex as appropriate and introduce
            // edge-crossing vertices as needed
            g.numVerts = 0;
            int lastFace = -1;
            int lastOverage = 0;  // 0: none; 1: edge; 2: overage
            for (int vert = 0; vert < NUM_HEX_VERTS + 1; vert++)
            {
                int v = vert % NUM_HEX_VERTS;

                FaceIJK fijk = fijkVerts[v];

                int overage = AdjustOverageClassII(adjRes, false, true);

                /*
                Check for edge-crossing. Each face of the underlying icosahedron is a
                different projection plane. So if an edge of the hexagon crosses an
                icosahedron edge, an additional vertex must be introduced at that
                intersection point. Then each half of the cell edge can be projected
                to geographic coordinates using the appropriate icosahedron face
                projection. Note that Class II cell edges have vertices on the face
                edge, with no edge line intersections.
                */
                if (H3Index.isResClassIII(res) && vert > 0 && fijk.face != lastFace && lastOverage != 1)
                {
                    // find hex2d of the two vertexes on original face
                    int lastV = (v + 5) % NUM_HEX_VERTS;
                    var orig2d0 = fijkVerts[lastV].coord.ToHex2d();

                    var orig2d1 = fijkVerts[v].coord.ToHex2d();

                    // find the appropriate icosa face edge vertexes
                    int maxDim = maxDimByCIIres[adjRes];
                    var v0 = new Vec2d(3.0 * maxDim, 0.0);
                    var v1 = new Vec2d(-1.5 * maxDim, 3.0 * M_SQRT3_2 * maxDim);
                    var v2 = new Vec2d(-1.5 * maxDim, -3.0 * M_SQRT3_2 * maxDim);

                    int face2 = (lastFace == centerIJK.face) ? fijk.face : lastFace;
                    Vec2d edge0;
                    Vec2d edge1;
                    switch (adjacentFaceDir[centerIJK.face][face2])
                    {
                        case IJ:
                            edge0 = v0;
                            edge1 = v1;
                            break;
                        case JK:
                            edge0 = v1;
                            edge1 = v2;
                            break;
                        case KI:
                        default:
                            //assert(adjacentFaceDir[centerIJK.face][face2] == KI);
                            edge0 = v2;
                            edge1 = v0;
                            break;
                    }

                    // find the intersection and add the lat/lon point to the result
                    var inter = new Vec2d(0, 0);
                    Vec2d._v2dIntersect(orig2d0, orig2d1, edge0, edge1, ref inter);

                    /*
                    If a point of intersection occurs at a hexagon vertex, then each
                    adjacent hexagon edge will lie completely on a single icosahedron
                    face, and no additional vertex is required.
                    */
                    bool isIntersectionAtVertex = Vec2d._v2dEquals(orig2d0, inter) || Vec2d._v2dEquals(orig2d1, inter);
                    if (!isIntersectionAtVertex)
                    {
                        g.verts[g.numVerts] = GeoCoord.FromHex2d(inter, centerIJK.face, adjRes, true);
                        g.numVerts++;
                    }
                }

                // convert vertex to lat/lon and add to the result
                // vert == NUM_HEX_VERTS is only used to test for possible intersection
                // on last edge
                if (vert < NUM_HEX_VERTS)
                {
                    var vec = fijk.coord.ToHex2d();
                    g.verts[g.numVerts] = GeoCoord.FromHex2d(vec, fijk.face, adjRes, true);
                    g.numVerts++;
                }

                lastFace = fijk.face;
                lastOverage = overage;
            }

            return g;
        }

        /// <summary>
        /// Generates the cell boundary in spherical coordinates for a pentagonal cell
        /// given by a FaceIJK address at a specified resolution.
        /// </summary>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <returns>The spherical coordinates of the cell boundary.</returns>
        public GeoBoundary PentagonToGeoBoundary(int res)
        {
            var g = new GeoBoundary();

            // the vertexes of an origin-centered pentagon in a Class II resolution on a
            // substrate grid with aperture sequence 33r. The aperture 3 gets us the
            // vertices, and the 3r gets us back to Class II.
            // vertices listed ccw from the i-axes
            var vertsCII = new CoordIJK[NUM_PENT_VERTS]
            {
                new CoordIJK(2, 1, 0),  // 0
                new CoordIJK(1, 2, 0),  // 1
                new CoordIJK(0, 2, 1),  // 2
                new CoordIJK(0, 1, 2),  // 3
                new CoordIJK(1, 0, 2),  // 4
            };

            // the vertexes of an origin-centered pentagon in a Class III resolution on
            // a substrate grid with aperture sequence 33r7r. The aperture 3 gets us the
            // vertices, and the 3r7r gets us to Class II. vertices listed ccw from the
            // i-axes
            var vertsCIII = new CoordIJK[NUM_PENT_VERTS]
            {
                new CoordIJK(5, 4, 0),  // 0
                new CoordIJK(1, 5, 0),  // 1
                new CoordIJK(0, 5, 4),  // 2
                new CoordIJK(0, 1, 5),  // 3
                new CoordIJK(4, 0, 5),  // 4
            };

            // get the correct set of substrate vertices for this resolution
            CoordIJK[] verts = H3Index.isResClassIII(res) ? vertsCIII : vertsCII;

            // adjust the center point to be in an aperture 33r substrate grid
            // these should be composed for speed
            FaceIJK centerIJK = this;
            centerIJK.coord = centerIJK.coord._downAp3()._downAp3r();

            // if res is Class III we need to add a cw aperture 7 to get to
            // icosahedral Class II
            int adjRes = res;
            if (H3Index.isResClassIII(res))
            {
                centerIJK.coord = centerIJK.coord._downAp7r();
                adjRes++;
            }

            // The center point is now in the same substrate grid as the origin
            // cell vertices. Add the center point substate coordinates
            // to each vertex to translate the vertices to that cell.
            FaceIJK[] fijkVerts = new FaceIJK[NUM_PENT_VERTS];
            for (int v = 0; v < NUM_PENT_VERTS; v++)
            {
                fijkVerts[v].face = centerIJK.face;
                fijkVerts[v].coord = (centerIJK.coord + verts[v]).Normalize();
            }

            // convert each vertex to lat/lon
            // adjust the face of each vertex as appropriate and introduce
            // edge-crossing vertices as needed
            g.numVerts = 0;
            var lastFijk = new FaceIJK(0, new CoordIJK(0, 0, 0));
            for (int vert = 0; vert < NUM_PENT_VERTS + 1; vert++)
            {
                int v = vert % NUM_PENT_VERTS;

                FaceIJK fijk = fijkVerts[v];

                var pentLeading4 = false;
                int overage = AdjustOverageClassII(adjRes, pentLeading4, true);
                if (overage == 2)  // in a different triangle
                {
                    while (true)
                    {
                        overage = AdjustOverageClassII(adjRes, pentLeading4, true);
                        if (overage != 2)  // not in a different triangle
                            break;
                    }
                }

                // all Class III pentagon edges cross icosa edges
                // note that Class II pentagons have vertices on the edge,
                // not edge intersections
                if (H3Index.isResClassIII(res) && vert > 0)
                {
                    // find hex2d of the two vertexes on the last face

                    FaceIJK tmpFijk = fijk;

                    var orig2d0 = lastFijk.coord.ToHex2d();

                    int currentToLastDir = adjacentFaceDir[tmpFijk.face][lastFijk.face];

                    var fijkOrient = faceNeighbors[tmpFijk.face][currentToLastDir];

                    tmpFijk.face = fijkOrient.face;
                    CoordIJK ijk = tmpFijk.coord;

                    // rotate and translate for adjacent face
                    for (int i = 0; i < fijkOrient.ccwRot60; i++)
                        ijk = ijk._ijkRotate60ccw();

                    CoordIJK transVec = fijkOrient.translate;
                    transVec *= unitScaleByCIIres[adjRes] * 3;
                    ijk = (ijk + transVec).Normalize();

                    var orig2d1 = ijk.ToHex2d();

                    // find the appropriate icosa face edge vertexes
                    int maxDim = maxDimByCIIres[adjRes];
                    var v0 = new Vec2d(3.0 * maxDim, 0.0);
                    var v1 = new Vec2d(-1.5 * maxDim, 3.0 * M_SQRT3_2 * maxDim);
                    var v2 = new Vec2d(-1.5 * maxDim, -3.0 * M_SQRT3_2 * maxDim);

                    Vec2d edge0;
                    Vec2d edge1;
                    switch (adjacentFaceDir[tmpFijk.face][fijk.face])
                    {
                        case IJ:
                            edge0 = v0;
                            edge1 = v1;
                            break;
                        case JK:
                            edge0 = v1;
                            edge1 = v2;
                            break;
                        case KI:
                        default:
                            //assert(adjacentFaceDir[tmpFijk.face][fijk.face] == KI);
                            edge0 = v2;
                            edge1 = v0;
                            break;
                    }

                    // find the intersection and add the lat/lon point to the result
                    var inter = new Vec2d(0, 0);
                    Vec2d._v2dIntersect(orig2d0, orig2d1, edge0, edge1, ref inter);
                    g.verts[g.numVerts] = GeoCoord.FromHex2d(inter, tmpFijk.face, adjRes, true);
                    g.numVerts++;
                }

                // convert vertex to lat/lon and add to the result
                // vert == NUM_PENT_VERTS is only used to test for possible intersection
                // on last edge
                if (vert < NUM_PENT_VERTS)
                {
                    var vec = fijk.coord.ToHex2d();
                    g.verts[g.numVerts] = GeoCoord.FromHex2d(vec, fijk.face, adjRes, true);
                    g.numVerts++;
                }

                lastFijk = fijk;
            }

            return g;
        }

        /// <summary>
        /// Find base cell given FaceIJK.
        /// 
        /// Given the face number and a resolution 0 ijk+ coordinate in that face's
        /// face-centered ijk coordinate system, return the base cell located at that
        /// coordinate.
        /// 
        /// Valid ijk+ lookup coordinates are from(0, 0, 0) to(2, 2, 2).
        /// </summary>
        public int ToBaseCell() => faceIjkBaseCells[face][coord.i][coord.j][coord.k].baseCell;

        /// <summary>
        /// Convert an FaceIJK address to the corresponding H3Index.
        /// </summary>
        /// <param name="res">The cell resolution.</param>
        /// <returns>The encoded H3Index (or 0 on failure).</returns>
        public H3Index ToH3Index(int res)
        {
            // initialize the index
            var h = new H3Index(H3Index.H3_INIT);
            h.Mode = H3_HEXAGON_MODE;
            h.Resolution = res;

            // check for res 0/base cell
            if (res == 0)
            {
                if (coord.i > MAX_FACE_COORD || coord.j > MAX_FACE_COORD || coord.k > MAX_FACE_COORD)
                    return H3Index.H3_INVALID_INDEX; // out of range input

                // TODO: fix
                h.BaseCell = this.ToBaseCell();
                return h;
            }

            // we need to find the correct base cell FaceIJK for this H3 index;
            // start with the passed in face and resolution res ijk coordinates
            // in that face's coordinate system
            //var fijkBC = new FaceIJK(this);
            var fijkBC = new FaceIJK(this);

            // build the H3Index from finest res up
            // adjust r for the fact that the res 0 base cell offsets the indexing digits
            //CoordIJK* ijk = &fijkBC.coord;
            var ijk = fijkBC.coord;
            for (int r = res - 1; r >= 0; r--)
            {
                var lastIJK = new CoordIJK(ijk);
                CoordIJK lastCenter;

                if (H3Index.isResClassIII(r + 1))
                {
                    // rotate ccw
                    ijk = ijk._upAp7();
                    lastCenter = ijk._downAp7();
                }
                else
                {
                    // rotate cw
                    ijk = ijk._upAp7r();
                    lastCenter = ijk._downAp7r();
                }

                var normalDiff = (lastIJK - lastCenter).Normalize();

                h.SetIndexDigit(r + 1, normalDiff.UnitIJKToDigit());
            }

            // fijkBC should now hold the IJK of the base cell in the
            // coordinate system of the current face

            if (fijkBC.coord.i > MAX_FACE_COORD || fijkBC.coord.j > MAX_FACE_COORD || fijkBC.coord.k > MAX_FACE_COORD)
                return H3Index.H3_INVALID_INDEX; // out of range input

            // TODO: check, added this check for debugging since it leads to invalid negative array indexes
            //if (fijkBC.coord.i < 0 || fijkBC.coord.j < 0 || fijkBC.coord.k < 0)
            //    return H3Index.H3_INVALID_INDEX; // out of range input

            // lookup the correct base cell
            var baseCell = fijkBC.ToBaseCell();
            h.BaseCell = baseCell;

            // rotate if necessary to get canonical base cell orientation
            // for this base cell
            int numRots = _faceIjkToBaseCellCCWrot60(fijkBC);
            if (_isBaseCellPentagon(baseCell))
            {
                // force rotation out of missing k-axes sub-sequence
                if ((int)h._h3LeadingNonZeroDigit() == (int)Direction.K_AXES_DIGIT)
                {
                    // check for a cw/ccw offset face; default is ccw
                    if (_baseCellIsCwOffset(baseCell, fijkBC.face))
                        h = h._h3Rotate60cw();
                    else
                        h = h._h3Rotate60ccw();
                }

                for (int i = 0; i < numRots; i++)
                    h = h._h3RotatePent60ccw();
            }
            else
            {
                for (int i = 0; i < numRots; i++)
                    h = h._h3Rotate60ccw();
            }

            return h;
        }

        /// <summary>
        /// Adjusts a FaceIJK address in place so that the resulting cell address is
        /// relative to the correct icosahedral face.
        /// </summary>
        /// <param name="res">The H3 resolution of the cell.</param>
        /// <param name="pentLeading4">Whether or not the cell is a pentagon with a leading digit 4.</param>
        /// <param name="isSubstrate">Whether or not the cell is in a substrate grid.</param>
        /// <returns>0 if on original face (no overage); 1 if on face edge (only occurs 
        /// on substrate grids); 2 if overage on new face interior</returns>
        public int AdjustOverageClassII(int res, bool pentLeading4, bool isSubstrate)
        {
            int overage = 0;

            CoordIJK ijk = coord;

            // get the maximum dimension value; scale if a substrate grid
            int maxDim = FaceIJK.maxDimByCIIres[res];
            if (isSubstrate)
                maxDim *= 3;

            // check for overage
            if (isSubstrate && ijk.i + ijk.j + ijk.k == maxDim)  // on edge
                overage = 1;
            else if (ijk.i + ijk.j + ijk.k > maxDim)  // overage
            {
                overage = 2;

                FaceOrientIJK fijkOrient;
                if (ijk.k > 0)
                {
                    if (ijk.j > 0)  // jk "quadrant"
                        fijkOrient = FaceIJK.faceNeighbors[face][JK];
                    else  // ik "quadrant"
                    {
                        fijkOrient = FaceIJK.faceNeighbors[face][KI];

                        // adjust for the pentagonal missing sequence
                        if (pentLeading4)
                        {
                            // translate origin to center of pentagon
                            var origin = new CoordIJK(maxDim, 0, 0);

                            var tmp = ijk - origin;

                            // rotate to adjust for the missing sequence
                            tmp = tmp._ijkRotate60cw();

                            // translate the origin back to the center of the triangle
                            ijk = tmp + origin;
                        }
                    }
                }
                else  // ij "quadrant"
                    fijkOrient = FaceIJK.faceNeighbors[face][IJ];

                face = fijkOrient.face;

                // rotate and translate for adjacent face
                for (int i = 0; i < fijkOrient.ccwRot60; i++)
                    ijk = ijk._ijkRotate60ccw();

                CoordIJK transVec = fijkOrient.translate;
                int unitScale = FaceIJK.unitScaleByCIIres[res];
                if (isSubstrate) unitScale *= 3;
                transVec *= unitScale;
                ijk = (ijk + transVec).Normalize();

                // overage points on pentagon boundaries can end up on edges
                if (isSubstrate && ijk.i + ijk.j + ijk.k == maxDim)  // on edge
                    overage = 1;
            }

            return overage;
        }






        #region Constants & Reference Data


        // indexes for faceNeighbors table
        /** Invalid faceNeighbors table direction */
        public const int INVALID = -1;
        /** Center faceNeighbors table direction */
        public const int CENTER = 0;
        /** IJ quadrant faceNeighbors table direction */
        public const int IJ = 1;
        /** KI quadrant faceNeighbors table direction */
        public const int KI = 2;
        /** JK quadrant faceNeighbors table direction */
        public const int JK = 3;

        /** @brief icosahedron face centers in lat/lon radians */
        public static GeoCoord[] faceCenterGeo = new GeoCoord[NUM_ICOSA_FACES]
        {
            new GeoCoord(0.803582649718989942, 1.248397419617396099),    // face  0
            new GeoCoord(1.307747883455638156, 2.536945009877921159),    // face  1
            new GeoCoord(1.054751253523952054, -1.347517358900396623),   // face  2
            new GeoCoord(0.600191595538186799, -0.450603909469755746),   // face  3
            new GeoCoord(0.491715428198773866, 0.401988202911306943),    // face  4
            new GeoCoord(0.172745327415618701, 1.678146885280433686),    // face  5
            new GeoCoord(0.605929321571350690, 2.953923329812411617),    // face  6
            new GeoCoord(0.427370518328979641, -1.888876200336285401),   // face  7
            new GeoCoord(-0.079066118549212831, -0.733429513380867741),  // face  8
            new GeoCoord(-0.230961644455383637, 0.506495587332349035),   // face  9
            new GeoCoord(0.079066118549212831, 2.408163140208925497),    // face 10
            new GeoCoord(0.230961644455383637, -2.635097066257444203),   // face 11
            new GeoCoord(-0.172745327415618701, -1.463445768309359553),  // face 12
            new GeoCoord(-0.605929321571350690, -0.187669323777381622),  // face 13
            new GeoCoord(-0.427370518328979641, 1.252716453253507838),   // face 14
            new GeoCoord(-0.600191595538186799, 2.690988744120037492),   // face 15
            new GeoCoord(-0.491715428198773866, -2.739604450678486295),  // face 16
            new GeoCoord(-0.803582649718989942, -1.893195233972397139),  // face 17
            new GeoCoord(-1.307747883455638156, -0.604647643711872080),  // face 18
            new GeoCoord(-1.054751253523952054, 1.794075294689396615),   // face 19
        };

        /** @brief icosahedron face centers in x/y/z on the unit sphere */
        public static Vec3d[] faceCenterPoint = new Vec3d[NUM_ICOSA_FACES]
        {
            new Vec3d(0.2199307791404606, 0.6583691780274996, 0.7198475378926182),     // face  0
            new Vec3d(-0.2139234834501421, 0.1478171829550703, 0.9656017935214205),    // face  1
            new Vec3d(0.1092625278784797, -0.4811951572873210, 0.8697775121287253),    // face  2
            new Vec3d(0.7428567301586791, -0.3593941678278028, 0.5648005936517033),    // face  3
            new Vec3d(0.8112534709140969, 0.3448953237639384, 0.4721387736413930),     // face  4
            new Vec3d(-0.1055498149613921, 0.9794457296411413, 0.1718874610009365),    // face  5
            new Vec3d(-0.8075407579970092, 0.1533552485898818, 0.5695261994882688),    // face  6
            new Vec3d(-0.2846148069787907, -0.8644080972654206, 0.4144792552473539),   // face  7
            new Vec3d(0.7405621473854482, -0.6673299564565524, -0.0789837646326737),   // face  8
            new Vec3d(0.8512303986474293, 0.4722343788582681, -0.2289137388687808),    // face  9
            new Vec3d(-0.7405621473854481, 0.6673299564565524, 0.0789837646326737),    // face 10
            new Vec3d(-0.8512303986474292, -0.4722343788582682, 0.2289137388687808),   // face 11
            new Vec3d(0.1055498149613919, -0.9794457296411413, -0.1718874610009365),   // face 12
            new Vec3d(0.8075407579970092, -0.1533552485898819, -0.5695261994882688),   // face 13
            new Vec3d(0.2846148069787908, 0.8644080972654204, -0.4144792552473539),    // face 14
            new Vec3d(-0.7428567301586791, 0.3593941678278027, -0.5648005936517033),   // face 15
            new Vec3d(-0.8112534709140971, -0.3448953237639382, -0.4721387736413930),  // face 16
            new Vec3d(-0.2199307791404607, -0.6583691780274996, -0.7198475378926182),  // face 17
            new Vec3d(0.2139234834501420, -0.1478171829550704, -0.9656017935214205),   // face 18
            new Vec3d(-0.1092625278784796, 0.4811951572873210, -0.8697775121287253),   // face 19
        };

        /* @brief icosahedron face ijk axes as azimuth in radians from face center to
         * vertex 0/1/2 respectively
         */
        public static double[][] faceAxesAzRadsCII = new double[NUM_ICOSA_FACES][]
        {
            new double[] { 5.619958268523939882, 3.525563166130744542, 1.431168063737548730 },  // face  0
            new double[] { 5.760339081714187279, 3.665943979320991689, 1.571548876927796127 },  // face  1
            new double[] { 0.780213654393430055, 4.969003859179821079, 2.874608756786625655 },  // face  2
            new double[] { 0.430469363979999913, 4.619259568766391033, 2.524864466373195467 },  // face  3
            new double[] { 6.130269123335111400, 4.035874020941915804, 1.941478918548720291 },  // face  4
            new double[] { 2.692877706530642877, 0.598482604137447119, 4.787272808923838195 },  // face  5
            new double[] { 2.982963003477243874, 0.888567901084048369, 5.077358105870439581 },  // face  6
            new double[] { 3.532912002790141181, 1.438516900396945656, 5.627307105183336758 },  // face  7
            new double[] { 3.494305004259568154, 1.399909901866372864, 5.588700106652763840 },  // face  8
            new double[] { 3.003214169499538391, 0.908819067106342928, 5.097609271892733906 },  // face  9
            new double[] { 5.930472956509811562, 3.836077854116615875, 1.741682751723420374 },  // face 10
            new double[] { 0.138378484090254847, 4.327168688876645809, 2.232773586483450311 },  // face 11
            new double[] { 0.448714947059150361, 4.637505151845541521, 2.543110049452346120 },  // face 12
            new double[] { 0.158629650112549365, 4.347419854898940135, 2.253024752505744869 },  // face 13
            new double[] { 5.891865957979238535, 3.797470855586042958, 1.703075753192847583 },  // face 14
            new double[] { 2.711123289609793325, 0.616728187216597771, 4.805518392002988683 },  // face 15
            new double[] { 3.294508837434268316, 1.200113735041072948, 5.388903939827463911 },  // face 16
            new double[] { 3.804819692245439833, 1.710424589852244509, 5.899214794638635174 },  // face 17
            new double[] { 3.664438879055192436, 1.570043776661997111, 5.758833981448388027 },  // face 18
            new double[] { 2.361378999196363184, 0.266983896803167583, 4.455774101589558636 },  // face 19
        };

        /** @brief Definition of which faces neighbor each other. */
        public static FaceOrientIJK[][] faceNeighbors = new FaceOrientIJK[NUM_ICOSA_FACES][]
        {
            new FaceOrientIJK[]
            {
                // face 0
                new FaceOrientIJK(0, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(4, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(1, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(5, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 1
                new FaceOrientIJK(1, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(0, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(2, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(6, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 2
                new FaceOrientIJK(2, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(1, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(3, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(7, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 3
                new FaceOrientIJK(3, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(2, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(4, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(8, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 4
                new FaceOrientIJK(4, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(3, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(0, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(9, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 5
                new FaceOrientIJK(5, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(10, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(14, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(0, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 6
                new FaceOrientIJK(6, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(11, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(10, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(1, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 7
                new FaceOrientIJK(7, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(12, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(11, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(2, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 8
                new FaceOrientIJK(8, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(13, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(12, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(3, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 9
                new FaceOrientIJK(9, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(14, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(13, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(4, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 10
                new FaceOrientIJK(10, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(5, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(6, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(15, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 11
                new FaceOrientIJK(11, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(6, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(7, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(16, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 12
                new FaceOrientIJK(12, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(7, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(8, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(17, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 13
                new FaceOrientIJK(13, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(8, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(9, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(18, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 14
                new FaceOrientIJK(14, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(9, new CoordIJK(2, 0, 2), 3), // ij quadrant
                new FaceOrientIJK(5, new CoordIJK(2, 2, 0), 3), // ki quadrant
                new FaceOrientIJK(19, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 15
                new FaceOrientIJK(15, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(16, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(19, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(10, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 16
                new FaceOrientIJK(16, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(17, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(15, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(11, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 17
                new FaceOrientIJK(17, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(18, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(16, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(12, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 18
                new FaceOrientIJK(18, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(19, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(17, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(13, new CoordIJK(0, 2, 2), 3), // jk quadrant
            },
            new FaceOrientIJK[]
            {
                // face 19
                new FaceOrientIJK(19, new CoordIJK(0, 0, 0), 0), // central face
                new FaceOrientIJK(15, new CoordIJK(2, 0, 2), 1), // ij quadrant
                new FaceOrientIJK(18, new CoordIJK(2, 2, 0), 5), // ki quadrant
                new FaceOrientIJK(14, new CoordIJK(0, 2, 2), 3), // jk quadrant
            }
        };

        /* @brief direction from the origin face to the destination face, relative to
         * the origin face's coordinate system, or -1 if not adjacent.
         */
        public static int[][] adjacentFaceDir = new int[NUM_ICOSA_FACES][] // int[NUM_ICOSA_FACES][NUM_ICOSA_FACES]
        {
            new int[] { 0,  KI, -1, -1, IJ, JK, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },  // face 0
            new int[] { IJ, 0,  KI, -1, -1, -1, JK, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },  // face 1
            new int[] { -1, IJ, 0,  KI, -1, -1, -1, JK, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },  // face 2
            new int[] { -1, -1, IJ, 0,  KI, -1, -1, -1, JK, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },  // face 3
            new int[] { KI, -1, -1, IJ, 0,  -1, -1, -1, -1, JK, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },  // face 4
            new int[] { JK, -1, -1, -1, -1, 0,  -1, -1, -1, -1, IJ, -1, -1, -1, KI, -1, -1, -1, -1, -1 },  // face 5
            new int[] { -1, JK, -1, -1, -1, -1, 0,  -1, -1, -1, KI, IJ, -1, -1, -1, -1, -1, -1, -1, -1 },  // face 6
            new int[] { -1, -1, JK, -1, -1, -1, -1, 0,  -1, -1, -1, KI, IJ, -1, -1, -1, -1, -1, -1, -1 },  // face 7
            new int[] { -1, -1, -1, JK, -1, -1, -1, -1, 0,  -1, -1, -1, KI, IJ, -1, -1, -1, -1, -1, -1 },  // face 8
            new int[] { -1, -1, -1, -1, JK, -1, -1, -1, -1, 0, -1, -1, -1, KI, IJ, -1, -1, -1, -1, -1 },   // face 9
            new int[] { -1, -1, -1, -1, -1, IJ, KI, -1, -1, -1, 0,  -1, -1, -1, -1, JK, -1, -1, -1, -1 },  // face 10
            new int[] { -1, -1, -1, -1, -1, -1, IJ, KI, -1, -1, -1, 0,  -1, -1, -1, -1, JK, -1, -1, -1 },  // face 11
            new int[] { -1, -1, -1, -1, -1, -1, -1, IJ, KI, -1, -1, -1, 0,  -1, -1, -1, -1, JK, -1, -1 },  // face 12
            new int[] { -1, -1, -1, -1, -1, -1, -1, -1, IJ, KI, -1, -1, -1, 0,  -1, -1, -1, -1, JK, -1 },  // face 13
            new int[] { -1, -1, -1, -1, -1, KI, -1, -1, -1, IJ, -1, -1, -1, -1, 0,  -1, -1, -1, -1, JK },  // face 14
            new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, JK, -1, -1, -1, -1, 0,  IJ, -1, -1, KI },  // face 15
            new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, JK, -1, -1, -1, KI, 0,  IJ, -1, -1 },  // face 16
            new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, JK, -1, -1, -1, KI, 0,  IJ, -1 },  // face 17
            new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, JK, -1, -1, -1, KI, 0,  IJ },  // face 18
            new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, JK, IJ, -1, -1, KI, 0 }    // face 19
        };

        /** @brief overage distance table */
        public static int[] maxDimByCIIres = new int[]
        {
            2,        // res  0
            -1,       // res  1
            14,       // res  2
            -1,       // res  3
            98,       // res  4
            -1,       // res  5
            686,      // res  6
            -1,       // res  7
            4802,     // res  8
            -1,       // res  9
            33614,    // res 10
            -1,       // res 11
            235298,   // res 12
            -1,       // res 13
            1647086,  // res 14
            -1,       // res 15
            11529602  // res 16
        };

        /** @brief unit scale distance table */
        public static int[] unitScaleByCIIres = new int[]
        {
            1,       // res  0
            -1,      // res  1
            7,       // res  2
            -1,      // res  3
            49,      // res  4
            -1,      // res  5
            343,     // res  6
            -1,      // res  7
            2401,    // res  8
            -1,      // res  9
            16807,   // res 10
            -1,      // res 11
            117649,  // res 12
            -1,      // res 13
            823543,  // res 14
            -1,      // res 15
            5764801  // res 16
        };

        #endregion
    }
}
