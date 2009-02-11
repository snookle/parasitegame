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

        public DynamicLevelObject(Game game, Vector2 startingPosition, string textureName): base(game, startingPosition)
        {
            // Load the Appropriate Texture
             Texture = game.Content.Load<Texture2D>(textureName);
            //Texture = null;

            // Crate the body definition
            BodyDef def = new BodyDef();
            def.Position.Set(startingPosition.X, startingPosition.Y);
            def.Angle = 0.50f;

            // Creates the body, and adds it to the world
            SetBodyDefinition(def);

            PolygonDef polyDef = new PolygonDef();
            polyDef.SetAsBox(Texture.Width / 2, Texture.Height / 2);
            //polyDef.SetAsBox(10, 10);
            polyDef.Friction = 0.3f;
            polyDef.Density = 1.0f;
            AddShapeDefinition(polyDef);
            PhysicsBody.SetMassFromShapes();
        }
    }
}
