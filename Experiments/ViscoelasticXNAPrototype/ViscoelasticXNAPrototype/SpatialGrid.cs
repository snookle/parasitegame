

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
        private float quadrantMarker;           // Used to detect quadrants within a grid box.
        public int numElements;

        public SpatialGrid(int gridWidth, int gridHeight, int gridSize)
        {
            this.width = gridWidth / gridSize;
            this.height = gridHeight / gridSize;
            this.gridSize = gridSize;

            this.quadrantMarker = gridSize / 2;

            grid = new List<BlobParticle>[height+1][];

            // May need to change the values to height+1, width+1
            for (int i = 0; i < this.height+1; i++)
            {
                grid[i] = new List<BlobParticle>[width+1];
                for (int j = 0; j < this.width+1; j++)
                {
                    grid[i][j] = new System.Collections.Generic.List<BlobParticle>();
                }
            }
        }

        public void RemoveParticle(BlobParticle particle)
        {
            int x = Convert.ToInt32(Math.Floor(particle.pX / gridSize));
            int y = Convert.ToInt32(Math.Floor(particle.pY / gridSize));

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
            numElements--;
        }

        public void AddParticle(BlobParticle particle)
        {
           int x = Convert.ToInt32(Math.Floor(particle.pX / gridSize));
            int y = Convert.ToInt32(Math.Floor(particle.pY / gridSize));

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
            numElements++;
        }

        public List<BlobParticle> GetNeighbours(BlobParticle particle)
        {
            int x = Convert.ToInt32(Math.Floor(particle.pX / gridSize));
            int y = Convert.ToInt32(Math.Floor(particle.pY / gridSize));

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

            float checkX = particle.pX - (x * gridSize);
            float checkY = particle.pY - (y * gridSize);

            List<BlobParticle> returnList = new List<BlobParticle>();

           // if (checkX == quadrantMarker || checkY == quadrantMarker)
           // {
                // If the particle is *exactly* on the centre line, go with normal code for now
                for (int i = y - (y == 0 ? 0 : 1); i < y + (y == height ? 0 : 1); i++)
                {
                    for (int j = x - (x == 0 ? 0 : 1); j < x + (x == width ? 0 : 1); j++)
                    {
                        if (grid[i][j] != null)
                        {
                            returnList.AddRange(grid[i][j]);

                            //for (int p = 0; p < grid[i][j].Count; p++)
                            //{
                            //    returnList.Add(grid[i][j][p]);
                            //}
                        }
                    }
                }
           // }
            /*
            else
            {
                // Figure out which quadrant it's in, and go from there.  Can simplify this later, cant be arsed writing that fancy shite!
                if (checkX < quadrantMarker && checkY < quadrantMarker)
                {
                    // 1
                    // Needs to check : (x-1,y-1), (x,y-1), (x-1,y), and (x,y)
                    for (int i = y - (y == 0 ? 0 : 1); i < y; i++)
                    {
                        for (int j = x - (x == 0 ? 0 : 1); j < x; j++)
                        {
                            if (grid[i][j] != null && grid[i][j].Count > 0)
                            {
                                returnList.AddRange(grid[i][j]);
                            }
                        }
                    }
                }
                else if (checkX > quadrantMarker && checkY < quadrantMarker)
                {
                    // 2
                    // Needs to check (x,y-1), (x+1,y-1), (x+1,y), and (x,y)
                    for (int i = y - (y == 0 ? 0 : 1); i < y; i++)
                    {
                        for (int j = x; j < x + (y == width ? 0 : 1); j++)
                        {
                            if (grid[i][j] != null && grid[i][j].Count > 0)
                            {
                                returnList.AddRange(grid[i][j]);
                            }
                        }
                    }
                }
                else if (checkX < quadrantMarker && checkY > quadrantMarker)
                {
                    // 3
                    // Needs to check (x-1,y), (x-1,y+1), (x,y+1), and (x,y)
                    for (int i = y; i < y + (y == height ? 0 : 1); i++)
                    {
                        for (int j = x - (x == 0 ? 0 : 1); j < x; j++)
                        {
                            if (grid[i][j] != null && grid[i][j].Count > 0)
                            {
                                returnList.AddRange(grid[i][j]);
                            }
                        }
                    }
                }
                else if (checkX > quadrantMarker && checkY > quadrantMarker)
                {
                    // 4
                    // Needs to check (x,y+1), (x+1,y+1), (x+1,y), and (x,y)
                    for (int i = y; i < y + (y == height ? 0 : 1); i++)
                    {
                        for (int j = x; j < x + (y == width ? 0 : 1); j++)
                        {
                            if (grid[i][j] != null && grid[i][j].Count > 0)
                            {
                                returnList.AddRange(grid[i][j]);
                            }
                        }
                    }
                }
            }*/

            /*List<BlobParticle> returnList = new List<BlobParticle>();
            
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j] != null)
                    {
                        for (int p = 0; p < grid[i][j].Count; p++)
                        {
                            returnList.Add(grid[i][j][p]);
                        }
                    }
                }
            }*/

            returnList.Remove(particle);

            return returnList;
        }
    }
}

