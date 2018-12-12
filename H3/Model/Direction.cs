using System;
using System.Collections.Generic;
using System.Text;

namespace H3.Model
{
    /* @brief H3 digit representing ijk+ axes direction.
     * Values will be within the lowest 3 bits of an integer.
     */
    public enum Direction
    {
        /** H3 digit in center */
        CENTER_DIGIT = 0,
        /** H3 digit in k-axes direction */
        K_AXES_DIGIT = 1,
        /** H3 digit in j-axes direction */
        J_AXES_DIGIT = 2,
        /** H3 digit in j == k direction */
        JK_AXES_DIGIT = J_AXES_DIGIT | K_AXES_DIGIT, /* 3 */
                                                     /** H3 digit in i-axes direction */
        I_AXES_DIGIT = 4,
        /** H3 digit in i == k direction */
        IK_AXES_DIGIT = I_AXES_DIGIT | K_AXES_DIGIT, /* 5 */
                                                     /** H3 digit in i == j direction */
        IJ_AXES_DIGIT = I_AXES_DIGIT | J_AXES_DIGIT, /* 6 */
                                                     /** H3 digit in the invalid direction */
        INVALID_DIGIT = 7,
        /** Valid digits will be less than this value. Same value as INVALID_DIGIT.
         */
        NUM_DIGITS = INVALID_DIGIT
    }
}
