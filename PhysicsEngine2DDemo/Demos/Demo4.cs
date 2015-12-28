using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine2D;
using XNAPrimitives2D;

namespace PhysicsEngine2DDemo.Demos
{
    public class Demo4 : Demo
    {
        private float direction;

        private bool isIntersect;
        private RaycastResult result;
        private Ray2 ray;
        private float distance = 50;

        public Demo4()
        {
            description = "Raycasting";
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
        }

        public override void Update(Vec2 mouse, bool leftClick, bool rightClick, float dt)
        {
            if (leftClick)
            {
                Polygon aabb = new Polygon(.5f, .5f);
                System.Random r = new System.Random();
                Body b = new Body(aabb, mouse, (float)r.NextDouble(), 0.15f);
                b.SetStatic();

                physicsWorld.AddBody(b);
            }

            if (rightClick)
            {
                Circle aabb = new Circle(0.5f);
                Body b = new Body(aabb, mouse, 1);
                b.SetStatic();

                physicsWorld.AddBody(b);
            }
            
            ray = new Ray2(Vec2.Zero, Vec2.Rotate(Vec2.UnitY, direction));

            isIntersect = physicsWorld.Raycast(ray, distance, out result);

            direction = (direction + dt * 0.1f) % (2 * Mathf.Pi);

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

            if(isIntersect)
            {
                Primitives2D.DrawLine(Vec2ToVector2(ray.origin), Vec2ToVector2(result.point), Color.Green);
                Primitives2D.DrawLine(Vec2ToVector2(result.point), 
                    Vec2ToVector2(result.point + result.normal), Color.Yellow);
            }
            else
            {
                Primitives2D.DrawLine(Vec2ToVector2(ray.origin),
                    Vec2ToVector2(ray.origin + ray.direction * distance), Color.Blue);
            }
        }
    }
}
