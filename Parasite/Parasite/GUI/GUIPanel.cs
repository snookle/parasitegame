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
        PrimitiveBatch primBatch;
        Vector2 captionDimensions = Vector2.Zero;
        Vector2 captionPosition;
        public string Caption;
        public Vector2 Dimensions;
        Vector2 titleBarDimensions;

                
        public GUIPanel(Game game, Vector2 location, Vector2 dimensions, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Caption = caption;
            Dimensions = dimensions;
            Bounds = new Rectangle(Convert.ToInt32(location.X), Convert.ToInt32(location.Y), Convert.ToInt32(dimensions.X), Convert.ToInt32(dimensions.Y));
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
            captionPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
            captionDimensions = font.MeasureString(Caption);

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
                //Checked = !Checked;
            }
            
            //if (fontDimensions == Vector2.Zero)
            //    Caption = Caption;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {                         
            batch.Begin();
           // if (Checked)
            {
           //     batch.Draw(checkSprite, Location, Color.White);
            }
          //  else
            {
           //     batch.Draw(uncheckSprite, Location, Color.White);
            }
           // batch.DrawString(font, caption, textPosition, ForegroundColor);
            batch.End();
        }
    }
}