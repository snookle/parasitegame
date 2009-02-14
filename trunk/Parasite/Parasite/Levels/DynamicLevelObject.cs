using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;

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

            // Crate the body definition
            BodyDef def = new BodyDef();
            def.Position.Set(startingPosition.X, startingPosition.Y);
            def.Angle = StartingRotation;

            // Creates the body, and adds it to the world
            SetBodyDefinition(def);

            /*CircleDef circleDef = new CircleDef();
            circleDef.Radius = 5;
            circleDef.Friction = 0.3f;
            circleDef.Density = 1.0f;
            AddShapeDefinition(circleDef);*/

            PolygonDef polyDef = new PolygonDef();
            if (textureName != "")
            {
                polyDef.SetAsBox(Texture.Width / 2, Texture.Height / 2);
            }
            else
            {
                polyDef.SetAsBox(dimensions.X, dimensions.Y);
            }
            polyDef.Friction = 0.3f;
            polyDef.Density = 1.0f;
            AddShapeDefinition(polyDef);
            PhysicsBody.SetMassFromShapes();
        }

        /// <summary>
        /// Resets DLO to it's starting position and rotation
        /// </summary>
        public void Reset()
        {
            WorldPosition = StartingPosition;
            Rotation = StartingRotation;

            PhysicsBody.PutToSleep();

            PhysicsBody.SetXForm(new Box2DX.Common.Vec2(StartingPosition.X, StartingPosition.Y), Rotation);
            PhysicsBody.SetLinearVelocity(new Box2DX.Common.Vec2(0, 0));
            PhysicsBody.SetAngularVelocity(0);

            PhysicsBody.WakeUp();

            //while(PhysicsBody.GetShapeList().GetNext()!=null){
            //    PhysicsBody.DestroyShape(PhysicsBody.GetShapeList().GetNext());
            //}
        }
    }
}
