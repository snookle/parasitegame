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
    /// <summary>
    /// Simple text label.
    /// </summary>
    class GUIListBoxItem : GUIComponent
    {
        SpriteBatch batch;
        SpriteFont font;
        PrimitiveBatch primBatch;

        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;

        public Vector2 boxDimensions = new Vector2(100,20);
        
        private string text;
        public string Text
        {
            set
            {
                text = value;
                fontDimensions = Vector2.Zero;
                if (font == null) return;
                fontDimensions = font.MeasureString(text);
                //set bounds
                Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(boxDimensions.X), Convert.ToInt32(boxDimensions.Y));
                //set position of the text
                textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
            }
            get
            {
                return text;
            }
        }
                
        public GUIListBoxItem(Game game, string name, string caption)
            : base(game)
        {
            Name = name;
            Text = caption;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposed)
            {
                if (disposing)
                {
                    if (batch != null)
                    {
                        batch.Dispose();
                        batch = null;
                    }
                    if (primBatch != null)
                    {
                        primBatch.Dispose();
                        primBatch = null;
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization code here
            batch = new SpriteBatch(Game.GraphicsDevice);
            font = Game.Content.Load<SpriteFont>(@"Fonts\Console");
            primBatch = new PrimitiveBatch(Game.GraphicsDevice);
            Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(boxDimensions.X), Convert.ToInt32(boxDimensions.Y));
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (textPosition == Vector2.Zero)
            {
                Text = text;
            }
            base.Update(gameTime);
        }

        public override void UpdateLocation(Vector2 newLocation)
        {
            Location = newLocation;
            Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(boxDimensions.X), Convert.ToInt32(boxDimensions.Y));
            textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
        }

        public override void Draw(GameTime gameTime)
        {
            primBatch.Begin(PrimitiveType.TriangleList);
            //white text area
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Top), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);

            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);

            primBatch.End();

            batch.Begin();
            batch.DrawString(font, text, textPosition, ForegroundColor);
            batch.End();
        }
    }
}