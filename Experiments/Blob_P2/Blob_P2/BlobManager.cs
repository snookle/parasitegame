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

        public float threshold = 20;
        public float disconnectThreshold = 20;
        public int numLinks = 6;

        // Spatial Grid
        private SpatialGrid grid;
        
        // Forces
        public Vector2 gravity = new Vector2(0, 0.5f);

<<<<<<< .mine
        public float threshold = 20;
        public float disconnectThreshold = 20;
        public int numLinks = 6;
        public float particleRadius = 5.0f;

=======
>>>>>>> .r129
        // Spring Variables
        private List<Spring> theSprings;
        public float springStiffness = 0.3f;
        public float springLength = 20;
        public float springFriction = 0.1f;

        // DDR
        private float restDensity = 20f;
        private float stiffness = 0.5f;
        private float nearStiffness = 0.01f;

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private bool simulating = true;

        VertexPositionColor[] line;         // Start / End Points
        Matrix world, projection, view;             // Transformation matrix
        BasicEffect basicEffect;                    // Standard Drawing Effects
        VertexDeclaration vertexDeclaration; 

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
            vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            line = new VertexPositionColor[2];

            basicEffect = new BasicEffect(GraphicsDevice, null);
            basicEffect.DiffuseColor = new Vector3(0.5f, 0.2f, 0.2f);
            basicEffect.Alpha = 0.5f;

        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            theSprite = this.Game.Content.Load<Texture2D>("Sprites\\Particle");
            spriteFont = Game.Content.Load<SpriteFont>("DebugFont");
            
            initSimulation();
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
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
                        
                        line[0] = new VertexPositionColor(new Vector3(theNeighbours[j].position, 0), Color.Red);
                        line[1] = new VertexPositionColor(new Vector3(theParticle.position, 0), Color.Red);

                        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                        {
                            pass.Begin();
                            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, line, 0, 2, new short[2] { 0, 1 }, 0, 1);
                            pass.End();
                        }
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

                float randomXVelocity = 5;

                float xPos = Mouse.GetState().X;
                float yPos = Mouse.GetState().Y;

                if (xPos < drawingOffset)
                {
                    xPos = drawingOffset + 1;
                }
                else if (xPos > 800 - drawingOffset)
                {
                    xPos = 800 - drawingOffset - 1;
                }

                if (yPos < 0)
                {
                    yPos = 1;
                }
                else if (yPos > 500)
                {
                    yPos = 499;
                }

                BlobParticle theParticle = new BlobParticle(new Vector2(xPos, yPos), theSprite, particleCount, particleRadius);
                theParticle.velocity = new Vector2(5,5);

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
           // doubleDensityRelaxation();
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
                        //connected[theParticle.id][neighbourParticle.id].viscoSolve();
                        connected[theParticle.id][neighbourParticle.id].solve();

                    }
                }
                //END CHECK SPRINGS
            }
        }

        public void applyViscosity()
        {

        }

        public void doubleDensityRelaxation()
        {
            //foreach (BlobParticle theParticle in theParticles)
            BlobParticle theParticle;
            BlobParticle theNeighbour;

            Vector2 differenceVector;

            int neighbourCount;

            float density;
            float nearDensity;

            int i;
            int j;

            float theDistance;
            float q;

            float pressure;
            float nearPressure;

            Vector2 unitR;

            Vector2 displacement;

            Vector2 dx;

            for (i = 0; i < theParticles.Count; i++)
            {
                theParticle = theParticles[i];
                density = 0;
                nearDensity = 0;

                List<BlobParticle> theNeighbours = grid.GetNeighbours(theParticle);
                neighbourCount = theNeighbours.Count;

                for (j = 0; j < neighbourCount; j++)
                {
                    theNeighbour = theNeighbours[j];

                    differenceVector = theNeighbour.position - theParticle.position;
                    if (differenceVector.X == 0 && differenceVector.Y == 0)
                    {
                        differenceVector.X = 0.000001f;
                        differenceVector.Y = 0.000001f;
                    }

                    theDistance = differenceVector.Length();

                    q = theDistance / threshold;

                    if (q < 1)
                    {
                        density += (float)Math.Pow((1 - q), 2);
                        nearDensity += (float)Math.Pow((1 - q), 3);
                    }
                }

                // Compute Pressure and Near Pressure
                pressure = stiffness * (density - restDensity);
                nearPressure = nearStiffness * nearDensity;

                dx = Vector2.Zero;

                //foreach (BlobParticle theNeighbour in theNeighbours)
                for (j = 0; j < neighbourCount; j++)
                {
                    theNeighbour = theNeighbours[j];

                    differenceVector = theNeighbour.position - theParticle.position;
                    if (differenceVector.X == 0 && differenceVector.Y == 0)
                    {
                        differenceVector.X = 0.000001f;
                        differenceVector.Y = 0.000001f;
                    }

                    theDistance = differenceVector.Length();

                    q = theDistance / threshold;

                    if (q < 1)
                    {
                        // Get Unit Vector
                        unitR = Vector2.Normalize(differenceVector);

                        // Apply Displacements
                        float displacementValue = (pressure * (1 - q)) + (nearPressure * (float)Math.Pow((1 - q), 2));

                        displacement = (unitR * displacementValue) / 2;

                        theNeighbour.applyForce(displacement);

                        dx -= displacement;
                    }
                }
                theParticle.applyForce(dx);
            }
        }

        private void simpleCollisionDetection(BlobParticle theParticle)
        {

            if (theParticle.position.X < 0 + drawingOffset)
            {
                theParticle.velocity.X *= -0.5f;
                if (theParticle.velocity.X < 2.0f && theParticle.velocity.X > -2.0f)
                    theParticle.velocity.X = 0.0f;
            
                theParticle.position.X = 0 + drawingOffset;
            }
            else if (theParticle.position.X > 800 - drawingOffset)
            {
                theParticle.velocity.X *= -0.5f;
                if (theParticle.velocity.X < 2.0f && theParticle.velocity.X > -2.0f)
                    theParticle.velocity.X = 0.0f;
            
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
                if (theParticle.velocity.Y < 2.0f && theParticle.velocity.Y > -2.0f )
                    theParticle.velocity.Y = 0.0f;
                theParticle.position.Y = 500;
            }
        }
    }
}