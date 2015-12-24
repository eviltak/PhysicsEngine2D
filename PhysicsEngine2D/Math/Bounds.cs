using Math = System.Math;

namespace PhysicsEngine2D
{
    public struct Bounds
    {
        public Vec2 min;
        public Vec2 max;

        public float Volume
        {
            get
            {
                return Math.Abs((max.y - min.y) * (max.x - min.x));
            }
        }

        public Bounds(Vec2 min, Vec2 max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Contains(Bounds bounds)
        {
            return Vec2.Min(min, bounds.min) == min && Vec2.Max(max, bounds.max) == max;
        }

        public Bounds Fatten(float increase)
        {
            return new Bounds(min - Vec2.One * increase, max + Vec2.One * increase);
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
            return new Bounds(Vec2.Min(a.min, b.min), Vec2.Max(a.max, b.max));
        }

        public static bool Overlaps(Bounds a, Bounds b)
        {
            if (a.max.x < b.min.x || a.min.x > b.max.x) return false;
            if (a.max.y < b.min.y || a.min.y > b.max.y) return false;

            return true;
        }

        public bool Raycast(Ray2 ray)
        {
            float tminX = (min.x - ray.origin.x) / ray.direction.x;
            float tmaxX = (max.x - ray.origin.x) / ray.direction.x;

            float tminY = (min.y - ray.origin.y) / ray.direction.y;
            float tmaxY = (max.y - ray.origin.y) / ray.direction.y;

            float tmin = Math.Max(Math.Min(tminX, tmaxX), Math.Min(tminY, tmaxY));
            float tmax = Math.Min(Math.Max(tminX, tmaxX), Math.Max(tminY, tmaxY));

            if (tmax < 0)
                return false;

            if (tmax < tmin)
                return false;

            return true;
        }
    }
}
    