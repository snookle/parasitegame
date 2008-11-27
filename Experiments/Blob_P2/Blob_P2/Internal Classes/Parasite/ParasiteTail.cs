using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blob_P2
{
    class ParasiteTail : ParasiteBodyPart
    {
        public bool isMoving = false;
        public bool isClicked = false;

        public ParasiteTail(Texture2D sprite, float weight)
            : base(sprite, weight)
        {
        }

        public void initTail()
        {
            // In the Flash Version : 
            // Enabled Button Mode, and a Mouseover 'hover'

            // if IKPoint != null, also added a 'clicktail' method on mousedown, and a stopclicktail
        }

        /*public override void specialMovement()
        {
            float xDist = MousePosition.X - position.X;
            float yDist = MousePosition.Y - position.Y;

            if (xDist < 25 && yDist < 25)
            {
                dragTail();
            }
            else if (isMoving)
            {
                releaseTail();
            }
        }*/

        /*public void ClickTail()
        {
            isClicked = true;
        }

        public void StopClickTail()
        {
            isClicked = false;
        }

        public void DragTail()
        {
            isMoving = true;
            IKPoint.startMove();
        }

        public void ReleaseTail()
        {
            isMoving = false;
            IKPoint.stopMove();
        }*/
    }
}
