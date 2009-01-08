using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blob_P2
{
    public class BlobParticle: PhysicsObject
    {
        public Vector2 velocity;
        //public Vector2 Position;
        public Vector2 centre;
        public Texture2D sprite;
        public Color colour = Color.Black;
        public float radius;
        public float radiusSquared;
        public Vector2 oldPosition;
        public Vector2 currentForce;

        public List<BlobParticle> neighbours;

        public BlobParticle(Game game, Vector2 Position, Texture2D sprite, int id, float radius) : base(game)
        {
            this.Position = Position;
            this.sprite = sprite;
            this.id = id;

            this.radius = radius;
            this.radiusSquared = radius * radius;
            this.type = PhysicsObjectType.potBlobParticle;
            centre = new Vector2(sprite.Width / 2, sprite.Height / 2);
            neighbours = new List<BlobParticle>();
            oldPosition = Position;
        }

        public void applyForce(Vector2 theForce)
        {
            currentForce = theForce;
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
