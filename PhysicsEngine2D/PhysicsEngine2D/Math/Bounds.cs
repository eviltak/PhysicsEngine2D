using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public struct Bounds
    {
        public Vector2 min;
        public Vector2 max;

        public Bounds(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Overlaps(Bounds other)
        {
            return Bounds.Overlaps(this, other);
        }

        public static bool Overlaps(Bounds a, Bounds b)
        {
            if (a.max.X < b.min.X || a.min.X > b.max.X) return false;
            if (a.max.Y < b.min.Y || a.min.Y > b.max.Y) return false;

            return true;
        }
    }
}
