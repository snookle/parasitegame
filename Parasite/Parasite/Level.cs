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
        private Vector2 selectionOffset = Vector2.Zero;


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
                if (input.IsKeyPressed(Keys.L))
                {

                }

                Vector2 mousePos = camera.MouseToWorld(); 
                
                //move the selected piece around
                if (selectedArt != null && input.IsMouseMoving() && input.IsKeyDown(Keys.M))
                {
                    selectedArt.EditorMove(selectionOffset, 20);
                }
                else
                {
                    foreach (LevelArt la in Art)
                    {
                        //check each level art for mouse collision and click
                        if (la.BoundingBox.Contains(Convert.ToInt32(mousePos.X), Convert.ToInt32(mousePos.Y)) && input.IsMouseButtonPressed("left"))
                        {
                            //deselect the current selected levelart (if there was one)
                            //before selecting the newly clicked piece
                            if (selectedArt != null)
                            {
                                selectedArt.EditorSelect(false);
                            }
                            //tell the piece that it's been selected
                            la.EditorSelect(true);
                            selectedArt = la;

                            //calculate an offset based on where the mouse was when it was clicked
                            //so we don't snap the level art origin to the mouse position
                            selectionOffset = camera.MouseToWorld() - la.WorldPosition;
                        }
                        else if(input.IsKeyPressed(Keys.D))
                        {
                            // If nothing is selected, and the D key is pressed, deselect
                            if (selectedArt != null)
                            {
                                selectedArt.EditorSelect(false);
                            }
                            selectedArt = null;
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
                //artBatch.Draw(la.Texture, la.GetScreenPosition(), la.Tint);

                artBatch.Draw(la.Texture, la.GetScreenPosition(), null, la.Tint, la.Rotation, la.Origin, la.Scale, SpriteEffects.None, la.ScreenDepth);
            }
            artBatch.End();
        }
    }
}
