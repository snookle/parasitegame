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
    class CheckStateEventArgs : EventArgs
    {
        public CheckStateEventArgs(bool ischecked) : base()
        {
            Checked = ischecked;
        }
        public bool Checked;
    };

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class GUICheckbox : GUIComponent
    {
        public delegate void CheckChangeEventHandler(GUIComponent sender, CheckStateEventArgs args);
        public event CheckChangeEventHandler CheckStateChange;

        SpriteBatch batch;
        SpriteFont font;

        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;

        public bool Checked;
        private Texture2D checkSprite;
        private Texture2D uncheckSprite;

        private string caption;
        public string Caption
        {
            set
            {
                caption = value;
                fontDimensions = Vector2.Zero;
                if (font == null) return;
                fontDimensions = font.MeasureString(caption);
                Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)(fontDimensions.X + textPaddingSide + 16), (int)(fontDimensions.Y));
                textPosition = new Vector2(Location.X + textPaddingSide + 16, Location.Y);
            }
            get
            {
                return caption;
            }
        }

                
        public GUICheckbox(Game game, Vector2 location, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Caption = caption;
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
                    CheckStateChange = null;
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
            checkSprite = Game.Content.Load<Texture2D>(@"GUI\GUICheckbox_Checked");
            uncheckSprite = Game.Content.Load<Texture2D>(@"GUI\GUICheckbox_Unchecked");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            Vector2 mouseLoc = input.MousePosition;
            if (input.IsMouseButtonClicked("left") && Bounds.Contains(Convert.ToInt32(mouseLoc.X), Convert.ToInt32(mouseLoc.Y)))
            {
                Checked = !Checked;
                if (CheckStateChange != null)
                {
                    CheckStateChange(this, new CheckStateEventArgs(Checked));
                }
            }
            
            if (fontDimensions == Vector2.Zero)
                Caption = Caption;

            base.Update(gameTime);
        }

        public override void UpdateLocation(Vector2 newLocation)
        {
            Location = newLocation;
            Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)(fontDimensions.X + textPaddingSide + 16), (int)(fontDimensions.Y));
            textPosition = new Vector2(Location.X + textPaddingSide + 16, Location.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            if (disposed)
                return;

            batch.Begin();
            if (Checked)
            {
                batch.Draw(checkSprite, Location, Color.White);
            }
            else
            {
                batch.Draw(uncheckSprite, Location, Color.White);
            }
            batch.DrawString(font, caption, textPosition, ForegroundColor);
            batch.End();
        }
    }
}