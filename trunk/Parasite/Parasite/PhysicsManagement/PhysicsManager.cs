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
using FarseerGames.FarseerPhysics;

namespace Parasite
{
    public interface IPhysicsManager { }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PhysicsManager : Microsoft.Xna.Framework.DrawableGameComponent, IPhysicsManager
    {
        InputHandler input;
        DeveloperConsole console;
        Camera camera;
        GUIManager guimanager;

        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (paused)
                {
                    //pause it
                    if (Simulator != null)
                        Simulator.Enabled = false;
                    
                    if (statusLabel != null)
                        statusLabel.Text = "Status: Paused";
                }
                else
                {
                    //unpause it
                    if (Simulator != null)
                        Simulator.Enabled = true;
                    
                    if (statusLabel != null)
                        statusLabel.Text = "Status: Running";
                }
            }
        }

        bool debugDraw = true;              // Debug draw should be on by default in edit mode
        private bool paused;

        float timeStep = 5.0f / 60f;

        float pausedStep = 0f;
        float defaultStep = 2.0f / 60f;

        int velocityIterations = 3;
        int positionIterations = 3;
        public PhysicsSimulator Simulator;
        FarseerGames.FarseerPhysics.PhysicsSimulatorView debugView;
        SpriteBatch debugBatch;
        GUIPhysicsControls controlPanel;
        GUILabel statusLabel;
        GUICheckbox cbDebugDraw;


        public PhysicsManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPhysicsManager), this);
        }

        void ConsoleMessageHandler(string command, string argument)
        {
            if (command == "phys_debug_draw")
            {
                if (string.IsNullOrEmpty(argument))
                {
                    console.Write("phys_debug_draw is " + (debugDraw ? "1" : "0"));
                }
                else
                {
                    try
                    {
                        int arg = Convert.ToInt32(argument);
                        if (arg == 0)
                            debugDraw = false;
                        else
                            debugDraw = true;
                    }
                    catch (Exception)
                    {
                        console.Write(argument + " is not a valid argument. Use 0/1.", ConsoleMessageType.Error);
                    }
                }
                console.CommandHandled = true;
            }
            else if (command == "phys_pause")
            {
                if (string.IsNullOrEmpty(argument))
                {
                    console.Write("phys_pause is " + (paused ? "1" : "0"));
                }
                else
                {
                    try
                    {
                        int arg = Convert.ToInt32(argument);
                        if (arg == 0)
                            Paused = false;
                        else
                            Paused = true;
                    }
                    catch (Exception)
                    {
                        console.Write(argument + " is not a valid argument. Use 0/1", ConsoleMessageType.Error);
                    }
                }
                console.CommandHandled = true;
            }
        }

        protected override void LoadContent()
        {
            debugBatch = new SpriteBatch(GraphicsDevice);
            debugView.LoadContent(GraphicsDevice, Game.Content);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandler));
            console = (DeveloperConsole)Game.Services.GetService(typeof(IDeveloperConsole));
            console.MessageHandler += new DeveloperConsole.DeveloperConsoleMessageHandler(ConsoleMessageHandler);
            camera = (Camera)Game.Services.GetService(typeof(ICamera));
            guimanager = (GUIManager)Game.Services.GetService(typeof(IGUIManager));
            
            // Define the World
            Simulator = new PhysicsSimulator(new Vector2(0, 8));
            debugView = new PhysicsSimulatorView(Simulator);

            base.Initialize();

            controlPanel = new GUIPhysicsControls(Game);
            controlPanel.Initialize();
            guimanager.AddComponent(controlPanel);
            GUIButton btn = (GUIButton)controlPanel.getComponent("pause");
            btn.OnMouseClick += new GUIButton.MouseClickHandler(PauseButtonMouseClick);

            btn = (GUIButton)controlPanel.getComponent("play");
            btn.OnMouseClick += new GUIButton.MouseClickHandler(PlayButtonMouseClick);

            statusLabel = (GUILabel)controlPanel.getComponent("status");
            //start off paused.
            Paused = true;

            GUICheckbox cb = (GUICheckbox)controlPanel.getComponent("debugdraw");
            cb.CheckStateChange += new GUICheckbox.CheckChangeEventHandler(DebugDrawCheckStateChanged);
            cb.Checked = debugDraw;
        }

        void DebugDrawCheckStateChanged(GUIComponent sender, CheckStateEventArgs args)
        {
            debugDraw = args.Checked;
        }

        void PlayButtonMouseClick(GUIComponent sender, OnMouseClickEventArgs args)
        {
            Paused = false;
        }

        void PauseButtonMouseClick(GUIComponent sender, OnMouseClickEventArgs args)
        {
            Paused = true;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            Simulator.Update(gameTime.ElapsedGameTime.Milliseconds * 0.005f);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (debugDraw)
            {
                debugBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState, Matrix.CreateTranslation(new Vector3((camera.ScreenCentre - camera.Position) * camera.ZoomLevel, 0)));
                debugView.Draw(debugBatch);
                debugBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
