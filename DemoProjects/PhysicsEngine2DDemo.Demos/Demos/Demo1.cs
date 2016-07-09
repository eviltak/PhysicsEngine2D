using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysicsEngine2D;

namespace PhysicsEngine2DDemo.Demos
{
	public class Demo1 : Demo
	{
		public Demo1()
		{
			description = "Body Sandbox";
		}

		public override void Initialize(float width, float height)
		{
			physicsWorld.Clear();

			Polygon box = new Polygon(width / 2, 0.5f);
			Body b = new Body(box, new Vec2(0, -height / 2 + 0.5f));
			b.SetStatic();
			physicsWorld.AddBody(b);

			Circle c = new Circle(width * 0.1f);
			b = new Body(c, Vec2.Zero, 0, 0.3f);
			physicsWorld.AddBody(b);
			b.SetStatic();

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