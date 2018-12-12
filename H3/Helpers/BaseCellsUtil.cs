using System;
using System.Collections.Generic;
using System.Text;
using static H3.Constants;
using H3.Model;

namespace H3
{
    class BaseCellsUtil
    {
        public const int INVALID_BASE_CELL = 127;

        /** Maximum input for any component to face-to-base-cell lookup functions */
        public const int MAX_FACE_COORD = 2;

        /* @brief Neighboring base cell ID in each IJK direction.
         *
         * For each base cell, for each direction, the neighboring base
         * cell ID is given. 127 indicates there is no neighbor in that direction.
         */
        public static int[][] baseCellNeighbors = new int[NUM_BASE_CELLS][] // [NUM_BASE_CELLS][7] 
        {
            new int[] { 0, 1, 5, 2, 4, 3, 8 },                               // base cell 0
            new int[] { 1, 7, 6, 9, 0, 3, 2 },                               // base cell 1
            new int[] { 2, 6, 10, 11, 0, 1, 5 },                             // base cell 2
            new int[] { 3, 13, 1, 7, 4, 12, 0 },                             // base cell 3
            new int[] { 4, INVALID_BASE_CELL, 15, 8, 3, 0, 12 },             // base cell 4 (pentagon)
            new int[] { 5, 2, 18, 10, 8, 0, 16 },                            // base cell 5
            new int[] { 6, 14, 11, 17, 1, 9, 2 },                            // base cell 6
            new int[] { 7, 21, 9, 19, 3, 13, 1 },                            // base cell 7
            new int[] { 8, 5, 22, 16, 4, 0, 15 },                            // base cell 8
            new int[] { 9, 19, 14, 20, 1, 7, 6 },                            // base cell 9
            new int[] { 10, 11, 24, 23, 5, 2, 18 },                          // base cell 10
            new int[] { 11, 17, 23, 25, 2, 6, 10 },                          // base cell 11
            new int[] { 12, 28, 13, 26, 4, 15, 3 },                          // base cell 12
            new int[] { 13, 26, 21, 29, 3, 12, 7 },                          // base cell 13
            new int[] { 14, INVALID_BASE_CELL, 17, 27, 9, 20, 6 },           // base cell 14 (pentagon)
            new int[] { 15, 22, 28, 31, 4, 8, 12 },                          // base cell 15
            new int[] { 16, 18, 33, 30, 8, 5, 22 },                          // base cell 16
            new int[] { 17, 11, 14, 6, 35, 25, 27 },                         // base cell 17
            new int[] { 18, 24, 30, 32, 5, 10, 16 },                         // base cell 18
            new int[] { 19, 34, 20, 36, 7, 21, 9 },                          // base cell 19
            new int[] { 20, 14, 19, 9, 40, 27, 36 },                         // base cell 20
            new int[] { 21, 38, 19, 34, 13, 29, 7 },                         // base cell 21
            new int[] { 22, 16, 41, 33, 15, 8, 31 },                         // base cell 22
            new int[] { 23, 24, 11, 10, 39, 37, 25 },                        // base cell 23
            new int[] { 24, INVALID_BASE_CELL, 32, 37, 10, 23, 18 },         // base cell 24 (pentagon)
            new int[] { 25, 23, 17, 11, 45, 39, 35 },                        // base cell 25
            new int[] { 26, 42, 29, 43, 12, 28, 13 },                        // base cell 26
            new int[] { 27, 40, 35, 46, 14, 20, 17 },                        // base cell 27
            new int[] { 28, 31, 42, 44, 12, 15, 26 },                        // base cell 28
            new int[] { 29, 43, 38, 47, 13, 26, 21 },                        // base cell 29
            new int[] { 30, 32, 48, 50, 16, 18, 33 },                        // base cell 30
            new int[] { 31, 41, 44, 53, 15, 22, 28 },                        // base cell 31
            new int[] { 32, 30, 24, 18, 52, 50, 37 },                        // base cell 32
            new int[] { 33, 30, 49, 48, 22, 16, 41 },                        // base cell 33
            new int[] { 34, 19, 38, 21, 54, 36, 51 },                        // base cell 34
            new int[] { 35, 46, 45, 56, 17, 27, 25 },                        // base cell 35
            new int[] { 36, 20, 34, 19, 55, 40, 54 },                        // base cell 36
            new int[] { 37, 39, 52, 57, 24, 23, 32 },                        // base cell 37
            new int[] { 38, INVALID_BASE_CELL, 34, 51, 29, 47, 21 },         // base cell 38 (pentagon)
            new int[] { 39, 37, 25, 23, 59, 57, 45 },                        // base cell 39
            new int[] { 40, 27, 36, 20, 60, 46, 55 },                        // base cell 40
            new int[] { 41, 49, 53, 61, 22, 33, 31 },                        // base cell 41
            new int[] { 42, 58, 43, 62, 28, 44, 26 },                        // base cell 42
            new int[] { 43, 62, 47, 64, 26, 42, 29 },                        // base cell 43
            new int[] { 44, 53, 58, 65, 28, 31, 42 },                        // base cell 44
            new int[] { 45, 39, 35, 25, 63, 59, 56 },                        // base cell 45
            new int[] { 46, 60, 56, 68, 27, 40, 35 },                        // base cell 46
            new int[] { 47, 38, 43, 29, 69, 51, 64 },                        // base cell 47
            new int[] { 48, 49, 30, 33, 67, 66, 50 },                        // base cell 48
            new int[] { 49, INVALID_BASE_CELL, 61, 66, 33, 48, 41 },         // base cell 49 (pentagon)
            new int[] { 50, 48, 32, 30, 70, 67, 52 },                        // base cell 50
            new int[] { 51, 69, 54, 71, 38, 47, 34 },                        // base cell 51
            new int[] { 52, 57, 70, 74, 32, 37, 50 },                        // base cell 52
            new int[] { 53, 61, 65, 75, 31, 41, 44 },                        // base cell 53
            new int[] { 54, 71, 55, 73, 34, 51, 36 },                        // base cell 54
            new int[] { 55, 40, 54, 36, 72, 60, 73 },                        // base cell 55
            new int[] { 56, 68, 63, 77, 35, 46, 45 },                        // base cell 56
            new int[] { 57, 59, 74, 78, 37, 39, 52 },                        // base cell 57
            new int[] { 58, INVALID_BASE_CELL, 62, 76, 44, 65, 42 },         // base cell 58 (pentagon)
            new int[] { 59, 63, 78, 79, 39, 45, 57 },                        // base cell 59
            new int[] { 60, 72, 68, 80, 40, 55, 46 },                        // base cell 60
            new int[] { 61, 53, 49, 41, 81, 75, 66 },                        // base cell 61
            new int[] { 62, 43, 58, 42, 82, 64, 76 },                        // base cell 62
            new int[] { 63, INVALID_BASE_CELL, 56, 45, 79, 59, 77 },         // base cell 63 (pentagon)
            new int[] { 64, 47, 62, 43, 84, 69, 82 },                        // base cell 64
            new int[] { 65, 58, 53, 44, 86, 76, 75 },                        // base cell 65
            new int[] { 66, 67, 81, 85, 49, 48, 61 },                        // base cell 66
            new int[] { 67, 66, 50, 48, 87, 85, 70 },                        // base cell 67
            new int[] { 68, 56, 60, 46, 90, 77, 80 },                        // base cell 68
            new int[] { 69, 51, 64, 47, 89, 71, 84 },                        // base cell 69
            new int[] { 70, 67, 52, 50, 83, 87, 74 },                        // base cell 70
            new int[] { 71, 89, 73, 91, 51, 69, 54 },                        // base cell 71
            new int[] { 72, INVALID_BASE_CELL, 73, 55, 80, 60, 88 },         // base cell 72 (pentagon)
            new int[] { 73, 91, 72, 88, 54, 71, 55 },                        // base cell 73
            new int[] { 74, 78, 83, 92, 52, 57, 70 },                        // base cell 74
            new int[] { 75, 65, 61, 53, 94, 86, 81 },                        // base cell 75
            new int[] { 76, 86, 82, 96, 58, 65, 62 },                        // base cell 76
            new int[] { 77, 63, 68, 56, 93, 79, 90 },                        // base cell 77
            new int[] { 78, 74, 59, 57, 95, 92, 79 },                        // base cell 78
            new int[] { 79, 78, 63, 59, 93, 95, 77 },                        // base cell 79
            new int[] { 80, 68, 72, 60, 99, 90, 88 },                        // base cell 80
            new int[] { 81, 85, 94, 101, 61, 66, 75 },                       // base cell 81
            new int[] { 82, 96, 84, 98, 62, 76, 64 },                        // base cell 82
            new int[] { 83, INVALID_BASE_CELL, 74, 70, 100, 87, 92 },        // base cell 83 (pentagon)
            new int[] { 84, 69, 82, 64, 97, 89, 98 },                        // base cell 84
            new int[] { 85, 87, 101, 102, 66, 67, 81 },                      // base cell 85
            new int[] { 86, 76, 75, 65, 104, 96, 94 },                       // base cell 86
            new int[] { 87, 83, 102, 100, 67, 70, 85 },                      // base cell 87
            new int[] { 88, 72, 91, 73, 99, 80, 105 },                       // base cell 88
            new int[] { 89, 97, 91, 103, 69, 84, 71 },                       // base cell 89
            new int[] { 90, 77, 80, 68, 106, 93, 99 },                       // base cell 90
            new int[] { 91, 73, 89, 71, 105, 88, 103 },                      // base cell 91
            new int[] { 92, 83, 78, 74, 108, 100, 95 },                      // base cell 92
            new int[] { 93, 79, 90, 77, 109, 95, 106 },                      // base cell 93
            new int[] { 94, 86, 81, 75, 107, 104, 101 },                     // base cell 94
            new int[] { 95, 92, 79, 78, 109, 108, 93 },                      // base cell 95
            new int[] { 96, 104, 98, 110, 76, 86, 82 },                      // base cell 96
            new int[] { 97, INVALID_BASE_CELL, 98, 84, 103, 89, 111 },       // base cell 97 (pentagon)
            new int[] { 98, 110, 97, 111, 82, 96, 84 },                      // base cell 98
            new int[] { 99, 80, 105, 88, 106, 90, 113 },                     // base cell 99
            new int[] { 100, 102, 83, 87, 108, 114, 92 },                    // base cell 100
            new int[] { 101, 102, 107, 112, 81, 85, 94 },                    // base cell 101
            new int[] { 102, 101, 87, 85, 114, 112, 100 },                   // base cell 102
            new int[] { 103, 91, 97, 89, 116, 105, 111 },                    // base cell 103
            new int[] { 104, 107, 110, 115, 86, 94, 96 },                    // base cell 104
            new int[] { 105, 88, 103, 91, 113, 99, 116 },                    // base cell 105
            new int[] { 106, 93, 99, 90, 117, 109, 113 },                    // base cell 106
            new int[] { 107, INVALID_BASE_CELL, 101, 94, 115, 104, 112 },    // base cell 107 (pentagon)
            new int[] { 108, 100, 95, 92, 118, 114, 109 },                   // base cell 108
            new int[] { 109, 108, 93, 95, 117, 118, 106 },                   // base cell 109
            new int[] { 110, 98, 104, 96, 119, 111, 115 },                   // base cell 110
            new int[] { 111, 97, 110, 98, 116, 103, 119 },                   // base cell 111
            new int[] { 112, 107, 102, 101, 120, 115, 114 },                 // base cell 112
            new int[] { 113, 99, 116, 105, 117, 106, 121 },                  // base cell 113
            new int[] { 114, 112, 100, 102, 118, 120, 108 },                 // base cell 114
            new int[] { 115, 110, 107, 104, 120, 119, 112 },                 // base cell 115
            new int[] { 116, 103, 119, 111, 113, 105, 121 },                 // base cell 116
            new int[] { 117, INVALID_BASE_CELL, 109, 118, 113, 121, 106 },   // base cell 117 (pentagon)
            new int[] { 118, 120, 108, 114, 117, 121, 109 },                 // base cell 118
            new int[] { 119, 111, 115, 110, 121, 116, 120 },                 // base cell 119
            new int[] { 120, 115, 114, 112, 121, 119, 118 },                 // base cell 120
            new int[] { 121, 116, 120, 119, 117, 113, 118 },                 // base cell 121
        };

