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
    class GUILabel : GUIComponent
    {
        SpriteBatch batch;
        SpriteFont font;
        PrimitiveBatch primBatch;

        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;

        Vector2 Dimensions = Vector2.Zero;
        
        private string text;
        public string Text
        {
            set
            {
                text = value;
                fontDimensions = Vector2.Zero;
                if (font == null) return;

                if (Dimensions == Vector2.Zero)
                {
                    fontDimensions = font.MeasureString(text);
                    Dimensions = new Vector2((fontDimensions.X + (textPaddingSide * 2)), (fontDimensions.Y + (textPaddingTopAndBottom * 2)));
                }

                //set bounds
                //Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(fontDimensions.X + (textPaddingSide * 2)), Convert.ToInt32(fontDimensions.Y + (textPaddingTopAndBottom * 2)));
                Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
                //set position of the text
                textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
            }
            get
            {
                return text;
            }
        }

                
        public GUILabel(Game game, Vector2 location, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Text = caption;
        }

        public GUILabel(Game game, Vector2 location, Vector2 dimensions, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Text = caption;
            Dimensions = dimensions;
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
            //Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(fontDimensions.X + (textPaddingSide * 2)), Convert.ToInt32(fontDimensions.Y + (textPaddingTopAndBottom * 2)));
            Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)Dimensions.X, (int)Dimensions.Y);
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