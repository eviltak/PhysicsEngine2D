using System;
using PhysicsEngine2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PhysicsEngine2DDemo.Demos
{
    // / <summary>
    // / This is the main type for your game
    // / </summary>
    public class DemoGame : Game
    {
        private SpriteBatch spriteBatch;

        private SpriteFont font;

        private float width = 40;
        private float height;

        private bool init;
        private int selectedDemo;

        private static Demo[] demos =
        {
            new Demo1(),
            new Demo2(),
            new Demo3(),
            new Demo4()
        };

		private Demo SelectedDemo => demos[selectedDemo];

        public DemoGame()
        {
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true,

                PreferredBackBufferWidth = 1200,
                PreferredBackBufferHeight = 675
            };

            width *= graphics.PreferredBackBufferWidth / 800f;

            IsFixedTimeStep = false;

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

            SelectedDemo.Initialize(width, height);
    
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

        private static MouseState lastMouseState;
        private static MouseState currentMouseState;
        private static KeyboardState lastKeyState;
        private static KeyboardState currentKeyState;

        // / <summary>
        // / Allows the game to run logic such as updating the world,
        // / checking for collisions, gathering input, and playing audio.
        // / </summary>
        // / <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!init) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            lastKeyState = currentKeyState;

            lastMouseState = currentMouseState;

            //  Get the mouse state relevant for this frame
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyUp(Keys.B) && lastKeyState.IsKeyDown(Keys.B))
                PhysicsWorld.bruteForce = !PhysicsWorld.bruteForce;

            if (currentKeyState.IsKeyUp(Keys.Right) && lastKeyState.IsKeyDown(Keys.Right))
			{
				selectedDemo = (selectedDemo + 1) % demos.Length;
				SelectedDemo.Initialize(width, height);
			}

			if (currentKeyState.IsKeyUp(Keys.Left) && lastKeyState.IsKeyDown(Keys.Left))
            {
				selectedDemo = Mathf.Mod(selectedDemo - 1, demos.Length);
                SelectedDemo.Initialize(width, height);
            }

            Vec2 mouse = new Vec2(
                (float)currentMouseState.X / GraphicsDevice.Viewport.Width * width - width * 0.5f,
                (1 - (float)currentMouseState.Y / GraphicsDevice.Viewport.Height) * height - height * 0.5f);


            SelectedDemo.Update(mouse, lastMouseState.LeftButton == ButtonState.Released
                && currentMouseState.LeftButton == ButtonState.Pressed, 
                lastMouseState.RightButton == ButtonState.Released
                && currentMouseState.RightButton == ButtonState.Pressed, dt);

            base.Update(gameTime);
        }

        // / <summary>
        // / This is called when the game should draw itself.
        // / </summary>
        // / <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "FPS : " + 1 / dt, new Vector2(10, 10), Color.White);
                
            spriteBatch.DrawString(font, "(B)rute force collisions: " + 
                (PhysicsWorld.bruteForce ? "Yes" : "No"), new Vector2(10, 35), Color.White);

			spriteBatch.DrawString(font, $"Demo {selectedDemo + 1} of {demos.Length}: " + 
                $"{SelectedDemo.description}", new Vector2(10, 60), Color.White);

            spriteBatch.End();

            SelectedDemo.Draw(spriteBatch, font, dt);

            base.Draw(gameTime);
        }

        
    }
}