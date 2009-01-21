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
    class Level : DrawableGameComponent
    {
        private List<LevelArt> Art = new List<LevelArt>();
        private SpriteBatch artBatch;
        private bool editing = true;
        private InputHandler input;
        private Camera camera;

        //LEVEL EDITOR SPECIFIC STUFF
        private LevelArt selectedArt = null;


        public Level(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            artBatch = new SpriteBatch(GraphicsDevice);
            Art.Add(new LevelArt(Game, new Vector2(1,1), "LevelArt\\WallTest01"));
            Art.Add(new LevelArt(Game, new Vector2(150,150), "LevelArt\\WallTest01"));
            input = (InputHandler)Game.Services.GetService(typeof(IInputHandler));
            camera = (Camera)Game.Services.GetService(typeof(ICamera));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (editing)
            {
                Vector2 mousePos = camera.MouseToWorld(); 
                //handle mouse movement
                if (selectedArt != null && input.IsMouseMoving())
                {
                    selectedArt.WorldPosition = camera.MouseToWorld();
                }
                else
                {
                    foreach (LevelArt la in Art)
                    {
                        if (la.BoundingBox.Contains(Convert.ToInt32(mousePos.X), Convert.ToInt32(mousePos.Y)) && input.IsMouseButtonPressed("left"))
                        {
                            if (selectedArt != null)
                            {
                                selectedArt.EditorSelect(false);
                            }
                            la.EditorSelect(true);
                            selectedArt = la;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            artBatch.Begin();
            foreach (LevelArt la in Art)
            {
                artBatch.Draw(la.Texture, la.GetScreenPosition(), la.Tint);
            }
            artBatch.End();
        }
    }
}
