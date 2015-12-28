using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine2D;

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
                for (float y = -height / 2 + 0.5f + hw ; y < 0; y += w)
                    physicsWorld.AddBody(new Body(s.Clone(),
                        new Vec2(x + Mathf.Lerp(0.02f, -0.02f, (float)r.NextDouble()), y)));
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
