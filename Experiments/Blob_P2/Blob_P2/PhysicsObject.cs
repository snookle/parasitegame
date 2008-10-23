using System;
using System.Collections.Generic;
using System.Text;

namespace Blob_P2
{
    enum PhysicsObjectType {potBlobParticle, potStaticBody};
    class PhysicsObject
    {
        public int id;
        public PhysicsObjectType type;
    }
}
