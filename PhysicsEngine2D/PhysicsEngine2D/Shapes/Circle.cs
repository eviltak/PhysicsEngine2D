using Microsoft.Xna.Framework;
using XNAPrimitives2D;

namespace PhysicsEngine2D
{
    public class Circle : Shape
    {
        public float radius;

        public Circle(float radius)
        {
            this.radius = radius;
            type = ShapeType.Circle;
        }

        public override void Draw()
        {
            Primitives2D.DrawCircle(body.position, radius, Color.Black);
        }

        public override Shape Clone()
        {
            return new Circle(radius);
        }

        public override Bounds GetBoundingBox(Vector2 position)
        {
            Vector2 min = position - Vector2.One * radius;
            Vector2 max = position + Vector2.One * radius;
            return new Bounds(min, max);
        }
    }
}
