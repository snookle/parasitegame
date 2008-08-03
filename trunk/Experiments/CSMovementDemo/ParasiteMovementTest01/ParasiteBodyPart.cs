using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ParasiteMovementTest01
{
    class ParasiteBodyPart
    {
        public Vector2 position;
        public Vector2 targetPosition;
        public Vector2 velocity;
        public int weight;
        public Texture2D sprite;
        public float rotation;
        public Vector2 centre;

        public ParasiteBodyPart(Texture2D loadedTexture, int _weight)
        {
            rotation = 0.0f;
            position = Vector2.Zero;
            sprite = loadedTexture;
            centre = new Vector2(sprite.Width / 2, sprite.Height / 2);
        }

        public void updatePosition(Vector2 theParentPosition)
        {
            targetPosition.X = theParentPosition.X + centre.X * 1.5f;
            float ax = (targetPosition.X - position.X) * 0.2f;

            position.X += ax;
            position.Y = theParentPosition.Y;
            
        }
    }
}
