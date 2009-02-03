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
    /// Simple button that has a MouseClick event.
    /// It will grow to fill the size of the caption.
    /// </summary>
    public class GUIButton : GUIComponent
    {
        //Event handler for when we're clicked
        protected event EventHandler MouseClick;

        //drws the button geometry
        PrimitiveBatch batch;
        
        //draws the button text
        SpriteBatch textBatch;
        SpriteFont font;
        
        //how much padding (in px) to have between the edge of the button and the caption
        private int textPaddingSide = 5;

        //how much padding to have on the top and bottom of the caption
        private int textPadding = 2;

        //width and height of the font
        Vector2 fontDimensions = Vector2.Zero;
        //screen position of the caption on the button
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
                //measure the caption so we know how big to make the button
                fontDimensions = font.MeasureString(caption);
                //set the bounds of the button to incorporate the caption + any padding
                Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)(fontDimensions.X + (textPaddingSide * 2)), (int)(fontDimensions.Y + (textPadding * 2)));
                //set the position of the caption.
                textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPadding);
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

            //if we're clicked on
            if (mState.LeftButton == ButtonState.Pressed && Bounds.Contains(mState.X, mState.Y))
            {
                if (MouseClick != null)
                {
                    //fire off any mouseclick event handlers that are listening
                    MouseClick(this, null);
                }
            }
            
            //if the font dimensions arent set yet then we can;t draw the button
            //so set the caption again (see caption "set" property).
            if (fontDimensions == Vector2.Zero)
                Caption = Caption;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //draw top left triangle of the button
            batch.Begin(PrimitiveType.TriangleList);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Left, Bounds.Top), BackgroundColor);
            batch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);

            //draw bottom right tri of the button
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

            //draw caption
            textBatch.Begin();
            textBatch.DrawString(font, caption, textPosition, ForegroundColor);
            textBatch.End();
           // batch.AddVertex(
            base.Draw(gameTime);
        }
    }
}