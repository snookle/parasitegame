using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blob_P2
{
    class MarchingCube
    {
        int numXDivisions;
        int numYDivisions;

        int boxWidth;
        int boxHeight;

        byte TOPLEFT = 1;
        byte TOPRIGHT = (1 << 1);
        byte BOTTOMLEFT = (1 << 2);
        byte BOTTOMRIGHT = (1 << 3);
        byte CORNERALL;
        byte CORNERNONE = 0;

        Vector2 SkinStartLocation;

        float threshold;

        public MarchingCube(float threshold)
        {
            this.threshold = threshold;
            this.CORNERALL = (byte)(TOPLEFT | TOPRIGHT | BOTTOMLEFT | BOTTOMRIGHT);
        }

        public float CalculateFieldStrength(BlobParticle particle, Vector2 point)
        {
            float fieldStrength = 0.0f;

            float distance = Vector2.Distance(particle.position, point);
			if (distance < particle.radiusSquared)
            {
				fieldStrength = 1.0f;
			}
			
			return fieldStrength;
        }

        public void Update(ref List<BlobParticle> particles)
        {
            int i = 0;
            int j = 0;
            //SkinStartLocation = null;
            for (i = 0; i < numXDivisions; i++)
            {
                for (j = 0; j < numYDivisions; j++)
                {
                    CheckCorners(i, j, ref particles);
                }
            }
        }

        private byte CheckCorners(int xValue, int yValue, ref List<BlobParticle> particles)
        {
			byte corners = CORNERNONE;
			bool checkValue = false;
			int particleCount = particles.Count;
            Vector2 topLeft = new Vector2(xValue * boxWidth, yValue * boxHeight);
			Vector2 topRight = new Vector2((xValue * boxWidth) + boxWidth, topLeft.Y);
			Vector2 bottomLeft = new Vector2(topLeft.X, (yValue * boxHeight) + boxHeight);
			Vector2 bottomRight = new Vector2(topRight.X, bottomLeft.Y);
            BlobParticle particle;

            for (int i = 0; i < particleCount; i++) {		
                particle = particles[i];
				if (Vector2.Distance(particle.position, topLeft) <= threshold) {
					corners |= TOPLEFT;
				} else if (Vector2.Distance(particle.position, topRight) <= threshold) {
					corners |= TOPRIGHT;
				} else if (Vector2.Distance(particle.position, bottomLeft) <= threshold) {
                    corners |= BOTTOMLEFT;
				} else if (Vector2.Distance(particle.position, bottomRight) <= threshold) {
                    corners |= BOTTOMRIGHT;				
				}
			}
			
			
			if (corners == CORNERNONE || corners == CORNERALL) {
				// if all 4 corners are empty or full...Do nothing
                return 0;
			} else {
                SkinStartLocation = (SkinStartLocation == null ? new Vector2(xValue, yValue) : SkinStartLocation);
				return corners;
			}
		}
    }
}
