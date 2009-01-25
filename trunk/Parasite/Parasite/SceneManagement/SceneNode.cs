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
        /// <summary>
        /// The position of the node in the world.
        /// </summary>
        public Vector2 WorldPosition;

        /// <summary>
        /// Whether or not this scene node is currently being followed by the Camera
        /// </summary>
        public bool IsCameraTarget;
        
        /// <summary>
        /// Depth on the screen. 0.0 from right in front of the camera,
        /// 1.0 for static background
        /// </summary>
        public float ScreenDepth;

        protected Camera camera;
        protected Game game;

        private Vector2 screenCentre;

        public SceneNode(Game game, Vector2 startingPosition)
        {
            Initialise(game, startingPosition);
        }

        public SceneNode(Game game)
        {
            Initialise(game, new Vector2(0,0));
        }

        private void Initialise(Game game, Vector2 startingPosition)
        {
            this.camera = (Camera)game.Services.GetService(typeof(ICamera));
            this.game = game;
            screenCentre.X = game.GraphicsDevice.Viewport.Width / 2;
            screenCentre.Y = game.GraphicsDevice.Viewport.Height / 2;
            ScreenDepth = 0.0f;
            WorldPosition = startingPosition;
        }

        /// <summary>
        /// Calculates where on the screen this scene node should be displayed relative to the camera.
        /// </summary>
        /// <returns>Vector2 representing the screen coords where this scenenode should be drawn.</returns>
        public Vector2 GetScreenPosition()
        {
            return (screenCentre - (camera.Position - WorldPosition)) * (1-ScreenDepth);
        }

    }
}
