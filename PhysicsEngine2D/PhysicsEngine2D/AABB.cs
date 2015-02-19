using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using XNAPrimitives2D;

namespace Physics2DTutorial
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

        public override void Draw()
        {
            Vector2[] v = new Vector2[4];

            v[0] = min;
            v[1] = new Vector2(max.X, min.Y);
            v[2] = max;
            v[3] = new Vector2(min.X, max.Y);

            Primitives2D.DrawPolygon(body.position, v, Color.Black);
        }
    }
}