        /* @brief Neighboring base cell rotations in each IJK direction.
         *
         * For each base cell, for each direction, the number of 60 degree
         * CCW rotations to the coordinate system of the neighbor is given.
         * -1 indicates there is no neighbor in that direction.
         */
        public static int[][] baseCellNeighbor60CCWRots = new int[NUM_BASE_CELLS][]
        {
            new int[] { 0, 5, 0, 0, 1, 5, 1 },   // base cell 0
            new int[] { 0, 0, 1, 0, 1, 0, 1 },   // base cell 1
            new int[] { 0, 0, 0, 0, 0, 5, 0 },   // base cell 2
            new int[] { 0, 5, 0, 0, 2, 5, 1 },   // base cell 3
            new int[] { 0, -1, 1, 0, 3, 4, 2 },  // base cell 4 (pentagon)
            new int[] { 0, 0, 1, 0, 1, 0, 1 },   // base cell 5
            new int[] { 0, 0, 0, 3, 5, 5, 0 },   // base cell 6
            new int[] { 0, 0, 0, 0, 0, 5, 0 },   // base cell 7
            new int[] { 0, 5, 0, 0, 0, 5, 1 },   // base cell 8
            new int[] { 0, 0, 1, 3, 0, 0, 1 },   // base cell 9
            new int[] { 0, 0, 1, 3, 0, 0, 1 },   // base cell 10
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 11
            new int[] { 0, 5, 0, 0, 3, 5, 1 },   // base cell 12
            new int[] { 0, 0, 1, 0, 1, 0, 1 },   // base cell 13
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 14 (pentagon)
            new int[] { 0, 5, 0, 0, 4, 5, 1 },   // base cell 15
            new int[] { 0, 0, 0, 0, 0, 5, 0 },   // base cell 16
            new int[] { 0, 3, 3, 3, 3, 0, 3 },   // base cell 17
            new int[] { 0, 0, 0, 3, 5, 5, 0 },   // base cell 18
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 19
            new int[] { 0, 3, 3, 3, 0, 3, 0 },   // base cell 20
            new int[] { 0, 0, 0, 3, 5, 5, 0 },   // base cell 21
            new int[] { 0, 0, 1, 0, 1, 0, 1 },   // base cell 22
            new int[] { 0, 3, 3, 3, 0, 3, 0 },   // base cell 23
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 24 (pentagon)
            new int[] { 0, 0, 0, 3, 0, 0, 3 },   // base cell 25
            new int[] { 0, 0, 0, 0, 0, 5, 0 },   // base cell 26
            new int[] { 0, 3, 0, 0, 0, 3, 3 },   // base cell 27
            new int[] { 0, 0, 1, 0, 1, 0, 1 },   // base cell 28
            new int[] { 0, 0, 1, 3, 0, 0, 1 },   // base cell 29
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 30
            new int[] { 0, 0, 0, 0, 0, 5, 0 },   // base cell 31
            new int[] { 0, 3, 3, 3, 3, 0, 3 },   // base cell 32
            new int[] { 0, 0, 1, 3, 0, 0, 1 },   // base cell 33
            new int[] { 0, 3, 3, 3, 3, 0, 3 },   // base cell 34
            new int[] { 0, 0, 3, 0, 3, 0, 3 },   // base cell 35
            new int[] { 0, 0, 0, 3, 0, 0, 3 },   // base cell 36
            new int[] { 0, 3, 0, 0, 0, 3, 3 },   // base cell 37
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 38 (pentagon)
            new int[] { 0, 3, 0, 0, 3, 3, 0 },   // base cell 39
            new int[] { 0, 3, 0, 0, 3, 3, 0 },   // base cell 40
            new int[] { 0, 0, 0, 3, 5, 5, 0 },   // base cell 41
            new int[] { 0, 0, 0, 3, 5, 5, 0 },   // base cell 42
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 43
            new int[] { 0, 0, 1, 3, 0, 0, 1 },   // base cell 44
            new int[] { 0, 0, 3, 0, 0, 3, 3 },   // base cell 45
            new int[] { 0, 0, 0, 3, 0, 3, 0 },   // base cell 46
            new int[] { 0, 3, 3, 3, 0, 3, 0 },   // base cell 47
            new int[] { 0, 3, 3, 3, 0, 3, 0 },   // base cell 48
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 49 (pentagon)
            new int[] { 0, 0, 0, 3, 0, 0, 3 },   // base cell 50
            new int[] { 0, 3, 0, 0, 0, 3, 3 },   // base cell 51
            new int[] { 0, 0, 3, 0, 3, 0, 3 },   // base cell 52
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 53
            new int[] { 0, 0, 3, 0, 3, 0, 3 },   // base cell 54
            new int[] { 0, 0, 3, 0, 0, 3, 3 },   // base cell 55
            new int[] { 0, 3, 3, 3, 0, 0, 3 },   // base cell 56
            new int[] { 0, 0, 0, 3, 0, 3, 0 },   // base cell 57
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 58 (pentagon)
            new int[] { 0, 3, 3, 3, 3, 3, 0 },   // base cell 59
            new int[] { 0, 3, 3, 3, 3, 3, 0 },   // base cell 60
            new int[] { 0, 3, 3, 3, 3, 0, 3 },   // base cell 61
            new int[] { 0, 3, 3, 3, 3, 0, 3 },   // base cell 62
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 63 (pentagon)
            new int[] { 0, 0, 0, 3, 0, 0, 3 },   // base cell 64
            new int[] { 0, 3, 3, 3, 0, 3, 0 },   // base cell 65
            new int[] { 0, 3, 0, 0, 0, 3, 3 },   // base cell 66
            new int[] { 0, 3, 0, 0, 3, 3, 0 },   // base cell 67
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 68
            new int[] { 0, 3, 0, 0, 3, 3, 0 },   // base cell 69
            new int[] { 0, 0, 3, 0, 0, 3, 3 },   // base cell 70
            new int[] { 0, 0, 0, 3, 0, 3, 0 },   // base cell 71
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 72 (pentagon)
            new int[] { 0, 3, 3, 3, 0, 0, 3 },   // base cell 73
            new int[] { 0, 3, 3, 3, 0, 0, 3 },   // base cell 74
            new int[] { 0, 0, 0, 3, 0, 0, 3 },   // base cell 75
            new int[] { 0, 3, 0, 0, 0, 3, 3 },   // base cell 76
            new int[] { 0, 0, 0, 3, 0, 5, 0 },   // base cell 77
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 78
            new int[] { 0, 0, 1, 3, 1, 0, 1 },   // base cell 79
            new int[] { 0, 0, 1, 3, 1, 0, 1 },   // base cell 80
            new int[] { 0, 0, 3, 0, 3, 0, 3 },   // base cell 81
            new int[] { 0, 0, 3, 0, 3, 0, 3 },   // base cell 82
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 83 (pentagon)
            new int[] { 0, 0, 3, 0, 0, 3, 3 },   // base cell 84
            new int[] { 0, 0, 0, 3, 0, 3, 0 },   // base cell 85
            new int[] { 0, 3, 0, 0, 3, 3, 0 },   // base cell 86
            new int[] { 0, 3, 3, 3, 3, 3, 0 },   // base cell 87
            new int[] { 0, 0, 0, 3, 0, 5, 0 },   // base cell 88
            new int[] { 0, 3, 3, 3, 3, 3, 0 },   // base cell 89
            new int[] { 0, 0, 0, 0, 0, 0, 1 },   // base cell 90
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 91
            new int[] { 0, 0, 0, 3, 0, 5, 0 },   // base cell 92
            new int[] { 0, 5, 0, 0, 5, 5, 0 },   // base cell 93
            new int[] { 0, 0, 3, 0, 0, 3, 3 },   // base cell 94
            new int[] { 0, 0, 0, 0, 0, 0, 1 },   // base cell 95
            new int[] { 0, 0, 0, 3, 0, 3, 0 },   // base cell 96
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 97 (pentagon)
            new int[] { 0, 3, 3, 3, 0, 0, 3 },   // base cell 98
            new int[] { 0, 5, 0, 0, 5, 5, 0 },   // base cell 99
            new int[] { 0, 0, 1, 3, 1, 0, 1 },   // base cell 100
            new int[] { 0, 3, 3, 3, 0, 0, 3 },   // base cell 101
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 102
            new int[] { 0, 0, 1, 3, 1, 0, 1 },   // base cell 103
            new int[] { 0, 3, 3, 3, 3, 3, 0 },   // base cell 104
            new int[] { 0, 0, 0, 0, 0, 0, 1 },   // base cell 105
            new int[] { 0, 0, 1, 0, 3, 5, 1 },   // base cell 106
            new int[] { 0, -1, 3, 0, 5, 2, 0 },  // base cell 107 (pentagon)
            new int[] { 0, 5, 0, 0, 5, 5, 0 },   // base cell 108
            new int[] { 0, 0, 1, 0, 4, 5, 1 },   // base cell 109
            new int[] { 0, 3, 3, 3, 0, 0, 0 },   // base cell 110
            new int[] { 0, 0, 0, 3, 0, 5, 0 },   // base cell 111
            new int[] { 0, 0, 0, 3, 0, 5, 0 },   // base cell 112
            new int[] { 0, 0, 1, 0, 2, 5, 1 },   // base cell 113
            new int[] { 0, 0, 0, 0, 0, 0, 1 },   // base cell 114
            new int[] { 0, 0, 1, 3, 1, 0, 1 },   // base cell 115
            new int[] { 0, 5, 0, 0, 5, 5, 0 },   // base cell 116
            new int[] { 0, -1, 1, 0, 3, 4, 2 },  // base cell 117 (pentagon)
            new int[] { 0, 0, 1, 0, 0, 5, 1 },   // base cell 118
            new int[] { 0, 0, 0, 0, 0, 0, 1 },   // base cell 119
            new int[] { 0, 5, 0, 0, 5, 5, 0 },   // base cell 120
            new int[] { 0, 0, 1, 0, 1, 5, 1 },   // base cell 121
        };

