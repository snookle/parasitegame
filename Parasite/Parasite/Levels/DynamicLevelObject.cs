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
        float Mass
        {
            get
            {
                if (PhysicsBody == null || PhysicsBody.Mass == 0)
                {
                    return 10;
                }
                else
                {
                    return PhysicsBody.Mass;
                }
            }
            set
            {
                if (PhysicsBody == null)
                    return;
                PhysicsBody.Mass = value;
            }
        }

        float Friction
        {
            get
            {
                if (PhysicsGeometry == null)
                {
                    return 0;
                }
                else
                    return PhysicsGeometry.FrictionCoefficient;
            }
            set
            {
                if (PhysicsGeometry == null)
                    return;
                else
                    PhysicsGeometry.FrictionCoefficient = value;
            }
        }
        float Bounciness
        {
            get
            {
                if (PhysicsGeometry == null)
                {
                    return 0;
                }
                else
                    return PhysicsGeometry.RestitutionCoefficient;
            }

            set
            {
                if (PhysicsGeometry == null)
                    return;
                else
                    PhysicsGeometry.RestitutionCoefficient = value;
            }
        }

        public DynamicLevelObject(Game game, Vector2 startingPosition, string textureName, float mass, float bounce, float friction)
            : base(game, startingPosition, textureName, true)
        {
            Mass = mass;
            Friction = friction;
            Bounciness = bounce;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            PhysicsBody.IsStatic = false;
           // PhysicsBody.Mass = Mass;
           // PhysicsGeometry.RestitutionCoefficient = Bounciness;
            //PhysicsGeometry.FrictionCoefficient = Friction;
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
