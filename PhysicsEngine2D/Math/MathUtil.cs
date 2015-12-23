using System;

using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public static class MathUtil
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

        public static bool BiasGreaterThan(float a, float b)
        {
            const float BiasRelative = 0.95f;
            const float BiasAbsolute = 0.01f;
            return a >= b * BiasRelative + a * BiasAbsolute;
        }

        public static Vector2 Rotate(Vector2 v, float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }

        public static Color Random(int seed)
        {
            Random r = new Random(seed);
            Color color = new Color();
            color.R = (byte)r.Next(0, 255);
            color.G = (byte)r.Next(0, 255);
            color.B = (byte)r.Next(0, 255);

            return color;
        }
    }
}
