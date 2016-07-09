using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine2D;

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
            DrawShape(body.shape, body.position, color);

            if (body.shape is Circle)
            {
                Vec2 r = body.transform.LocalToWorldDirection(-Vec2.UnitY * (body.shape as Circle).radius);
                Primitives2D.DrawLine(body.position.ToVector2(), (body.position + r).ToVector2(), color);
            }
        }

        protected static void DrawShape(Shape shape, Vec2 position, Color color)
        {
            if (shape is Polygon)
            {
                Polygon polygon = shape as Polygon;
                Vec2[] verts = new Vec2[polygon.VertexCount];

                for (int i = 0; i < polygon.VertexCount; i++)
                    verts[i] = polygon.transform.localToWorldRotation * polygon.vertices[i];

                Primitives2D.DrawPolygon(position.ToVector2(), Array.ConvertAll(verts, v => v.ToVector2()), color);
            }
            else if (shape is Circle)
            {
                Circle circle = shape as Circle;
                Primitives2D.DrawCircle(position.ToVector2(), circle.radius, color);
            }
        }
    }
}