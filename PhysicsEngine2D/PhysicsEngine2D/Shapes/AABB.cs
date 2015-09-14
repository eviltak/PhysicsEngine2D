using System;
using Microsoft.Xna.Framework;
using XNAPrimitives2D;

namespace PhysicsEngine2D
{
    public class AABB : Shape
    {
        public Vector2 min;
        public Vector2 max;

        public AABB(Vector2 min, Vector2 max)
        {
            this.max = max;
            this.min = min;
            type = ShapeType.AABB;
        }

        public AABB(float width, float height)
        {
            var wh = new Vector2(width / 2, height / 2);
            max = wh;
            min = -wh;
        }

        public override void Draw()
        {
            Vector2[] v = new Vector2[4];

            v[0] = min;
            v[1] = new Vector2(max.X, min.Y);
            v[2] = max;
            v[3] = new Vector2(min.X, max.Y);

            Primitives2D.DrawPolygon(body.position, v, Color.Black);
        }

        public override Shape Clone()
        {
            return new AABB(min, max);
        }

        public override Bounds GetBoundingBox(Vector2 position)
        {
            return new Bounds(position + min, position + max);
        }
    }
}
