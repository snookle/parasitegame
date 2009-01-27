using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public interface ISceneCameraComponent { }
    public class SceneCameraComponent : GameComponent, ISceneCameraComponent
    {
        public Rectangle ViewableArea;
        public Vector2 Position;
        private Vector2 finalPosition; //used for smooth scrolling.
        public SceneNode Target;
        float accel = 0.0f;
        float mu = 0.0f;
        private InputHandler input;
        private bool userControlled;
        Vector2 ScreenCentre;

        public void SetFinalPosition(Vector2 pos)
        {
            if (mu < 0.9f)
                mu = 0.0f;
            accel = 0.1f;
            finalPosition = pos;
        }

        public SceneCameraComponent(Game game) : base(game)
        {
            //register self as a game service.
            Game.Services.AddService(typeof(ISceneCameraComponent), this);
            userControlled = false;
        }

        public override void Initialize()
        {
            ScreenCentre.X = Game.GraphicsDevice.Viewport.Width / 2;
            ScreenCentre.Y = Game.GraphicsDevice.Viewport.Height / 2;
            base.Initialize();
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandlerComponent));
        }

        //returns the mouse as a position in the world
        public Vector2 MouseToWorld()
        {
            Vector2 mouseVec = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            return -(ScreenCentre - (mouseVec + Position));// +mouseVec;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (userControlled)
                SetFinalPosition(finalPosition - input.MouseDisplacement());
            else
            {
                //we have a target hopefully
                if (Target != null)
                {
                    Vector2 intermediatePosition = Target.Position - Vector2.SmoothStep(Target.Position, Position, 0.2f); //(Target.Position - Position) * 0.2f;
                    Position += intermediatePosition;
                    
                }
            }

            //dont set a new target position until we've gotten a new one (mu will be < 0.15 then)

           // Position = Target.Position;
        }

        public void SetTarget(SceneNode node)
        {
            userControlled = false;
            
            //we had a previous target
            Target = node;

            if (Position != Target.Position)
            {
                SetFinalPosition(Target.Position);
            }
        }
    }
}
