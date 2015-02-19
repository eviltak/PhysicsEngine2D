using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine2D
{
    public abstract class Shape
    {
        public enum ShapeType { AABB, Circle };

        public Body body;
        public ShapeType type;

        public abstract void Draw();
    }
}
