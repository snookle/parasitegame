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
    public struct BoundingRectangle
    {
        public float l, b, r, t;

        public BoundingRectangle(float l, float b, float r, float t)
        {
            this.l = l;
            this.b = b;
            this.r = r;
            this.t = t;
        }

        public BoundingRectangle(params Vector2[] points)
        {
            if (points.Length < 2)
                throw new Exception("BoundingRectangle can only be made for two or more points");

            l = float.PositiveInfinity;
            r = float.NegativeInfinity;
            b = float.NegativeInfinity;
            t = float.PositiveInfinity;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 v = points[i];

                l = (float)Math.Min(l, v.X);
                r = (float)Math.Max(r, v.X);
                b = (float)Math.Max(b, v.Y);
                t = (float)Math.Min(t, v.Y);
            }
        }

        public bool Intersects(BoundingRectangle other)
        {
            return (l <= other.r && other.l <= r && t <= other.b && other.t <= b);
        }

        public bool Contains(Vector2 v)
        {
            return (l < v.X && r > v.X && b > v.Y && t < v.Y);
        }
    }
}
