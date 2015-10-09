using Microsoft.Xna.Framework;
using XNAPrimitives2D;

namespace PhysicsEngine2D
{
    public class Circle : Shape
    {
        public float radius;

        public Circle(float radius) : base()
        {
            this.radius = radius;
            type = ShapeType.Circle;
        }

        public override void Draw()
        {
            Primitives2D.DrawCircle(body.position, radius, Color.Black);
            Vector2 r = MathUtil.Rotate(-Vector2.UnitY * radius, body.orientation);
            Primitives2D.DrawLine(body.position, body.position + r, Color.Black);
        }

        public override Shape Clone()
        {
            return new Circle(radius);
        }

        public override Bounds GetBoundingBox()
        {
            Vector2 min = body.position - Vector2.One * radius;
            Vector2 max = body.position + Vector2.One * radius;
            return new Bounds(min, max);
        }

        public override void ComputeMass(float density)
        {
            float mass = MathHelper.Pi * radius * radius * density;
            float inertia = mass * radius * radius;

            body.inverseMass = mass == 0 ? 0 : 1 / mass;
            body.inverseInertia = inertia == 0 ? 0 : 1 / inertia;
        }
    }
}
