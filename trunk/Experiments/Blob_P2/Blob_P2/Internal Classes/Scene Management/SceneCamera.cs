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


namespace Blob_P2
{
    public interface ICameraComponent {}
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SceneCamera : Microsoft.Xna.Framework.GameComponent, ICameraComponent
    {
        private Game game;
        private Matrix view;
        private Matrix projection;
        private Vector3 position;
        private Vector3 target;
        private Vector3 up;

        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public Matrix View
        {
            get { return view; }
        }

        public SceneCamera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game)
        {
            this.game = game;
            this.position = position;
            this.target = target;
            this.up = up;

            //add itself as a game service
            game.Services.AddService(typeof(ICameraComponent), this);
        }

        public override void Initialize()
        {
            InitializeCamera();
            base.Initialize();
        }

        private void InitializeCamera()
        {
            float aspectRatio = (float)game.GraphicsDevice.Viewport.Width / (float)game.GraphicsDevice.Viewport.Height;

            //create the projection matrix
            Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.1f, 1000.0f, out projection);

            //create the view matrix
            Matrix.CreateLookAt(ref position, ref target, ref up, out view);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //update the view matrix.
            //Vector3 transform = new Vector3();
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                position.X += 10;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                position.X -= 10;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                position.Y -= 10;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                position.Y += 10;
            

            Matrix.CreateLookAt(ref position, ref target, ref up, out view);
        }

    }
}