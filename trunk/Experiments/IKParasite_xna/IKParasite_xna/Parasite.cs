using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace IKParasite_xna
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Parasite : Microsoft.Xna.Framework.DrawableGameComponent
    {

        // Parasite Part Variables
        private ParasiteHead head;

        private List<ParasiteBodyPart> bodyparts;

        private ParasiteTail tail;

        // Whole Parasite
        private List<ParasiteBodyPart> theParasite;

        // Movement vars
        private bool leftMovement = false;
        private bool rightMovement = false;

        private bool rigid = false;
        private bool tailMoving = false;

        private Texture2D theSprite;

        // System Stuff
        private SpriteBatch spriteBatch;

        public Parasite(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
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
            CreateParasite(10);
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
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(theSprite, head.position, null, Color.White, head.rotation, head.centre, 1, SpriteEffects.None, 1);

            for (int i = 0; i < bodyparts.Count; i++)
            {
                //spriteBatch.Draw(theSprite, bodyparts[i].position, null, Color.White, bodyparts[i].rotation, bodyparts[i].centre, 1, SpriteEffects.None, 1);
                float scale = (bodyparts.Count - i);
                scale /= bodyparts.Count;
                spriteBatch.Draw(theSprite, bodyparts[i].position, null, Color.White, bodyparts[i].rotation, bodyparts[i].centre, new Vector2(scale, scale), SpriteEffects.None, 1);
            }

            //spriteBatch.Draw(theSprite, tail.position, null, Color.White, tail.rotation, tail.centre, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(theSprite, tail.position, null, Color.White, tail.rotation, tail.centre, new Vector2(0.5f, 0.5f), SpriteEffects.None, 1);

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

            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            head.position = mousePos;
            //head.IKPoint.moveTo(mousePos.X, mousePos.Y);

            //head.velocity.Y += 0.2f;

            for (int i = 0; i < theParasite.Count; i++)
            {
                theParasite[i].Init();
                theParasite[i].UpdatePoint();
            }

            base.Update(gameTime);
        }

        public void init()
        {
            bodyparts = new List<ParasiteBodyPart>();
            theParasite = new List<ParasiteBodyPart>();
        }

        public void CreateParasite(int numParts)
        {
            // Create the Tail
            tail = new ParasiteTail(theSprite, 1.0f);
            tail.position = new Vector2(150 + (50 * numParts), 100);

            // Create Body Parts
            for (int i = 0; i < numParts; i++)
            {
                ParasiteBodyPart bodyPart = new ParasiteBodyPart(theSprite, 1f);
                bodyPart.position = new Vector2(150 + (50 * i), 100);
                bodyparts.Add(bodyPart);
            }

            // Create the Head
            head = new ParasiteHead(theSprite, 1);
            head.position = new Vector2(100, 100);
            theParasite.Add(head);

            // IKPoints

            IKMember headIK = new IKMember(head,10);
            IKMember lastIK = headIK;

            IKMember currentIK;

            head.AddIKPoint(headIK);

            for (int i = 0; i < numParts; i++)
            {
                theParasite.Add(bodyparts[i]);

                currentIK = new IKMember(bodyparts[i],10-i);
                 
                if (i != 0)
                {
                    currentIK.addNeighbour(lastIK);
                }

                lastIK.addNeighbour(currentIK);
                bodyparts[i].AddIKPoint(currentIK);
                lastIK = currentIK;

                currentIK = null;
            }

            theParasite.Add(tail);
            // Uncomment for Rad Wiggle Motion!
            // currentIK = new IKMember(tail, 1);
            
            currentIK = new IKMember(tail, 10);

            currentIK.addNeighbour(lastIK);
            lastIK.addNeighbour(currentIK);

            tail.AddIKPoint(currentIK);

            tail.initTail();
        }
    }
}