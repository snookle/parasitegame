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

        private Object ignoreException = null;

        private char[] numberSuperScript = {')', '!', '@', '#', '$', '%', '^', '&' , '*', '(' };

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

        /// <summary>
        /// Returns whether or not the mouse has moved since the last update.
        /// </summary>
        /// <returns></returns>
        public bool IsMouseMoving()
        {
            Vector2 lastVec = new Vector2(lastMouseState.X, lastMouseState.Y);
            Vector2 currVec = new Vector2(currentMouseState.X, currentMouseState.Y);
            return (lastVec != currVec);
        }

        /// <summary>
        /// Gets the amount in pixels that the mouse has moved since the last update
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMouseDisplacement()
        {
            Vector2 returnVec = new Vector2(lastMouseState.X - currentMouseState.X, lastMouseState.Y - currentMouseState.Y);
            return returnVec;
        }

        /// <summary>
        /// Returns whether or not a mouse button is held down 
        /// </summary>
        /// <param name="mouseButton">String representing the mouse button to query</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns whether a mouse button has been clicked.
        /// This differs from IsMouseButtonPressed as it will only return true on the first check if the mouse button is held down.
        /// </summary>
        /// <param name="mouseButton">String representing which mouse button to query</param>
        /// <returns></returns>
        public bool IsMouseButtonClicked(string mouseButton)
        {
            switch (mouseButton.ToLower())
            {
                case "left": return currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
                case "right": return currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;
                case "middle": return currentMouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released;
                default: return false;
            }
        }

        /// <summary>
        /// Returns true if the key has been pressed and then released.
        /// </summary>
        /// <param name="callee">Object that is calling this function</param>
        /// <param name="key">Key to query</param>
        /// <returns></returns>
        public bool IsKeyPressed(Object callee, Keys key)
        {
            if (ignoreException != null)
            {
                if (callee != ignoreException)
                    return false;
            }
            return (currentKeyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key));
        }

        /// <summary>
        /// Returns whether or not a key is down. Will continue retuning true till that key is released.
        /// </summary>
        /// <param name="callee">Object that is calling this function</param>
        /// <param name="key">Key to query</param>
        /// <returns>True if the button is being held down.</returns>
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

        
        /// <summary>
        /// Supresses keystrokes to all objects except the one specified here
        /// This is used by the console to stop anything else listening for keystrokes
        /// while the console is down.
        /// </summary>
        /// <param name="obj">Object to allow keystrokes to.</param>
        public void IgnoreAllExcept(Object obj)
        {
            ignoreException = obj;
        }

        public void ClearIgnore(object obj)
        {
            if (ignoreException == obj)
                ignoreException = null;
        }
        
        /// <summary>
        /// Get all the keys that have been pressed (but not held down)
        /// </summary>
        /// <param name="callee">Object that is calling this function.</param>
        /// <returns></returns>
        public Keys[] GetPressedKeys(Object callee)
        {
            if (ignoreException != null)
                if (ignoreException != callee)
                    return new Keys[0];

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

        /// <summary>
        /// Returns all the currently pressed keys as a string.
        /// </summary>
        /// <param name="callee"></param>
        /// <returns></returns>
        public string[] GetPressedKeysAsStrings(object callee)
        {
            if (callee != null)
                if (callee != ignoreException)
                    return new string[0];
            return ProcessKeyPresses(GetPressedKeys(callee));
        }

        /// <summary>
        /// Returns the string equivalent of the keys given in "keys".
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string[] ProcessKeyPresses(Keys[] keys)
        {
            string[] strings = new string[keys.Length];
            int i = 0;
            foreach (Keys k in keys)
            {
                string key = Convert.ToString(k).ToLower();
                strings[i] = "";
                //handle some stupid special cases :(
                //handle space.
                if (key == "space")
                {
                    strings[i++] = " ";
                    continue;
                }
                
                if (key == "oemperiod")
                {
                    if (IsShiftDown())
                    {
                        strings[i++] = ">";
                    }
                    else
                    {
                        strings[i++] = ".";
                    }
                    continue;
                }

                if (key == "oemquotes")
                {
                    if (IsShiftDown())
                    {
                        strings[i++] = "\"";
                    }
                    else
                    {
                        strings[i++] = "'";
                    }
                    continue;
                }

                if (key == "oempipe")
                {
                    if (IsShiftDown())
                    {
                        strings[i++] = "|";
                    }
                    else
                    {
                        strings[i++] = "\\";
                    }
                    continue;
                }

                if (key == "oemminus")
                {
                    if (IsShiftDown())
                    {
                        strings[i++] = "_";
                    }
                    else
                    {
                        strings[i++] = "-";
                    }
                    continue;
                }

                if (key == "oemcomma")
                {
                   strings[i++] = ",";
                   continue;
                }

                if ("abcdefghijklmnopqrstuvwxyz".Contains(key))
                {
                    if (IsShiftDown())
                    {
                        strings[i++] = key.ToUpper();
                    }
                    else
                    {
                        strings[i++] = key;
                    }
                    continue;
                }

                if ("d1d2d3d4d5d6d7d8d9d0".Contains(key))
                {
                    string num = key.Remove(0, 1);
                    if (IsShiftDown())
                    {
                        strings[i++] = numberSuperScript[Convert.ToInt32(num)].ToString();

                    }
                    else
                    {
                        strings[i++] = num;
                    }
                    continue;
                }

                //any cases that can't be turned into chars
                //like enter, arrow keys, ctrl, alt etc
                //just have their code appended to the array
                if (!key.Contains("shift"))
                   strings[i++] = key;

            }
            return strings;
        }

        /// <summary>
        /// Get the amount that the scroll wheel has changed since the last update.
        /// </summary>
        /// <returns></returns>
        public int GetScrollWheelAmount()
        {
            /*if ((currentMouseState.ScrollWheelValue - lastScrollWheelValue) != 0)
            {
                int amount = currentMouseState.ScrollWheelValue - lastScrollWheelValue;
                lastScrollWheelValue = currentMouseState.ScrollWheelValue;
                return amount;
            }
            return 0;*/

            return currentMouseState.ScrollWheelValue;
        }

        public bool IsShiftDown()
        {
            return (currentKeyboardState.IsKeyDown(Keys.LeftShift) || currentKeyboardState.IsKeyDown(Keys.RightShift));
        }
    }
}