        /* @brief Resolution 0 base cell lookup table for each face.
         *
         * Given the face number and a resolution 0 ijk+ coordinate in that face's
         * face-centered ijk coordinate system, gives the base cell located at that
         * coordinate and the number of 60 ccw rotations to rotate into that base
         * cell's orientation.
         *
         * Valid lookup coordinates are from (0, 0, 0) to (2, 2, 2).
         *
         * This table can be accessed using the functions `ToBaseCells` and
         * `_faceIjkToBaseCellCCWrot60`
         */
        public static BaseCellOrient[][][][] faceIjkBaseCells = new BaseCellOrient[NUM_ICOSA_FACES][][][] // [NUM_ICOSA_FACES][3][3][3]
        {
            // face 0
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (16, 0), (18, 0), (24, 0) }, // j 0
                    new BaseCellOrient[] { (33, 0), (30, 0), (32, 3) }, // j 1
                    new BaseCellOrient[] { (49, 1), (48, 3), (50, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (8, 0), (5, 5), (10, 5) },   // j 0
                    new BaseCellOrient[] { (22, 0), (16, 0), (18, 0) }, // j 1
                    new BaseCellOrient[] { (41, 1), (33, 0), (30, 0) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (4, 0), (0, 5), (2, 5) },    // j 0
                    new BaseCellOrient[] { (15, 1), (8, 0), (5, 5) },   // j 1
                    new BaseCellOrient[] { (31, 1), (22, 0), (16, 0) }  // j 2
                }
            },
            // face 1
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (2, 0), (6, 0), (14, 0) },   // j 0
                    new BaseCellOrient[] { (10, 0), (11, 0), (17, 3) }, // j 1
                    new BaseCellOrient[] { (24, 1), (23, 3), (25, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (0, 0), (1, 5), (9, 5) },    // j 0
                    new BaseCellOrient[] { (5, 0), (2, 0), (6, 0) },    // j 1
                    new BaseCellOrient[] { (18, 1), (10, 0), (11, 0) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (4, 1), (3, 5), (7, 5) },    // j 0
                    new BaseCellOrient[] { (8, 1), (0, 0), (1, 5) },    // j 1
                    new BaseCellOrient[] { (16, 1), (5, 0), (2, 0) }    // j 2
                }
            },
            // face 2
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (7, 0), (21, 0), (38, 0) },  // j 0
                    new BaseCellOrient[] { (9, 0), (19, 0), (34, 3) },  // j 1
                    new BaseCellOrient[] { (14, 1), (20, 3), (36, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (3, 0), (13, 5), (29, 5) },  // j 0
                    new BaseCellOrient[] { (1, 0), (7, 0), (21, 0) },   // j 1
                    new BaseCellOrient[] { (6, 1), (9, 0), (19, 0) }    // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (4, 2), (12, 5), (26, 5) },  // j 0
                    new BaseCellOrient[] { (0, 1), (3, 0), (13, 5) },   // j 1
                    new BaseCellOrient[] { (2, 1), (1, 0), (7, 0) }     // j 2
                }
            },
            // face 3
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (26, 0), (42, 0), (58, 0) }, // j 0
                    new BaseCellOrient[] { (29, 0), (43, 0), (62, 3) }, // j 1
                    new BaseCellOrient[] { (38, 1), (47, 3), (64, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (12, 0), (28, 5), (44, 5) }, // j 0
                    new BaseCellOrient[] { (13, 0), (26, 0), (42, 0) }, // j 1
                    new BaseCellOrient[] { (21, 1), (29, 0), (43, 0) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (4, 3), (15, 5), (31, 5) },  // j 0
                    new BaseCellOrient[] { (3, 1), (12, 0), (28, 5) },  // j 1
                    new BaseCellOrient[] { (7, 1), (13, 0), (26, 0) }   // j 2
                }
            },
            // face 4
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (31, 0), (41, 0), (49, 0) }, // j 0
                    new BaseCellOrient[] { (44, 0), (53, 0), (61, 3) }, // j 1
                    new BaseCellOrient[] { (58, 1), (65, 3), (75, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (15, 0), (22, 5), (33, 5) }, // j 0
                    new BaseCellOrient[] { (28, 0), (31, 0), (41, 0) }, // j 1
                    new BaseCellOrient[] { (42, 1), (44, 0), (53, 0) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (4, 4), (8, 5), (16, 5) },   // j 0
                    new BaseCellOrient[] { (12, 1), (15, 0), (22, 5) }, // j 1
                    new BaseCellOrient[] { (26, 1), (28, 0), (31, 0) }  // j 2
                }
            },
            // face 5
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (50, 0), (48, 0), (49, 3) }, // j 0
                    new BaseCellOrient[] { (32, 0), (30, 3), (33, 3) }, // j 1
                    new BaseCellOrient[] { (24, 3), (18, 3), (16, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (70, 0), (67, 0), (66, 3) }, // j 0
                    new BaseCellOrient[] { (52, 3), (50, 0), (48, 0) }, // j 1
                    new BaseCellOrient[] { (37, 3), (32, 0), (30, 3) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (83, 0), (87, 3), (85, 3) }, // j 0
                    new BaseCellOrient[] { (74, 3), (70, 0), (67, 0) }, // j 1
                    new BaseCellOrient[] { (57, 1), (52, 3), (50, 0) }  // j 2
                }
            },
            // face 6
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (25, 0), (23, 0), (24, 3) }, // j 0
                    new BaseCellOrient[] { (17, 0), (11, 3), (10, 3) }, // j 1
                    new BaseCellOrient[] { (14, 3), (6, 3), (2, 3) }    // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (45, 0), (39, 0), (37, 3) }, // j 0
                    new BaseCellOrient[] { (35, 3), (25, 0), (23, 0) }, // j 1
                    new BaseCellOrient[] { (27, 3), (17, 0), (11, 3) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (63, 0), (59, 3), (57, 3) }, // j 0
                    new BaseCellOrient[] { (56, 3), (45, 0), (39, 0) }, // j 1
                    new BaseCellOrient[] { (46, 1), (35, 3), (25, 0) }  // j 2
                }
            },
            // face 7
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (36, 0), (20, 0), (14, 3) }, // j 0
                    new BaseCellOrient[] { (34, 0), (19, 3), (9, 3) },  // j 1
                    new BaseCellOrient[] { (38, 3), (21, 3), (7, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (55, 0), (40, 0), (27, 3) }, // j 0
                    new BaseCellOrient[] { (54, 3), (36, 0), (20, 0) }, // j 1
                    new BaseCellOrient[] { (51, 3), (34, 0), (19, 3) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (72, 0), (60, 3), (46, 3) }, // j 0
                    new BaseCellOrient[] { (73, 3), (55, 0), (40, 0) }, // j 1
                    new BaseCellOrient[] { (71, 3), (54, 3), (36, 0) }  // j 2
                }
            },
            // face 8
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (64, 0), (47, 0), (38, 3) }, // j 0
                    new BaseCellOrient[] { (62, 0), (43, 3), (29, 3) }, // j 1
                    new BaseCellOrient[] { (58, 3), (42, 3), (26, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (84, 0), (69, 0), (51, 3) }, // j 0
                    new BaseCellOrient[] { (82, 3), (64, 0), (47, 0) }, // j 1
                    new BaseCellOrient[] { (76, 3), (62, 0), (43, 3) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (97, 0), (89, 3), (71, 3) }, // j 0
                    new BaseCellOrient[] { (98, 3), (84, 0), (69, 0) }, // j 1
                    new BaseCellOrient[] { (96, 3), (82, 3), (64, 0) }  // j 2
                }
            },
            // face 9
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (75, 0), (65, 0), (58, 3) }, // j 0
                    new BaseCellOrient[] { (61, 0), (53, 3), (44, 3) }, // j 1
                    new BaseCellOrient[] { (49, 3), (41, 3), (31, 3) }  // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (94, 0), (86, 0), (76, 3) }, // j 0
                    new BaseCellOrient[] { (81, 3), (75, 0), (65, 0) }, // j 1
                    new BaseCellOrient[] { (66, 3), (61, 0), (53, 3) }  // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (107, 0), (104, 3), (96, 3) }, // j 0
                    new BaseCellOrient[] { (101, 3), (94, 0), (86, 0) },  // j 1
                    new BaseCellOrient[] { (85, 3), (81, 3), (75, 0) }    // j 2
                }
            },
            // face 10
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (57, 0), (59, 0), (63, 3) },  // j 0
                    new BaseCellOrient[] { (74, 0), (78, 3), (79, 3) },  // j 1
                    new BaseCellOrient[] { (83, 3), (92, 3), (95, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (37, 0), (39, 0), (45, 3) },  // j 0
                    new BaseCellOrient[] { (52, 0), (57, 0), (59, 0) },  // j 1
                    new BaseCellOrient[] { (70, 3), (74, 0), (78, 3) }   // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (24, 0), (23, 3), (25, 3) },  // j 0
                    new BaseCellOrient[] { (32, 3), (37, 0), (39, 3) },  // j 1
                    new BaseCellOrient[] { (50, 3), (52, 0), (57, 0) }   // j 2
                }
            },
            // face 11
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (46, 0), (60, 0), (72, 3) },  // j 0
                    new BaseCellOrient[] { (56, 0), (68, 3), (80, 3) },  // j 1
                    new BaseCellOrient[] { (63, 3), (77, 3), (90, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (27, 0), (40, 3), (55, 3) },  // j 0
                    new BaseCellOrient[] { (35, 0), (46, 0), (60, 0) },  // j 1
                    new BaseCellOrient[] { (45, 3), (56, 0), (68, 3) }   // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (14, 0), (20, 3), (36, 3) },  // j 0
                    new BaseCellOrient[] { (17, 3), (27, 0), (40, 3) },  // j 1
                    new BaseCellOrient[] { (25, 3), (35, 0), (46, 0) }   // j 2
                }
            },
            // face 12
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (71, 0), (89, 0), (97, 3) },   // j 0
                    new BaseCellOrient[] { (73, 0), (91, 3), (103, 3) },  // j 1
                    new BaseCellOrient[] { (72, 3), (88, 3), (105, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (51, 0), (69, 3), (84, 3) },  // j 0
                    new BaseCellOrient[] { (54, 0), (71, 0), (89, 0) },  // j 1
                    new BaseCellOrient[] { (55, 3), (73, 0), (91, 3) }   // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (38, 0), (47, 3), (64, 3) },  // j 0
                    new BaseCellOrient[] { (34, 3), (51, 0), (69, 3) },  // j 1
                    new BaseCellOrient[] { (36, 3), (54, 0), (71, 0) }   // j 2
                }
            },
            // face 13
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (96, 0), (104, 0), (107, 3) },  // j 0
                    new BaseCellOrient[] { (98, 0), (110, 3), (115, 3) },  // j 1
                    new BaseCellOrient[] { (97, 3), (111, 3), (119, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (76, 0), (86, 3), (94, 3) },   // j 0
                    new BaseCellOrient[] { (82, 0), (96, 0), (104, 0) },  // j 1
                    new BaseCellOrient[] { (84, 3), (98, 0), (110, 3) }   // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (58, 0), (65, 3), (75, 3) },  // j 0
                    new BaseCellOrient[] { (62, 3), (76, 0), (86, 3) },  // j 1
                    new BaseCellOrient[] { (64, 3), (82, 0), (96, 0) }   // j 2
                }
            },
            // face 14
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (85, 0), (87, 0), (83, 3) },     // j 0
                    new BaseCellOrient[] { (101, 0), (102, 3), (100, 3) },  // j 1
                    new BaseCellOrient[] { (107, 3), (112, 3), (114, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (66, 0), (67, 3), (70, 3) },     // j 0
                    new BaseCellOrient[] { (81, 0), (85, 0), (87, 0) },     // j 1
                    new BaseCellOrient[] { (94, 3), (101, 0), (102, 3) }    // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (49, 0), (48, 3), (50, 3) },  // j 0
                    new BaseCellOrient[] { (61, 3), (66, 0), (67, 3) },  // j 1
                    new BaseCellOrient[] { (75, 3), (81, 0), (85, 0) }   // j 2
                }
            },
            // face 15
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (95, 0), (92, 0), (83, 0) },  // j 0
                    new BaseCellOrient[] { (79, 0), (78, 0), (74, 3) },  // j 1
                    new BaseCellOrient[] { (63, 1), (59, 3), (57, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (109, 0), (108, 0), (100, 5) }, // j 0
                    new BaseCellOrient[] { (93, 1), (95, 0), (92, 0) },    // j 1
                    new BaseCellOrient[] { (77, 1), (79, 0), (78, 0) }     // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (117, 4), (118, 5), (114, 5) },  // j 0
                    new BaseCellOrient[] { (106, 1), (109, 0), (108, 0) },  // j 1
                    new BaseCellOrient[] { (90, 1), (93, 1), (95, 0) }      // j 2
                }
            },
            // face 16
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (90, 0), (77, 0), (63, 0) },  // j 0
                    new BaseCellOrient[] { (80, 0), (68, 0), (56, 3) },  // j 1
                    new BaseCellOrient[] { (72, 1), (60, 3), (46, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (106, 0), (93, 0), (79, 5) }, // j 0
                    new BaseCellOrient[] { (99, 1), (90, 0), (77, 0) },  // j 1
                    new BaseCellOrient[] { (88, 1), (80, 0), (68, 0) }   // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (117, 3), (109, 5), (95, 5) },  // j 0
                    new BaseCellOrient[] { (113, 1), (106, 0), (93, 0) },  // j 1
                    new BaseCellOrient[] { (105, 1), (99, 1), (90, 0) }    // j 2
                }
            },
            // face 17
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (105, 0), (88, 0), (72, 0) },  // j 0
                    new BaseCellOrient[] { (103, 0), (91, 0), (73, 3) },  // j 1
                    new BaseCellOrient[] { (97, 1), (89, 3), (71, 3) }    // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (113, 0), (99, 0), (80, 5) },   // j 0
                    new BaseCellOrient[] { (116, 1), (105, 0), (88, 0) },  // j 1
                    new BaseCellOrient[] { (111, 1), (103, 0), (91, 0) }   // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (117, 2), (106, 5), (90, 5) },  // j 0
                    new BaseCellOrient[] { (121, 1), (113, 0), (99, 0) },  // j 1
                    new BaseCellOrient[] { (119, 1), (116, 1), (105, 0) }  // j 2
                }
            },
            // face 18
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (119, 0), (111, 0), (97, 0) },  // j 0
                    new BaseCellOrient[] { (115, 0), (110, 0), (98, 3) },  // j 1
                    new BaseCellOrient[] { (107, 1), (104, 3), (96, 3) }   // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (121, 0), (116, 0), (103, 5) },  // j 0
                    new BaseCellOrient[] { (120, 1), (119, 0), (111, 0) },  // j 1
                    new BaseCellOrient[] { (112, 1), (115, 0), (110, 0) }   // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (117, 1), (113, 5), (105, 5) },  // j 0
                    new BaseCellOrient[] { (118, 1), (121, 0), (116, 0) },  // j 1
                    new BaseCellOrient[] { (114, 1), (120, 1), (119, 0) }   // j 2
                }
            },
            // face 19
            new BaseCellOrient[][][]
            {
                // i 0
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (114, 0), (112, 0), (107, 0) },  // j 0
                    new BaseCellOrient[] { (100, 0), (102, 0), (101, 3) },  // j 1
                    new BaseCellOrient[] { (83, 1), (87, 3), (85, 3) }      // j 2
                },
                // i 1
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (118, 0), (120, 0), (115, 5) },  // j 0
                    new BaseCellOrient[] { (108, 1), (114, 0), (112, 0) },  // j 1
                    new BaseCellOrient[] { (95, 1), (100, 0), (102, 0) }    // j 2
                },
                // i 2
                new BaseCellOrient[][]
                {
                    new BaseCellOrient[] { (117, 0), (121, 5), (119, 5) },  // j 0
                    new BaseCellOrient[] { (109, 1), (118, 0), (120, 0) },  // j 1
                    new BaseCellOrient[] { (95, 1), (108, 1), (114, 0) }    // j 2
                }
            }
        };

        // (int, (double, double, double), bool, (int, int))
        // (face, (coord), isPentagon, (cwOffsetPent))
        public static BaseCellData[] baseCellData = new BaseCellData[NUM_BASE_CELLS]
        {
            (1, (1, 0, 0), false, (0, 0)),      // base cell 0
            (2, (1, 1, 0), false, (0, 0)),      // base cell 1
            (1, (0, 0, 0), false, (0, 0)),      // base cell 2
            (2, (1, 0, 0), false, (0, 0)),      // base cell 3
            (0, (2, 0, 0), true, (-1, -1)),     // base cell 4
            (1, (1, 1, 0), false, (0, 0)),      // base cell 5
            (1, (0, 0, 1), false, (0, 0)),      // base cell 6
            (2, (0, 0, 0), false, (0, 0)),      // base cell 7
            (0, (1, 0, 0), false, (0, 0)),      // base cell 8
            (2, (0, 1, 0), false, (0, 0)),      // base cell 9
            (1, (0, 1, 0), false, (0, 0)),      // base cell 10
            (1, (0, 1, 1), false, (0, 0)),      // base cell 11
            (3, (1, 0, 0), false, (0, 0)),      // base cell 12
            (3, (1, 1, 0), false, (0, 0)),      // base cell 13
            (11, (2, 0, 0), true, (2, 6)),      // base cell 14
            (4, (1, 0, 0), false, (0, 0)),      // base cell 15
            (0, (0, 0, 0), false, (0, 0)),      // base cell 16
            (6, (0, 1, 0), false, (0, 0)),      // base cell 17
            (0, (0, 0, 1), false, (0, 0)),      // base cell 18
            (2, (0, 1, 1), false, (0, 0)),      // base cell 19
            (7, (0, 0, 1), false, (0, 0)),      // base cell 20
            (2, (0, 0, 1), false, (0, 0)),      // base cell 21
            (0, (1, 1, 0), false, (0, 0)),      // base cell 22
            (6, (0, 0, 1), false, (0, 0)),      // base cell 23
            (10, (2, 0, 0), true, (1, 5)),      // base cell 24
            (6, (0, 0, 0), false, (0, 0)),      // base cell 25
            (3, (0, 0, 0), false, (0, 0)),      // base cell 26
            (11, (1, 0, 0), false, (0, 0)),     // base cell 27
            (4, (1, 1, 0), false, (0, 0)),      // base cell 28
            (3, (0, 1, 0), false, (0, 0)),      // base cell 29
            (0, (0, 1, 1), false, (0, 0)),      // base cell 30
            (4, (0, 0, 0), false, (0, 0)),      // base cell 31
            (5, (0, 1, 0), false, (0, 0)),      // base cell 32
            (0, (0, 1, 0), false, (0, 0)),      // base cell 33
            (7, (0, 1, 0), false, (0, 0)),      // base cell 34
            (11, (1, 1, 0), false, (0, 0)),     // base cell 35
            (7, (0, 0, 0), false, (0, 0)),      // base cell 36
            (10, (1, 0, 0), false, (0, 0)),     // base cell 37
            (12, (2, 0, 0), true, (3, 7)),      // base cell 38
            (6, (1, 0, 1), false, (0, 0)),      // base cell 39
            (7, (1, 0, 1), false, (0, 0)),      // base cell 40
            (4, (0, 0, 1), false, (0, 0)),      // base cell 41
            (3, (0, 0, 1), false, (0, 0)),      // base cell 42
            (3, (0, 1, 1), false, (0, 0)),      // base cell 43
            (4, (0, 1, 0), false, (0, 0)),      // base cell 44
            (6, (1, 0, 0), false, (0, 0)),      // base cell 45
            (11, (0, 0, 0), false, (0, 0)),     // base cell 46
            (8, (0, 0, 1), false, (0, 0)),      // base cell 47
            (5, (0, 0, 1), false, (0, 0)),      // base cell 48
            (14, (2, 0, 0), true, (0, 9)),      // base cell 49
            (5, (0, 0, 0), false, (0, 0)),      // base cell 50
            (12, (1, 0, 0), false, (0, 0)),     // base cell 51
            (10, (1, 1, 0), false, (0, 0)),     // base cell 52
            (4, (0, 1, 1), false, (0, 0)),      // base cell 53
            (12, (1, 1, 0), false, (0, 0)),     // base cell 54
            (7, (1, 0, 0), false, (0, 0)),      // base cell 55
            (11, (0, 1, 0), false, (0, 0)),     // base cell 56
            (10, (0, 0, 0), false, (0, 0)),     // base cell 57
            (13, (2, 0, 0), true, (4, 8)),      // base cell 58
            (10, (0, 0, 1), false, (0, 0)),     // base cell 59
            (11, (0, 0, 1), false, (0, 0)),     // base cell 60
            (9, (0, 1, 0), false, (0, 0)),      // base cell 61
            (8, (0, 1, 0), false, (0, 0)),      // base cell 62
            (6, (2, 0, 0), true, (11, 15)),     // base cell 63
            (8, (0, 0, 0), false, (0, 0)),      // base cell 64
            (9, (0, 0, 1), false, (0, 0)),      // base cell 65
            (14, (1, 0, 0), false, (0, 0)),     // base cell 66
            (5, (1, 0, 1), false, (0, 0)),      // base cell 67
            (16, (0, 1, 1), false, (0, 0)),     // base cell 68
            (8, (1, 0, 1), false, (0, 0)),      // base cell 69
            (5, (1, 0, 0), false, (0, 0)),      // base cell 70
            (12, (0, 0, 0), false, (0, 0)),     // base cell 71
            (7, (2, 0, 0), true, (12, 16)),     // base cell 72
            (12, (0, 1, 0), false, (0, 0)),     // base cell 73
            (10, (0, 1, 0), false, (0, 0)),     // base cell 74
            (9, (0, 0, 0), false, (0, 0)),      // base cell 75
            (13, (1, 0, 0), false, (0, 0)),     // base cell 76
            (16, (0, 0, 1), false, (0, 0)),     // base cell 77
            (15, (0, 1, 1), false, (0, 0)),     // base cell 78
            (15, (0, 1, 0), false, (0, 0)),     // base cell 79
            (16, (0, 1, 0), false, (0, 0)),     // base cell 80
            (14, (1, 1, 0), false, (0, 0)),     // base cell 81
            (13, (1, 1, 0), false, (0, 0)),     // base cell 82
            (5, (2, 0, 0), true, (10, 19)),     // base cell 83
            (8, (1, 0, 0), false, (0, 0)),      // base cell 84
            (14, (0, 0, 0), false, (0, 0)),     // base cell 85
            (9, (1, 0, 1), false, (0, 0)),      // base cell 86
            (14, (0, 0, 1), false, (0, 0)),     // base cell 87
            (17, (0, 0, 1), false, (0, 0)),     // base cell 88
            (12, (0, 0, 1), false, (0, 0)),     // base cell 89
            (16, (0, 0, 0), false, (0, 0)),     // base cell 90
            (17, (0, 1, 1), false, (0, 0)),     // base cell 91
            (15, (0, 0, 1), false, (0, 0)),     // base cell 92
            (16, (1, 0, 1), false, (0, 0)),     // base cell 93
            (9, (1, 0, 0), false, (0, 0)),      // base cell 94
            (15, (0, 0, 0), false, (0, 0)),     // base cell 95
            (13, (0, 0, 0), false, (0, 0)),     // base cell 96
            (8, (2, 0, 0), true, (13, 17)),     // base cell 97
            (13, (0, 1, 0), false, (0, 0)),     // base cell 98
            (17, (1, 0, 1), false, (0, 0)),     // base cell 99
            (19, (0, 1, 0), false, (0, 0)),     // base cell 100
            (14, (0, 1, 0), false, (0, 0)),     // base cell 101
            (19, (0, 1, 1), false, (0, 0)),     // base cell 102
            (17, (0, 1, 0), false, (0, 0)),     // base cell 103
            (13, (0, 0, 1), false, (0, 0)),     // base cell 104
            (17, (0, 0, 0), false, (0, 0)),     // base cell 105
            (16, (1, 0, 0), false, (0, 0)),     // base cell 106
            (9, (2, 0, 0), false, (0, 0)),      // base cell 107
            (15, (1, 0, 1), false, (0, 0)),     // base cell 108
            (15, (1, 0, 0), false, (0, 0)),     // base cell 109
            (18, (0, 1, 1), false, (0, 0)),     // base cell 110
            (18, (0, 0, 1), false, (0, 0)),     // base cell 111
            (19, (0, 0, 1), false, (0, 0)),     // base cell 112
            (17, (1, 0, 0), false, (0, 0)),     // base cell 113
            (19, (0, 0, 0), false, (0, 0)),     // base cell 114
            (18, (0, 1, 0), false, (0, 0)),     // base cell 115
            (18, (1, 0, 1), false, (0, 0)),     // base cell 116
            (19, (2, 0, 0), true, (-1, -1)),    // base cell 117
            (19, (1, 0, 0), false, (0, 0)),     // base cell 118
            (18, (0, 0, 0), false, (0, 0)),     // base cell 119
            (19, (1, 0, 1), false, (0, 0)),     // base cell 120
            (18, (1, 0, 0), false, (0, 0)),     // base cell 121
        };

        /* @brief Return whether or not the indicated base cell is a pentagon. */
        public static bool _isBaseCellPentagon(int baseCell) => baseCellData[baseCell].isPentagon;

        /* @brief Return whether the indicated base cell is a pentagon where all
         * neighbors are oriented towards it. */
        public static bool _isBaseCellPolarPentagon(int baseCell) => baseCell == 4 || baseCell == 117;

        

        /* @brief Find base cell given FaceIJK.
         *
         * Given the face number and a resolution 0 ijk+ coordinate in that face's
         * face-centered ijk coordinate system, return the number of 60' ccw rotations
         * to rotate into the coordinate system of the base cell at that coordinates.
         *
         * Valid ijk+ lookup coordinates are from (0, 0, 0) to (2, 2, 2).
         */
        public static int _faceIjkToBaseCellCCWrot60(FaceIJK h) => faceIjkBaseCells[h.face][h.coord.i][h.coord.j][h.coord.k].ccwRot60;

        /* @brief Find the FaceIJK given a base cell. */
        public static void _baseCellToFaceIjk(int baseCell, ref FaceIJK h)
        {
            h = baseCellData[baseCell].homeFijk;
        }

        /* @brief Return whether or not the tested face is a cw offset face. */
        public static bool _baseCellIsCwOffset(int baseCell, int testFace) => 
            baseCellData[baseCell].cwOffsetPent[0] == testFace || baseCellData[baseCell].cwOffsetPent[1] == testFace;

        /* @brief Return the neighboring base cell in the given direction. */
        public static int _getBaseCellNeighbor(int baseCell, Direction dir) => baseCellNeighbors[baseCell][(int)dir];

        /* @brief Return the direction from the origin base cell to the neighbor.
         * Returns INVALID_DIGIT if the base cells are not neighbors.
         */
        public static Direction _getBaseCellDirection(int originBaseCell, int neighboringBaseCell)
        {
            for (int dir = (int)Direction.CENTER_DIGIT; dir < (int)Direction.NUM_DIGITS; dir++)
            {
                int testBaseCell = _getBaseCellNeighbor(originBaseCell, (Direction)dir);
                if (testBaseCell == neighboringBaseCell)
                    return (Direction)dir;
            }
            return Direction.INVALID_DIGIT;
        }
    }
}
