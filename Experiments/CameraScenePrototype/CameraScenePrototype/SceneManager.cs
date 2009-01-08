using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
namespace CameraScenePrototype
{
    class SceneManager
    {
        Game game;
        public SceneCameraComponent Camera;
        List<SceneNode> nodes = new List<SceneNode>();
        SpriteBatch batch;

        public SceneManager(Game game)
        {
            this.game = game;
            batch = new SpriteBatch(game.GraphicsDevice);
            Camera = new SceneCameraComponent(game);
            game.Components.Add(Camera);
        }

        public void Initialise()
        {
            
        }

        public void Add(SceneNode newNode)
        {
            nodes.Add(newNode);
        }

        public void Update()
        {
            foreach (SceneNode node in nodes)
            {
                node.Update();
            }
        }

        public void Draw(GameTime time)
        {
            batch.Begin();
            foreach (SceneNode node in nodes)
            {
                node.Draw(time, batch);
            }
            batch.End();
        }
    }
}
