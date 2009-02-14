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
using Box2DX;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace Parasite
{
    public interface IPhysicsManager { }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PhysicsManager : Microsoft.Xna.Framework.DrawableGameComponent, IPhysicsManager
    {
        private World world;
        Body groundBody;
        InputHandler input;
        DeveloperConsole console;

        bool debugDraw = true;              // Debug draw should be on by default in edit mode
        private bool simulate = true;       // Simulate on by default
        private bool paused;

        float timeStep = 2.0f / 60f;

        float pausedStep = 0f;
        float defaultStep = 2.0f / 60f;

        int velocityIterations = 3;
        int positionIterations = 3;

        public PhysicsManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPhysicsManager), this);
        }

        public bool Paused
        {
            set
            {
                paused = value;
                if (paused)
                {
                    timeStep = pausedStep;
                }
                else
                {
                    timeStep = defaultStep;
                }
            }
            get
            {
                return paused;
            }
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
            base.LoadContent();
            world.SetDebugDraw(new PhysicsDebugDraw(Game));
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

            // Define the World
            AABB worldAABB;

            // Define the Upper, and Lower bounds
            worldAABB.UpperBound = new Vec2(1024 / 2, 768 / 2);
            worldAABB.LowerBound = new Vec2((1024 / 2) * -1, (768 / 2) * -1);

            // Define Gravity
            Vec2 gravity = new Vec2(0f, 10f);

            world = new World(worldAABB, gravity, true);

            // The Ground
            //BodyDef groundDef = new BodyDef();
            //groundDef.Position.Set(0, 100);
            //groundBody = world.CreateBody(groundDef);

            // PAUSE
            Paused = true;

            // Ground Polygon Definition
            //PolygonDef shapeDef = new PolygonDef();
            //shapeDef.SetAsBox(100, 5);
            //groundBody.CreateShape(shapeDef);
            base.Initialize();
        }

        public void AddStaticCollisionMesh(Vector2[] vs)
        {
            if (vs.Length > 4)
            {
                throw new Exception("Collision meshes must not have more than 4 verticies.");
            }
            PolygonDef sp = new PolygonDef();
            sp.VertexCount = 4;
            sp.Type = ShapeType.PolygonShape;
            for(int i = 0; i < vs.Length; i++)
            {
                Vector2 vec = vs[i];
                sp.Vertices[i].Set(vec);
            }
            groundBody.CreateShape(sp);

        }

        public Body CreateBody(BodyDef def)
        {
            return world.CreateBody(def);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //gameTime.ElapsedGameTime.TotalMilliseconds
            //simulate the world.

            if(input.IsKeyPressed(this,Keys.Enter)){
                simulate = !simulate;
                if (!simulate)
                {
                    // reset all blocks to starting position
                }
            }

            if (simulate && !debugDraw)
            {
                world.Step(timeStep, velocityIterations, positionIterations);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (simulate && debugDraw)
            {
                world.Step(timeStep, velocityIterations, positionIterations);
            }
            base.Draw(gameTime);
        }
    }
}