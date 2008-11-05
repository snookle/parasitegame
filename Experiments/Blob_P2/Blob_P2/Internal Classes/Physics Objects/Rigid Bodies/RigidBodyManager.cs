using System;
using System.Xml;
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
    public class RigidBodyManager : StaticBodyManager
    {
        public RigidBodyManager(Game game)
            :base(game)
        {
        }

        public override void NewBody(Color colour, params Vector2[] vertices)
        {
            RigidBody rb = new RigidBody(PhysicsOverlord.GetInstance().GetID(), GraphicsDevice, colour, vertices);
            NewBody(rb);
        }

        public override void NewBody(PhysicsObject newbody)
        {
            bodies.Add(newbody.id, newbody);
            SpatialGrid.GetInstance().AddObject(newbody);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            foreach (RigidBody body in bodies.Values)
            {
                body.applyForce(new Vector2(0, 0.5f));
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (RigidBody body in bodies.Values)
            {
                body.Draw();
            }

            base.Draw(gameTime);
        }



    }
}
