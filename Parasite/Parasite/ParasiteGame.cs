using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// This is the main type for your game
    /// </summary>
    public class ParasiteGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        DeveloperConsole console;

        public ParasiteGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            

            graphics.ApplyChanges();
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            // TODO: Add your initialization logic here
            Components.Add(new GUIManager(this));
            Components.Add(new Camera(this));
            Components.Add(new InputHandler(this));
            Components.Add(new PhysicsManager(this));
            Components.Add(new Level(this, "level1"));

            console = new DeveloperConsole(this);
            Components.Add(console);

            // Gosh I'm tricksy
            console.DrawOrder = 10000;
            
            console.MessageHandler += new DeveloperConsole.DeveloperConsoleMessageHandler(ConsoleMessageHandler);

            // attempt at AA : Taken from http://www.xnasociety.com/articles/4
            graphics.PreferMultiSampling = true;
            graphics.PreparingDeviceSettings += DeviceSettings;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// Configures the device to use the highest sample type available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // Gets the values of the MultiSampleType enum and stores them in an array
            MultiSampleType[] types = GetEnumValues<MultiSampleType>();

            // Specifies which graphics adapter to create the device on
            GraphicsAdapter adapter = e.GraphicsDeviceInformation.Adapter;

            // Loops backwards starting at the highest sampletype, and continues until it reaches the lowest
            for (int i = types.Length - 1; i >= 0; i--)
            {
                if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, adapter.CurrentDisplayMode.Format, false, types[i]))
                {
                    // If the sampletype is available, use it, and end the loop
                    e.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = 0;
                    e.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = types[i];
                    break;
                }
            }
        }

        /// <summary>
        /// Returns the values of an enum into an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T[] GetEnumValues<T>()
        {
            if(!typeof(T).IsEnum)
                return null;

            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
            T[]         values = new T[fields.Length];

            for(int i = 0; i<fields.Length;i++){
                values[i] = (T)fields[i].GetValue(null);
            }

            return values;
        }

        void ConsoleMessageHandler(string command, string argument)
        {
            if (command == "exit" || command == "quit")
            {
                console.CommandHandled = true;
                this.Exit();
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.FourSamples;
            GraphicsDevice.RenderState.MultiSampleAntiAlias = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
