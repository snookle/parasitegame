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
        private int numParticles = 500;
        private List<BlobParticle> theParticles;
        private List<Spring> theSprings;
        private bool[][] connections;

        private SpatialGrid theGrid;

        // Constants
        private float threshold = 50.0f;
        private float restDensity = 10f;
        private float stiffness = 0.004f;
        private float nearStiffness = 0.01f;
        private float springStiffness = 0.3f;
        private float plasticityConstant = 0.3f;

        private float particleCollisionRadius = 30f;        // Used in Body-Particle Collisions
        private float friction = 0f;                      // Used in Body-Particle Collisions

        private float unknownVariableO = 0.8f;              // Increased for Highly Viscosity
        private float unknownVariableB = 0f;                // Non-Zero value for Low Viscosity
        private float yeildRatio = 0.1f;                      // Can be used to control stickyness ? 
        private float restLengthConstant = 20.0f;           // Not Needed anymore ?
        private Vector2 gravity = new Vector2(0, 0.5f);

        private Texture2D theSprite;
        private SpriteFont theFont;

        private SpriteBatch spriteBatch;

        private PerformanceTimer pt;

        private string nc = "";

        //SpriteBatch spriteBatch;
        //GraphicsDeviceManager graphics;

        public Blob(Game game, PerformanceTimer pt)
            : base(game)
        {
            // TODO: Construct any child components here  
            this.pt = pt;
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

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            theSprite = this.Game.Content.Load<Texture2D>("Sprites\\Particle");
            theFont = Game.Content.Load<SpriteFont>("TimerFont");
            initSimulation();

            base.LoadContent();
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

        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.DrawString(theFont, nc, new Vector2(500, 10), Color.Black);
            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < theParticles.Count; i++)
            {
               this.spriteBatch.Draw(theSprite, theParticles[i].position, Color.White);
            }
            this.spriteBatch.End();
            
            base.Draw(gameTime);
          
        }
        
        public void initSimulation()
        {
            theParticles = new List<BlobParticle>();
            theSprings = new List<Spring>();
            //connections = new bool[numParticles][numParticles]();
            theGrid = new SpatialGrid(800, 600, Convert.ToInt32(threshold));

            // init connections
            connections = new bool[numParticles][];
            for (int i = 0; i < numParticles; i++)
            {
                connections[i] = new bool[numParticles];
            }

            Random RandomGenerator = new Random();
        }

        public void addParticle()
        {
            Random RandomGenerator = new Random();

            float randomXVelocity = RandomGenerator.Next(20);
            float randomYVelocity = RandomGenerator.Next(20);

            BlobParticle theParticle = new BlobParticle(new Vector2(300, 100), theParticles.Count, theSprite);
            theParticle.velocity.X = randomXVelocity / 20 - 0.5f;
            theParticle.velocity.Y = randomYVelocity / 20 - 0.5f;

            theParticles.Add(theParticle);

            theGrid.AddParticle(theParticle);
        }

        // Algorithm 1
        public void doSimulation()
        {
            // if not enough particles, add them
            if (theParticles.Count < numParticles)
            {
                addParticle();
            }

            pt.StartTimer("doSimulation");
            pt.StartTimer("gravity");
            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < theParticles.Count; i++)
            {
                // Apply Gravity
                theParticles[i].applyForce(gravity);
            }
            pt.StopTimer("gravity");

            pt.StartTimer("applyViscosity");
            applyViscosity();
            pt.StopTimer("applyViscosity");

            pt.StartTimer("gridSorting");
            //foreach (BlobParticle theParticle in theParticles)
            for(int i = 0; i < theParticles.Count; i++)
            {
                BlobParticle theParticle = theParticles[i];
                theGrid.RemoveParticle(theParticle);
                // Save Prev Pos
                theParticle.previousPosition = theParticle.position;
                // Advance to Predicted Pos
                theParticle.position += theParticle.velocity;
                theGrid.AddParticle(theParticle);
            }
            pt.StopTimer("gridSorting");

            // Add and remove springs, change the rest lengths
            pt.StartTimer("adjustSprings");
            adjustSprings();
            pt.StopTimer("adjustSprings");
            // Modify Positions according to springs, double Density and collisions
            pt.StartTimer("applySpringDisplacements");
            applySpringDisplacements();
            pt.StopTimer("applySpringDisplacements");

            pt.StartTimer("doubleDensityRelaxation");
            doubleDensityRelaxation();
            pt.StopTimer("doubleDensityRelaxation");

            pt.StartTimer("resolveCollisions");
            resolveCollisions();
            //resolveCollisions_alt();
            pt.StopTimer("resolveCollisions");

            //for (int i = 0; i < theParticles.Count; i++)
            //{
                // Use previous position to compute next velocity
                //theParticles[i].velocity = (theParticles[i].position - theParticles[i].previousPosition);
            //}

            pt.StopTimer("doSimulation");
        }

        // Algorithm 2
		//needs re-arranging to reduce the number of loopings
        public void doubleDensityRelaxation()
        {
            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < theParticles.Count; i++)
            {
                BlobParticle theParticle = theParticles[i];
                float density = 0;
                float nearDensity = 0;

                List<BlobParticle> theNeighbours = theGrid.GetNeighbours(theParticle);
                //foreach (BlobParticle theNeighbour in theNeighbours)
                nc = theNeighbours.Count.ToString();
                for (int j = 0; j < theNeighbours.Count; ++j)
                {
                    BlobParticle theNeighbour = theNeighbours[j];
                    Vector2 r = theNeighbour.position - theParticle.position;

                    if (r == Vector2.Zero)
                    {
                        r.Y = 0.01f;
                        r.X = 0.01f;
                    }

                    float theDistance = Math.Abs(r.Length());
                    float q = theDistance / threshold;

                    if (q < 1)
                    {
                        density += Convert.ToSingle(Math.Pow((1 - q), 2));
                        nearDensity += Convert.ToSingle(Math.Pow((1 - q), 3));
                    }
                }
                // Compute Pressure and Near Pressuren.Pressure");
                float pressure = stiffness * (density - restDensity);
                float nearPressure = nearStiffness * nearDensity;
                Vector2 dx = new Vector2(0.0f);
                
                //foreach (BlobParticle theNeighbour in theNeighbours)
                for (int j = 0; j < theNeighbours.Count; j++)
                {
                    BlobParticle theNeighbour = theNeighbours[j];
                    Vector2 r = theNeighbour.position - theParticle.position;

                    if (r == Vector2.Zero)
                    {
                        r.Y = 0.01f;
                        r.X = 0.01f;
                    }

                    float theDistance = Math.Abs(r.Length());
                    float q = theDistance / threshold;

                    if (q < 1)
                    {
                        // Get Unit Vector
                        Vector2 unitR = r;
                        unitR.Normalize();

                        // Apply Displacements
                        float displacementValue = (pressure * (1 - q)) + (nearPressure * Convert.ToSingle(Math.Pow((1 - q), 2)));
                        Vector2 displacement = unitR * displacementValue;

                        theGrid.RemoveParticle(theNeighbour);
                        theNeighbour.position += (displacement / 2);
                        theGrid.AddParticle(theNeighbour);

                        dx -= (displacement / 2);                        
                    }
                }
                theGrid.RemoveParticle(theParticle);
                theParticle.position += dx;
                theGrid.AddParticle(theParticle);
            }
        }

        // Algorithm 3
        public void applySpringDisplacements()
        {
            //foreach (Spring theSpring in theSprings)
            for (int i = 0; i < theSprings.Count; i++)
            {
                Spring theSpring = theSprings[i];

                Vector2 r = theSpring.childParticle.position-theSpring.parentParticle.position;

                if (r == Vector2.Zero)
                {
                    r.Y = 0.01f;
                    r.X = 0.01f;
                }

                float theDistance = Math.Abs(r.Length());

                Vector2 unitR = r;
                unitR.Normalize();

                float displacementValue = springStiffness * (1-(theSpring.springLength/threshold))*(theSpring.springLength-theDistance);
                Vector2 displacement = unitR * displacementValue;

                theGrid.RemoveParticle(theSpring.parentParticle);
                theGrid.RemoveParticle(theSpring.childParticle);

                theSpring.parentParticle.position -= (displacement / 2);
                theSpring.childParticle.position += (displacement / 2);

                theGrid.AddParticle(theSpring.parentParticle);
                theGrid.AddParticle(theSpring.childParticle);
            }
        }

        // Algortihm 4
		//Needs rearranging to reduce the number of loopings.
        public void adjustSprings()
        {
            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < theParticles.Count; i++)
            {
                BlobParticle theParticle = theParticles[i];
                List<BlobParticle> theNeighbours = theGrid.GetNeighbours(theParticle);
                //foreach (BlobParticle theNeighbour in theNeighbours)
                for (int j = 0; j < theNeighbours.Count; j++)
                {
                    BlobParticle theNeighbour = theNeighbours[j];
                    Vector2 r = theNeighbour.position - theParticle.position;

                    float theDistance = Math.Abs(r.Length());
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

            // Possible Problem #1
            //foreach (Spring theSpring in theSprings)
			for (int i = 0; i < theSprings.Count; i++)
            {
				Spring theSpring = theSprings[i];
                Vector2 r = theSpring.childParticle.position - theSpring.parentParticle.position;

                if (r == Vector2.Zero)
                {
                    r.Y = 0.01f;
                    r.X = 0.01f;
                }

                float theDistance = Math.Abs(r.Length());

                float deformation = yeildRatio * theSpring.springLength;
                if (theDistance > theSpring.springLength + deformation)
                {
                    // Stretch
                    theSpring.springLength += plasticityConstant * (theDistance - theSpring.springLength - deformation);
                }
                else if (theDistance < theSpring.springLength - deformation)
                {
                    // Compress
                    theSpring.springLength -= plasticityConstant * (theSpring.springLength - deformation - theDistance);
                }
            }

            //foreach (Spring theSpring in theSprings)
            for(int i = 0; i < theSprings.Count; i++)
            {
				Spring theSpring = theSprings[i];
                if (theSpring.springLength > threshold)
                {
                    // Remove Spring
                    connections[theSpring.parentParticle.idNumber][theSpring.childParticle.idNumber] = false;
                    theSprings.Remove(theSpring);
                    i--; //danger will robinson!
                }
            }
        } 

        // Algorithm 5
        public void applyViscosity()
        {
            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < theParticles.Count; i++)
            {
                BlobParticle theParticle = theParticles[i];
                List<BlobParticle> theNeighbours = theGrid.GetNeighbours(theParticle);
                //foreach (BlobParticle theNeighbour in theNeighbours)
                for (int j = 0; j < theNeighbours.Count; j++)
                {
                    BlobParticle theNeighbour = theNeighbours[j];
                    Vector2 r = theNeighbour.position - theParticle.position;

                    if (r == Vector2.Zero)
                    {
                        r.Y = 0.01f;
                        r.X = 0.01f;
                    }

                    float theDistance = r.Length();
                    float q = theDistance / threshold;

                    if (q < 1)
                    {
                        Vector2 unitR = r;
                        unitR.Normalize();

                        float u = Vector2.Dot((theParticle.velocity-theNeighbour.velocity),unitR);
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

            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < theParticles.Count; i++)
            {
                BlobParticle theParticle = theParticles[i];

                theGrid.RemoveParticle(theParticle);
                theParticle.velocity = (theParticle.position - theParticle.previousPosition);

                if (theParticle.position.Y > 550)
                {
                    theParticle.position.Y = 550;
                    theParticle.velocity.Y *= -0.4f;
                }
                else if (theParticle.position.Y < 0)
                {
                    theParticle.position.Y = 0;
                    theParticle.velocity.Y *= -0.4f;
                }

                if (theParticle.position.X > 700)
                {
                    theParticle.position.X = 700;
                    theParticle.velocity.X *= -0.4f;
                }
                else if (theParticle.position.X < 0)
                {
                    theParticle.position.X = 0;
                    theParticle.velocity.X *= -0.4f;
                }
                theGrid.AddParticle(theParticle);

            }
        }

        public void resolveCollisions_alt()
        {
            for (int i = 0; i < theParticles.Count; i++)
            {
                // Collision = d<particleCollisionRadius);
                if (theParticles[i].position.Y > 500)
                {
                    // and v(i) = currentpos-prevpos
                    Vector2 vI = theParticles[i].position - theParticles[i].previousPosition;

                    // and v(p) = body velocity at contact point
                    Vector2 vP = Vector2.Zero;
                    
                    // and v = (v(i) - v(p))
                    Vector2 v = vI - vP;

                    // nUnit = object normal using distance field gradient ... ? 
                    Vector2 nUnit = new Vector2(0, -1);

                    // where v(normal) = (v.dot(nUnit) * nUnit) 
                    Vector2 vNormal = Vector2.Dot(v, nUnit) * nUnit;

                    // and v(tangent) = v - v(normal)
                    Vector2 vTangent = v - vNormal;
                    
                    // Impulse = v(normal) - (µ * v(tangent));
                    Vector2 impulse = vNormal - friction * vTangent;

                    // Apply to Particle
                    theParticles[i].position -= impulse;

                    // Extract Particle if still in body
                    if (theParticles[i].position.Y > 500)
                    {
                        //theParticles[i].position.Y = 500;
                    }

                }

            }
        }

    }
}