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

namespace Parasite
{
    public interface ICamera { }
    public class Camera : GameComponent, ICamera
    {
        //Viewable area will be a rectangle representing what the camera can see
        //It will be used for occlusion culling when drawing the level.
        public Rectangle ViewableArea;

        //The current position of the camera
        public Vector2 Position;

        public float ZoomLevel = 1f;

        //The current target that our camera is following.
        public SceneNode Target;

        //controls whether or not this camera is being controlled by the user.
        private bool userControlled;

        private InputHandler input;
        private Vector2 ScreenCentre;

        public Camera(Game game) : base(game)
        {
            //register self as a game service.
            Game.Services.AddService(typeof(ICamera), this);
            userControlled = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            ScreenCentre.X = Game.GraphicsDevice.Viewport.Width / 2;
            ScreenCentre.Y = Game.GraphicsDevice.Viewport.Height / 2;
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandler));
        }

        //returns the mouse as a position in the world
        public Vector2 MouseToWorld()
        {
            return ScreenToWorld(input.MousePosition);
        }

        //converts a screen position into its world position
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return -(ScreenCentre - (screenPosition + Position));// +mouseVec;
        }

        /// <summary>
        /// Converts a world coordinate to a position on the screen. 
        /// </summary>
        /// <param name="worldPosition">A position in the world</param>
        /// <returns>The position on the screen that something at the world position would appear at.</returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return (ScreenCentre - (Position - worldPosition));
        }
        /// <summary>
        /// Decides if any part of a bounding box is visible on the screen
        /// </summary>
        /// <param name="box">The box to check for screen visibility.</param>
        /// <returns>True if any part of the box is visible on the screen.</returns>
        public bool IsVisible(BoundingBox box)
        {
            throw new NotImplementedException("IsVisible is not implemented yet");
            return false;
        }

        //Updates the camera position.
        //Either from the target or the user.
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (userControlled)
            {
                // Mouse Control
                // Position = Position - input.GetMouseDisplacement();

                // Keyboard Control
                if (input.IsKeyDown(this, Keys.Right))
                {
                    Position = Position + new Vector2(5, 0);
                }
                else if (input.IsKeyDown(this, Keys.Left))
                {
                    Position = Position - new Vector2(5, 0);
                }

                if (input.IsKeyDown(this, Keys.Up))
                {
                    Position = Position - new Vector2(0, 5);
                }
                else if (input.IsKeyDown(this, Keys.Down))
                {
                    Position = Position + new Vector2(0, 5);
                }

                // Attempt at zoom
                if (input.IsKeyDown(this, Keys.Add))
                {
                    if (ZoomLevel < 10)
                    {
                        ZoomLevel += 0.25f;
                    }
                }
                else if (input.IsKeyDown(this, Keys.Subtract))
                {
                    if(ZoomLevel > 0.25f){
                        ZoomLevel -= 0.25f;
                    }
                }
            }
            else
            {
                if (Target != null)
                {
                    Vector2 intermediatePosition = (Target.WorldPosition - Position) * 0.2f;
                    Position += intermediatePosition;

                }
            }
        }

        //Sets the target for this camera to follow.
        public void SetTarget(SceneNode node)
        {
            if (Target != null)
            {
                Target.IsCameraTarget = false;
            }
            userControlled = false;          
            Target = node;
            Target.IsCameraTarget = true;
        }

        //Clears the current target and returns control of the camera to the user.
        public void ClearTarget()
        {
            userControlled = true;
            Target.IsCameraTarget = false;
            Target = null;
        }
    }
}
