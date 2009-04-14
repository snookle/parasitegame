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
    /// Superclass for all GUI components.
    /// Only handles methods that are global to all components
    /// Such as whether or not a component has focus.
    /// </summary>
    class GUIComponent : IDisposable
    {
        public GUIComponent Parent;
        public string Name;
        public Rectangle Bounds;
        public Vector2 Location;
        public bool HasFocus;

        public bool Hidden = false;

        protected Camera camera;
        protected InputHandler input;
        protected Game Game;

        public Color ForegroundColor = Color.Black;
        public Color BackgroundColor = Color.LightGray;
        public Color HighlightColor = Color.LightSlateGray;

        protected int textPaddingSide = 5;
        protected int textPaddingTopAndBottom = 2;

        protected bool disposed = false;
        protected GUIManager guimanager;

        public GUIComponent(Game game)
        {
            Game = game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public virtual void Initialize()
        {
            camera = (Camera)Game.Services.GetService(typeof(ICamera));
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandler));
            guimanager = (GUIManager)Game.Services.GetService(typeof(IGUIManager));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual void UpdateLocation(Vector2 newLocation)
        {
            Location = newLocation;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (Bounds != null)
            {
                Vector2 mousePos = input.MousePosition;

                //see if the component has focus or not
                if (input.IsMouseButtonPressed("left")) {
                    if (Bounds.Contains(Convert.ToInt32(mousePos.X), Convert.ToInt32(mousePos.Y))) {
                        //we've been clicked on - assume we now have focus.
                        HasFocus = true;
                    } else {
                        //mouse has been clicked, but not on us - assume we no longer have focus
                        HasFocus = false;
                    }

                }
            }

            
        }

        public virtual void Draw(GameTime gameTime)
        {

        }
    }
}