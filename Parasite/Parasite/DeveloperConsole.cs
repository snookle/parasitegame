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
    public interface IDeveloperConsole { };
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class DeveloperConsole : Microsoft.Xna.Framework.DrawableGameComponent, IDeveloperConsole
    {
        private InputHandler input;
        private bool show = false;
        private Queue<String> lines;
        private int maxLines = 15;

        private PrimitiveBatch batch;
        private SpriteFont font;
        private SpriteBatch textBatch;

        private Rectangle bounds;
        private int consoleHeight;
        private float textHeight;
        private int lineLength = 128;

        private string inputString;
        private List<string> history;
        private int historyIndex;

        public delegate void DeveloperConsoleMessageHandler(string command, string argument);
        public event DeveloperConsoleMessageHandler MessageHandler;

        public DeveloperConsole(Game game)
            : base(game)
        {
            show = false;
            game.Services.AddService(typeof(IDeveloperConsole), this);
            lines = new Queue<string>();
            history = new List<string>();
            historyIndex = 0;
        }
        protected override void LoadContent()
        {
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandler));
            batch = new PrimitiveBatch(GraphicsDevice);
            textBatch = new SpriteBatch(GraphicsDevice);

            font = Game.Content.Load<SpriteFont>(@"Fonts\Console");
            textHeight = font.MeasureString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ").Y;
            consoleHeight = Convert.ToInt32(textHeight * maxLines + (maxLines * 2));
            bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 0);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (input.IsKeyPressed(this, Keys.OemTilde))
            {
                show = !show;
               // bounds.Height = 0;
            }

            if (show)
            {
                input.IgnoreAllExcept(this);
                foreach(Keys k in input.GetPressedKeys(this))
                {
                    string key = Convert.ToString(k).ToLower();
                    //uncomment to see all the raw string data.
                    //inputString += key;
                    
                    //handle some stupid special cases :(
                    //handle space.
                    if (key == "space")
                    {
                        inputString += " ";
                        continue;
                    }

                    //handle enter key.
                    if (key == "enter")
                    {
                        lines.Enqueue(inputString);
                        AddToHistory(inputString);
                        string command = "";
                        string argument = "";

                        if (inputString.IndexOf(' ') > 0)
                        {
                            command = inputString.Substring(0, inputString.IndexOf(' '));
                            argument = inputString.Substring(inputString.IndexOf(' ')).Trim(' ', '"');
                        }
                        else
                        {
                            command = inputString.Trim();
                        }
                        if (MessageHandler != null)
                        {
                            MessageHandler(command, argument);
                        }

                        inputString = "";
                        continue;
                    }

                    //handle full stop
                    if (key == "oemperiod")
                    {
                        inputString += ".";
                        continue;
                    }

                    //handle quotes
                    if (key == "oemquotes")
                    {
                        if (input.IsKeyDown(this, Keys.RightShift) || input.IsKeyDown(this, Keys.LeftShift))
                        {
                            inputString += "\"";
                        }
                        else
                        {
                            inputString += "'";
                        }
                        continue;
                    }

                    //handle pipe and \
                    if (key == "oempipe")
                    {
                        if (input.IsKeyDown(this, Keys.RightShift) || input.IsKeyDown(this, Keys.LeftShift))
                        {
                            inputString += "|";
                        }
                        else
                        {
                            inputString += "\\";
                        }
                    }

                    //handle backspace
                    if (key == "back")
                    {
                        if (String.IsNullOrEmpty(inputString))
                            continue;
                       inputString = inputString.Remove(inputString.Length-1);
                    }

                    //handle up arrow (for history)
                    if (key == "up")
                    {
                        if (history.Count <= 0 || historyIndex == history.Count)
                            continue;
                        inputString = history[historyIndex];
                        historyIndex++;
                    }

                    //handle down arrow (for history)
                    if (key == "down")
                    {
                        if (history.Count <= 0 || historyIndex <= 0)
                            continue;
                        historyIndex--;
                        inputString = history[historyIndex];
                    }

                    //handle digits and their associated superscript functions
                    if ("d1d2d3d4d5d6d7d8d9d0".Contains(key))
                    {
                        if (input.IsKeyDown(this, Keys.RightShift) || input.IsKeyDown(this, Keys.LeftShift))
                        {
                            //handle superscript functions
                        }
                        else
                        {
                            inputString += key.Remove(0, 1);
                        }
                    }

                    //if it's not one of our wanted special cases
                    //and it's alpha, then display it.
                    if ("abcdefghijklmnopqrstuvwxyz".Contains(key))
                    {
                        if (input.IsKeyDown(this, Keys.RightShift) || input.IsKeyDown(this, Keys.LeftShift))
                        {
                            inputString += key.ToUpper();
                        }
                        else
                        {
                            inputString += key;
                        }
                    }
                }
                
            }
            else
            {
                input.IgnoreAllExcept(null);
            }
            base.Update(gameTime);
        }

        private void AddToHistory(string str)
        {
            history.Insert(0, str);
            historyIndex = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            //if (!show)
           //     return;
            if (bounds.Height < consoleHeight && show)
                bounds.Inflate(0, 20);

            if (bounds.Height > 0 && !show)
                bounds.Inflate(0, -20);

            Color consoleColor = Color.Gray;
            consoleColor.A = 200;
            batch.Begin(PrimitiveType.TriangleList);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), consoleColor);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), consoleColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), consoleColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), consoleColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), consoleColor);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), consoleColor);
            batch.End();
            textBatch.Begin();
            if (lines.Count > 0)
            {
                string[] strArray = lines.ToArray<String>();
                for (int i = 0; i < lines.Count; i++)
                {
                    textBatch.DrawString(font, strArray[lines.Count-i-1], new Vector2(bounds.Left + 5, bounds.Bottom - textHeight - ((i+1)* textHeight)), Color.White);                   
                }
            }
            textBatch.DrawString(font, "] " + inputString + "_", new Vector2(bounds.Left + 5, bounds.Bottom - textHeight), Color.White);
            textBatch.End();
            

            base.Draw(gameTime);
        }

        public void Write(String str)
        {
            string[] splits = str.Split('\n');
            foreach (string stri in splits)
                lines.Enqueue(stri);
            if (lines.Count > maxLines)
                lines.Dequeue();
        }
    }
}