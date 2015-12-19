using PhysicsEngine2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XNAPrimitives2D;

namespace PhysicsEngine2DDemo
{
    // / <summary>
    // / This is the main type for your game
    // / </summary>
    public class Game1 : Game
    {
        private SpriteBatch spriteBatch;

        private PhysicsWorld physicsWorld;

        private SpriteFont font;

        private float width = 30;
        private float height;

        private bool init;

        public Game1()
        {
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true,

                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
            };

            width *= graphics.PreferredBackBufferWidth / 800f;

            IsFixedTimeStep = false;

            physicsWorld = new PhysicsWorld();
        }

        // / <summary>
        // / Allows the game to perform any initialization it needs to before starting to run.
        // / This is where it can query for any required services and load any non-graphic
        // / related content.  Calling base.Initialize will enumerate through any components
        // / and initialize them as well.
        // / </summary>
        protected override void Initialize()
        {
            height = width / GraphicsDevice.Viewport.AspectRatio;
            Primitives2D.Initialize(GraphicsDevice, height);

            Polygon box = new Polygon(width / 2, 0.5f);
            Body b = new Body(box, new Vector2(0, -(height - 1) / 2));
            b.SetStatic();
            physicsWorld.AddBody(b);

            
            Circle c = new Circle(width * 0.1f);
            b = new Body(c, Vector2.Zero, 0, 0.3f);
            b.SetStatic();
            physicsWorld.AddBody(b);
            

            box = new Polygon(0.5f, height / 2 - 0.5f);
            b = new Body(box, new Vector2(-width / 2 + 0.5f, 0.5f));
            b.SetStatic();
            physicsWorld.AddBody(b);

            b = new Body(box.Clone(), new Vector2(width / 2 - 0.5f, 0.5f));
            b.SetStatic();
            physicsWorld.AddBody(b);

            // Test scenes: Will soon be integrated into demo
            /*
            Shape s = new Polygon(.5f, .5f);
            
            // Stacking
            System.Random r = new System.Random();
            //  Test for stacking, uncomment to test
            for (float x = -width / 2 + 2; x <= width / 2 - 2; x += 1.25f)
                for (float y = -height / 2 + 1.5f; y < 10; y += 1.0f)
                    physicsWorld.AddBody(new Body(s.Clone(),
                        new Vector2(x + MathHelper.Lerp(0.01f, -0.01f, (float)r.NextDouble()), y)));
            */

            /*
            Shape s = new Polygon(.5f, .5f);

            //  Pyramid (code taken directly from Box2D-Lite)
            Vector2 x = new Vector2(-width / 2 + 3, -height/2 + 1.5f);

            const int N = 30;

            for (int i = 0; i < N; ++i)
            {
                Vector2 y = x;

                for (int j = i; j < N; ++j)
                {
                    b = new Body(s.Clone(), y, 0, 0.15f);
                    physicsWorld.AddBody(b);

                    y += Vector2.UnitX * 1.125f;
                }

                // x += Vec2(0.5625f, 1.125f);
                x += new Vector2(0.5625f, 1.0f);
            }
            */
    
            lastMouseState = Mouse.GetState();
            lastKeyState = Keyboard.GetState();

            base.Initialize();

            init = true;
        }

        // / <summary>
        // / LoadContent will be called once per game and is the place to load
        // / all of your content.
        // / </summary>
        protected override void LoadContent()
        {
            //  Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("spriteFont1");
        }

        // / <summary>
        // / UnloadContent will be called once per game and is the place to unload
        // / all content.
        // / </summary>
        protected override void UnloadContent()
        {
            //  TODO: Unload any non ContentManager content here
        }

        private MouseState lastMouseState;
        private MouseState currentMouseState;
        private KeyboardState lastKeyState;
        private KeyboardState currentKeyState;

        // / <summary>
        // / Allows the game to run logic such as updating the world,
        // / checking for collisions, gathering input, and playing audio.
        // / </summary>
        // / <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!init) return;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            lastKeyState = currentKeyState;

            lastMouseState = currentMouseState;

            //  Get the mouse state relevant for this frame
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyUp(Keys.B) && lastKeyState.IsKeyDown(Keys.B))
                PhysicsWorld.bruteForce = !PhysicsWorld.bruteForce;

            Vector2 mouse = new Vector2(
                (float)currentMouseState.X / GraphicsDevice.Viewport.Width * width - width * 0.5f,
                (1 - (float)currentMouseState.Y / GraphicsDevice.Viewport.Height) * height - height * 0.5f);

            if (lastMouseState.LeftButton == ButtonState.Released
                && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Polygon aabb = new Polygon(.5f, .5f);
                System.Random r = new System.Random();
                Body b = new Body(aabb, mouse, (float)r.NextDouble(), 0.15f);

                physicsWorld.AddBody(b);
            }

            if (lastMouseState.RightButton == ButtonState.Released
                && currentMouseState.RightButton == ButtonState.Pressed)
            {
                Circle aabb = new Circle(0.5f);
                Body b = new Body(aabb, mouse, 1);

                physicsWorld.AddBody(b);
            }


            physicsWorld.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        // / <summary>
        // / This is called when the game should draw itself.
        // / </summary>
        // / <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "FPS : " + 1 / dt, new Vector2(10, 10), Color.White);

            spriteBatch.DrawString(font, "No of bodies: " + physicsWorld.bodies.Count,
                new Vector2(10, 35), Color.White);

            spriteBatch.DrawString(font, "(B)rute force collisions: " +
                                         (PhysicsWorld.bruteForce ? "Yes" : "No"), new Vector2(10, 60), Color.White);

            spriteBatch.End();

            foreach(Body body in physicsWorld.bodies)
                DrawBody(body, Color.Black);

            base.Draw(gameTime);
        }

        private static void DrawBody(Body body, Color color)
        {
            if (body.shape is Polygon)
            {
                Polygon polygon = body.shape as Polygon;
                Vector2[] verts = new Vector2[polygon.VertexCount];

                for (int i = 0; i < polygon.VertexCount; i++)
                    verts[i] = polygon.u * polygon.vertices[i];

                Primitives2D.DrawPolygon(body.position, verts, color);
            }
            else
            {
                Circle circle = body.shape as Circle;
                Primitives2D.DrawCircle(body.position, circle.radius, Color.Black);
                Vector2 r = MathUtil.Rotate(-Vector2.UnitY * circle.radius, circle.body.orientation);
                Primitives2D.DrawLine(body.position, body.position + r, Color.Black);
            }
        }
    }
}
