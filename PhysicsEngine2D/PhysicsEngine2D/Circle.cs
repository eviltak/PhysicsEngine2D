using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using XNAPrimitives2D;

namespace Physics2DTutorial
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
    }
}
