using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blob_P2
{
    public class ParasiteBodyPart: PhysicsObject
    {
        public Texture2D sprite;

        public Vector2 velocity = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        public Vector2 force = Vector2.Zero;
        public Vector2 centre = Vector2.Zero;
        public Vector2 oldPosition = Vector2.Zero;
        public ParasiteStatus status;

        public Vector2 relativeGravity;

        public enum ParasiteStatus { crawling, climbing, swimming };

        public float rotation;

        public ParasiteBodyPart nextPart;       // Possibly Control this via 'Parasite'
        public ParasiteBodyPart prevPart;

        public IKMember IKPoint = null;



        public float weight;

        //public float id;        // id for the spatial grid

        public ParasiteBodyPart(int id, Texture2D sprite, float weight)
        {
            this.id = id;
            this.type = PhysicsObjectType.potParasiteBodyPart;

            this.sprite = sprite;
            this.weight = weight;

            centre = new Vector2(sprite.Width / 2, sprite.Height / 2);

            this.position = new Vector2(100, 100);
        }

        public void Init()
		{
            force = Vector2.Zero;
		}

        public void AddIKPoint(IKMember thePoint)
        {
            IKPoint = thePoint;
        }

        public void ApplyForce(Vector2 newForce)
        {
            this.velocity += newForce;
        }

        /*
         * Currently Being Performed in a ... really odd fashion
         */
        public virtual void UpdatePoint()
        {
            doCollisionCheck();

            oldPosition = position;

            SpatialGrid.GetInstance().RemoveObject(this);
            velocity += force;

            if (IKPoint != null)
            {
               IKPoint.update();
            }

            velocity *= 0.95f;
            position += velocity;

            if (position.Y > 500)
            {
                position.Y = 500;
                velocity.Y = 0;
            }
            else if (position.Y < 0)
            {
                position.Y = 0;
                velocity.Y = 0;
            }

            if (position.X > 800)
            {
                position.X = 800;
                velocity.X *= -0.5f;
            }
            else if (position.X < 0)
            {
                position.X = 0;
                velocity.X *= -0.5f;
            }
            SpatialGrid.GetInstance().AddObject(this);
        }

        public void doCollisionCheck()
        {
            List<PhysicsObject> neighbours;
            PhysicsObject neighbourObject;
            int neighbourCount;

            int particleCount = 0;

            neighbours = SpatialGrid.GetInstance().GetPNeighbours(this);
            neighbourCount = neighbours.Count;

            for (int j = 0; j < neighbourCount; j++)
            {
                neighbourObject = neighbours[j];
                // IF STATIC BODY
                Vector2 result;
                if (neighbourObject.type == PhysicsObjectType.potStaticBody)
                {
                    if ((result = ((StaticBody)neighbourObject).Collides(this)) != Vector2.Zero)
                    {
                        // ATTEMPTED NEW COLLISION CODE
                        float kr = 0f;
                        float friction = 1f;

                            Vector2 tempVel = this.velocity;
                            //Vector2 tempVel = this.velocity;
                            Vector2 velocityN = Vector2.Dot(tempVel,result) * result;
                            Vector2 velocityT = tempVel - velocityN;

                            // Check if object is moving towards wall.
                            //if (Vector2.Dot(tempVel, result) < 0)
                            //{
                                SpatialGrid.GetInstance().RemoveObject(this);
                                // No Friction
                                //Vector2 newVel = velocityT - kr * velocityN;

                                // Fake Friction
                                Vector2 newVTan = Vector2.Max(Vector2.Zero, velocityT - velocityT * friction);
                                Vector2 newVel = newVTan + kr * velocityN;

                                this.velocity = newVel;
                                this.position = this.oldPosition;

                                //if (Vector2.Dot(tempVel, result) < 1)
                                //{
                                    Vector2 contactForce = -(Vector2.Dot(result, position-oldPosition) * result);
                                    //this.velocity = contactForce;
                                    this.ApplyForce(contactForce);
                                //}
                                this.position += this.velocity;
                                SpatialGrid.GetInstance().AddObject(this);
                            //}
                        //status = ParasiteStatus.crawling;
                    }
                }
                else if (neighbourObject.type == PhysicsObjectType.potBlobParticle)
                {
                    // IF COLLIDING : DISABLE GRAVITY
                    relativeGravity = new Vector2(0, 1.5f);
                    particleCount++;
                }
                else
                {
                    // relativeGravity = new Vector2(0, 0.9f);
                }

            }

            if (particleCount == 0)
            {
                relativeGravity = new Vector2(0, 0.9f);
                status = ParasiteStatus.crawling;
            }
            else
            {
                this.velocity *= 0.93f;
                status = ParasiteStatus.swimming;
            }
        }

        public void SpecialMovement()
        {
        }
    }    
}
