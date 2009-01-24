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
        
        public Texture2D Texture;
        public Color Tint = Color.White;
        
        //name of the content resource for this texture
        //ie: "LevelArt\\WallTest01"
        private string TextureName;

        public Rectangle BoundingBox;

        public Vector2 Origin;
        public Vector2 Scale;
        public float Rotation;

        public bool EditorSelected;

        private PrimitiveBatch boundingBatch;

        public LevelArt(Game game, Vector2 startingPosition, string textureName) : base (game, startingPosition)
        {
            this.game = game;
            TextureName = textureName;
            LoadContent();
        }

        public LevelArt(Game game, Vector2 startingPosition, Texture2D texture)
            : base(game, startingPosition)
        {
            this.game = game;
            Texture = texture;
            BoundingBox = new Rectangle((int)WorldPosition.X - Texture.Width / 2, (int)WorldPosition.Y - Texture.Height / 2, Texture.Width, Texture.Height);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Scale = new Vector2(1, 1);
            Rotation = 0f;
            boundingBatch = new PrimitiveBatch(game.GraphicsDevice);
        }

        public void LoadContent()
        {
            Texture = game.Content.Load<Texture2D>(TextureName);
            BoundingBox = new Rectangle((int)WorldPosition.X - Texture.Width / 2, (int)WorldPosition.Y - Texture.Height / 2, Texture.Width, Texture.Height);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Scale = new Vector2(1, 1);
            Rotation = 0f;
            boundingBatch = new PrimitiveBatch(game.GraphicsDevice);
        }
        
        /// <summary>
        /// Notify this piece that it's been selected, and tint it a different colour so the user
        /// knows it too.
        /// </summary>
        /// <param name="select">Whether or not this level art has been selected by the editor</param>
        public void EditorSelect(bool select)
        {
            EditorSelected = select;
            if (EditorSelected)
                Tint = Color.Yellow;
            else
                Tint = Color.White;
        }

        /// <summary>
        /// Checks if selected point on the texture is transparent
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CheckTrans(Vector2 position)
        {
            position += Origin;
            Rectangle sourceRectangle = new Rectangle((int)position.X, (int)position.Y, 1, 1);
            Color[] retrievedColour = new Color[1];

            Texture.GetData<Color>(0, sourceRectangle, retrievedColour, 0, 1);

            if (retrievedColour[0] == Color.TransparentBlack || retrievedColour[0] == Color.TransparentWhite)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the bounding box and world position for an object as it is moved by the editor
        /// </summary>
        /// <param name="offset">The offset from the art origin to the mouse cursor</param>
        /// <param name="snapAmount">The resolution of the "snap to grid" 1 means no snap. 2 means snap to every 2nd pixel, 5 to every 5th pixel etc. </param>
        public void EditorMove(Vector2 offset, int gridResolution)
        {
            if (gridResolution <= 0)
                throw new Exception("GRID RESOLUTION CANNOT BE <= 0!");

            //selectedArt.WorldPosition = camera.MouseToWorld();
            Vector2 tempWorldPos = camera.MouseToWorld() - offset;
            
            //find the direction that we're going to move too.
            Vector2 direction = Vector2.Subtract(tempWorldPos, WorldPosition);
            
            //set the world position to where the mouse is pointing.
            WorldPosition = tempWorldPos;
            

            //snap the piece to the grid
            while (WorldPosition.X % gridResolution != 0)
            {
                if (direction.X > 0)
                    WorldPosition.X -= 1;
                else
                    WorldPosition.X += 1;
            }

            while (WorldPosition.Y % gridResolution != 0)
            {
                if (direction.Y > 0)
                    WorldPosition.Y -= 1;
                else
                    WorldPosition.Y += 1;
            }

            BoundingBox = new Rectangle((int)WorldPosition.X - (int)Origin.X, (int)WorldPosition.Y - (int)Origin.Y, Texture.Width, Texture.Height);
        }

        public void EditorRotate(Vector2 offset, float rotationAmount)
        {
            // Update origin based on mouse pos?
            this.Rotation += rotationAmount;

            BoundingBox = new Rectangle((int)WorldPosition.X - (int)Origin.X, (int)WorldPosition.Y - (int)Origin.Y, Texture.Width, Texture.Height);
        }

        public void DrawBoundingBox()
        {
                boundingBatch.Begin(PrimitiveType.LineList);
                //top line
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left - 1, BoundingBox.Top -1)), Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right + 1, BoundingBox.Top - 1)), Color.Green);

                //left line
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left - 1, BoundingBox.Top)), Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left - 1, BoundingBox.Bottom +1 )), Color.Green);

                //right line
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right, BoundingBox.Top)), Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right, BoundingBox.Bottom +1 )), Color.Green);

                //bottom line
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left, BoundingBox.Bottom)), Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right, BoundingBox.Bottom)), Color.Green);
                boundingBatch.End();
        }
    }
}
