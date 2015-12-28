using System;

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

        public override bool Raycast(Ray2 ray, float distance, out RaycastResult result)
        {
            result = new RaycastResult();

            Vec2 delta = ray.origin - transform.position;

            // Since  length of ray direction is always 1, therefore a = 1
            float b = 2 * Vec2.Dot(ray.direction, delta);
            float c = delta.SqrMagnitude - radius * radius;

            float d = b * b - 4 * c;

            if (d < 0)
            {
                return false;
            }

            float t;

            if (d < Mathf.Epsilon)
            {
                t = -b / 2;
            }
            else
            {
                d = (float)Math.Sqrt(d);
                t = (-b - d) / 2;
            }

            result.point = ray.origin + ray.direction * t;
            result.distance = t;
            result.normal = (result.point - transform.position).Normalized;

            return t <= distance;
        }
    }
}
