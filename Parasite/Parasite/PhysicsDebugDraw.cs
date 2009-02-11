using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Parasite
{
    class PhysicsDebugDraw : DebugDraw
    {
        Game game;
        GraphicsDevice graphics;
        PrimitiveBatch primBatch;

        public PhysicsDebugDraw(Game game)
            : base()
        {
            this.game = game;
            graphics = game.GraphicsDevice;
            primBatch = new PrimitiveBatch(game.GraphicsDevice);
            Flags = DrawFlags.Aabb | DrawFlags.CoreShape | DrawFlags.Obb | DrawFlags.Shape;
        }

        public override void DrawPolygon(Vec2[] vertices, int vertexCount, Box2DX.Dynamics.Color color)
        {
            if (primBatch == null)
                return;

            primBatch.Begin(PrimitiveType.LineList);
            for (int i = 0; i < vertexCount; i++)
            {
                primBatch.AddVertex(new Vector2(512 + vertices[i].X, 384 + vertices[i].Y), new Microsoft.Xna.Framework.Graphics.Color(color.R, color.G, color.B));
                if (i == vertexCount - 1)
                {
                    primBatch.AddVertex(new Vector2(512 + vertices[0].X, 384 + vertices[0].Y), new Microsoft.Xna.Framework.Graphics.Color(color.R, color.G, color.B));
                }
                else
                {
                    primBatch.AddVertex(new Vector2(512 + vertices[i+1].X, 384 + vertices[i+1].Y), new Microsoft.Xna.Framework.Graphics.Color(color.R, color.G, color.B));
                }
                
            }
            primBatch.End();
        }

        public override void DrawCircle(Vec2 center, float radius, Box2DX.Dynamics.Color color)
        {
            throw new NotImplementedException();
        }

        public override void DrawSegment(Vec2 p1, Vec2 p2, Box2DX.Dynamics.Color color)
        {
            throw new NotImplementedException();
        }

        public override void DrawSolidCircle(Vec2 center, float radius, Vec2 axis, Box2DX.Dynamics.Color color)
        {
            throw new NotImplementedException();
        }

        public override void DrawSolidPolygon(Vec2[] vertices, int vertexCount, Box2DX.Dynamics.Color color)
        {
         //   throw new NotImplementedException();
        }

        public override void DrawXForm(XForm xf)
        {
            throw new NotImplementedException();
        }
    }
}
