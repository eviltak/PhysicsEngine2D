using System;
using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public struct Matrix2
    {
        public float m00, m01;
        public float m10, m11;

        public Matrix2(float m00, float m01, float m10, float m11)
        {
            this.m00 = m00; this.m01 = m01;
            this.m10 = m10; this.m11 = m11;
        }

        public Matrix2(float radians)
        {
            m00 = m10 = m11 = m01 = 0;
            SetRotation(radians);
        }

        //Creates a Rotation Matrix from the passed angle.
        public void SetRotation(float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            m00 = cos; m01 = -sin;
            m10 = sin; m11 = cos;
        }

        //Returns the Transpose of the matrix. Transpose of matrix is also world to object space matrix.
        public Matrix2 Transpose()
        {
            return new Matrix2(m00, m10,
                               m01, m11);
        }

        //Operators

        //Matrix * Vector
        public static Vector2 operator *(Matrix2 mat, Vector2 vec)
        {
            return new Vector2(mat.m00 * vec.X + mat.m01 * vec.Y, 
                               mat.m10 * vec.X + mat.m11 * vec.Y);
        }

        //Matrix * Matrix
        public static Matrix2 operator *(Matrix2 a, Matrix2 b)
        {
            return new Matrix2(a.m00 * b.m00 + a.m01 * b.m10, a.m00 * b.m01 + a.m01 * b.m11, 
                               a.m10 * b.m00 + a.m11 * b.m10, a.m10 * b.m01 + a.m11 * b.m11);
        }
    }
}
