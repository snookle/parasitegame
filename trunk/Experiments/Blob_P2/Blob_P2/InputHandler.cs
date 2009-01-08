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


namespace Blob_P2
{
    public interface IInputHandlerComponent { }
    public class InputHandler : GameComponent, IInputHandlerComponent
    {
        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;

        private MouseState lastMouseState;
        private MouseState currentMouseState;
        private Vector2 screenCentre;

        public InputHandler(Game game)
            : base(game)
        {
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            game.Services.AddService(typeof(IInputHandlerComponent), this);


        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            screenCentre = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            Mouse.SetPosition(Convert.ToInt32(screenCentre.X), Convert.ToInt32(screenCentre.Y));

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            base.Update(gameTime);
        }
        public bool MouseMoving()
        {
            Vector2 lastVec = new Vector2(lastMouseState.X, lastMouseState.Y);
            Vector2 currVec = new Vector2(currentMouseState.X, currentMouseState.Y);
            return (lastVec != currVec);
        }

        public Vector2 MouseDisplacement()
        {
            Vector2 returnVec = new Vector2(lastMouseState.X - currentMouseState.X, lastMouseState.Y - currentMouseState.Y);

            //if (!MouseMoving())
              //  Mouse.SetPosition(Convert.ToInt32(screenCentre.X), Convert.ToInt32(screenCentre.Y));

            return returnVec;
        }

        public bool IsKeyPressed(Keys key)
        {
            
            return (currentKeyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key));
        }

        public bool IsKeyDown(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key));
        }

        public bool IsKeyUp(Keys key)
        {
            return (currentKeyboardState.IsKeyUp(key));
        }
    }
}