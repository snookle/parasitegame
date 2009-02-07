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
    public enum ConsoleMessageType { Info, Warning, Error };
    public interface IDeveloperConsole { };
    public class DeveloperConsoleMessage
    {
        public string Message;
        public ConsoleMessageType MessageType;
        public DeveloperConsoleMessage(string message, ConsoleMessageType type)
        {
            Message = message;
            MessageType = type;
        }
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class DeveloperConsole : Microsoft.Xna.Framework.DrawableGameComponent, IDeveloperConsole
    {
        private InputHandler input;
        private bool show = false;
        private Queue<DeveloperConsoleMessage> lines;
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

        public bool CommandHandled = false;

        private Dictionary<ConsoleMessageType, Color> MessageColors;

        public DeveloperConsole(Game game)
            : base(game)
        {
            show = false;
            game.Services.AddService(typeof(IDeveloperConsole), this);
            lines = new Queue<DeveloperConsoleMessage>();
            history = new List<string>();
            historyIndex = 0;

            MessageColors = new Dictionary<ConsoleMessageType, Color>();
            MessageColors.Add(ConsoleMessageType.Info, Color.White);
            MessageColors.Add(ConsoleMessageType.Error, Color.Red);
            MessageColors.Add(ConsoleMessageType.Warning, Color.Yellow);

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
                return;
            }

            if (show)
            {
                input.IgnoreAllExcept(this);
                foreach(String key in input.GetPressedKeysAsStrings(this))
                {
                    //handle enter key.
                    if (key == "enter")
                    {
                        lines.Enqueue(new DeveloperConsoleMessage(inputString, ConsoleMessageType.Info));
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
                            CommandHandled = false;
                            MessageHandler(command, argument);
                            if (!CommandHandled)
                            {
                                Write("Unknown command: " + inputString, ConsoleMessageType.Error);
                            }
                        }

                        inputString = "";
                        continue;
                    }
                    
                    //handle backspace
                    if (key == "back")
                    {
                        if (String.IsNullOrEmpty(inputString))
                            continue;
                        inputString = inputString.Remove(inputString.Length-1);
                        continue;
                    }

                    //handle up arrow (for history)
                    if (key == "up")
                    {
                        if (history.Count <= 0 || historyIndex == history.Count)
                            continue;
                        inputString = history[historyIndex];
                        historyIndex++;
                        continue;
                    }

                    //handle down arrow (for history)
                    if (key == "down")
                    {
                        if (history.Count <= 0 || historyIndex <= 0)
                            continue;
                        historyIndex--;
                        inputString = history[historyIndex];
                        continue;
                    }

                    inputString += key;
                }

                
            }
            else
            {
                input.ClearIgnore(this);
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

            int lineLength = 50;

            if (lines.Count > 0)
            {
                DeveloperConsoleMessage[] msgArray = lines.ToArray<DeveloperConsoleMessage>();
                for (int i = 0; i < lines.Count; i++)
                {
                    // Attempt at word wrapping...
                    /*if (strArray[lines.Count - i - 1].Length > lineLength)
                    {
                        int lineBreaks = (int)(strArray[lines.Count - i - 1].Length / lineLength);
                        string TempString;
                        for (int n = 0; n < lineBreaks; n++)
                        {
                            TempString = strArray[lines.Count - i - 1].Substring(lineLength * n, lineLength);
                            textBatch.DrawString(font, TempString + '\n', new Vector2(bounds.Left + 5, bounds.Bottom - textHeight - ((n + i + 2) * textHeight)), Color.White);
                        }
                    }
                    else
                    {*/
                        DeveloperConsoleMessage msg = msgArray[lines.Count - i - 1];
                        textBatch.DrawString(font, msg.Message, new Vector2(bounds.Left + 5, bounds.Bottom - textHeight - ((i + 1) * textHeight)), MessageColors[msg.MessageType]);
                    //}
                }
            }
            textBatch.DrawString(font, "] " + inputString + "_", new Vector2(bounds.Left + 5, bounds.Bottom - textHeight), Color.White);
            textBatch.End();
            

            base.Draw(gameTime);
        }

        public void Write(String str)
        {
            Write(str, ConsoleMessageType.Info);
        }

        public void Write(String str, ConsoleMessageType type)
        {
            string[] splits = str.Split('\n');
            foreach (string stri in splits)
                lines.Enqueue(new DeveloperConsoleMessage(stri, type));
            if (lines.Count > maxLines)
                lines.Dequeue();
        }

    }
}