using System;
using System.Collections.Generic;

using Physics2DTutorial;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using XNAPrimitives2D;

namespace Physics2DDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Scene scene;

        SpriteFont font;

        public Game1()
            : base()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            scene = new Scene();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Primitives2D.Initialize(GraphicsDevice);

            AABB aabb = new AABB(new Vector2(-GraphicsDevice.Viewport.Width / 2f, -50),
                new Vector2(GraphicsDevice.Viewport.Width / 2f, 50));
            Body b = new Body(aabb, new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height - 50), 
                0, 0.1f, 0.1f, 0.25f, 1);

            scene.AddBody(b);

            Circle c = new Circle(50);
            b = new Body(c, new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f - 25), 
                0, 0.3f, 0.2f, 0.15f, 1);

            scene.AddBody(b);

            lastMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("spriteFont1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        MouseState lastMouseState, currentMouseState;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            lastMouseState = currentMouseState;

            // Get the mouse state relevant for this frame
            currentMouseState = Mouse.GetState();

            if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed) {
                AABB aabb = new AABB(Vector2.One * -10, Vector2.One * 10);
                Body b = new Body(aabb, new Vector2(currentMouseState.X, currentMouseState.Y),
                    1, 0.1f, 0.015f, 0.02f, 1);

                scene.AddBody(b);
            }

            if (lastMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed) {
                Circle aabb = new Circle(20);
                Body b = new Body(aabb, new Vector2(currentMouseState.X, currentMouseState.Y),
                    1, 0.1f, 0.015f, 0.02f, 1);

                scene.AddBody(b);
            }

            scene.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "FPS : " + (1 / dt).ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.End();

            scene.Draw();

            base.Draw(gameTime);
        }
    }
}
