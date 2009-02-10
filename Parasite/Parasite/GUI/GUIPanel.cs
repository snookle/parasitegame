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
    class OnBoxOk : EventArgs
    {
        public OnBoxOk(string command, string argument)
            : base()
        {
            Command = command;
            Argument = argument;
        }
        public string Command;
        public string Argument;
    };

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
        Rectangle titleBarBounds;
        bool ignoreMouseOutOfBounds = false;

        public bool minimised = false;
        

        List<GUIComponent> components = new List<GUIComponent>();
                
        public GUIPanel(Game game, Vector2 location, Vector2 dimensions, string name, string caption)
            : base(game)
        {
            Location = location;
            Name = name;
            Caption = caption;
            Dimensions = dimensions;
            Bounds = new Rectangle(Convert.ToInt32(location.X), Convert.ToInt32(location.Y), Convert.ToInt32(dimensions.X), Convert.ToInt32(dimensions.Y));
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        foreach (GUIComponent c in components)
                        {
                            c.Dispose();
                        }
                        components.Clear();
                    }

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
            captionPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
            captionDimensions = font.MeasureString(Caption);
            titleBarBounds = new Rectangle(Convert.ToInt32(Bounds.X), Convert.ToInt32(Bounds.Y), Convert.ToInt32(Bounds.Width), Convert.ToInt32(captionDimensions.Y + (2 * textPaddingTopAndBottom)));
        }

        public void AddComponent(GUIComponent c)
        {
            c.UpdateLocation(c.Location + new Vector2(Location.X, Location.Y + titleBarBounds.Height));
            components.Add(c);
        }

            public GUIComponent getComponent(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            foreach (GUIComponent g in components)
            {
                if (g.Name.ToLower() == name.ToLower())
                {
                    return g;
                }
            }
            return null;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            Vector2 mouseLoc = input.MousePosition;
            if (input.IsMouseButtonPressed("left") && (titleBarBounds.Contains(Convert.ToInt32(mouseLoc.X), Convert.ToInt32(mouseLoc.Y)) || ignoreMouseOutOfBounds))
            {
                ignoreMouseOutOfBounds = true;
                UpdateLocation(input.GetMouseDisplacement());
            }
            else if (input.IsMouseButtonClicked("right") && (titleBarBounds.Contains(Convert.ToInt32(mouseLoc.X), Convert.ToInt32(mouseLoc.Y)) || ignoreMouseOutOfBounds))
            {
                minimised = !minimised;
            } else 
            {
                ignoreMouseOutOfBounds = false;
            }
            
            //if (fontDimensions == Vector2.Zero)
            //    Caption = Caption;
            for (int i = 0; i < components.Count; i++)
            {
                GUIComponent c = components[i];
                if (c != null)
                    c.Update(gameTime);
                c = null;
                if (disposed) return;
            }
            base.Update(gameTime);
        }

        public override void UpdateLocation(Vector2 delta)
        {
            Location = Location - delta;
            Bounds = new Rectangle(Convert.ToInt32(Location.X), Convert.ToInt32(Location.Y), Convert.ToInt32(Dimensions.X), Convert.ToInt32(Dimensions.Y));
            captionPosition = new Vector2(Location.X + textPaddingSide, Location.Y + textPaddingTopAndBottom);
            captionDimensions = font.MeasureString(Caption);
            titleBarBounds = new Rectangle(Convert.ToInt32(Bounds.X), Convert.ToInt32(Bounds.Y), Convert.ToInt32(Bounds.Width), Convert.ToInt32(captionDimensions.Y + (2 * textPaddingTopAndBottom)));
            foreach (GUIComponent c in components)
            {
                c.UpdateLocation(c.Location - delta);
            }
        }

        public override void Draw(GameTime gameTime)
        {

            primBatch.Begin(PrimitiveType.TriangleList);
            if (!minimised)
            {
                //black outline
                primBatch.AddVertex(new Vector2(Bounds.Left - 1, Bounds.Bottom + 1), Color.Black);
                primBatch.AddVertex(new Vector2(Bounds.Left - 1, Bounds.Top - 1), Color.Black);
                primBatch.AddVertex(new Vector2(Bounds.Right + 1, Bounds.Top - 1), Color.Black);
                primBatch.AddVertex(new Vector2(Bounds.Right + 1, Bounds.Top - 1), Color.Black);
                primBatch.AddVertex(new Vector2(Bounds.Right + 1, Bounds.Bottom + 1), Color.Black);
                primBatch.AddVertex(new Vector2(Bounds.Left - 1, Bounds.Bottom + 1), Color.Black);
            }

            //title bar
            primBatch.AddVertex(new Vector2(titleBarBounds.Left, titleBarBounds.Bottom), Color.Gray);
            primBatch.AddVertex(new Vector2(titleBarBounds.Left, titleBarBounds.Top), Color.Gray);
            primBatch.AddVertex(new Vector2(titleBarBounds.Right, titleBarBounds.Top), Color.SlateGray);
            primBatch.AddVertex(new Vector2(titleBarBounds.Right, titleBarBounds.Top), Color.SlateGray);
            primBatch.AddVertex(new Vector2(titleBarBounds.Right, titleBarBounds.Bottom), Color.SlateGray);
            primBatch.AddVertex(new Vector2(titleBarBounds.Left, titleBarBounds.Bottom), Color.Gray);

            if (!minimised)
            {
                //panel body
                primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
                primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Top + titleBarBounds.Height), BackgroundColor);
                primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top + titleBarBounds.Height), BackgroundColor);
                primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Top + titleBarBounds.Height), BackgroundColor);
                primBatch.AddVertex(new Vector2(Bounds.Right, Bounds.Bottom), BackgroundColor);
                primBatch.AddVertex(new Vector2(Bounds.Left, Bounds.Bottom), BackgroundColor);
            }
            
            primBatch.End();
         
            
            batch.Begin();
            batch.DrawString(font, Caption, captionPosition, Color.White);
            batch.End();

            if (!minimised)
            {
                foreach (GUIComponent c in components)
                {
                    c.Draw(gameTime);
                }
            }
        }
    }
}