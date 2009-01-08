using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blob_P2
{
    public enum PhysicsObjectType { potBlobParticle, potStaticBody, potParasiteBodyPart };
    public class PhysicsObject : SceneNode
    {
        public PhysicsObject(Game game)
            : base(game, "", Vector2.Zero)
        { }
        public int id;
        public PhysicsObjectType type;
    }
}
