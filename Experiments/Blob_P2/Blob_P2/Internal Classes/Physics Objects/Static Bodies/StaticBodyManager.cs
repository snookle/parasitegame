using System;
using System.Xml;
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
            NewBody(sb);
        }

        public void NewBody(StaticBody newbody)
        {
            bodies.Add(newbody.id, newbody);
            SpatialGrid.GetInstance().AddObject(newbody);
        }

        public void Load()
        {
            XmlReader reader = XmlReader.Create("bob.xml");
            bool done = false;
            while (!done)
            {
                reader.Read();
                if (reader.Name == "StaticBodyList")
                {
                    //processs static body data
                    reader.ReadStartElement("StaticBodyList");
                    reader.ReadAttributeValue(); //read count;
                    if (reader.Name != "Count")
                        throw new Exception("INVALID XML!");
                    int count = Convert.ToInt32(reader.Value);
                    for (int i = 0; i < count; i++)
                    {
                        StaticBody sb = new StaticBody(reader);
                        
                    }
                    
                    done = true;
                }
            }
        }

        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            //settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create("bob.xml", settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("StaticBodyList");
            writer.WriteAttributeString("Count", bodies.Count.ToString());
            foreach (StaticBody sb in bodies.Values)
            {
                sb.SaveBody(ref writer);
            }
            writer.WriteEndElement(); //</StaticBodyList>
            writer.WriteEndDocument();
            writer.Close();
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