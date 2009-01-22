using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Parasite
{
    /// <summary>
    /// This is a base class for all game objects that will be drawn on the screen.
    /// Contains methods that decide where on the screen the node should be drawn based
    /// on the camera position.
    /// </summary>
    public class SceneNode
    {
        //The position of the node in the world.
        public Vector2 WorldPosition;
        public bool IsCameraTarget;
        public float ScreenDepth;
        
        //The current camera that is displaying this node.
        protected Camera camera;
        protected Game game;

        private Vector2 screenCentre;

        public SceneNode(Game game, Vector2 startingPosition)
        {
            this.camera = (Camera)game.Services.GetService(typeof(ICamera));
            this.game = game;
            screenCentre.X = game.GraphicsDevice.Viewport.Width / 2;
            screenCentre.Y = game.GraphicsDevice.Viewport.Height / 2;
            ScreenDepth = 0.0f;
            WorldPosition = startingPosition;
        }

        public Vector2 GetScreenPosition()
        {
            return (screenCentre - (camera.Position - WorldPosition)) * (1-ScreenDepth);
        }

    }
}
