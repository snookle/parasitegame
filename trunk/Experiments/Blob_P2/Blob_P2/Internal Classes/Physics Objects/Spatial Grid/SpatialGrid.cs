using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blob_P2
{
    public class SpatialGrid
    {
        private static SpatialGrid instance = null;
        private Dictionary<int, PhysicsObject>[][] grid;        // The Grid of Particles

        private int width;
        private int height;

        private float gridSize;

        private bool setup = false;

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
                    grid[i][j] = new Dictionary<int, PhysicsObject>(50);
                }
            }
        }

        public static SpatialGrid GetInstance()
        {
            if (instance == null)
            {
                instance = new SpatialGrid(0,0,1);
            }       
            return instance;

        }

        public void SetDimensions(int gridWidth, int gridHeight, float gridSize)
        {
            if (setup)
                throw new Exception("Dimensions already set!");
            instance = new SpatialGrid(gridWidth, gridHeight, gridSize);
            instance.setup = true;
        }

        public void AddObject(PhysicsObject obj)
        {
            if (!setup)
                throw new Exception("SetDimensions must be called first!");

            if (obj.type == PhysicsObjectType.potBlobParticle)
            {
                int x = (int)(((BlobParticle)obj).Position.X / gridSize);
                int y = (int)(((BlobParticle)obj).Position.Y / gridSize);
                if (grid[x][y].ContainsKey(obj.id))
                    RemoveObject(obj);
                grid[x][y].Add(obj.id, obj);
            }
            else if (obj.type == PhysicsObjectType.potStaticBody)
            {
                //here we have to calculate all the gridsquares that the object may occupy
                //and add a reference to the object to all grid squares
                StaticBody body = (StaticBody)obj;
                int xStart = (int)Math.Floor(body.BoundingBox.l / gridSize);
                int yStart = (int)Math.Floor(body.BoundingBox.t / gridSize);
                //throw new Exception(body.x.ToString());
                if (body.BoundingBox.r > gridSize || body.BoundingBox.b > gridSize)
                {
                    //calculate the number of squares this body is going to cover
                    int xEnd = (int)(Math.Floor(body.BoundingBox.r / gridSize));        // CHANGED - Used to have xStart + ... why ? 
                    int yEnd = (int)(Math.Floor(body.BoundingBox.b / gridSize));
                   // throw new Exception(xStart.ToString() + " to " + xEnd.ToString() + " " + yEnd.ToString());
                    for (int i = xStart; i < xEnd; i++)
                    {
                        for (int j = yStart; j < yEnd; j++)
                        {
                            grid[i][j].Add(obj.id, obj);
                        }
                    }
                }
                else //the body is not bigger than a single gridsquare, so it only needs to be added to one.
                {
                    grid[xStart][yStart].Add(obj.id, obj);
                }
            }
            else if (obj.type == PhysicsObjectType.potParasiteBodyPart)
            {
                int x = (int)Math.Floor(((ParasiteBodyPart)obj).Position.X / gridSize);
                int y = (int)Math.Floor(((ParasiteBodyPart)obj).Position.Y / gridSize);
                if (grid[x][y].ContainsKey(obj.id))
                    RemoveObject(obj);
               // grid[x][y].Add(obj.id, obj);
            }
            else
                throw new Exception("OMFG DIDNT ADD ANYTHING TO THE GRID FUCK!");
        }

        public void RemoveObject(PhysicsObject obj)
        {
            if (!setup)
                throw new Exception("SetDimensions must be called first!");

            if (obj.type == PhysicsObjectType.potBlobParticle)
            {
                int x = (int)(((BlobParticle)obj).Position.X / gridSize);
                int y = (int)(((BlobParticle)obj).Position.Y / gridSize);
                grid[x][y].Remove(((BlobParticle)obj).id);
            }
            else if (obj.type == PhysicsObjectType.potStaticBody)
            {
                StaticBody body = (StaticBody)obj;
                int xStart = (int)Math.Floor(body.BoundingBox.l / gridSize);
                int yStart = (int)Math.Floor(body.BoundingBox.t / gridSize);
                //th    row new Exception(body.x.ToString());
                if (body.BoundingBox.r > gridSize || body.BoundingBox.b > gridSize)
                {
                    //calculate the number of squares this body is going to cover
                    int xEnd = (int)(Math.Floor(body.BoundingBox.r / gridSize));
                    int yEnd = (int)(Math.Floor(body.BoundingBox.b / gridSize));
                    // throw new Exception(xStart.ToString() + " to " + xEnd.ToString() + " " + yEnd.ToString());
                    for (int i = xStart; i < xEnd; i++)
                    {
                        for (int j = yStart; j < yEnd; j++)
                        {
                            grid[i][j].Remove(obj.id);
                        }
                    }
                }
                else //the body is not bigger than a single gridsquare, so it only needs to be added to one.
                {
                    grid[xStart][yStart].Add(obj.id, obj);
                }
            }
            else if (obj.type == PhysicsObjectType.potParasiteBodyPart)
            {
                int x = (int)Math.Floor(((ParasiteBodyPart)obj).Position.X / gridSize);
                int y = (int)Math.Floor(((ParasiteBodyPart)obj).Position.Y / gridSize);
                //grid[x][y].Remove(((ParasiteBodyPart)obj).id);
            }
        }

        public List<PhysicsObject> GetNeighbours(BlobParticle particle)
        {
            if (!setup)
                throw new Exception("SetDimensions must be called first!");

            int x = (int)(particle.Position.X / gridSize);
            int y = (int)(particle.Position.Y / gridSize);

            List<PhysicsObject> returnList = new List<PhysicsObject>(100);
            
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

        public List<PhysicsObject> GetPNeighbours(ParasiteBodyPart bodyPart)
        {
            if (!setup)
                throw new Exception("SetDimensions must be called first!");

            int x = (int)Math.Floor(bodyPart.Position.X / gridSize);
            int y = (int)Math.Floor(bodyPart.Position.Y / gridSize);

            List<PhysicsObject> returnList = new List<PhysicsObject>();

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

            returnList.Remove(bodyPart);

            return returnList;
        }

    }
}
