using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blob_P2
{
    class Spring
    {
        public BlobParticle object1;        // First Mass @ Tip of Spring
        public BlobParticle object2;        // Second Mass @ Other Spring

        public float springYeildRatio = 0.3f;
        public float plasticityConstant = 0.3f;

        public float springConstant;        // Stiffness
        public float springLength;
        public float frictionConstant;

        public Spring(BlobParticle object1, BlobParticle object2, float springConstant, float springLength, float frictionConstant)
        {
            this.object1 = object1;
            this.object2 = object2;

            this.springConstant = springConstant;
            this.springLength = springLength;
            this.frictionConstant = frictionConstant;
        }

        public void solve()
        {
            // Vector between two points
            Vector2 springVector = object1.Position - object2.Position;

            // Distance Between two Points
            float r = springVector.Length();

            // Initialise Force
            Vector2 force = Vector2.Zero;

            // To Avoid / by 0 errors ...
            if (r != 0)
            {
                // Spring Force Applied


                Vector2 temp = (springVector / r) * ((r - springLength) * (-springConstant));
                force += temp;
            }

            Vector2 frictionForce = object1.velocity - object2.velocity;
            frictionForce *= -frictionConstant;
            force += frictionForce;

            object1.applyForce(force);
            object2.applyForce(force * -1);
        }

        public void viscoSolve()
        {
            Vector2 springVector = object2.Position - object1.Position;
            Vector2 force = Vector2.Zero;

            float theDistance = springVector.Length();

            if (theDistance != 0)
            {
                float deformation = springYeildRatio * theDistance;

                if (theDistance > (springLength + deformation))
                {
                    // Stretch
                    springLength += plasticityConstant * (theDistance - springLength - deformation);
                }
                else if (theDistance < (springLength - deformation))
                {
                    // Compress
                    springLength -= plasticityConstant * (springLength - deformation - theDistance);
                }

                float displacementValue = springConstant * (1 - (springLength / 20)) * (springLength - theDistance);

                Vector2 unitR = Vector2.Normalize(springVector);
                force += (unitR * displacementValue) / 2;

                //Vector2 frictionForce = object1.velocity - object2.velocity;
                //frictionForce *= -frictionConstant;
                //force += frictionForce;

                object1.applyForce(force * -1);
                object2.applyForce(force);         
            }
        }


    }
}
