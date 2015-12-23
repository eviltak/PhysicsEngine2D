using Math = System.Math;
using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public struct Bounds
    {
        public Vector2 min;
        public Vector2 max;

        public float Volume
        {
            get
            {
                return Math.Abs((max.Y - min.Y) * (max.X - min.X));
            }
        }

        public Bounds(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Contains(Bounds bounds)
        {
            return Vector2.Min(min, bounds.min) == min && Vector2.Max(max, bounds.max) == max;
        }

        public Bounds Fatten(float increase)
        {
            return new Bounds(min - Vector2.One * increase, max + Vector2.One * increase);
        }

        public bool Overlaps(Bounds other)
        {
            return Overlaps(this, other);
        }

        public Bounds Union(Bounds other)
        {
            return Union(this, other);
        }

        public static Bounds Union(Bounds a, Bounds b)
        {
            return new Bounds(Vector2.Min(a.min, b.min), Vector2.Max(a.max, b.max));
        }

        public static bool Overlaps(Bounds a, Bounds b)
        {
            if (a.max.X < b.min.X || a.min.X > b.max.X) return false;
            if (a.max.Y < b.min.Y || a.min.Y > b.max.Y) return false;

            return true;
        }
    }
}
