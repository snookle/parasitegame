using System;
using System.Collections.Generic;
using System.Text;

namespace Blob_P2
{
    public class PrimitiveShape
    {
        public Color ShapeColor = Color.White;

        Vector2[] vertices;
        Vector2[] transformedVertices;
        BoundingRectangle bounds;
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

        public BoundingRectangle Bounds
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

        private void CalculatePointsAndBounds()
        {
            for (int i = 0; i < vertices.Length; i++)
                transformedVertices[i] =
                            Vector2.Transform(
                                     vertices[i],
                                     Matrix.CreateRotationZ(rotation)) + position;

            bounds = new BoundingRectangle(transformedVertices);
        }

        public static bool TestCollision(PrimitiveShape shape1, PrimitiveShape shape2)
        {
            return false;
        }
    }

}
