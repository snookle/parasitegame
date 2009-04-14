using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;


namespace Parasite
{
    class DynamicLevelObject : LevelArt
    {
        public DynamicLevelObject(Game game, Vector2 startingPosition, string textureName)
            : base(game, startingPosition, textureName, true)
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();
            PhysicsBody.IsStatic = false;
            PhysicsBody.Mass = 10;
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
