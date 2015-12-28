using System;

namespace PhysicsEngine2D
{
    public struct Ray2
    {
        public const float Tmax = float.MaxValue;

        public Vec2 origin;
        public Vec2 direction;

        public Ray2(Vec2 orig, Vec2 dir)
        {
            origin = orig;
            direction = dir.Normalized;
        }

        public bool IntersectSegment(Vec2 a, Vec2 b, out float t)
        {
            return IntersectSegment(a, b, Tmax, out t);
        }

        public bool IntersectSegment(Vec2 a, Vec2 b, float distance, out float t)
        {
            Vec2 v1 = origin - a;
            Vec2 v2 = b - a;
            Vec2 perpD = Vec2.Cross(1, direction);

            float denom = Vec2.Dot(v2, perpD);

            if (Math.Abs(denom) < Mathf.Epsilon)
            {
                t = Tmax;
                return false;
            }

            t = Vec2.Cross(v2, v1) / denom;
            float s = Vec2.Dot(v1, perpD) / denom;

            return t >= 0.0f && s >= 0.0f && s <= 1.0f;
        }
    }
}
