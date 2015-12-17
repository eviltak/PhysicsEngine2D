using Microsoft.Xna.Framework;

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
