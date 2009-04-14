using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Factories;

namespace Parasite
{
    class LevelArt : SceneNode
    {
        
        public Texture2D Texture = null;
        public Microsoft.Xna.Framework.Graphics.Color Tint = Microsoft.Xna.Framework.Graphics.Color.White;
        
        //name of the content resource for this texture
        //ie: "LevelArt\\WallTest01"
        private string TextureName;

        public Rectangle BoundingBox;

        public Vector2 Scale = new Vector2(1, 1);

        public bool EditorSelected;

        private PrimitiveBatch boundingBatch;

        /// <summary>
        /// Whether or not this level art has physics geometry associated with it
        /// </summary>
        public bool Physical = false;


        public LevelArt(Game game, Vector2 startingPosition, string textureName, bool physical) : base (game, startingPosition)
        {
            TextureName = textureName;
            Physical = physical;
            LoadContent();
        }

        public LevelArt(Game game, BinaryReader file) : base(game)
        {
            LoadLevelData(file);
            LoadContent();
        }

        public string Name
        {
            get
            {
                return TextureName;
            }
        }

        public virtual void LoadContent()
        {
            Texture = game.Content.Load<Texture2D>(TextureName);
           // Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            //batch to draw the bounding box
            boundingBatch = new PrimitiveBatch(game.GraphicsDevice);

            if (Physical)
            {
                //create the physics body and geometry
                uint[] data = new uint[Texture.Width * Texture.Height];

                //Transfer the texture data to the array
                Texture.GetData(data);
                //get geom, assign to static body
                Vertices verts = Vertices.CreatePolygon(data, Texture.Width, Texture.Height);
                Vector2 polyOrigin = verts.GetCentroid();
                Origin.X = (float)Math.Round(polyOrigin.X);
                Origin.Y = (float)Math.Round(polyOrigin.Y);

                PhysicsBody = BodyFactory.Instance.CreatePolygonBody(physicsManager.Simulator, verts, 1);
                PhysicsBody.Position = WorldPosition;
                PhysicsBody.IsStatic = true;
                PhysicsGeometry = GeomFactory.Instance.CreatePolygonGeom(physicsManager.Simulator, PhysicsBody, verts, 0);
            }

            BoundingBox = new Rectangle((int)WorldPosition.X - Texture.Width / 2, (int)WorldPosition.Y - Texture.Height / 2, Texture.Width, Texture.Height);
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
                Tint = Microsoft.Xna.Framework.Graphics.Color.Yellow;
            else
                Tint = Microsoft.Xna.Framework.Graphics.Color.White;
        }

        /// <summary>
        /// Checks if selected point on the texture is transparent
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CheckTrans(Vector2 position)
        {
            return false;
            position += Origin;
            Rectangle sourceRectangle = new Rectangle((int)position.X, (int)position.Y, 1, 1);
            Microsoft.Xna.Framework.Graphics.Color[] retrievedColour = new Microsoft.Xna.Framework.Graphics.Color[1];

            Texture.GetData<Microsoft.Xna.Framework.Graphics.Color>(0, sourceRectangle, retrievedColour, 0, 1);

            if (retrievedColour[0] == Microsoft.Xna.Framework.Graphics.Color.TransparentBlack || retrievedColour[0] == Microsoft.Xna.Framework.Graphics.Color.TransparentWhite)
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
            if (PhysicsBody != null)
            {
                PhysicsBody.Position = WorldPosition;
            }
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
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left - 1, BoundingBox.Top - 1)), Microsoft.Xna.Framework.Graphics.Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right + 1, BoundingBox.Top - 1)), Microsoft.Xna.Framework.Graphics.Color.Green);

                //left line
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left - 1, BoundingBox.Top)), Microsoft.Xna.Framework.Graphics.Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left - 1, BoundingBox.Bottom + 1)), Microsoft.Xna.Framework.Graphics.Color.Green);

                //right line
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right, BoundingBox.Top)), Microsoft.Xna.Framework.Graphics.Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right, BoundingBox.Bottom + 1)), Microsoft.Xna.Framework.Graphics.Color.Green);

                //bottom line
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Left, BoundingBox.Bottom)), Microsoft.Xna.Framework.Graphics.Color.Green);
                boundingBatch.AddVertex(camera.WorldToScreen(new Vector2(BoundingBox.Right, BoundingBox.Bottom)), Microsoft.Xna.Framework.Graphics.Color.Green);
                boundingBatch.End();
        }

        public void SaveLevelData(BinaryWriter file)
        {
            //SceneNode info that we want to keep.\
            //world position
            file.Write(WorldPosition.X);
            file.Write(WorldPosition.Y);
            
            //screen depth
            file.Write(ScreenDepth);

            //LevelArt specific data.            
            //Tint
            file.Write(Tint.R);
            file.Write(Tint.B);
            file.Write(Tint.G);
            file.Write(Tint.A);

            //Texture filename
            file.Write(TextureName);

            //rotation
            file.Write(Rotation);

            //Scale
            file.Write(Scale.X);
            file.Write(Scale.Y);
        }

        public void LoadLevelData(BinaryReader file)
        {
            WorldPosition = new Vector2(file.ReadSingle(), file.ReadSingle());

            ScreenDepth = file.ReadSingle();

            Tint = new Microsoft.Xna.Framework.Graphics.Color(file.ReadByte(), file.ReadByte(), file.ReadByte(), file.ReadByte());

            TextureName = file.ReadString();

            Rotation = file.ReadSingle();

            Scale = new Vector2(file.ReadSingle(), file.ReadSingle());
        }


    }
}
