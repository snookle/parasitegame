using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IKParasite_xna
{
    class ParasiteBodyPart
    {
        public Texture2D sprite;

        public Vector2 velocity = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        public Vector2 force = Vector2.Zero;
        public Vector2 centre = Vector2.Zero;

        public float rotation;

        public ParasiteBodyPart nextPart;       // Possibly Control this via 'Parasite'
        public ParasiteBodyPart prevPart;

        public IKMember IKPoint = null;

        public float weight;

        public ParasiteBodyPart(Texture2D sprite, float weight)
        {
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
        }

        public void SpecialMovement()
        {
        }
    }    
}
