using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blob_P2
{
    public class IKMember
    {
        public IKMember father = null;
        public float distance;
        public float defaultdistance;
        public String name = "";

        public ParasiteBodyPart skin;

        private IKMember fprn1 = null;
        private IKMember fprn2 = null;
        private float fprangle1 = 0;

        private List<IKMember> nnb;

        public bool IKLocked;
        public float lockedAngle;

        public float currentAngle = 0;
        public bool rigid = false;

        public IKMember(ParasiteBodyPart skin, float distance)
        {
            this.distance = distance;
            this.defaultdistance = distance;        // default distance
            this.skin = skin;
            
            // init nnb
            nnb = new List<IKMember>();            
        }

        public void enableDrag()
        {
        }

        public void disableDrag()
        {
        }

        public void addNeighbour(IKMember neighbour)
        {
            nnb.Add(neighbour);
        }

        public void AddAngleConstraint(IKMember n1, float angle1, IKMember n2)
        {
            fprn1 = n1;
            fprn2 = n2;
            fprangle1 = angle1;
        }

        public void RemoveAngleConstraint()
        {
            fprn1 = null;
            fprn2 = null;
            fprangle1 = 0;
        }

        public void update()
        {
            move(skin.position.X, skin.position.Y, father);
        }

        public void moveTo(float x, float y)
        {
            move(x, y, null);
        }

        public void move(float _x, float _y, IKMember father){
            if (father == null)
            {
                //skin.x = _x;
                //skin.y = _y;
                skin.position.X = _x;
                skin.position.Y = _y;
            }
            else
            {
                MakeMove(this, father);
            }

            // SetAngle(father);

            for (int i = 0; i < nnb.Count; i++)
            {
                if (nnb[i] != father)
                {
                    nnb[i].move(_x, _y, this);
                }
            }
        }

        public void lockAngle(Boolean locked)
        {
            IKLocked = locked;

            lockedAngle = currentAngle;
        }

        public void MakeMove(IKMember child, IKMember father)
        {
            if (!IKLocked)
            {
                float dy = child.skin.position.Y - father.skin.position.Y;
                float dx = child.skin.position.X - father.skin.position.X;
                float a1 = (float)Math.Atan2(dy, dx);

                currentAngle = a1;

                child.skin.position.X = father.skin.position.X + (float)Math.Cos(a1) * distance;
                child.skin.position.Y = father.skin.position.Y + (float)Math.Sin(a1) * distance;
                //child.skin.rotation = (float)((Math.PI * a1) * 180 / Math.PI);
            }
            else
            {
                // Locked angle code ?
                child.skin.position.X = father.skin.position.X + (float)Math.Cos(lockedAngle) * distance;
                child.skin.position.Y = father.skin.position.Y + (float)Math.Sin(lockedAngle) * distance;
            }
        }

        public void SetAngle(IKMember father)
        {
            float angle = currentAngle;
            IKMember a;
            IKMember b;

            if (fprn1 != null)
            {
                IKMember node0 = fprn1;
                IKMember node1 = fprn2;
                float node2 = fprangle1;

                if (node1 == father)
                {
                    a = node1;
                    b = node0;
                    angle = (float)(2 * Math.PI - node2);
                }
                else
                {
                    a = node0;
                    b = node1;
                    angle = node2;
                }

                float ax = a.skin.position.X - skin.position.X;
                float ay = a.skin.position.Y - skin.position.Y;
                float aangle = (float)Math.Atan2(ay, ax);

                b.skin.position.X = skin.position.X + (float)Math.Cos(aangle + angle) * distance;
                b.skin.position.Y = skin.position.Y + (float)Math.Sin(aangle + angle) * distance;
            }
        }

    }

}

