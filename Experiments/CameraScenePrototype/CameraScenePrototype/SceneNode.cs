using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CameraScenePrototype
{
    class SceneNode
    {
        public Vector2 Position;
        public Texture2D Texture;
        public bool IsCameraTarget;

        protected string textureName;

        protected SceneCameraComponent camera;
        protected Game game;

        Vector2 ScreenCentre;

        public SceneNode(Game game, string TextureName, Vector2 startingPosition)
        {
            this.game = game;
            camera = (SceneCameraComponent)game.Services.GetService(typeof(ISceneCameraComponent));
            ScreenCentre.X = game.GraphicsDevice.Viewport.Width / 2;
            ScreenCentre.Y = game.GraphicsDevice.Viewport.Height / 2;
            textureName = TextureName;
            Position = startingPosition;
            LoadContent();
        }

        public void LoadContent()
        {
            Texture = game.Content.Load<Texture2D>(textureName);
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            //this is the only calculation that has to be added to any draw calls
            //it may be moved out of SceneNode, or should add a new method
            //that calculates screen pos at any time.
            Vector2 screenPos = ScreenCentre + (Position - camera.Position);
            batch.Draw(Texture, screenPos, Color.White);
        }

        public virtual void Update()
        {

        }
    }
}
