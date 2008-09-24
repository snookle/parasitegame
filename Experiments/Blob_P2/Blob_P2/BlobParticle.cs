using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blob_P2
{
    class BlobParticle : PhysicsObject
    {
        public Vector2 velocity;
        public Vector2 position;
        public Vector2 centre;
        public Texture2D sprite;
        public Color colour = Color.Black;

        public float threshold = 20;
        public float disconnectThreshold = 20;
        public int numLinks = 6;

        public List<BlobParticle> neighbours;

        public BlobParticle(Vector2 position, Texture2D sprite, int id)
        {
            this.position = position;
            this.sprite = sprite;
            this.id = id;

            centre = new Vector2(sprite.Width / 2, sprite.Height / 2);

            neighbours = new List<BlobParticle>();
        }

        public void applyForce(Vector2 theForce)
        {
            this.velocity += theForce;
        }

        public void addNeighbour(BlobParticle theNeighbour){
            neighbours.Add(theNeighbour);
        }

        public void removeNeighbour(BlobParticle theNeighbour){
            neighbours.Remove(theNeighbour);
        }

    }
}
