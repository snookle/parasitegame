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


namespace Parasite
{
    class LevelArt : SceneNode
    {
        
        private Game game;
        public Texture2D Texture;
        public Color Tint = Color.White;
        //name of the content resource for this texture
        private string TextureName;
        public Rectangle BoundingBox;

        public Vector2 Origin;
        public Vector2 Scale;
        public float Rotation;

        public LevelArt(Game game, Vector2 startingPosition, string textureName) : base (game, startingPosition)
        {
            this.game = game;
            TextureName = textureName;
            LoadContent();
        }

        public void LoadContent()
        {
            Texture = game.Content.Load<Texture2D>(TextureName);
            BoundingBox = new Rectangle((int)WorldPosition.X - Texture.Width / 2, (int)WorldPosition.Y - Texture.Height / 2, Texture.Width, Texture.Height);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Scale = new Vector2(1, 1);
            Rotation = 0f;
        }


        private bool editorSelected;
        public void EditorSelect(bool select)
        {
            editorSelected = select;
            if (editorSelected)
                Tint = Color.Green;
            else
                Tint = Color.White;
        }

        /// <summary>
        /// Update the bounding box for an object as it is moved
        /// </summary>
        public void EditorMove(Vector2 offset)
        {
            //selectedArt.WorldPosition = camera.MouseToWorld();
            WorldPosition = camera.MouseToWorld() - offset;
            BoundingBox = new Rectangle((int)WorldPosition.X - (int)Origin.X, (int)WorldPosition.Y - (int)Origin.Y, Texture.Width, Texture.Height);
        }
    }
}
