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
            Vec2 min = body.position - Vec2.One * radius;
            Vec2 max = body.position + Vec2.One * radius;
            return new Bounds(min, max);
        }

        public override void ComputeMass(float density)
        {
            float mass = Mathf.Pi * radius * radius * density;
            float inertia = mass * radius * radius;

            body.inverseMass = mass == 0 ? 0 : 1 / mass;
            body.inverseInertia = inertia == 0 ? 0 : 1 / inertia;
        }
    }
}
