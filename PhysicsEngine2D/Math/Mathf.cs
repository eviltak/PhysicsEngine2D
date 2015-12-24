using System;
using System.Collections.Specialized;

namespace PhysicsEngine2D
{
    public static class Mathf
    {
        public static float Epsilon = 1e-10f;
        public const float Pi = 3.1415926f;

        public static bool Approximately(float a, float b)
        {
            return Math.Abs(a - b) < Epsilon;
        }

        public static bool BiasGreaterThan(float a, float b)
        {
            const float BiasRelative = 0.95f;
            const float BiasAbsolute = 0.01f;
            return a >= b * BiasRelative + a * BiasAbsolute;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static Vec2 Lerp(Vec2 a, Vec2 b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
