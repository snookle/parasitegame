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
        public int maxParticles = 1500;
        public int currentNumParticles = 0;
        public int particleCount = 0;
        private List<BlobParticle> theParticles;
        public Texture2D theSprite;
        private Spring[][] connected;

        private int drawingOffset = 10;

        // Spatial Grid
        private SpatialGrid grid;
        
        // Forces
        public Vector2 gravity = new Vector2(0, 0.5f);

        public float threshold = 20;
        public float disconnectThreshold = 20;
        public int numLinks = 6;

        // Spring Variables
        private List<Spring> theSprings;
        public float springStiffness = 0.3f;
        public float springLength = 20;
        public float springFriction = 0.1f;

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private bool simulating = true;

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
            spriteFont = Game.Content.Load<SpriteFont>("DebugFont");
            
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

            this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);

            BlobParticle theParticle;
            spriteBatch.DrawString(spriteFont, particleCount.ToString(), new Vector2(10, 10), Color.Black);
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            float theDistance;
            List<BlobParticle> theNeighbours;
            int neighbourCount;
            int j;
            for (int i = 0; i < particleCount; i++)
            {
                theParticle = theParticles[i];

                theParticle.colour = Color.Black;

                theDistance = (mousePos - theParticle.position).Length();

                if (theDistance < 5)
                {
                    // Mouseover
                    theParticle.colour = Color.Red;
                    // Set Neighbour Colour
                    theNeighbours = grid.GetNeighbours(theParticle);
                    spriteBatch.DrawString(spriteFont, "Num Neighbours : "+theNeighbours.Count.ToString(), new Vector2(10, 20), Color.Black);
                    neighbourCount = theNeighbours.Count;
                    for (j = 0; j < neighbourCount; j++)
                    {
                        this.spriteBatch.Draw(theSprite, theNeighbours[j].position, null, Color.Chartreuse, 0, theNeighbours[j].centre, 1, SpriteEffects.None, 0);
                    }
                }
                this.spriteBatch.Draw(theSprite, theParticle.position, null, theParticle.colour, 0, theParticle.centre, 1, SpriteEffects.None, 1);
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
            if (simulating)
            {
                doSimulation();
            }
            base.Update(gameTime);
        }


        public void initSimulation()
        {
            theParticles = new List<BlobParticle>();
            theSprings = new List<Spring>();

            grid = new SpatialGrid(800, 600, disconnectThreshold);

            connected = new Spring[maxParticles + 1][];
            for (int i = 0; i < maxParticles; i++)
            {
                connected[i] = new Spring[maxParticles + 1];
            }
        }

        public void increaseParticles()
        {
            currentNumParticles++;
        }

        public void stopstart()
        {
            if (simulating)
            {
                simulating = false;
            }
            else
            {
                simulating = true;
            }
        }

        public void addParticles(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Random RandomGenerator = new Random();

                float randomXVelocity = RandomGenerator.Next(20);
                float randomYVelocity = RandomGenerator.Next(20);

                BlobParticle theParticle = new BlobParticle(new Vector2(300, 100), theSprite, particleCount);
                theParticle.velocity = new Vector2(randomXVelocity / 20 - 0.5f, randomYVelocity / 20 - 0.5f);

                theParticles.Add(theParticle);
                grid.AddParticle(theParticle);

                particleCount++;
                currentNumParticles--;
            }
        }


        public void doSimulation()
        {
            if (currentNumParticles > 0)
            {
                if (particleCount < maxParticles)
                {
                    addParticles(1);
                }
            }
            moveParticles();
           // checkSprings();
        }

        public void checkSprings()
        {
            /*BlobParticle theParticle;
            BlobParticle neighbourParticle;
            Vector2 differenceVector;
            int neighourCount;
            List<BlobParticle> neighbours;

           /* for (int i = 0; i < particleCount; i++)
            {
 

                
            }*/
        }

        public void moveParticles()
        {
            BlobParticle theParticle;
            BlobParticle neighbourParticle;
            Vector2 differenceVector;
            int neighourCount;
            List<BlobParticle> neighbours;
            for (int i = 0; i < particleCount; i++)
            {
                theParticle = theParticles[i];
                grid.RemoveParticle(theParticle);
                theParticle.position += theParticle.velocity;

                // Apply Gravity
                theParticle.applyForce(gravity);

                simpleCollisionDetection(theParticle);
                grid.AddParticle(theParticle);

                //BEGIN CHECK SPRINGS
                neighbours = grid.GetNeighbours(theParticle);
                neighourCount = neighbours.Count;
                for (int j = 0; j < neighourCount; j++)
                {
                    neighbourParticle = neighbours[j];

                    differenceVector = theParticle.position - neighbourParticle.position;
                    float distance = differenceVector.Length();

                    if (distance <= threshold && connected[theParticle.id][neighbourParticle.id] == null)
                    {
                        theParticle.addNeighbour(neighbourParticle);
                        neighbourParticle.addNeighbour(theParticle);

                        connected[theParticle.id][neighbourParticle.id] = new Spring(theParticle, neighbourParticle, springStiffness, springLength, springFriction);
                    }
                    else if (distance > disconnectThreshold && connected[theParticle.id][neighbourParticle.id] != null)
                    {
                        theParticle.removeNeighbour(neighbourParticle);
                        neighbourParticle.removeNeighbour(theParticle);
                        connected[theParticle.id][neighbourParticle.id] = null;
                    }

                    if (connected[theParticle.id][neighbourParticle.id] != null)
                    {
                        connected[theParticle.id][neighbourParticle.id].solve();
                    }
                }
                //END CHECK SPRINGS
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
                theParticle.position.Y = 1;
            }
            else if (theParticle.position.Y > 500)
            {
                theParticle.velocity.Y *= -0.5f;
                theParticle.position.Y = 500;
            }
        }
    }
}