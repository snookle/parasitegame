using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Blob_P2
{
    public class PhysicsOverlord
    {
        private static PhysicsOverlord instance = null;
        private Queue<int> oldIDs;
        private int numberOfObjects;
        private int lastNewID;
        
        public PhysicsOverlord()
        {
            oldIDs = new Queue<int>();
        }
        
        public static PhysicsOverlord GetInstance()
        {
            if (instance == null)
            {
                instance = new PhysicsOverlord();
            }       
            return instance;

        }

        public int GetID()
        {
            numberOfObjects++;

            if (oldIDs.Count != 0)
            {
                return oldIDs.Dequeue();
            }
            else
                return ++lastNewID;
        }

        public void RetireId(int id)
        {
            oldIDs.Enqueue(id);
            numberOfObjects--;
        }

    }

}
