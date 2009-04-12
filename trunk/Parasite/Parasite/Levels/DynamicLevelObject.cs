using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Parasite
{
    class DynamicLevelObject : SceneNode
    {
        public Texture2D Texture = null;

        public DynamicLevelObject(Game game, Vector2 startingPosition, string textureName, Vector2 dimensions): base(game, startingPosition)
        {
            StartingPosition = startingPosition;
            //StartingRotation = 0.50f;
            // Load the Appropriate Texture
            if (textureName != "")
            {
                Texture = game.Content.Load<Texture2D>(textureName);
                this.Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            }
            else
            {
                Texture = null;
            }

            // Create the body and geom definition
        }

        /// <summary>
        /// Resets DLO to it's starting position and rotation
        /// </summary>
        public void Reset()
        {
            WorldPosition = StartingPosition;
            Rotation = StartingRotation;
        }
    }
}
