using Microsoft.Xna.Framework;

namespace PhysicsEngine2D
{
    public abstract class Shape
    {
        public enum ShapeType
        {
            AABB, Circle,
            Count //Count is just so that the number of shapes can be referenced easily
        };

        public Body body;
        public ShapeType type;

        public abstract void Draw();
        public abstract Shape Clone();
        public abstract Bounds GetBoundingBox(Vector2 position);
    }
}
