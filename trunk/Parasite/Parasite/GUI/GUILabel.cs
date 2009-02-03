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
    /// Simple Label component.
    /// </summary>
    class GUILabel : GUIComponent
    {
        //draws text
        SpriteBatch batch;
        SpriteFont font;

        //padding to have at the sides of the text
        private int textPaddingSide = 5;
        //padding to have at the top and bottom of the text
        private int textPaddingTopAndBottom = 2;

        //width and height of the string
        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;
        public String Text = "";

        public GUILabel(Game game, Vector2 location, string name, string text)
            : base(game)
        {
            Location = location;
            Name = name;
            Text = text;
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
        }

        public override void Draw(GameTime gameTime)
        {                         
            batch.Begin();
            batch.DrawString(font, Text + (HasFocus ? "|" : ""), textPosition, Color.Black);
            batch.End();
        }
    }
}