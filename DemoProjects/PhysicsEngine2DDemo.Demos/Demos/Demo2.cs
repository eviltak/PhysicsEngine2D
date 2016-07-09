using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine2D;

using System;

namespace PhysicsEngine2DDemo.Demos
{
    public class Demo2 : Demo
    {
        public Demo2()
        {
            description = "Stacking Demo";
        }

        public override void Initialize(float width, float height)
        {
            physicsWorld.Clear();

            physicsWorld.timeScale = 1f;

            Polygon box = new Polygon(width / 2, 0.5f);
            Body b = new Body(box, new Vec2(0, -height / 2 + 0.5f));
            b.SetStatic();
            physicsWorld.AddBody(b);

            box = new Polygon(0.5f, height / 2 - 0.5f);
            b = new Body(box, new Vec2(-width / 2 + 1f, 0.6f));
            b.SetStatic();
            physicsWorld.AddBody(b);

            b = new Body(box.Clone(), new Vec2(width / 2 - 1f, 0.6f));
            b.SetStatic();
            physicsWorld.AddBody(b);

            float hw = 1.25f;
            float w = hw * 2;

            Shape s = new Polygon(hw, hw);

            // Stacking
            System.Random r = new System.Random();
            for (float x = -width / 2 + w * 1.5f; x <= width / 2 - w * 1.5f; x += w * 1.5f)
                for (float y = -height / 2 + 0.5f + hw; y < 0; y += w)
                    physicsWorld.AddBody(new Body(s.Clone(),
                        new Vec2(x + Mathf.Lerp(0.02f, -0.02f, (float)r.NextDouble()), y)));
        }

        public override void Update(Vec2 mouse, bool leftClick, bool rightClick, float dt)
        {
            if (leftClick)
            {
                Polygon aabb = new Polygon(.5f, .5f);
                Random r = new Random();
                Body b = new Body(aabb, mouse, (float)r.NextDouble(), 0.15f);

                physicsWorld.AddBody(b);
            }

            if (rightClick)
            {
                Circle aabb = new Circle(0.5f);
                Body b = new Body(aabb, mouse, 1);

                physicsWorld.AddBody(b);
            }

            physicsWorld.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font, float dt)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "No of bodies: " + physicsWorld.bodies.Count,
                new Vector2(10, 85), Color.White);

            spriteBatch.End();

            foreach (Body body in physicsWorld.bodies)
                DrawBody(body, Color.White);

            //DebugDraw();
        }

        private DebugDrawer debugDrawer = new DebugDrawer();

        private void DebugDraw()
        {
            physicsWorld.DebugDraw(debugDrawer);
        }

        private class DebugDrawer : IDebugDrawer
        {
            private Random random = new Random();

            private Color RandomColor { get { return new Color(random.Next(255), random.Next(255), random.Next(255)); } }

            private Color[] colors = new Color[5];

            public DebugDrawer()
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = RandomColor;
                }
            }

            void IDebugDrawer.Draw(Vec2[] vertices, params object[] data)
            {
                Primitives2D.DrawPolygon(Vec2.Zero.ToVector2(), Array.ConvertAll(vertices, v => v.ToVector2()), colors[(int)data[0] % colors.Length]);
            }

            void IDebugDrawer.Draw(Vec2 center, float radius, params object[] data)
            {
                Primitives2D.DrawCircle(Vec2.Zero.ToVector2(), radius, colors[(int)data[0] % colors.Length]);
            }
        }
    }
}