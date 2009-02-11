using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Box2DX;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace Parasite
{
    public interface IPhysicsManager { }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PhysicsManager : Microsoft.Xna.Framework.DrawableGameComponent, IPhysicsManager
    {
        private World world;
        Body groundBody;

        public PhysicsManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPhysicsManager), this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            world.SetDebugDraw(new PhysicsDebugDraw(Game));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Define the World
            AABB worldAABB;

            // Define the Upper, and Lower bounds
            worldAABB.UpperBound = new Vec2(200,200);
            worldAABB.LowerBound = new Vec2(-200,-200);

            // Define Gravity
            Vec2 gravity = new Vec2(0f, 10f);

            world = new World(worldAABB, gravity, true);

            // The Ground
            BodyDef groundDef = new BodyDef();
            groundDef.Position.Set(0, 100);
            groundBody = world.CreateBody(groundDef);

            // Ground Polygon Definition
            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox(100, 5);
            groundBody.CreateShape(shapeDef);
            base.Initialize();
        }

        public void AddStaticCollisionMesh(Vector2[] vs)
        {
            if (vs.Length > 4)
            {
                throw new Exception("Collision meshes must not have more than 4 verticies.");
            }
            PolygonDef sp = new PolygonDef();
            sp.VertexCount = 4;
            sp.Type = ShapeType.PolygonShape;
            for(int i = 0; i < vs.Length; i++)
            {
                Vector2 vec = vs[i];
                sp.Vertices[i].Set(vec);
            }
            groundBody.CreateShape(sp);

        }

        public Body CreateBody(BodyDef def)
        {
            return world.CreateBody(def);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //gameTime.ElapsedGameTime.TotalMilliseconds
            //simulate the world.

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            world.Step(1.0f / 60f, 10, 10);
            base.Draw(gameTime);
        }
    }
}