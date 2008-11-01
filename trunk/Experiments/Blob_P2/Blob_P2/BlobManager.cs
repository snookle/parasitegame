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

        public float threshold = 30;
        public float disconnectThreshold = 30;
        public int numLinks = 6;
        public float particleRadius = 5.0f;

        // Spatial Grid
        
        // Forces
        public Vector2 gravity = new Vector2(0, 0.5f);

        // Spring Variables
        private List<Spring> theSprings;
        public float springStiffness = 0.3f;
        public float springLength = 30;
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
        //blob shading
        Effect blobShader;
        EffectParameter waveParam, distortionParam, centerCoordParam;
        RenderTarget2D blobRenderTarget;
        Texture2D blobTexture;
        Vector2 blobCenterCoord = new Vector2(0.5f);

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
            theSprite = this.Game.Content.Load<Texture2D>("Sprites\\100xparticle");

            spriteFont = Game.Content.Load<SpriteFont>("DebugFont");

            blobShader = Game.Content.Load<Effect>("blobshader");
            waveParam = blobShader.Parameters["wave"];
            distortionParam = blobShader.Parameters["distortion"];
            centerCoordParam = blobShader.Parameters["centerCoord"];

            blobRenderTarget = Game1.CloneRenderTarget(GraphicsDevice, 1);
            blobTexture = new Texture2D(GraphicsDevice, blobRenderTarget.Width, blobRenderTarget.Height, 1,
                TextureUsage.None, blobRenderTarget.Format);


            initSimulation();
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            //save old render target (backbuffer)
            RenderTarget2D temp = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
            
            //render to the blob rendertarget
            GraphicsDevice.SetRenderTarget(0, blobRenderTarget);
            GraphicsDevice.Clear(Color.White);


            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            BlobParticle theParticle;
            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            for (int i = 0; i < particleCount; i++)
            {
                theParticle = theParticles[i];
                theParticle.colour = Color.Red;

                spriteBatch.Draw(theSprite, theParticle.position, null, theParticle.colour, 0, theParticle.centre, 1.0f, SpriteEffects.None, 1);

            }
            spriteBatch.End();

            //set the render target back to our old one
            GraphicsDevice.SetRenderTarget(0, temp);
            //grab the texture we just drew particles on
            blobTexture = blobRenderTarget.GetTexture();

            // use Immediate mode and our effect to draw the scene again, using our pixel shader.
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            blobShader.Begin();
            blobShader.CurrentTechnique.Passes[0].Begin();
            spriteBatch.Draw(blobTexture, Vector2.Zero, Color.White);
            spriteBatch.End();
            blobShader.CurrentTechnique.Passes[0].End();
            blobShader.End();

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

            Game1.grid = new SpatialGrid(800, 600, disconnectThreshold);

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

                BlobParticle theParticle = new BlobParticle(new Vector2(xPos, yPos), theSprite, PhysicsOverlord.GetInstance().GetID(), particleRadius);
                theParticle.colour = Color.Red;
                theParticle.velocity = new Vector2(5,5);

                theParticles.Add(theParticle);
                Game1.grid.AddObject(theParticle);

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
            PhysicsObject neighbourObject;
            Vector2 differenceVector;
            int neighourCount;
            int j;
            float distance;
            List<PhysicsObject> neighbours;

            for (int i = 0; i < particleCount; i++)
            {
                theParticle = theParticles[i];
                Game1.grid.RemoveObject(theParticle);
                theParticle.oldPosition = theParticle.position;
                theParticle.position += theParticle.velocity;

                // Apply Gravity
                theParticle.applyForce(gravity);

                simpleCollisionDetection(theParticle);
                Game1.grid.AddObject(theParticle);

                //BEGIN CHECK SPRINGS
                neighbours = Game1.grid.GetNeighbours(theParticle);
                neighourCount = neighbours.Count;
                for (j = 0; j < neighourCount; j++)
                {
                    neighbourObject = neighbours[j];
                    if (neighbourObject.type == PhysicsObjectType.potBlobParticle)
                    {
                        differenceVector = theParticle.position - ((BlobParticle)neighbourObject).position;
                        distance = differenceVector.Length();

                        if (distance <= threshold && connected[theParticle.id][((BlobParticle)neighbourObject).id] == null)
                        {
                            theParticle.addNeighbour(((BlobParticle)neighbourObject));
                            ((BlobParticle)neighbourObject).addNeighbour(theParticle);

                            connected[theParticle.id][((BlobParticle)neighbourObject).id] = new Spring(theParticle, ((BlobParticle)neighbourObject), springStiffness, springLength, springFriction);
                        }
                        else if (distance > disconnectThreshold && connected[theParticle.id][((BlobParticle)neighbourObject).id] != null)
                        {
                            theParticle.removeNeighbour(((BlobParticle)neighbourObject));
                            ((BlobParticle)neighbourObject).removeNeighbour(theParticle);
                            connected[theParticle.id][((BlobParticle)neighbourObject).id] = null;
                        }

                        if (connected[theParticle.id][((BlobParticle)neighbourObject).id] != null)
                        {
                            //connected[theParticle.id][neighbourParticle.id].viscoSolve();
                            connected[theParticle.id][((BlobParticle)neighbourObject).id].solve();

                        }
                    }
                    else if (neighbourObject.type == PhysicsObjectType.potStaticBody)
                    {
                        Vector2 result;
                        if ((result = ((StaticBody)neighbourObject).Collides(theParticle)) != Vector2.Zero)
                        {
                            theParticle.velocity = Vector2.Reflect(theParticle.velocity, result);

                            //move the partcle back before it collided to stop particles getting stuck.
                //            while (((StaticBody)neighbourObject).Collides(theParticle) != Vector2.Zero) {
                  //              theParticle.position *= theParticle.velocity;
                    //        }
                            //theParticle.position = theParticle.oldPosition;
                        }
                         
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

                List<BlobParticle> theNeighbours = null;// grid.GetNeighbours(theParticle);
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