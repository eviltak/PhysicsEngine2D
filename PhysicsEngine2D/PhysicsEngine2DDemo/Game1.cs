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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Scene scene;

        SpriteFont font;

        float width = 20;
        float height;

        bool init = false;

        public Game1()
            : base()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferMultiSampling = true;

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;

            // graphics.ToggleFullScreen();

            width *= graphics.PreferredBackBufferWidth / 800f;

            scene = new Scene();
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

            AABB aabb;
            Body b;

            aabb = new AABB(width, 1);
            b = new Body(aabb, new Vector2(0, -(height - 1) / 2), 0, 0.1f, 0.25f);

            scene.AddBody(b);

            Circle c = new Circle(1);
            b = new Body(c, Vector2.Zero, 0, 0.3f);
            scene.AddBody(b);

            aabb = new AABB(1, height - 1);
            b = new Body(aabb, new Vector2(-width / 2 + 0.5f, 0.5f), 0);
            scene.AddBody(b);

            b = new Body(aabb.Clone(), new Vector2(width / 2 - 0.5f, 0.5f), 0);
            scene.AddBody(b);


            Shape s = new Circle(0.5f);
            s = new AABB(1, 1);

            //  Test for stacking, uncomment to test
            /*for (float x = -width / 2 + 2; x <= width / 2 - 2; x += 1.5f)
                for (float y = -height / 2 + 1.5f; y < height; y += 1.5f)
                    scene.AddBody(new Body(s.Clone(), new Vector2(x, y), 1, 0.1f, 0.15f));*/

            //  Testing pyramid (code taken directly from Box2D-Lite)
            /*Vector2 x = new Vector2(-width / 2 -0.5f, -height + 3.5f);
            Vector2 y;

            const int n = 38;

            for (int i = 0; i < n; ++i)
            {
                y = x;

                for (int j = i; j < n; ++j)
                {
                    b = new Body(s.Clone(), y, 10, 0.15f, 0.2f);
                    scene.AddBody(b);

                    y += Vector2.UnitX * 1.125f;
                }

                // x += Vec2(0.5625f, 1.125f);
                x += new Vector2(0.5625f, 2.0f);
            }*/


            lastMouseState = Mouse.GetState();

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

        MouseState lastMouseState, currentMouseState;

        // / <summary>
        // / Allows the game to run logic such as updating the world,
        // / checking for collisions, gathering input, and playing audio.
        // / </summary>
        // / <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!init) return;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            lastMouseState = currentMouseState;

            //  Get the mouse state relevant for this frame
            currentMouseState = Mouse.GetState();

            Vector2 mouse = new Vector2((float)currentMouseState.X / GraphicsDevice.Viewport.Width * width - width * 0.5f,
                (1 - (float)currentMouseState.Y / GraphicsDevice.Viewport.Height) * height - height * 0.5f);

            if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed) {
                AABB aabb = new AABB(1, 1);
                Body b = new Body(aabb, mouse, 1, 0.1f, 0.015f);

                scene.AddBody(b);
            }

            if (lastMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed) {
                Circle aabb = new Circle(0.707f);
                Body b = new Body(aabb, mouse, 1, 0.1f, 0.015f);

                scene.AddBody(b);
            }

            scene.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

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
            spriteBatch.DrawString(font, "FPS : " + (1 / dt).ToString(), new Vector2(10, 10), Color.White);

            spriteBatch.DrawString(font, "No of bodies: " + scene.bodies.Count, new Vector2(10, 35), Color.White);
            spriteBatch.End();

            scene.Draw();

            base.Draw(gameTime);
        }
    }
}
