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
    /// A panel is a container of GUI components. Acts sort of like a moveable window.
    /// </summary>
    class GUIPanel : GUIComponent
    {
        SpriteBatch batch;
        SpriteFont font;
        private int textPaddingSide = 5;
        Vector2 fontDimensions = Vector2.Zero;
        Vector2 textPosition;
        public Color ForegroundColor = Color.Black;
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

                
        public GUIPanel(Game game, Vector2 location, string name, string caption)
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
            }
            
            if (fontDimensions == Vector2.Zero)
                Caption = Caption;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {                         
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