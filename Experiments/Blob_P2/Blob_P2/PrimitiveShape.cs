using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Blob_P2
{
   
    public class PrimitiveShape
    {
        public Color ShapeColor = Color.White;
        Vector2[] vertices;
        Vector2[] transformedVertices;
        Rectangle bounds;
        Vector2 position = Vector2.Zero;
        float rotation = 0f;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (!position.Equals(value))
                {
                    position = value;
                    CalculatePointsAndBounds();
                }
            }
        }

        public float Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    CalculatePointsAndBounds();
                }
            }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public PrimitiveShape(params Vector2[] vertices)
        {
            this.vertices = (Vector2[])vertices.Clone();
            this.transformedVertices = new Vector2[vertices.Length];
            CalculatePointsAndBounds();
        }

        public void Draw(PrimitiveBatch batch)
        {
            batch.Begin(PrimitiveType.LineList);
            for (int i = 0; i < transformedVertices.Length; i++)
            {
                batch.AddVertex(transformedVertices[i], ShapeColor);
                batch.AddVertex(transformedVertices[
                                   (i + 1) % transformedVertices.Length], ShapeColor);
            }
            batch.End();
        }

        private Rectangle MakeBoundingRectangle(Vector2[] verticies)
        {
            Vector2 xy = new Vector2(0,0);
            Vector2 wh = new Vector2(0,0);
            for (int i = 0; i < vertices.Length; i++)
            {
                Math.Min(xy.X, vertices[i].X);
                Math.Min(xy.Y, vertices[i].Y);
                Math.Max(wh.X, vertices[i].X);
                Math.Max(wh.Y, vertices[i].Y);
            }
            return new Rectangle((int)xy.X, (int)xy.Y, (int)(wh.X - xy.X), (int)(wh.Y - xy.Y));
        }

        private void CalculatePointsAndBounds()
        {
            for (int i = 0; i < vertices.Length; i++)
                transformedVertices[i] =
                            Vector2.Transform(
                                     vertices[i],
                                     Matrix.CreateRotationZ(rotation)) + position;

            bounds = MakeBoundingRectangle(transformedVertices);

        }

        public static bool TestCollision(PrimitiveShape shape1, PrimitiveShape shape2)
        {
            return false;
        }
    }

}
