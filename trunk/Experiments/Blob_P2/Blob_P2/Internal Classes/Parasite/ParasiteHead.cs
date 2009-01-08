using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blob_P2
{
    public class ParasiteHead : ParasiteBodyPart
    {
        public ParasiteHead(Game game, int id, Texture2D sprite, float weight)
            : base(game, id, sprite, weight)
        {
        }

    }
}
