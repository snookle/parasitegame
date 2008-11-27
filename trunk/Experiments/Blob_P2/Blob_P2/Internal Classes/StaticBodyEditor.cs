using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Blob_P2
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class StaticBodyEditor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Game1 game;
        List<Vector2> shape = new List<Vector2>();
        bool makingShape = false;
        bool finishShape = false;
        PrimitiveBatch edgeBatch;
        PrimitiveBatch vertexBatch;
        SpriteBatch finishText;
        SpriteFont finishFont;

        bool mouseDown = false;

        public StaticBodyEditor(Game game)
            : base(game)
        {

            this.game = (Game1)game;
        }

        protected override void LoadContent()
        {
            vertexBatch = new PrimitiveBatch(Game.GraphicsDevice);
            edgeBatch = new PrimitiveBatch(Game.GraphicsDevice);
            finishText = new SpriteBatch(Game.GraphicsDevice);
            finishFont = Game.Content.Load<SpriteFont>("DebugFont");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (game.state == GameState.gsSimulate)
            {
                // LEFT BUTTON - Increase Particles
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    game.theBlob.increaseParticles();
                }

                // SPACE - Change Mode
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) //was simulating, now edit mode.
                {
                    game.state = GameState.gsEdit;
                    game.theBlob.stopstart();
                }
            }
            else if (game.state == GameState.gsEdit)
            {
                // SPACE - Change Mode 
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) //was editing, now simulate.
                {
                    game.state = GameState.gsSimulate;
                    game.theBlob.stopstart();
                }

                // LEFT BUTTON - Create Shape Vertex
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mouseDown && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    mouseDown = true;
                    if (!makingShape)
                    {
                        makingShape = true;
                    }
                    else
                    {
                        if (!shape.Contains(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
                        {
                            //if finishShape is true, it means that the user had their mouse near the 
                            //first vertex, so we want to use that as the last point instead of the
                            //mouse location.
                            if (finishShape)
                                shape.Add(shape[0]);
                            else
                                shape.Add(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.F) || finishShape)
                        {
                            //if F key or the user is close enough to the
                            //first vertex then complete Shape
                            if (shape.Count > 1)
                            {
                                if (Keyboard.GetState().IsKeyDown(Keys.R))
                                {
                                    // Create Rigid Shape
                                    Vector2[] shape2 = new Vector2[shape.Count];
                                    shape.CopyTo(shape2);

                                    game.rigidBodyManager.NewBody(Color.SlateGray, shape2);
                                    makingShape = false;
                                    finishShape = false;
                                    shape.Clear();
                                }
                                else
                                {
                                    // Default
                                    FinishShape();
                                }
                                mouseDown = false;
                            }

                        } 
                    }
                } else if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    mouseDown = false;
                }

            }
            base.Update(gameTime);
        }

        //Actually adds the new shape to the body manager.
        //and resets the Editor so we can build a new shape.
        private void FinishShape()
        {
            Vector2[] shape2 = new Vector2[shape.Count];
            shape.CopyTo(shape2);

            game.staticBodyManager.NewBody(Color.Black, shape2);
            makingShape = false;
            finishShape = false;
            shape.Clear();
        }
        //TODO: draw the bounding box at the same time
        //this means moving the bounding box drawing code out of the
        //staticbody class and into the bounding rectangle, so you can just go
        //BoundingRectangle.Draw();
        public override void Draw(GameTime gameTime)
        {
            if (game.state != GameState.gsEdit)
                return;
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            edgeBatch.Begin(PrimitiveType.LineList);
            vertexBatch.Begin(PrimitiveType.PointList);
            int i;
            for (i = 0; i < shape.Count; i++)
            {
                edgeBatch.AddVertex(shape[i], Color.Green);
                edgeBatch.AddVertex(shape[i + ( i == (shape.Count - 1) ? 0 : 1)], Color.Green);
                vertexBatch.AddVertex(shape[i], Color.Yellow);
            }
            if (makingShape && shape.Count > 0)
            {
                edgeBatch.AddVertex(shape[i - 1], Color.Green);
                edgeBatch.AddVertex(mousePos, Color.Yellow);
            }
            
            vertexBatch.End();
            edgeBatch.End();

            if (makingShape && shape.Count > 1)
            {
                if (Vector2.Distance(mousePos, shape[0]) < 10f)
                {
                    finishText.Begin();
                    finishText.DrawString(finishFont, "Finish Shape", mousePos, Color.Blue);
                    finishText.End();
                    finishShape = true;
                }
                else
                {
                    finishShape = false;
                }
            }
            base.Draw(gameTime);
        }

    }
}