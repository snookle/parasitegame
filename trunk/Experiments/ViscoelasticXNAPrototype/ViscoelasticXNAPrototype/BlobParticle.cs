using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ViscoelasticXNAPrototype
{
    class BlobParticle
    {
        public Vector2 position;            // Particle Position
        public Vector2 velocity;            // Particle Velocity
        public Vector2 previousPosition;    // Previous Particle Position
        public int idNumber;
        public Texture2D sprite;

        public BlobParticle(Vector2 position,int idNumber, Texture2D sprite)
        {
            this.position = position;
            this.velocity = new Vector2(0, 0);
            this.sprite = sprite;
        }

        public void applyForce(Vector2 force)
        {
            this.velocity += force;
        }
    }
}
