using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ViscoelasticXNAPrototype
{
    class Neighbour
    {
        public BlobParticle theParticle;
        public Spring theSpring;

        public Neighbour(BlobParticle theParticle)
        {
            this.theParticle = theParticle;
        }
    }
}
