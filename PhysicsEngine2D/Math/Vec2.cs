using System;

namespace PhysicsEngine2D
{
    public struct Vec2
    {
        public float x, y;

        public Vec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2(float value) : this(value, value) { }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }

        public float SqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        public Vec2 Normalized
        {
            get
            {
                float invMag = 1 / Magnitude;
                return this * invMag;
            }
        }

        public void Normalize()
        {
            float invMag = 1 / Magnitude;
            this.x *= invMag;
            y *= invMag;
        }



        public static Vec2 Zero
        {
            get { return new Vec2(0); }
        }

        public static Vec2 One
        {
            get { return new Vec2(1); }
        }

        public static Vec2 UnitX
        {
            get { return new Vec2(1, 0); }
        }

        public static Vec2 UnitY
        {
            get { return new Vec2(0, 1); }
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        public static Vec2 operator -(Vec2 v)
        {
            return new Vec2(-v.x, -v.y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        public static Vec2 operator *(Vec2 v, float s)
        {
            return new Vec2(v.x * s, v.y * s);
        }

        public static Vec2 operator *(float s, Vec2 v)
        {
            return v * s;
        }

        public static bool operator ==(Vec2 a, Vec2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }

        public static bool operator !=(Vec2 a, Vec2 b)
        {
            return !(a == b);
        }

        public static Vec2 operator /(Vec2 v, float s)
        {
            return v * (1 / s);
        }

        public static float Dot(Vec2 a, Vec2 b)
        {
            return a.x * b.x + a.y * b.y;
        }
        
        /// <summary>
        /// Cross two vectors.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="b"></param>
        /// <returns>A scalar representing the Z axis.</returns>
        public static float Cross(Vec2 v, Vec2 b)
        {
            return v.x * b.y - v.y * b.x;
        }

        /// <summary>
        /// Cross a vector with the Z axis (scalar).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns>A vector perpendicular to <paramref name="v"/> and the Z axis</returns>
        public static Vec2 Cross(Vec2 v, float s)
        {
            return new Vec2(s * v.y, -s * v.x);
        }

        /// <summary>
        /// Cross the Z axis (scalar) with a vector.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns>A vector perpendicular to <paramref name="v"/> and the Z axis</returns>
        public static Vec2 Cross(float s, Vec2 v)
        {
            return new Vec2(-s * v.y, s * v.x);
        }
        
        public static Vec2 Rotate(Vec2 v, float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            return new Vec2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        public static float DistanceSquared(Vec2 a, Vec2 b)
        {
            return (a - b).SqrMagnitude;
        }

        public static float Distance(Vec2 a, Vec2 b)
        {
            return (a - b).Magnitude;
        }

        public static Vec2 Min(Vec2 a, Vec2 b)
        {
            return new Vec2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
        }

        public static Vec2 Max(Vec2 a, Vec2 b)
        {
            return new Vec2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
        }
    }
}
