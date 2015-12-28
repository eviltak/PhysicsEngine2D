using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine2D;
using XNAPrimitives2D;

namespace PhysicsEngine2DDemo.Demos
{
    public abstract class Demo
    {
        public string description;

        protected PhysicsWorld physicsWorld;

        protected Demo()
        {
            physicsWorld = new PhysicsWorld();
        }

        public abstract void Initialize(float width, float height);
        public abstract void Update(Vec2 mouse, bool leftClick, bool rightClick, float dt);
        public abstract void Draw(SpriteBatch spriteBatch, SpriteFont font, float dt);

        protected static void DrawBody(Body body, Color color)
        {
            if (body.shape is Polygon)
            {
                Polygon polygon = body.shape as Polygon;
                Vec2[] verts = new Vec2[polygon.VertexCount];

                for (int i = 0; i < polygon.VertexCount; i++)
                    verts[i] = polygon.transform.localToWorldRotation * polygon.vertices[i];

                Primitives2D.DrawPolygon(Vec2ToVector2(body.position), Array.ConvertAll(verts, Vec2ToVector2), color);
            }
            else if (body.shape is Circle)
            {
                Circle circle = body.shape as Circle;
                Primitives2D.DrawCircle(Vec2ToVector2(body.position), circle.radius, color);
                Vec2 r = body.transform.LocalToWorldDirection(-Vec2.UnitY * circle.radius);
                Primitives2D.DrawLine(Vec2ToVector2(body.position), Vec2ToVector2(body.position + r), color);
            }
        }

        protected static Vector2 Vec2ToVector2(Vec2 v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}