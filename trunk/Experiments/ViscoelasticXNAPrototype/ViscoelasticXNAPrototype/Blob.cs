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
        private int currentNumParticles = 0;
        private List<BlobParticle> theParticles;
        private List<Spring> theSprings;
        private bool[][] connections;
        private int currentSpringCount = 0;

        private SpatialGrid theGrid;

        // Constants
        private float threshold = 50.0f;
        private float restDensity = 0.05f;
        private float stiffness = 0.004f;
        private float nearStiffness = 0.01f;
        private float springStiffness = 0.3f;
        private float plasticityConstant = 0.3f;

        private float particleCollisionRadius = 30f;        // Used in Body-Particle Collisions
        private float friction = 0f;                      // Used in Body-Particle Collisions

        private float unknownVariableO = 0.8f;              // Increased for Highly Viscosity
        private float unknownVariableB = 0f;                // Non-Zero value for Low Viscosity
        private float yeildRatio = 0.2f;                      // Can be used to control stickyness ? 
        private float restLengthConstant = 20.0f;           // Not Needed anymore ?
        //private Vector2 gravity = new Vector2(0, 0.5f);
        private float gravityX;
        private float gravityY = 0.8f;

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

        public void increaseParticles()
        {
            currentNumParticles++;
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
            spriteBatch.DrawString(theFont, "Springs: " + currentSpringCount.ToString(), new Vector2(500, 10), Color.Black);
            spriteBatch.DrawString(theFont, "Particles: " + currentNumParticles.ToString(), new Vector2(500, 50), Color.Black);

            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < currentNumParticles; i++)
            {
               this.spriteBatch.Draw(theSprite, new Vector2(theParticles[i].pX, theParticles[i].pY), Color.White);
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
            theParticle.vX = randomXVelocity / 20 - 0.5f;
            theParticle.vY = randomYVelocity / 20 - 0.5f;

            theParticles.Add(theParticle);

            theGrid.AddParticle(theParticle);
        }

        // Algorithm 1
        public void doSimulation()
        {
            // if not enough particles, add them
            if (theParticles.Count < currentNumParticles)
            {
                addParticle();
            }
            if (currentNumParticles == 0) return;
            pt.StartTimer("doSimulation");
            pt.StartTimer("gravity");
            //foreach (BlobParticle theParticle in theParticles)
            for (int i = 0; i < currentNumParticles; i++)
            {
                // Apply Gravity
                theParticles[i].applyForce(gravityX, gravityY);
            }
            pt.StopTimer("gravity");

            pt.StartTimer("applyViscosity");
            applyViscosity();
            pt.StopTimer("applyViscosity");
            for (int i = 0; i < currentNumParticles; i++)
            {
                if (theParticles[i].pX == float.NaN || theParticles[i].pX >  15000)
                {
                    ;
                }
            }
            pt.StartTimer("gridSorting");
            //foreach (BlobParticle theParticle in theParticles)
            for(int i = 0; i < currentNumParticles; i++)
            {
                
                theGrid.RemoveParticle(theParticles[i]);
                // Save Prev Pos
                //theParticles[i].previousPosition = theParticles[i].position;
                theParticles[i].ppX = theParticles[i].pX;
                theParticles[i].ppY = theParticles[i].pY;
                // Advance to Predicted Pos
                //theParticles[i].position += theParticles[i].velocity;
                theParticles[i].pX += theParticles[i].vX;
                theParticles[i].pY += theParticles[i].vY;
                theGrid.AddParticle(theParticles[i]);
            }
            pt.StopTimer("gridSorting");
            for (int i = 0; i < currentNumParticles; i++)
            {
                if (theParticles[i].pX == float.NaN || theParticles[i].pX > 15000)
                {
                    ;
                }
            }
            // Add and remove springs, change the rest lengths
            pt.StartTimer("adjustSprings");
            adjustSprings();
            pt.StopTimer("adjustSprings");
            for (int i = 0; i < currentNumParticles; i++)
            {
                if (theParticles[i].pX == float.NaN || theParticles[i].pX > 15000)
                {
                    ;
                }
            }
            // Modify Positions according to springs, double Density and collisions
            pt.StartTimer("applySpringDisplacements");
            applySpringDisplacements();
            pt.StopTimer("applySpringDisplacements");
            for (int i = 0; i < currentNumParticles; i++)
            {
                if (theParticles[i].pX == float.NaN || theParticles[i].pX > 15000)
                {
                    ;
                }
            }

            pt.StartTimer("doubleDensityRelaxation");
            doubleDensityRelaxation();
            pt.StopTimer("doubleDensityRelaxation");
            for (int i = 0; i < currentNumParticles; i++)
            {
                if (theParticles[i].pX == float.NaN || theParticles[i].pX > 15000)
                {
                    ;
                }
            }

            pt.StartTimer("resolveCollisions");
            resolveCollisions();
            //resolveCollisions_alt();
            pt.StopTimer("resolveCollisions");
            for (int i = 0; i < currentNumParticles; i++)
            {
                if (theParticles[i].pX == float.NaN || theParticles[i].pX > 15000)
                {
                    ;
                }
            }

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
            BlobParticle theParticle;
            BlobParticle theNeighbour;
            
            float rY;
            float rX;
            
            int neighbourCount;
            
            float density;
            float nearDensity;
            
            int i;
            int j;

            float theDistance;
            float q;

            float pressure;
            float nearPressure;

            float rUnitX;
            float rUnitY;

            float displacementX;
            float displacementY;

            float dxX = 0.0f;
            float dxY = 0.0f;

            for (i = 0; i < currentNumParticles; i++)
            {
                theParticle = theParticles[i];
                density = 0;
                nearDensity = 0;

                List<BlobParticle> theNeighbours = theGrid.GetNeighbours(theParticle);
                neighbourCount = theNeighbours.Count;
                for (j = 0; j < neighbourCount; ++j)
                {
                    theNeighbour = theNeighbours[j];
                    rX = theNeighbour.vX - theParticle.vX;
                    rY = theNeighbour.vY - theParticle.vY;

                    if (rX == 0 && rY == 0)
                    {
                        rY = 0.01f;
                        rX = 0.01f;
                    }

                    theDistance = (float)Math.Abs(Math.Sqrt((rX * rX) + (rY * rY)));
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
                
                //foreach (BlobParticle theNeighbour in theNeighbours)
                for (j = 0; j < neighbourCount; j++)
                {
                    theNeighbour = theNeighbours[j];
					rX = theNeighbour.pX - theParticle.pX;
					rY = theNeighbour.pY - theParticle.pY;

                    if (rX == 0 && rY == 0)
                    {
                        rY = 0.01f;
                        rX = 0.01f;
                    }

                    theDistance = (float)Math.Abs(Math.Sqrt((rX * rX) + (rY * rY)));
                    q = theDistance / threshold;

                    if (q < 1)
                    {
                        // Get Unit Vector
                        rUnitX = Math.Abs(rX / theDistance);
                        rUnitY = Math.Abs(rY / theDistance);

                        // Apply Displacements
                        float displacementValue = (pressure * (1 - q)) + (nearPressure * (float)Math.Pow((1 - q), 2));
                        
						displacementX = (rX * displacementValue) * 2;
						displacementY = (rY * displacementValue) * 2;

                        theGrid.RemoveParticle(theNeighbour);
                        theNeighbour.pX += displacementX;
                        theNeighbour.pY += displacementY;
                        theGrid.AddParticle(theNeighbour);

                        dxX -= displacementX;                        
						dxY -= displacementY;
                    }
                }
                theGrid.RemoveParticle(theParticle);
                theParticle.pX += dxX;
				theParticle.pY += dxY;
                theGrid.AddParticle(theParticle);
            }
        }

        // Algorithm 3
        public void applySpringDisplacements()
        {
            //foreach (Spring theSpring in theSprings)
            Spring theSpring;
            float springLength;
            int i;

            float rX;
            float rY;

            float theDistance;

            float rUnitX;
            float rUnitY;

            float posModifierX;
            float posModifierY;

            float displacementValue;

            for (i = 0; i < currentSpringCount; i++)
            {
                theSpring = theSprings[i];
                springLength = theSpring.springLength;
            
				rX = theSpring.childParticle.pX - theSpring.parentParticle.pX;
				rY = theSpring.childParticle.pY - theSpring.parentParticle.pY;

                theDistance = (float)Math.Abs(Math.Sqrt((rX * rX) + (rY * rY)));

                //Vector2 unitR = r;


                displacementValue = springStiffness * (1 - (springLength / threshold)) * (springLength - theDistance);
                
                //normalise vector
                rUnitX = Math.Abs(rX / theDistance);
                rUnitY = Math.Abs(rY / theDistance);
                
                theGrid.RemoveParticle(theSpring.parentParticle);
                theGrid.RemoveParticle(theSpring.childParticle);
				
				//Vector2 posModifier = ((r * displacementValue) / 2);
                posModifierX = (rUnitX * displacementValue) / 2;
                posModifierY = (rUnitY * displacementValue) / 2;
                theSpring.parentParticle.pX -= posModifierX;
				theSpring.parentParticle.pY -= posModifierY;
                theSpring.childParticle.pX += posModifierX;
                theSpring.childParticle.pY += posModifierY;
				
                theGrid.AddParticle(theSpring.parentParticle);
                theGrid.AddParticle(theSpring.childParticle);
            }
        }

        // Algortihm 4
		//Needs rearranging to reduce the number of loopings.
        public void adjustSprings()
        {
            //foreach (BlobParticle theParticle in theParticles)
            BlobParticle theParticle;
            BlobParticle theNeighbour;
            int i;
            int neighbourCount;
            int j;

            float rX;
            float rY;

            float theDistance;
            float q;

            Spring theSpring;

            float deformation;

            List<BlobParticle> theNeighbours;

            float springLength;
            
            for (i = 0; i < currentNumParticles; i++)
            {
                theParticle = theParticles[i];
                theNeighbours = theGrid.GetNeighbours(theParticle);
                neighbourCount = theNeighbours.Count;
                for (j = 0; j < neighbourCount; j++)
                {
                    //Vector2 r = theNeighbours[j].position - theParticle.position;
                    theNeighbour = theNeighbours[j];
                    rX = theNeighbour.pX - theParticle.pX;
                    rY = theNeighbour.pY - theParticle.pY;

                    theDistance = (float)Math.Abs(Math.Sqrt((rX * rX) + (rY * rY)));
                    q = theDistance / threshold;

                    if (q < 1)
                    {
                        if (!connections[theParticle.idNumber][theNeighbour.idNumber])
                        {
                            // if not currently connected with a spring
                            // Create a spring
                            theSpring = new Spring(threshold, theParticle, theNeighbour);
                            connections[theParticle.idNumber][theNeighbour.idNumber] = true;
                            theSprings.Add(theSpring);
                            currentSpringCount++;
                        }
                    }
                }
            }

            // Possible Problem #1
            //foreach (Spring theSpring in theSprings)
			for (i = 0; i < currentSpringCount; i++)
            {
                theSpring = theSprings[i];
                //Vector2 r = theSprings[i].childParticle.position - theSprings[i].parentParticle.position;
                rX = theSpring.childParticle.pX - theSpring.parentParticle.pX;
                rY = theSpring.childParticle.pY - theSpring.parentParticle.pY;
                springLength = theSpring.springLength;
                if (rX == 0 && rY == 0)
                {
                    rY = 0.01f;
                    rX = 0.01f;
                }

                theDistance = (float)Math.Abs(Math.Sqrt((rX * rX) + (rY * rY)));

                deformation = yeildRatio * springLength;
                if (theDistance > (springLength + deformation))
                {
                    // Stretch
                    theSpring.springLength += plasticityConstant * (theDistance - springLength - deformation);
                }
                else if (theDistance < (springLength - deformation))
                {
                    // Compress
                    theSpring.springLength -= plasticityConstant * (springLength - deformation - theDistance);
                }
            }

            //foreach (Spring theSpring in theSprings)
            for(i = 0; i < currentSpringCount; i++)
            {
                theSpring = theSprings[i];
                springLength = theSpring.springLength;
                if (springLength > threshold)
                {
                    // Remove Spring
                    connections[theSpring.parentParticle.idNumber][theSpring.childParticle.idNumber] = false;
                    theSprings.Remove(theSpring);
                    currentSpringCount--;
                    i--; //danger will robinson!
                }
            }
        } 

        // Algorithm 5
        public void applyViscosity()
        {
            //foreach (BlobParticle theParticle in theParticles)
            BlobParticle theParticle;
            BlobParticle theNeighbour;

            float rX;
            float rY;

            int i;
            int j;

            int numNeighbours;

            List<BlobParticle> theNeighbours;

            float theDistance;
            float q;
            float u;

            float velX;
            float velY;

            float impulseX;
            float impulseY;

            float rUnitX;
            float rUnitY;

            for (i = 0; i < currentNumParticles; i++)
            {
                theParticle = theParticles[i];
                theNeighbours = theGrid.GetNeighbours(theParticles[i]);
                numNeighbours = theNeighbours.Count;
                //foreach (BlobParticle theNeighbour in theNeighbours)
                for (j = 0; j < numNeighbours; j++)
                {
                    theNeighbour = theNeighbours[j];
                    rX = theNeighbour.pX - theParticle.pX;
                    rY = theNeighbour.pY - theParticle.pY;
                    if (rX == 0 && rY == 0)
                    {
                        rY = 0.00000001f;
                        rX = 0.00000001f;
                    }

                    theDistance = (float)Math.Abs(Math.Sqrt((rX * rX) + (rY * rY)));
                    q = theDistance / threshold;

                    if (q < 1)
                    {
                        //normalise vector
                        rUnitX = Math.Abs(rX / theDistance);
                        rUnitY = Math.Abs(rY / theDistance);
                        
                        velX = theParticle.vX - theNeighbour.vX;
                        velY = theParticle.vY - theNeighbour.vY;
                        //u = Vector2.Dot((theParticles[i].velocity - theNeighbours[j].velocity), r);
                        //dot product
                        u = (velX + rUnitX) * (velY + rUnitY);
                        
                        if (u > 0)
                        {
                            // Linear and Quadratic Impulses - lol
                            float impulseValue = (float)(1 - q) * (float)(unknownVariableO * u + unknownVariableB * Math.Pow(u, 2));
                            impulseX = (rUnitX * impulseValue) / 2;
                            impulseY = (rUnitY * impulseValue) / 2;
                            if (impulseX > 100)
                            {
                                float p = 0;
                            }
                            theParticle.vX -= impulseX;
                            theParticle.vY -= impulseY;

                            theNeighbour.vX += impulseX;
                            theNeighbour.vY += impulseY;
                        }
                    }
                }
            }
        }

        // Algorithm 6
        public void resolveCollisions()
        {

            //foreach (BlobParticle theParticle in theParticles)
            BlobParticle theParticle;
            for (int i = 0; i < currentNumParticles; i++)
            {
                theParticle = theParticles[i];

                theGrid.RemoveParticle(theParticle);
                //theParticle.velocity = (theParticle.position - theParticle.previousPosition);
                theParticle.vX = theParticle.pX - theParticle.ppX;
                theParticle.vY = theParticle.pY - theParticle.ppY;

                if (theParticle.pY > 550)
                {
                    theParticle.pY = 550;
                    theParticle.vY *= -0.4f;
                }
                else if (theParticle.pY < 0)
                {
                    theParticle.pY = 0;
                    theParticle.vY *= -0.4f;
                }

                if (theParticle.pX > 700)
                {
                    theParticle.pX = 700;
                    theParticle.vX *= -0.4f;
                }
                else if (theParticle.pX < 0)
                {
                    theParticle.pX = 0;
                    theParticle.vX *= -0.4f;
                }
                theGrid.AddParticle(theParticle);

            }
        }

       /* public void resolveCollisions_alt()
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
        }*/

    }
}