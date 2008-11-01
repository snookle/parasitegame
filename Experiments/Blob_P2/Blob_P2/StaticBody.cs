using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Text;

namespace Blob_P2
{
    class StaticBody : PhysicsObject
    {
        public BoundingRectangle BoundingBox;
        private PrimitiveBatch batch;
        private GraphicsDevice graphics;
        private Color colour;        
        //for triangulation
        Vector2[] sourceVertices;
        int[] sourceIndices;
        int numVertices;
        int numPrimitives;

        BasicEffect effect;
        VertexDeclaration vertDecl;
        VertexBuffer vertBuffer;
        IndexBuffer indexBuffer;
        
        //rotation
        float angle = 0.0f;
        Vector2 centre;

        public StaticBody (int id, GraphicsDevice graphics, Color drawColour, params Vector2[] newpoints)
	    {
            this.id = id;
            this.type = PhysicsObjectType.potStaticBody;
            BoundingBox = new BoundingRectangle(newpoints);
            sourceVertices = newpoints;
            this.graphics = graphics;
            this.colour = drawColour;
            Init();
            TriangulatePoly();
            centre = new Vector2(BoundingBox.l + ((BoundingBox.r - BoundingBox.l) / 2), BoundingBox.t + ((BoundingBox.b - BoundingBox.t) / 2));
    	}

        private Vector2 RotateAroundPoint(Vector2 point, Vector2 originPoint, Vector3 rotationAxis, float radiansToRotate)
        {
            Vector2 diffVect = point - originPoint;

            Vector2 rotatedVect = Vector2.Transform(diffVect, Matrix.CreateFromAxisAngle(rotationAxis, radiansToRotate));

            rotatedVect += originPoint;

            return rotatedVect;
        }

        private void Rotate(float angle)
        {
            Game1.grid.RemoveObject(this);
            for (int i = 0; i < sourceVertices.Length; i++)
            {

                sourceVertices[i] = RotateAroundPoint(sourceVertices[i], centre, Vector3.Backward, angle);
            }
            BoundingBox = new BoundingRectangle(sourceVertices);
            TriangulatePoly();
            Game1.grid.AddObject(this);
        }

        private void Init()
        {
            effect = new BasicEffect(graphics, null);
            // projection uses CreateOrthographicOffCenter to create 2d projection
            // matrix with 0,0 in the upper left.
            effect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphics.Viewport.Width,
                graphics.Viewport.Height, 0,
                0, 1);
            effect.VertexColorEnabled = true;
            batch = new PrimitiveBatch(graphics);   

        }

