using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Blob_P2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BlobManager theBlob;

        FrameRateCounter frc;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

      
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        ///
        Vector2[] sourceVertices;
        StaticBody sb;
        public static SpatialGrid grid;
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            
            // TODO: use this.Content to load your game content here
            sourceVertices = new Vector2[]
			{
				new Vector2(100, 100),
				new Vector2(200, 100),
                new Vector2(300, 300),
                new Vector2(71, 363)
			};

            sb = new StaticBody(PhysicsOverlord.GetInstance().GetID(), GraphicsDevice, Color.Yellow, sourceVertices);

            theBlob = new BlobManager(this);
            this.Components.Add(theBlob);
            theBlob.Initialize();

            frc = new FrameRateCounter(this);
            Components.Add(frc);
            frc.Initialize();

            grid.AddObject(sb);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                theBlob.stopstart();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                theBlob.increaseParticles();
            }
            this.IsMouseVisible = true;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.White);
            

            base.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(Content.Load<SpriteFont>("DebugFont"), Mouse.GetState().ToString(), new Vector2(10, 10), Color.Red);
            spriteBatch.End();
            // TODO: Add your drawing code here
            sb.Draw();
        }

        public static RenderTarget2D CloneRenderTarget(GraphicsDevice device, int numberLevels)
        {
            return new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                numberLevels,
                device.DisplayMode.Format,
                device.PresentationParameters.MultiSampleType,
                device.PresentationParameters.MultiSampleQuality
            );
        }
    }
}
