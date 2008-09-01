using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace ViscoelasticXNAPrototype
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Blob : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Variables
        private int numParticles = 100;
        private List<BlobParticle> theParticles;
        private List<Spring> theSprings;
        private bool[][] connections;

        private SpatialGrid theGrid;

        // Constants
        private float threshold = 0.5f;
        private float restDensity = 10f;
        private float stiffness = 0.004f;
        private float nearStiffness = 0.01f;
        private float springStiffness = 0.3f;
        private float plasticityConstant = 0.3f;
        private float friction = 0.5f;
        private float unknownVariableO = 0.05f;
        private float unknownVariableB = 0;
        private float yeildRatio = 0.2f;
        private float restLengthConstant = 0.5f;
        private Vector2 gravity = new Vector2(0, 0.5f);

        private Texture2D theSprite;

        private SpriteBatch spriteBatch;

        //SpriteBatch spriteBatch;
        //GraphicsDeviceManager graphics;

        public Blob(Game game)
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

            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            initSimulation();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
            doSimulation();
        }

        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            foreach (BlobParticle theParticle in theParticles)
            {
               this.spriteBatch.Draw(theSprite, theParticle.position, Color.White);
            }
            this.spriteBatch.End();
            
            base.Draw(gameTime);
        }
        
        public void initSimulation()
        {
            theParticles = new List<BlobParticle>();
            theSprings = new List<Spring>();
            //connections = new bool[numParticles][numParticles]();
            theGrid = new SpatialGrid(800, 600, 20);

            // init connections
            connections = new bool[numParticles][];
            for (int i = 0; i < numParticles; i++)
            {
                connections[i] = new bool[numParticles];
            }

            Random RandomGenerator = new Random();

            // Create the Particles
            for (int i = 0; i < numParticles; i++)
            {
                int RandomWidth = RandomGenerator.Next(700);
                int RandomHeight = RandomGenerator.Next(400);
                float randomXVelocity = RandomGenerator.Next(20);
                float randomYVelocity = RandomGenerator.Next(20);

                theSprite = this.Game.Content.Load<Texture2D>("Sprites\\Particle");

                BlobParticle theParticle = new BlobParticle(new Vector2(RandomWidth, RandomHeight), i, theSprite);
                theParticle.velocity.X = randomXVelocity/20-0.5f;
                theParticle.velocity.Y = randomYVelocity/20-0.5f;

                theParticles.Add(theParticle);

                // Initial Grid Add
                theGrid.AddParticle(theParticle);
            }
        }

        // Algorithm 1
        public void doSimulation()
        {
            foreach (BlobParticle theParticle in theParticles)
            {
                // Apply Gravity
                 theParticle.applyForce(gravity);
            }

            //applyViscosity();

            foreach (BlobParticle theParticle in theParticles)
            {
                theGrid.RemoveParticle(theParticle);
                // Save Prev Pos
                theParticle.previousPosition = theParticle.position;
                // Advance to Predicted Pos
                theParticle.position += theParticle.velocity;
                theGrid.AddParticle(theParticle);
            }

            // Add and remove springs, change the rest lengths
            adjustSprings();
            // Modify Positions according to springs, double Density and collisions
            applySpringDisplacements();
            //doubleDensityRelaxation();
            resolveCollisions();

            foreach (BlobParticle theParticle in theParticles)
            {
                // Use previous position to compute next velocity
                // theParticle.velocity = (theParticle.position - theParticle.previousPosition);
            }
        }

        // Algorithm 2
        public void doubleDensityRelaxation()
        {
            foreach (BlobParticle theParticle in theParticles)
            {
                float density = 0;
                float nearDensity = 0;

                List<BlobParticle> theNeighbours = theGrid.GetNeighbours(theParticle);
                foreach (BlobParticle theNeighbour in theNeighbours)
                {
                    Vector2 r = theParticle.position - theNeighbour.position;
                    float theDistance = r.Length();
                    float q = theDistance / threshold;

                    if (q < 1)
                    {
                        density += Convert.ToSingle(Math.Pow((1 - q), 2));
                        nearDensity += Convert.ToSingle(Math.Pow((1 - q), 3));
                    }
                }

                // Compute Pressure and Near Pressure
                float pressure = stiffness * (density - nearDensity);
                float nearPressure = nearStiffness * nearDensity;
                Vector2 dx = new Vector2(0.0f);

                foreach (BlobParticle theNeighbour in theNeighbours)
                {
                    Vector2 r = theParticle.position - theNeighbour.position;
                    float theDistance = r.Length();
                    float q = theDistance / threshold;

                    if (q < 1)
                    {
                        // Get Unit Vector
                        Vector2 unitR = r;
                        unitR.Normalize();

                        // Apply Displacements
                        float displacementValue = (pressure * (1 - q)) + (nearPressure * Convert.ToSingle(Math.Pow((1 - q), 2)));
                        Vector2 displacement = unitR * displacementValue;

                        theNeighbour.position += (displacement / 2);
                        dx -= (displacement / 2);                        
                    }
                }

                theParticle.position += dx;
            }
        }

        // Algorithm 3
        public void applySpringDisplacements()
        {
            foreach (Spring theSpring in theSprings)
            {
                Vector2 r = theSpring.childParticle.position-theSpring.parentParticle.position;
                float theDistance = r.Length();
                Vector2 unitR = r;
                unitR.Normalize();

                float displacementValue = springStiffness * (1-theSpring.springLength/threshold)*(theSpring.springLength-theDistance);
                Vector2 displacement = unitR * displacementValue;

                theSpring.parentParticle.position -= (displacement / 2);
                theSpring.childParticle.position += (displacement / 2);
            }
        }

        // Algortihm 4
        public void adjustSprings()
        {
            foreach (BlobParticle theParticle in theParticles)
            {
                List<BlobParticle> theNeighbours = theGrid.GetNeighbours(theParticle);
                foreach (BlobParticle theNeighbour in theNeighbours)
                {
                    Vector2 r = theNeighbour.position - theParticle.position;
                    float theDistance = r.Length();
                    float q = theDistance / threshold;

                    if (q < 1)
                    {
                        if (!connections[theParticle.idNumber][theNeighbour.idNumber])
                        {
                            // if not currently connected with a spring
                            // Create a spring
                            Spring newSpring = new Spring(threshold, theParticle, theNeighbour);
                            connections[theParticle.idNumber][theNeighbour.idNumber] = true;
                            theSprings.Add(newSpring);
                        }
                    }
                }
            }

            foreach (Spring theSpring in theSprings)
            {
                Vector2 r = theSpring.childParticle.position - theSpring.parentParticle.position;
                float theDistance = r.Length();

                float deformation = yeildRatio * theSpring.springLength;
                if (theDistance > restLengthConstant + deformation)
                {
                    // Stretch
                    theSpring.springLength += plasticityConstant * (theDistance - restLengthConstant - deformation);
                }
                else if (theDistance < restLengthConstant - deformation)
                {
                    // Compress
                    theSpring.springLength -= plasticityConstant * (restLengthConstant - deformation - theDistance);
                }
            }

            //foreach (Spring theSpring in theSprings)
            for(int i=0;i<theSprings.Count;i++)
            {
                if (theSprings[i].springLength > threshold)
                {
                    // Remove Spring
                    connections[theSprings[i].parentParticle.idNumber][theSprings[i].childParticle.idNumber] = false;
                    theSprings.Remove(theSprings[i]);
                    i--;
                }
            }
        } 

        // Algorithm 5
        public void applyViscosity()
        {
            foreach (BlobParticle theParticle in theParticles)
            {
                List<BlobParticle> theNeighbours = theGrid.GetNeighbours(theParticle);
                foreach (BlobParticle theNeighbour in theNeighbours)
                {
                    Vector2 r = theNeighbour.position - theParticle.position;
                    float theDistance = r.Length();
                    float q = theDistance / threshold;

                    if (q < 1)
                    {
                        Vector2 unitR = r;
                        unitR.Normalize();

                        float u = Vector2.Dot(unitR,(theNeighbour.velocity-theParticle.velocity));
                        if (u > 0)
                        {
                            // Linear and Quadratic Impulses - lol
                            float impulseValue = Convert.ToSingle((1 - q) * (unknownVariableO * u + unknownVariableB * Math.Pow(u, 2)));
                            Vector2 impulse = unitR * impulseValue;

                            theParticle.velocity -= (impulse / 2);
                            theNeighbour.velocity += (impulse / 2);
                        }
                    }
                }
            }
        }

        // Algorithm 6
        public void resolveCollisions()
        {
            foreach (BlobParticle theParticle in theParticles)
            {
                theParticle.velocity = (theParticle.position - theParticle.previousPosition);

                if (theParticle.position.Y > 550)
                {
                    theParticle.position.Y = 550;
                    theParticle.velocity.Y *= -0.5f;
                }
                else if (theParticle.position.Y < 0)
                {
                    theParticle.position.Y = 0;
                    theParticle.velocity.Y *= -0.5f;
                }

                if (theParticle.position.X > 700)
                {
                    theParticle.position.X = 700;
                    theParticle.velocity.X *= -0.5f;
                }
                else if (theParticle.position.X < 0)
                {
                    theParticle.position.X = 0;
                    theParticle.velocity.X *= -0.5f;
                }                
            }
        }

    }
}