        private void TriangulatePoly()
        {
            Triangulator.Triangulator.Triangulate(sourceVertices, Triangulator.WindingOrder.CounterClockwise, out sourceVertices, out sourceIndices);

            // save out some data
            numVertices = sourceVertices.Length;
            numPrimitives = sourceIndices.Length / 3;

            // create the vertex buffer and index buffer using the arrays
            VertexPositionColor[] verts = new VertexPositionColor[sourceVertices.Length];
            for (int i = 0; i < sourceVertices.Length; i++)
            {
                verts[i] = new VertexPositionColor(new Vector3(sourceVertices[i], 0f), colour);
            }
            
            vertBuffer = new VertexBuffer(graphics, verts.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
            vertBuffer.SetData(verts);
            
            // branch here to convert our indices to shorts if possible for wider GPU support
            if (verts.Length < 65535)
            {
                short[] indices = new short[sourceIndices.Length];
                for (int i = 0; i < sourceIndices.Length; i++)
                    indices[i] = (short)sourceIndices[i];
                indexBuffer = new IndexBuffer(
                    graphics,
                    indices.Length * sizeof(short),
                    BufferUsage.WriteOnly,
                    IndexElementSize.SixteenBits);
                indexBuffer.SetData(indices);
            }
            else
            {
                indexBuffer = new IndexBuffer(
                    graphics,
                    sourceIndices.Length * sizeof(int),
                    BufferUsage.WriteOnly,
                    IndexElementSize.ThirtyTwoBits);
                indexBuffer.SetData(sourceIndices);
            }

            vertDecl = new VertexDeclaration(graphics, VertexPositionColor.VertexElements);
        }

        public Vector2 Collides(PhysicsObject obj)
        {
            if (obj.type == PhysicsObjectType.potBlobParticle)
            {
                if (BoundingBox.Contains(((BlobParticle)obj).position))
                {
                    if (ContainsPoint(((BlobParticle)obj).position))
                    {
                        int linePoint1 = -1;
                        int linePoint2 = -1;
                        float closestDistance = float.PositiveInfinity;
                        Vector2 closestPoint = Vector2.Zero;
                        //now do some more intensive collision detection
                        for (int i = 0; i < sourceVertices.Length; i+=2)
                        {
                            Vector2 point = ClosestPointOnLineSegment(ref sourceVertices[i], ref sourceVertices[(i + 1) % sourceVertices.Length], ref ((BlobParticle)obj).position);
                            float currentDistance = Vector2.Distance(closestPoint, point);
                            if (currentDistance < closestDistance)
                            {
                                closestDistance = currentDistance;
                                closestPoint = point;
                                linePoint1 = i;
                                linePoint2 = (i + 1) % sourceVertices.Length;
                            }
                        }
                        if (linePoint1 != -1 && linePoint1 < 500)
                        {
                            Vector2 one = sourceVertices[linePoint1];
                            Vector2 two = sourceVertices[linePoint2];
                            Vector2 returnvec = Vector2.Multiply(one, two);
                            return Vector2.Normalize(returnvec);
                        }
                    }
                }
            }
            return Vector2.Zero;
        }
        
        public Vector2 ClosestPointOnLineSegment(
ref Vector2 lineA,
ref Vector2 lineB,
ref Vector2 point)
        {
            Vector2 v = lineB - lineA;
            v.Normalize();
            float t = Vector2.Dot(v, point - lineA);
            if (t < 0) return lineA;
            float d = (lineB - lineA).Length();
            if (t > d) return lineB;
            return lineA + v * t;
        }

        public bool ContainsPoint(Vector2 point)
        {

            bool oddNodes = false;

            int j = sourceVertices.Length - 1;
            float x = point.X;
            float y = point.Y;

            for (int i = 0; i < sourceVertices.Length; i++)
            {
                Vector2 tpi = sourceVertices[i];
                Vector2 tpj = sourceVertices[j];

                if (tpi.Y < y && tpj.Y >= y || tpj.Y < y && tpi.Y >= y)
                    if (tpi.X + (y - tpi.Y) / (tpj.Y - tpi.Y) * (tpj.X - tpi.X) < x)
                        oddNodes = !oddNodes;

                j = i;
            }

            return oddNodes;
        }

        //thanks to Joseph Duchesne for this method
        //http://www.idevgames.com/forum/showthread.php?t=7458
        private bool segmentsIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float[,] tarray = new float[4, 2];  //<===== Find the inner bounding of rect ABCD

            float ax = a.X;
            float ay = a.Y;
            float bx = b.X;
            float by = b.Y;
            float cx = c.X;
            float cy = c.Y;
            float dx = d.X;
            float dy = d.Y;

            if (ax < bx)
            {
                tarray[0, 0] = ax;
                tarray[1, 0] = bx;
            }
            else
            {
                tarray[0, 0] = bx;
                tarray[1, 0] = ax;
            }
            if (ay < by)
            {
                tarray[0, 1] = ay;
                tarray[1, 1] = by;
            }
            else
            {
                tarray[0, 1] = by;
                tarray[1, 1] = ay;
            }
            if (cx < dx)
            {
                tarray[2, 0] = cx;
                tarray[3, 0] = dx;
            }
            else
            {
                tarray[2, 0] = dx;
                tarray[3, 0] = cx;
            }
            if (cy < dy)
            {
                tarray[2, 1] = cy;
                tarray[3, 1] = dy;
            }
            else
            {
                tarray[2, 1] = dy;
                tarray[3, 1] = cy;
            }

            float[,] tarray2 = new float[2, 2];

            if (tarray[0, 0] < tarray[2, 0])
                tarray2[0, 0] = tarray[2, 0];
            else
                tarray2[0, 0] = tarray[0, 0];
            if (tarray[0, 1] < tarray[2, 1])
                tarray2[0, 1] = tarray[2, 1];
            else
                tarray2[0, 1] = tarray[0, 1];
            if (tarray[1, 0] < tarray[3, 0])
                tarray2[1, 0] = tarray[1, 0];
            else
                tarray2[1, 0] = tarray[3, 0];
            if (tarray[1, 1] < tarray[3, 1])
                tarray2[1, 1] = tarray[1, 1];
            else
                tarray2[1, 1] = tarray[3, 1];

            float mab = (ay - by) / (ax - bx); //<===== Find Slopes of Lines
            float mcd = (cy - dy) / (cx - dx);

            if (mab == mcd)
                return false;  //the lines are parallel

            float yiab = ((ax - bx) * ay - ax * (ay - by)) / (ax - bx);
            float yicd = ((cx - dx) * cy - cx * (cy - dy)) / (cx - dx);
            float x = (yicd - yiab) / (mab - mcd);
            float y = mab * x + yiab;

            return (
                x > tarray2[0, 0] &&
                x < tarray2[1, 0] &&
                y > tarray2[0, 1] &&
                y < tarray2[1, 1]);
        }

        public void Draw()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle = 0.01f;
                //angle = MathHelper.Clamp(angle, -360, 360);
                Rotate(angle);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle = -0.01f;
                //angle = MathHelper.Clamp(angle, -360, 360);
                Rotate(angle);
            }


            //show bounding box
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                batch.Begin(PrimitiveType.LineList);
                //top line
                batch.AddVertex(new Vector2(BoundingBox.l, BoundingBox.t), Color.Green);
                batch.AddVertex(new Vector2(BoundingBox.r, BoundingBox.t), Color.Green);

                //left line
                batch.AddVertex(new Vector2(BoundingBox.l, BoundingBox.t), Color.Green);
                batch.AddVertex(new Vector2(BoundingBox.l, BoundingBox.b), Color.Green);

                //right line
                batch.AddVertex(new Vector2(BoundingBox.r, BoundingBox.t), Color.Green);
                batch.AddVertex(new Vector2(BoundingBox.r, BoundingBox.b), Color.Green);

                //bottom line
                batch.AddVertex(new Vector2(BoundingBox.l, BoundingBox.b), Color.Green);
                batch.AddVertex(new Vector2(BoundingBox.r, BoundingBox.b), Color.Green);
                batch.End();
            }

            graphics.VertexDeclaration = vertDecl;
            graphics.Indices = indexBuffer;
            graphics.Vertices[0].SetSource(vertBuffer, 0, VertexPositionColor.SizeInBytes);

            // if holding 'W' key, render in wireframe
            	if (Keyboard.GetState().IsKeyDown(Keys.W))
            		graphics.RenderState.FillMode = FillMode.WireFrame;
            	else
            		graphics.RenderState.FillMode = FillMode.Solid;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitives);
                pass.End();
            }
            effect.End();
        }
    }
}
