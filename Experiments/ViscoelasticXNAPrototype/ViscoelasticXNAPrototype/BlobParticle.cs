using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ViscoelasticXNAPrototype
{
    class BlobParticle
    {
        //public Vector2 position;            // Particle Position
        public float pX;
        public float pY;
        //public Vector2 velocity;            // Particle Velocity
        public float vX;
        public float vY;

        //public Vector2 previousPosition;    // Previous Particle Position
        public float ppX;
        public float ppY;

        public int idNumber;
        public Texture2D sprite;
        public int numSprings;

        public Vector2 centre;

        public List<Neighbour> neighbours;

        public BlobParticle(Vector2 position,int idNumber, Texture2D sprite)
        {
            this.pX = position.X;
            this.pY = position.Y;
            this.idNumber = idNumber;
            this.vX = 0.0f;
            this.vY = 0.0f;
            this.sprite = sprite;

            centre = new Vector2(sprite.Width / 2, sprite.Height / 2);

            this.neighbours = new List<Neighbour>();
        }

        public void applyForce(float forceX, float forceY)
        {
            vX += forceX;
            vY += forceY;
        }
    }
}
