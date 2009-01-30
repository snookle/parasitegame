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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GUIButton : GUIComponent
    {
        protected event EventHandler MouseClick;
        PrimitiveBatch batch;
        SpriteBatch textBatch;
        SpriteFont font;
        private int textPaddingSide = 5;
        private int textPadding = 2;
        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;
        public Color BackgroundColor = Color.LightGray;
        public Color ForegroundColor = Color.Black;

        private string caption;
        public string Caption
        {
            set
            {
                caption = value;
                fontDimensions = Vector2.Zero;
                if (font == null) return;
                fontDimensions = font.MeasureString(caption);
                Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)(fontDimensions.X + (textPaddingSide * 2)), (int)(fontDimensions.Y + (textPadding * 2)));
                textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPadding); //+ (fontDimensions/2);
            }
            get
            {
                return caption;
            }
        }

                
        public GUIButton(Game game, Vector2 location, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Caption = caption;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization code here
            batch = new PrimitiveBatch(Game.GraphicsDevice);
            textBatch = new SpriteBatch(Game.GraphicsDevice);
            font = Game.Content.Load<SpriteFont>(@"Fonts\Console");

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState mState = Mouse.GetState();

            if (mState.LeftButton == ButtonState.Pressed && Bounds.Contains(mState.X, mState.Y))
            {
                if (MouseClick != null)
                {
                    MouseClick(this, null);
                }
            }
            
            if (fontDimensions == Vector2.Zero)
                Caption = Caption;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            batch.Begin(PrimitiveType.TriangleList);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Top), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);


            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);
            batch.End();
            
            //rad dropshadow
            batch.Begin(PrimitiveType.LineList);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), Color.Black);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), Color.Black);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), Color.Black);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), Color.Black);
            batch.End();

            
            textBatch.Begin();
            textBatch.DrawString(font, caption, textPosition, ForegroundColor);
            textBatch.End();
           // batch.AddVertex(
            base.Draw(gameTime);
        }
    }
}