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
    public class Parasite : Microsoft.Xna.Framework.DrawableGameComponent
    {

        // Parasite Part Variables
        public ParasiteHead head;

        private List<ParasiteBodyPart> bodyparts;

        private ParasiteTail tail;

        private RenderTarget2D mBackgroundRender;
        private RenderTarget2D mBackgroundRenderRotated;

        // Whole Parasite
        private List<ParasiteBodyPart> theParasite;

        // Movement vars
        private bool leftMovement = false;
        private bool rightMovement = false;

        private Vector2 gravity = new Vector2(0f, 0.9f);

        private bool rigid = false;
        private bool tailMoving = false;
        private float launchForce;

        private Texture2D theSprite;

        private Game1 game;

        // System Stuff
        private SpriteBatch spriteBatch;
        private SceneCameraComponent camera;

        public Parasite(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.game = (Game1)game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();

            /**
			 * Parasite made up of 4 parts.
			 * -Head
			 * -BPart01
			 * -BPart02
			 * -BPart03
			 * -Tail
			 */
            init();
            CreateParasite(6);
            camera = (SceneCameraComponent)Game.Services.GetService(typeof(ISceneCameraComponent));
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: Load any content

            base.LoadContent();
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            theSprite = this.Game.Content.Load<Texture2D>("Sprites\\ParasiteBodyPart");

            mBackgroundRender = new RenderTarget2D(this.GraphicsDevice, 100, 100, 1, SurfaceFormat.Color);
            mBackgroundRenderRotated = new RenderTarget2D(this.GraphicsDevice, 100, 100, 1, SurfaceFormat.Color);
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
           // spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None, Matrix.CreateTranslation(camera.Position));
            spriteBatch.Begin();
            spriteBatch.DrawString(Game.Content.Load<SpriteFont>("DebugFont"), "Tail world pos: " + tail.Position, new Vector2(10, 70), Color.Red);
            spriteBatch.DrawString(Game.Content.Load<SpriteFont>("DebugFont"), "Head world pos: " + head.Position, new Vector2(10, 110), Color.Red);
                        spriteBatch.DrawString(Game.Content.Load<SpriteFont>("DebugFont"), "tail screen pos: " + tail.GetScreenPosition(), new Vector2(10, 130), Color.Red);

            spriteBatch.Draw(theSprite, head.GetScreenPosition(), null, Color.White, 0, head.centre, 1, SpriteEffects.None, 1);

            for (int i = 0; i < bodyparts.Count; i++)
            {
                //spriteBatch.Draw(theSprite, bodyparts[i].position, null, Color.White, 0f, bodyparts[i].centre, 1, SpriteEffects.None, 1);
                float scale = (bodyparts.Count - i+1);
                scale /= bodyparts.Count;
                spriteBatch.Draw(theSprite, bodyparts[i].GetScreenPosition(), null, Color.White, bodyparts[i].rotation, bodyparts[i].centre, new Vector2(scale, scale), SpriteEffects.None, 1);
            }

            //spriteBatch.Draw(theSprite, tail.position, null, Color.Chocolate, 0, tail.centre, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(theSprite, tail.GetScreenPosition(), null, Color.Chocolate, tail.rotation, tail.centre, new Vector2(0.8f, 0.8f), SpriteEffects.None, 1);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            //head.position = mousePos;
            if (game.state == GameState.gsSimulate)
            {

                KeyboardState aKeyboard = Keyboard.GetState();
                MouseState aMouse = Mouse.GetState();
                Vector2 mousePos = camera.MouseToWorld();//new Vector2(aMouse.X, aMouse.Y);

                int swimCount = 0;
                Boolean swimming = false;

                if (head.status == ParasiteBodyPart.ParasiteStatus.swimming)
                {
                    swimCount++;
                }

                if (tail.status == ParasiteBodyPart.ParasiteStatus.swimming)
                {
                    swimCount++;
                }

                for (int i = 0; i < bodyparts.Count; i++)
                {
                    if (bodyparts[i].status == ParasiteBodyPart.ParasiteStatus.swimming)
                    {
                        swimCount++;
                    }
                }

                if (swimCount > (bodyparts.Count + 2)/2)
                {
                    swimming = true;
                }


                Boolean applyGravity = true;

                if (!rigid)
                {
                    if (aKeyboard.IsKeyDown(Keys.Left))
                    {
                        head.velocity.X -= 0.2f;
                    }
                    else if (aKeyboard.IsKeyDown(Keys.Right))
                    {
                        head.velocity.X += 0.2f;
                    }

                    if (swimming)
                    {
                        if (aKeyboard.IsKeyDown(Keys.Up))
                        {
                            head.velocity.Y -= 0.2f;
                        }
                        else if (aKeyboard.IsKeyDown(Keys.Down))
                        {
                            head.velocity.Y += 0.2f;
                        }
                    }
                }

                if ((tail.Position - mousePos).Length() < 10 * bodyparts.Count && head.velocity.X < 1f && head.velocity.Y < 1f)
                {
                    tail.Position = mousePos;
                    //head.position = mousePos;

                    applyGravity = false;
                }

                if (aMouse.LeftButton == ButtonState.Pressed && (tail.Position - mousePos).Length() < 15)
                {
                    // LOCK all angles
                    if (rigid)
                    {
                        float maxDistance = theParasite.Count * 10;
                        float theDistance = (head.Position - tail.Position).Length();

                        // max distance between head/tail = theParasite.Count * defaultDistance.
                        // in this case, it is : 8 * 10 = 80.
                        head.IKPoint.distance = (theDistance / theParasite.Count);
                        tail.IKPoint.distance = (theDistance / theParasite.Count);
                        for (int i = 0; i < bodyparts.Count; i++)
                        {
                            bodyparts[i].IKPoint.distance = (theDistance / theParasite.Count);
                        }

                        launchForce = maxDistance - theDistance;
                    }
                    else
                    {
                        lockAngles();
                        rigid = true;
                    }
                }
                else
                {
                    if (rigid)
                    {
                        unlockAngles();

                        head.IKPoint.distance = head.IKPoint.defaultdistance;
                        tail.IKPoint.distance = tail.IKPoint.defaultdistance;
                        for (int i = 0; i < bodyparts.Count; i++)
                        {
                            bodyparts[i].IKPoint.distance = bodyparts[i].IKPoint.defaultdistance;
                        }

                        rigid = false;

                        if (launchForce > 0)
                        {
                            // Shoot him off!
                            head.velocity.X = (float)Math.Cos(tail.IKPoint.currentAngle) * launchForce;
                            head.velocity.Y = (float)Math.Sin(tail.IKPoint.currentAngle) * launchForce;
                        }
                    }
                }

                tail.Init();
                tail.UpdatePoint();

                if (applyGravity)
                    tail.ApplyForce(tail.relativeGravity);

                for (int i = 0; i < bodyparts.Count; i++)
                {
                    bodyparts[i].Init();
                    bodyparts[i].UpdatePoint();

                    if (bodyparts[i].status == ParasiteBodyPart.ParasiteStatus.swimming)
                    {
                        applyGravity = false;
                    }

                    if (applyGravity)
                        bodyparts[i].ApplyForce(bodyparts[i].relativeGravity);
                }

                head.Init();
                head.UpdatePoint();

                if(applyGravity)
                    head.ApplyForce(head.relativeGravity);

                //head.IKPoint.moveTo(mousePos.X, mousePos.Y);

                //head.velocity.Y += 0.2f;
            }
            //camera.Position = new Vector3(this.head.position, 0);
            base.Update(gameTime);
        }

        public void lockAngles()
        {
            tail.IKPoint.lockAngle(true);
            for (int i = 0; i < bodyparts.Count; i++)
            {
               bodyparts[i].IKPoint.lockAngle(true);
            }
        }

        public void unlockAngles()
        {
            tail.IKPoint.lockAngle(false);
            for (int i = 0; i < bodyparts.Count; i++)
            {
                bodyparts[i].IKPoint.lockAngle(false);
            }
        }

        public void init()
        {
            bodyparts = new List<ParasiteBodyPart>();
            theParasite = new List<ParasiteBodyPart>();
        }

        public void CreateParasite(int numParts)
        {
            // Create the Tail
            tail = new ParasiteTail(Game, PhysicsOverlord.GetInstance().GetID(), theSprite, 1.0f);
            tail.Position = new Vector2(50 * numParts, 100);
            tail.relativeGravity = gravity;

            // Create Body Parts
            for (int i = 0; i < numParts; i++)
            {
                ParasiteBodyPart bodyPart = new ParasiteBodyPart(Game, PhysicsOverlord.GetInstance().GetID(), theSprite, 1f);
                bodyPart.Position = new Vector2(50 * i, 100);
                bodyparts.Add(bodyPart);
                bodyPart.relativeGravity = gravity;
            }

            // Create the Head
            head = new ParasiteHead(Game, PhysicsOverlord.GetInstance().GetID(), theSprite, 1);
            head.Position = new Vector2(100, 100);
            head.relativeGravity = gravity;

            // IKPoints

            theParasite.Add(head);

            IKMember headIK = new IKMember(head, 10);
            IKMember lastIK = headIK;

            head.AddIKPoint(headIK);

            for (int i = 0; i < bodyparts.Count; i++)
            {
                ParasiteBodyPart bodyPart = bodyparts[i];

                theParasite.Add(bodyPart);

                IKMember ik = new IKMember(bodyPart, 10);
                  
                if (i != 0)
                {
                    ik.addNeighbour(lastIK);
                }

                lastIK.addNeighbour(ik);

                bodyPart.AddIKPoint(ik);

                lastIK = ik;
            }

            // Uncomment for Rad Wiggle Motion!
            // currentIK = new IKMember(tail, 1);

            //currentIK = new IKMember(tail, 10);
            theParasite.Add(tail);
            
            IKMember tailIK = new IKMember(tail, 10);

            tailIK.addNeighbour(lastIK);
            lastIK.addNeighbour(tailIK);

            tail.AddIKPoint(tailIK);

            tail.initTail();
        }
    }
}