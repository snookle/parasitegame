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
        
        /// <summary>
        /// Returns an instance of the PhysicsOverlorddddd
        /// </summary>
        /// <returns>returns an instance of PhysicsOverlord</returns>
        public static PhysicsOverlord GetInstance()
        {
            if (instance == null)
            {
                instance = new PhysicsOverlord();
            }       
            return instance;

        }

        /// <summary>
        /// Assigns an ID to a physics object
        /// </summary>
        /// <returns>Returns the latest ID</returns>
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

        /// <summary>
        /// Retries an ID that is no longer being used
        /// </summary>
        /// <param name="id">the id to retire</param>
        public void RetireId(int id)
        {
            oldIDs.Enqueue(id);
            numberOfObjects--;
        }

    }

}
