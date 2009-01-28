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
using System.Reflection;


namespace Parasite
{
    public interface IInputHandler{ }

    /// <summary>
    /// InputHandler
    /// Inherits from: GameComponent, IInputHandler
    /// This component handles and pre-processes the players user input.
    /// 
    /// In the future it will handle key bindings.
    /// </summary>
    public class InputHandler : GameComponent, IInputHandler
    {
        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;

        private MouseState lastMouseState;
        private MouseState currentMouseState;
        private Vector2 screenCentre;

        public Vector2 MousePosition;

        private DeveloperConsole console;
        private int lastScrollWheelValue;

        public InputHandler(Game game) : base(game)
        {
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
            lastScrollWheelValue = currentMouseState.ScrollWheelValue;

            game.Services.AddService(typeof(IInputHandler), this);

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

            console = (DeveloperConsole)Game.Services.GetService(typeof(IDeveloperConsole));
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

            MousePosition.X = currentMouseState.X;
            MousePosition.Y = currentMouseState.Y;

            GetScrollWheelAmount();

            base.Update(gameTime);
        }

        public bool IsMouseMoving()
        {
            Vector2 lastVec = new Vector2(lastMouseState.X, lastMouseState.Y);
            Vector2 currVec = new Vector2(currentMouseState.X, currentMouseState.Y);
            return (lastVec != currVec);
        }

        public Vector2 GetMouseDisplacement()
        {
            Vector2 returnVec = new Vector2(lastMouseState.X - currentMouseState.X, lastMouseState.Y - currentMouseState.Y);
            return returnVec;
        }

        public bool IsMouseButtonPressed(string mouseButton)
        {
            switch (mouseButton.ToLower())
            {
                case "left": return currentMouseState.LeftButton == ButtonState.Pressed;
                case "right": return currentMouseState.RightButton == ButtonState.Pressed;
                case "middle": return currentMouseState.MiddleButton == ButtonState.Pressed;
                default: return false;
            }
        }

        public bool IsKeyPressed(Object callee, Keys key)
        {
            if (ignoreException != null)
            {
                if (callee != ignoreException)
                    return false;
            }
            return (currentKeyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key));
        }

        public bool IsKeyDown(Object callee, Keys key)
        {
            if (ignoreException != null)
            {
                if (callee != ignoreException)
                    return false;
            }
            return (currentKeyboardState.IsKeyDown(key));
        }

        public bool IsKeyUp(Object callee, Keys key)
        {
            if (ignoreException != null)
            {
                if (callee != ignoreException)
                    return false;
            }
            return (currentKeyboardState.IsKeyUp(key));
        }
        private Object ignoreException = null;
        public void IgnoreAllExcept(Object obj)
        {
            ignoreException = obj;
        }
        
        public Keys[] GetPressedKeys(Object callee)
        {
            Keys[] newKeys = currentKeyboardState.GetPressedKeys();
            Keys[] oldKeys = lastKeyboardState.GetPressedKeys();
            List<Keys> returnKeys = new List<Keys>();
            foreach (Keys k in newKeys)
            {
                if (oldKeys.Contains<Keys>(k))
                    continue;
                returnKeys.Add(k);
            }
            return returnKeys.ToArray();
        }

        public int GetScrollWheelAmount()
        {
            if ((currentMouseState.ScrollWheelValue - lastScrollWheelValue) != 0)
            {
                int amount = currentMouseState.ScrollWheelValue - lastScrollWheelValue;
                lastScrollWheelValue = currentMouseState.ScrollWheelValue;
                return amount;
            }
            return 0;
        }
    }
}