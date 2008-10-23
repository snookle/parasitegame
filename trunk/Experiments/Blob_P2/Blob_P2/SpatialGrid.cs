using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blob_P2
{
    class SpatialGrid
    {
        private Dictionary<int, PhysicsObject>[][] grid;        // The Grid of Particles

        private int width;
        private int height;

        private float gridSize;

        public SpatialGrid(int gridWidth, int gridHeight, float gridSize)
        {
            this.width = Convert.ToInt32(gridWidth / gridSize);
            this.height = Convert.ToInt32(gridHeight / gridSize);

            this.gridSize = gridSize;

            // Initialise the Grid
            grid = new Dictionary<int, PhysicsObject>[width+1][];

            for (int i = 0; i < this.width+1; i++)
            {
                grid[i] = new Dictionary<int, PhysicsObject>[height+1];
                for (int j = 0; j < this.height+1; j++)
                {
                    grid[i][j] = new Dictionary<int, PhysicsObject>();
                }
            }
        }

        public void AddObject(PhysicsObject obj)
        {
            if (obj.type == PhysicsObjectType.potBlobParticle)
            {
                int x = (int)Math.Floor(((BlobParticle)obj).position.X / gridSize);
                int y = (int)Math.Floor(((BlobParticle)obj).position.Y / gridSize);
                grid[x][y].Add(obj.id, obj);
            }
            else if (obj.type == PhysicsObjectType.potStaticBody)
            {
                //here we have to calculate all the gridsquares that the object may occupy
                //and add a reference to the object to all grid squares
            }
        }

        public void RemoveObject(PhysicsObject obj)
        {
            if (obj.type == PhysicsObjectType.potBlobParticle)
            {

                int x = (int)Math.Floor(((BlobParticle)obj).position.X / gridSize);
                int y = (int)Math.Floor(((BlobParticle)obj).position.Y / gridSize);
                grid[x][y].Remove(((BlobParticle)obj).id);
            }


        }

        public List<PhysicsObject> GetNeighbours(BlobParticle particle)
        {
            int x = (int)Math.Floor(particle.position.X / gridSize);
            int y = (int)Math.Floor(particle.position.Y / gridSize);

            List<PhysicsObject> returnList = new List<PhysicsObject>();
            
            /*for (int i = x - (x == 0 ? 0 : 1); i <= x + (x == width + 1 ? 0 : 1); i++)
            {
                for (int j = y - (y == 0 ? 0 : 1); j <= y + (y == height + 1 ? 0 : 1); j++)
                {
                    returnList.AddRange(grid[i][j]);
                }
            }*/

            int xStart = x - 1;
            int xEnd = x + 1;
            int yStart = y - 1;
            int yEnd = y + 1;
            
            if (xStart <= 0)
            {
                xStart = 0;
            }

            if (xEnd >= width)
            {
                xEnd = width;
            }

            if (yStart <= 0)
            {
                yStart = 0;
            }

            if (yEnd >= height)
            {
                yEnd = height;
            }

            for (int i = xStart; i <= xEnd; i++)
            {
                for (int j = yStart; j <= yEnd; j++)
                {
                    returnList.AddRange(grid[i][j].Values);
                }
            }

            returnList.Remove(particle);

            return returnList;
        }


    }
}
