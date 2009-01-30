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
    class GUIEditBox : GUIComponent
    {
        PrimitiveBatch primBatch;
        SpriteBatch batch;
        SpriteFont font;
        private int textPaddingSide = 5;
        private int textPaddingTopAndBottom = 2;
        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;
        public Color ForegroundColor = Color.Black;
        public String Text = "";
        public int Length;

                
        public GUIEditBox(Game game, Vector2 location, string name, int editboxLength)
            : base(game)
        {
            Location = location;
            Name = name;
            Length = editboxLength;
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
            fontDimensions = font.MeasureString("Y");
            Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(Length), Convert.ToInt32(fontDimensions.Y + (textPaddingTopAndBottom * 2)));
            textPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Vector2 mouseLoc = input.MousePosition;
            if (HasFocus)
            {
                input.IgnoreAllExcept(this);
                foreach (string key in input.GetPressedKeysAsStrings(this))
                {
                    if (key == "back")
                    {
                        if (!String.IsNullOrEmpty(Text))
                          Text = Text.Remove(Text.Length - 1, 1);
                        continue;
                    }

                    if (key == "enter")
                    {

                    }

                    //now ignore everything else except letters and digits
                    if (key.Length > 1)
                        continue;

                    Text += key;
                    fontDimensions = font.MeasureString(Text);
                }
            }
            else
            {
                input.ClearIgnore(this);
            }

            if ((fontDimensions.X + (2*textPaddingSide)) >= Bounds.Width)
            {
                Bounds.Width = Convert.ToInt32(fontDimensions.X + (textPaddingSide * 2));
            }
        }

        public override void Draw(GameTime gameTime)
        {                         
            batch.Begin();
            primBatch.Begin(PrimitiveType.TriangleList);
            //black outline
            primBatch.AddVertex(new Vector2(Bounds.Left-1, Bounds.Bottom+1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Left-1, Bounds.Top-1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Right+1, Bounds.Top-1), Color.Black);
            
            primBatch.AddVertex(new Vector2(Bounds.Right + 1, Bounds.Top - 1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Right + 1, Bounds.Bottom + 1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Left - 1, Bounds.Bottom + 1), Color.Black);

            //white text area
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), Color.White);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Top), Color.White);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), Color.White);

            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), Color.White);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), Color.White);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), Color.White);

            
            
            primBatch.End();

            batch.DrawString(font, Text + (HasFocus ? "|" : ""), textPosition, Color.Black);
            batch.End();
        }
    }
}