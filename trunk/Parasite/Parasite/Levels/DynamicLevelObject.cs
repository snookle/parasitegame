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
            Texture = game.Content.Load<Texture2D>(textureName);
            BodyDef def = new BodyDef();
            def.Position.Set(startingPosition.X, startingPosition.Y);
            SetBodyDefinition(def);
            PolygonDef polyDef = new PolygonDef();
            polyDef.SetAsBox(Texture.Width / 4, Texture.Height / 4);
            polyDef.Friction = 0.3f;
            polyDef.Density = 1.0f;
            AddShapeDefinition(polyDef);
            PhysicsBody.SetMassFromShapes();
        }
    }
}
