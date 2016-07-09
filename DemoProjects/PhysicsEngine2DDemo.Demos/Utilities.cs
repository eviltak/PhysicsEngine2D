using System;
using Microsoft.Xna.Framework;
using PhysicsEngine2D;

namespace PhysicsEngine2DDemo.Demos
{
    internal static class Utilities
    {
        public static Vector2 ToVector2(this Vec2 v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}

