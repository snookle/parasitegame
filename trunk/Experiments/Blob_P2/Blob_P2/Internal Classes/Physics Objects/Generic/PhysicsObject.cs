using System;
using System.Collections.Generic;
using System.Text;

namespace Blob_P2
{
    public enum PhysicsObjectType { potBlobParticle, potStaticBody, potParasiteBodyPart };
    public class PhysicsObject
    {
        public int id;
        public PhysicsObjectType type;
    }
}
