using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blob_P2
{
    class ParasiteHead : ParasiteBodyPart
    {
        public ParasiteHead(int id, Texture2D sprite, float weight)
            : base(id, sprite, weight)
        {
        }

    }
}
