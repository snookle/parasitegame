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
    /// Simple edit box component.
    /// When the component has focus, then any keystrokes will be directed to this component.
    /// it will expand horizontlaly to encompass all the text.
    /// </summary>
    class GUIEditBox : GUIComponent
    {
        //draws the edit box geometry
        PrimitiveBatch primBatch;
        //draws text
        SpriteBatch batch;
        SpriteFont font;

        //width and height of the string
        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;
        public String Text = "";

        //length (in px) of the edit box
        public int Length;

                
        public GUIEditBox(Game game, Vector2 location, string name, int editboxLength, string startingText)
            : base(game)
        {
            Location = location;
            Name = name;
            Length = editboxLength;
            Text = startingText;
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
            //get the height of this font (we use capital Y because it's generally pretty tall)
            fontDimensions = font.MeasureString("Y");
            //set bounds
            Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(Length), Convert.ToInt32(fontDimensions.Y + (textPaddingTopAndBottom * 2)));
            //set position of the text
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
            //if we have focus (see GUIComponent)
            if (HasFocus)
            {
                //tell the input manager to only send keystrokes here
                input.IgnoreAllExcept(this);
                foreach (string key in input.GetPressedKeysAsStrings(this))
                {
                    //handle special case keys.
                    if (key == "back")
                    {
                        if (!String.IsNullOrEmpty(Text))
                          Text = Text.Remove(Text.Length - 1, 1);
                        continue;
                    }

                    //should we fire a TextUpdate event here so that other stuff can listen for the changes in this edit box?
                    if (key == "enter")
                    {

                    }

                    //now ignore everything else except letters and digits
                    if (key.Length > 1)
                        continue;

                    //append the text string with the keypresses
                    Text += key;

                    //measure the new string so that we can increase the size of our text box later on
                    fontDimensions = font.MeasureString(Text);
                }
            }
            else
            {
                // if we dont have focus, then let the input manager send keystrokes eveywhere.
                input.ClearIgnore(this);
            }
            //if the text width is going to be larger than the current bounds
            if ((fontDimensions.X + (2*textPaddingSide)) >= Bounds.Width)
            {
                //increate the size of the box
                Bounds.Width = Convert.ToInt32(fontDimensions.X + (textPaddingSide * 2));
            }
        }

        public override void Draw(GameTime gameTime)
        {                         
            primBatch.Begin(PrimitiveType.TriangleList);
            //black outline
            primBatch.AddVertex(new Vector2(Bounds.Left-1, Bounds.Bottom+1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Left-1, Bounds.Top-1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Right+1, Bounds.Top-1), Color.Black);
            
            primBatch.AddVertex(new Vector2(Bounds.Right + 1, Bounds.Top - 1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Right + 1, Bounds.Bottom + 1), Color.Black);
            primBatch.AddVertex(new Vector2(Bounds.Left - 1, Bounds.Bottom + 1), Color.Black);

            //white text area
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Top), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);

            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), BackgroundColor);
            primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);

            primBatch.End();

            batch.Begin();
            batch.DrawString(font, Text + (HasFocus ? "|" : ""), textPosition, ForegroundColor);
            batch.End();
        }
    }
}