using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blob_P2
{
    class SpatialGrid
    {
        private List<BlobParticle>[][] grid;        // The Grid of Particles

        private int width;
        private int height;

        private float gridSize;

        public SpatialGrid(int gridWidth, int gridHeight, float gridSize)
        {
            this.width = Convert.ToInt32(gridWidth / gridSize);
            this.height = Convert.ToInt32(gridHeight / gridSize);

            this.gridSize = gridSize;

            // Initialise the Grid
            grid = new List<BlobParticle>[width+1][];

            for (int i = 0; i < this.width+1; i++)
            {
                grid[i] = new List<BlobParticle>[height+1];
                for (int j = 0; j < this.height+1; j++)
                {
                    grid[i][j] = new List<BlobParticle>();
                }
            }
        }

        public void AddParticle(BlobParticle particle)
        {
            int x = (int)Math.Floor(particle.position.X / gridSize);
            int y = (int)Math.Floor(particle.position.Y / gridSize);

            grid[x][y].Add(particle);
        }

        public void RemoveParticle(BlobParticle particle)
        {
            int x = (int)Math.Floor(particle.position.X / gridSize);
            int y = (int)Math.Floor(particle.position.Y / gridSize);

            grid[x][y].Remove(particle);
        }

        public List<BlobParticle> GetNeighbours(BlobParticle particle)
        {
            int x = (int)Math.Floor(particle.position.X / gridSize);
            int y = (int)Math.Floor(particle.position.Y / gridSize);

            List<BlobParticle> returnList = new List<BlobParticle>();
            
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
                    returnList.AddRange(grid[i][j]);
                }
            }

            returnList.Remove(particle);

            return returnList;
        }


    }
}
