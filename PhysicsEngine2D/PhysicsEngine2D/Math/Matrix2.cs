using System;
using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public struct Matrix2
    {
        public float M00, M01;
        public float M10, M11;

        public Matrix2(float m00, float m01, float m10, float m11)
        {
            M00 = m00; M01 = m01;
            M10 = m10; M11 = m11;
        }

        public Matrix2(float radians)
        {
            M00 = M10 = M11 = M01 = 0;
            SetRotation(radians);
        }

        //Creates a Rotation Matrix from the passed angle.
        public void SetRotation(float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            M00 = cos; M01 = -sin;
            M10 = sin; M11 = cos;
        }

        //Returns the Transpose of the matrix. Transpose of matrix is also world to object space matrix.
        public Matrix2 Transpose()
        {
            return new Matrix2(M00, M10,
                               M01, M11);
        }

        //Operators

        //Matrix * Vector
        public static Vector2 operator *(Matrix2 mat, Vector2 vec)
        {
            return new Vector2(mat.M00 * vec.X + mat.M01 * vec.Y, 
                               mat.M10 * vec.X + mat.M11 * vec.Y);
        }

        //Matrix * Matrix
        public static Matrix2 operator *(Matrix2 A, Matrix2 B)
        {
            return new Matrix2(A.M00 * B.M00 + A.M01 * B.M10, A.M00 * B.M01 + A.M01 * B.M11, 
                               A.M10 * B.M00 + A.M11 * B.M10, A.M10 * B.M01 + A.M11 * B.M11);
        }
    }
}
