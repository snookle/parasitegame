using System;
using System.Collections.Generic;
using System.Text;

namespace ViscoelasticXNAPrototype
{
    class Spring
    {
        public float springLength;
        public BlobParticle parentParticle;
        public BlobParticle childParticle;

        public Spring(float springLength,BlobParticle parentParticle,BlobParticle childParticle)
        {
            this.springLength = springLength;
            this.parentParticle = parentParticle;
            this.childParticle = childParticle;
        }
    }
}
