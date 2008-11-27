using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Blob_P2
{
    public class RigidBody : StaticBody
    {
        public Vector2 velocity;
        //public Vector2 position;

        public RigidBody(int id, GraphicsDevice graphics, Color drawColour, params Vector2[] newpoints)
            : base(id,graphics,drawColour,newpoints)
        {
            velocity = Vector2.Zero;
        }

        public override void Draw()
        {
            SpatialGrid.GetInstance().RemoveObject(this);
            // Apply Velocity
            for (int i = 0; i < sourceVertices.Length; i++)
            {
                sourceVertices[i] += this.velocity;
            }
            BoundingBox = new BoundingRectangle(sourceVertices);
            TriangulatePoly();
            SpatialGrid.GetInstance().AddObject(this);
            // Call Parent Draw Methods
            base.Draw();
        }

        public void applyForce(Vector2 theForce)
        {
            this.velocity += theForce;
        }
    }
}
