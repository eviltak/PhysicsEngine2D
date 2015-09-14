using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    static class MathUtil
    {
        //Crossing two vectors which results in a scalar which means (scalar * imaginary z-axis)
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        //Crossing vector with z-axis
        public static Vector2 Cross(Vector2 a, float s)
        {
            return new Vector2(s * a.Y, -s * a.X);
        }

        public static Vector2 Cross(float s, Vector2 a)
        {
            return new Vector2(-s * a.Y, s * a.X);
        }
    }
}
