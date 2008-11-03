using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Blob_P2
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class StaticBodyManager : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private Dictionary<int, StaticBody> bodies = new Dictionary<int, StaticBody>();
        public StaticBodyManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Creates a new Static Body
        /// </summary>
        /// <param name="colour">The Colour of the Static Body</param>
        /// <param name="vertices">The vertices that make up the shape</param>
        public void NewBody(Color colour, params Vector2[] vertices)
        {
            StaticBody sb = new StaticBody(PhysicsOverlord.GetInstance().GetID(), GraphicsDevice, colour, vertices);

            bodies.Add(sb.id, sb);
            SpatialGrid.GetInstance().AddObject(sb);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(StaticBody body in bodies.Values)
            {
                //StaticBody body = bodies[i];
                body.Draw();
            }
            base.Draw(gameTime);
        }
    }
}