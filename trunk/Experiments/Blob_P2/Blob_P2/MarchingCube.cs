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

        public MarchingCube()
        {

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

            for (var i = 0; i < numXDivisions; i++)
            {
                for (var j = 0; j < numYDivisions; j++)
                {
                    CheckCorners(i, j, ref particles);
                }
            }
        }

        private void checkCorners(int xValue, int yValue, ref List<BlobParticle> particles)
        {
			// Boolean array holds corner values
			// [0,1,2,3]
			//
			// or in the square : 
			//
			// 0 1
			// 2 3
			
			byte corners = 0;
			bool checkValue = false;
			int particleCount = particles.Count;
            Vector2 topLeft = new Vector2(xValue * boxWidth, yValue * boxHeight);
			Vector2 topRight = new Vector2((xValue * boxWidth) + boxWidth, TopLeft.Y);
			Vector2 bottomLeft = new Vector2(topLeft.X, (yValue * boxHeight) + boxHeight);
			Vector2 bottomRight = new Vector2(topRight.X, bottomLeft.Y);
            BlobParticle particle;

            for (int i = 0; i < particleCount; i++) {		
				// Top Left
                particle = particles[i];
				if (Vector2.Distance(particle.position, topLeft) <= threshold) {
					corners |= TOPLEFT;
				} else if (Vector2.Distance(particle.position, topRight) <= threshold) {
					corners |
					if(dotHighlight){
						highlightSprite.graphics.beginFill(0x88D372, 0.5);
						highlightSprite.graphics.drawCircle(topRightX, topRightY, 2);
						highlightSprite.graphics.endFill();
					}
				}
				
				// Bottom Left
				if (getDistance(theParticles[i], bottomLeftX, bottomLeftY) <= threshold) {
					booleanArray[2] = true;
					if(dotHighlight){
						highlightSprite.graphics.beginFill(0x88D372, 0.5);
						highlightSprite.graphics.drawCircle(bottomLeftX, bottomLeftY, 2);
						highlightSprite.graphics.endFill();
					}
				}
				
				// Bottom Right
				if (getDistance(theParticles[i], bottomRightX, bottomRightY) <= threshold) {
					booleanArray[3] = true;
					if(dotHighlight){
						highlightSprite.graphics.beginFill(0x88D372, 0.5);
						highlightSprite.graphics.drawCircle(bottomRightX, bottomRightY, 2);
						highlightSprite.graphics.endFill();
					}
				}
			}
			
			
			if (booleanArray[0] == false && booleanArray[1] == false && booleanArray[2] == false && booleanArray[3] == false) {
				// if all 4 corners are empty...Do nothing
				return null
			} else if (booleanArray[0] == true && booleanArray[1] == true && booleanArray[2] == true && booleanArray[3] == true) {
				// If all 4 corners are full... Do nothing
				return null
			} else {
				if (showSkin) {
					// Draw the skin for the selected point...
					drawSkin(booleanArray, xValue, yValue);
				}
				return booleanArray;
			}
		}
    }
}
