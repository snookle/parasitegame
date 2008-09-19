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
    public class BlobManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Variables
        public int numParticles = 500;
        public int currentNumParticles = 0;
        public int particleCount = 0;
        private List<BlobParticle> theParticles;
        public Texture2D theSprite;
        private Spring[][] connected;

        private int drawingOffset = 100;

        // Spatial Grid
        private SpatialGrid grid;
        
        // Forces
        public Vector2 gravity = new Vector2(0, 0.5f);

        public float threshold = 20;
        public float disconnectThreshold = 20;
        public int numLinks = 6;

        // Spring Variables
        private List<Spring> theSprings;
        public float springStiffness = 0.5f;
        public float springLength = 20;
        public float springFriction = 0.05f;

        private SpriteBatch spriteBatch;

        public BlobManager(Game game)
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
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            theSprite = this.Game.Content.Load<Texture2D>("Sprites\\Particle");
            initSimulation();

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            BlobParticle theParticle;

            for (int i = 0; i < particleCount; i++)
            {
                theParticle = theParticles[i];
                this.spriteBatch.Draw(theSprite, theParticle.position, null, Color.White, 0, theParticle.centre, 1, SpriteEffects.None, 0);
            }
            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            doSimulation();
            base.Update(gameTime);
        }


        public void initSimulation()
        {
            theParticles = new List<BlobParticle>();
            theSprings = new List<Spring>();

            grid = new SpatialGrid(500+drawingOffset, 500, (int)disconnectThreshold);

            connected = new Spring[numParticles + 1][];
            for (int i = 0; i < numParticles; i++)
            {
                connected[i] = new Spring[numParticles + 1];
            }
        }

        public void increaseParticles()
        {
            currentNumParticles++;
        }

        public void addParticles(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Random RandomGenerator = new Random();

                float randomXVelocity = RandomGenerator.Next(20);
                float randomYVelocity = RandomGenerator.Next(20);

                BlobParticle theParticle = new BlobParticle(new Vector2(300, 100), theSprite);
                theParticle.velocity = new Vector2(randomXVelocity / 20 - 0.5f, randomYVelocity / 20 - 0.5f);

                theParticles.Add(theParticle);

                particleCount++;
                currentNumParticles--;
            }
        }


        public void doSimulation()
        {
            if (currentNumParticles > 0)
            {
                if (theParticles.Count < numParticles)
                {
                    addParticles(1);
                }
            }
            moveParticles();
            checkSprings();
        }

        public void checkSprings()
        {
            BlobParticle theParticle;
            BlobParticle neighbourParticle;
            Spring newSpring;

            for (int i = 0; i < particleCount; i++)
            {
                theParticle = theParticles[i];
                for (int j = i + 1; j < particleCount; j++)
                {
                    neighbourParticle = theParticles[j];

                    Vector2 differenceVector = theParticle.position - neighbourParticle.position;
                    float distance = differenceVector.Length();

                    if (distance <= threshold && connected[i][j] == null)
                    {
                        theParticle.addNeighbour(neighbourParticle);
                        neighbourParticle.addNeighbour(theParticle);

                        newSpring = new Spring(theParticle, neighbourParticle, springStiffness, springLength, springFriction);
                        connected[i][j] = newSpring;
                    }
                    else if (distance > disconnectThreshold && connected[i][j] != null)
                    {
                        theParticle.removeNeighbour(neighbourParticle);
                        neighbourParticle.removeNeighbour(theParticle);
                        connected[i][j] = null;
                    }

                    if (connected[i][j] != null)
                    {
                        connected[i][j].solve();
                    }
                }
            }
        }

        public void moveParticles()
        {
            BlobParticle theParticle;
            for (int i = 0; i < particleCount; i++)
            {
                theParticle = theParticles[i];
                theParticle.position += theParticle.velocity;

                // Apply Gravity
                theParticle.applyForce(gravity);

                simpleCollisionDetection(theParticle);
            }
        }

        public void applyViscosity()
        {

        }

        private void simpleCollisionDetection(BlobParticle theParticle)
        {

            if (theParticle.position.X < 0 + drawingOffset)
            {
                theParticle.velocity.X *= -0.5f;
                theParticle.position.X = 0 + drawingOffset;
            }
            else if (theParticle.position.X > 800 - drawingOffset)
            {
                theParticle.velocity.X *= -0.5f;
                theParticle.position.X = 800 - drawingOffset;
            }

            if (theParticle.position.Y < 0)
            {
                theParticle.velocity.Y *= -0.5f;
                theParticle.position.Y = 0;
            }
            else if (theParticle.position.Y > 500)
            {
                theParticle.velocity.Y *= -0.5f;
                theParticle.position.Y = 500;
            }
        }
    }
}