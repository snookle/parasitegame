

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ViscoelasticXNAPrototype
{
    class SpatialGrid
    {
        private List<BlobParticle>[][] grid;

        private int width;
        private int height;
        private int gridSize;

        public SpatialGrid(int gridWidth, int gridHeight, int gridSize)
        {
            this.width = gridWidth / gridSize;
            this.height = gridHeight / gridSize;
            this.gridSize = gridSize;

            grid = new List<BlobParticle>[gridHeight][];

            // May need to change the values to height+1, width+1
            for (int i = 0; i < this.height+1; i++)
            {
                grid[i] = new List<BlobParticle>[gridWidth];
                for (int j = 0; j < this.width+1; j++)
                {
                    grid[i][j] = new System.Collections.Generic.List<BlobParticle>();
                }
            }
        }

        public void RemoveParticle(BlobParticle particle)
        {
            int x = Convert.ToInt32(Math.Floor(particle.position.X / gridSize));
            int y = Convert.ToInt32(Math.Floor(particle.position.Y / gridSize));

            if (y < 0)
            {
                y = 0;
            }
            else if (y > height)
            {
                y = height;
            }

            if (x < 0)
            {
                x = 0;
            }
            else if (x > width)
            {
                x = width;
            }
            
            grid[y][x].Remove(particle);
        }

        public void AddParticle(BlobParticle particle)
        {
            int x = Convert.ToInt32(Math.Floor(particle.position.X / gridSize));
            int y = Convert.ToInt32(Math.Floor(particle.position.Y / gridSize));

            if (y < 0)
            {
                y = 0;
            }
            else if (y > height)
            {
                y = height;
            }

            if (x < 0)
            {
                x = 0;
            }
            else if (x > width)
            {
                x = width;
            }
            
            grid[y][x].Add(particle);
        }

        public List<BlobParticle> GetNeighbours(BlobParticle particle)
        {
            int x = Convert.ToInt32(Math.Floor(particle.position.X / gridSize));
            int y = Convert.ToInt32(Math.Floor(particle.position.Y / gridSize));

            if (y < 0)
            {
                y = 0;
            }
            else if (y > height)
            {
                y = height;
            }

            if (x < 0)
            {
                x = 0;
            }
            else if (x > width)
            {
                x = width;
            }

            List<BlobParticle> returnList = new List<BlobParticle>();

            for (int i = y - (y == 0 ? 0 : 1); i < y + (y == height ? 0 : 1); i++)
            {
                for (int j = x - (x == 0 ? 0 : 1); j < x + (x == width ? 0 : 1); j++)
                {
                    //if (grid[i][j] != null && grid[i][j].Count > 0)
                    //{
                        returnList.AddRange(grid[i][j]);
                    //}
                }
            }

            returnList.Remove(particle);

            return returnList;
        }
    }
}

