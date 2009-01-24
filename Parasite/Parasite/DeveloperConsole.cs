using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Parasite
{
    public interface IDeveloperConsole { };
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class DeveloperConsole : Microsoft.Xna.Framework.DrawableGameComponent, IDeveloperConsole
    {
        private InputHandler input;
        private bool show = false;
        private Queue<String> lines;
        private int maxLines = 15;

        private PrimitiveBatch batch;
        private SpriteFont font;
        private SpriteBatch textBatch;

        private Rectangle bounds;
        private int consoleHeight;
        private float textHeight;


        public DeveloperConsole(Game game)
            : base(game)
        {
            show = false;
            game.Services.AddService(typeof(IDeveloperConsole), this);
            lines = new Queue<string>();
        }
        protected override void LoadContent()
        {
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandler));
            batch = new PrimitiveBatch(GraphicsDevice);
            textBatch = new SpriteBatch(GraphicsDevice);

            font = Game.Content.Load<SpriteFont>(@"Fonts\Console");
            textHeight = font.MeasureString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ").Y;
            consoleHeight = Convert.ToInt32(textHeight * maxLines + (maxLines * 2));
            bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 0);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (input.IsKeyPressed(Keys.OemTilde))
            {
                show = !show;
               // bounds.Height = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //if (!show)
           //     return;
            if (bounds.Height < consoleHeight && show)
                bounds.Inflate(0, 20);

            if (bounds.Height > 0 && !show)
                bounds.Inflate(0, -20);

            Color consoleColor = Color.Gray;
            consoleColor.A = 200;
            batch.Begin(PrimitiveType.TriangleList);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), consoleColor);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), consoleColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), consoleColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), consoleColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), consoleColor);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), consoleColor);
            batch.End();
            textBatch.Begin();
            if (lines.Count > 0)
            {
                string[] strArray = lines.ToArray<String>();
                for (int i = 0; i < lines.Count; i++)
                {
                    textBatch.DrawString(font, strArray[i], new Vector2(bounds.Left + 5, bounds.Bottom - ((i+1)* textHeight)), Color.White);                   
                }
            }
            textBatch.End();
            

            base.Draw(gameTime);
        }

        public void Write(String str)
        {
            lines.Enqueue(str);
            if (lines.Count > maxLines)
                lines.Dequeue();
        }
    }
}