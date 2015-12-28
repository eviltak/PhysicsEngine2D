using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine2D;

namespace PhysicsEngine2DDemo.Demos
{
    public class Demo3 : Demo
    {
        public Demo3()
        {
            description = "Pyramid";
        }

        public override void Initialize(float width, float height)
        {
            physicsWorld.Clear();

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

            Shape s = new Polygon(.5f, .5f);

            //  Pyramid (code taken directly from Box2D Lite)
            Vec2 x = new Vec2(-width / 2 + 3, -height / 2 + 1.5f);

            const int N = 40;

            for (int i = 0; i < N; ++i)
            {
                Vec2 y = x;

                for (int j = i; j < N; ++j)
                {
                    b = new Body(s.Clone(), y, 0, 0.15f);
                    physicsWorld.AddBody(b);

                    y += Vec2.UnitX * 1.125f;
                }

                // x += Vec2(0.5625f, 1.125f);
                x += new Vec2(0.5625f, 1.0f);
            }
        }

        public override void Update(Vec2 mouse, bool leftClick, bool rightClick, float dt)
        {
            if (leftClick)
            {
                Polygon aabb = new Polygon(.5f, .5f);
                System.Random r = new System.Random();
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

        }
    }
}
