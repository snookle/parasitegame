using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Parasite
{
    class Level : DrawableGameComponent
    {
        private List<LevelArt> Art = new List<LevelArt>();
        private List<DynamicLevelObject> DynamicObjects = new List<DynamicLevelObject>();

        private SpriteBatch artBatch;
        private InputHandler input;
        private Camera camera;
        private DeveloperConsole console;
        private GUIManager guimanager;

        private Dictionary<string, LevelArt> textures = new Dictionary<string, LevelArt>();
        private string LevelFilename = "";


        //LEVEL EDITOR SPECIFIC STUFF
        private bool editing = true;
        private LevelArt selectedArt = null;
        private Vector2 selectionOffset = Vector2.Zero;
        private float previousY = 0;
        private int gridSize = 5;
        private bool showGrid = false;
        private PrimitiveBatch gridBatch;

        public Level(Game game, string filename)
            : base(game)
        {
            LevelFilename = filename;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            artBatch = new SpriteBatch(GraphicsDevice);
            gridBatch = new PrimitiveBatch(GraphicsDevice);

            input = (InputHandler)Game.Services.GetService(typeof(IInputHandler));
            camera = (Camera)Game.Services.GetService(typeof(ICamera));
            console = (DeveloperConsole)Game.Services.GetService(typeof(IDeveloperConsole));

            guimanager = (GUIManager)Game.Services.GetService(typeof(IGUIManager));

            console.MessageHandler += new DeveloperConsole.DeveloperConsoleMessageHandler(ConsoleMessageHandler);
            LoadLevel(LevelFilename);

            InitEditor();
            //Random r = new Random();
            //for (var i = 0; i < 100; i++)
            //{
            //    DynamicObjects.Add(new DynamicLevelObject(Game, new Vector2(r.Next(200)-100, 50 - r.Next(400)), @"LevelArt\WallTest01")); 
            //}
            DynamicObjects.Add(new DynamicLevelObject(Game, new Vector2(-170, -100), @"LevelArt\WallTest01"));
            DynamicObjects.Add(new DynamicLevelObject(Game, new Vector2(170, -100), @"LevelArt\WallTest01")); 
        }

        /// <summary>
        /// Initialises the level editor GUI
        /// </summary>
        public void InitEditor()
        {
            // Set up GUI
            GUIButton but_loadtexture = guimanager.AddButton(new Vector2(10, 10), "loadtexture", "Load Texture", new GUIButton.MouseClickHandler(testFunction));
            
            //guimanager.AddButton(new Vector2(10,50),"texturedphysobj","Load Textured Physics Object", new GuiBut

            guimanager.AddEditBox(new Vector2(10, 70), "levelname", 100, "levelname");
            guimanager.AddButton(new Vector2(10, 100), "loadlevel", "Load Level");
            guimanager.AddButton(new Vector2(10, 130), "savelevel", "Save Level");

            //guimanager.AddLabel(new Vector2(50, 150), "label", "THIS IS A LABEL! WOOO!");

            console.Write("Level Editor Loaded.");
        }

        public void testFunction(GUIComponent sender, OnMouseClickEventArgs args)
        {
            //console.Write("BUTTON PRESSED");
            //GUIEditBox testbox = (GUIEditBox)guimanager.GetComponent("texturename");
            //console.Write(testbox.Text);
            //LoadTexture(testbox.Text, new Vector2(0, 0));
            GUIOpenFileDialog ofd = new GUIOpenFileDialog(Game);
            ofd.Initialize();
            ofd.OnBoxOk += new GUIOpenFileDialog.BoxOkHandler(selectTexture);
            guimanager.AddComponent(ofd);
        }

        void selectTexture(string name, Vector2 location)
        {
            LoadTexture(name.Substring(0,name.Length-4), location);
        }

        /// <summary>
        /// Attempt at loading dynamic textures based on an input name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LevelArt LoadTexture(string name, Vector2 location)
        {
            LevelArt texture;
            console.Write("Attempting to Load Texture " + name);
            {
                texture = new LevelArt(Game, location, @"LevelArt\" + name);
               // textures.Add(name, texture);
                Art.Add(texture);
                console.Write("Texture Loaded.");

                return texture;
            }
        }

        /// <summary>
        /// Attempt at removing textures
        /// </summary>
        /// <param name="sprite"></param>
        public void removeTexture(LevelArt sprite)
        {
            Art.Remove(sprite);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (DynamicLevelObject da in DynamicObjects)
            {
                da.Update();
            }

            if (editing)
            {
                //Save the level
                if (input.IsKeyPressed(this, Keys.S))
                {
                    
                }

                if (input.IsKeyPressed(this, Keys.L))
                {
                    LoadTexture("WallTest01", camera.MouseToWorld());
                }

                Vector2 mousePos = camera.MouseToWorld(); 
                
                //show the grid
                if (selectedArt != null && input.IsKeyDown(this, Keys.M))
                    showGrid = true;
                else
                    showGrid = false;

                // Remove if Del is pressed
                if (selectedArt != null && input.IsKeyPressed(this, Keys.Delete))
                {
                    removeTexture(selectedArt);
                    selectedArt = null;
                }

                //move the selected piece around
                if (selectedArt != null && input.IsMouseMoving() && input.IsKeyDown(this, Keys.M))
                {
                    selectedArt.EditorMove(selectionOffset, gridSize);

                    console.Write(camera.MouseToWorld().ToString());
                    console.Write("Moving : " + selectedArt.WorldPosition.ToString());

                }
                else if (selectedArt != null && input.IsMouseMoving() && input.IsKeyDown(this, Microsoft.Xna.Framework.Input.Keys.R))
                {
                    // Rotation
                    if (mousePos.Y < previousY)
                    {
                        selectedArt.EditorRotate(selectionOffset, -0.1f);
                    }
                    else
                    {
                        selectedArt.EditorRotate(selectionOffset, 0.1f);
                    }

                    previousY = mousePos.Y;
                }
                else
                {
                    foreach (LevelArt la in Art)
                    {
                        //check each level art for mouse collision and click
                        if (la.BoundingBox.Contains(Convert.ToInt32(mousePos.X), Convert.ToInt32(mousePos.Y)) && input.IsMouseButtonPressed("left") && !la.CheckTrans(input.MousePosition - la.GetScreenPosition()))
                        {
                            //deselect the current selected levelart (if there was one)
                            //before selecting the newly clicked piece
                            if (selectedArt != null && selectedArt != la)
                            {
                                selectedArt.EditorSelect(false);
                            }

                            //tell the piece that it's been selected
                            la.EditorSelect(true);
                            selectedArt = la;

                            if ((GUITexureManager)guimanager.GetComponent("texturemanager") != null)
                            {
                                ((GUIManager)Game.Services.GetService(typeof(IGUIManager))).RemoveComponent("texturemanager");
                            }
                            EditorDisplayTextureInformation();

                            //calculate an offset based on where the mouse was when it was clicked
                            //so we don't snap the level art origin to the mouse position
                            selectionOffset = camera.MouseToWorld() - la.WorldPosition;
                        }
                        else if (input.IsKeyPressed(this, Microsoft.Xna.Framework.Input.Keys.D))
                        {
                            // If nothing is selected, and the D key is pressed, deselect
                            if (selectedArt != null)
                            {
                                selectedArt.EditorSelect(false);
                                if ((GUITexureManager)guimanager.GetComponent("texturemanager") != null)
                                {
                                    ((GUIManager)Game.Services.GetService(typeof(IGUIManager))).RemoveComponent("texturemanager");
                                }
                            }
                            selectedArt = null;
                        }
                    }
                }
            }
        }

        private void EditorDisplayTextureInformation()
        {
            GUITexureManager textureman = new GUITexureManager(Game);
            textureman.initTextureInformation(selectedArt);
            textureman.Initialize();
            //ofd.OnBoxOk += new GUIOpenFileDialog.BoxOkHandler(selectTexture);
            guimanager.AddComponent(textureman);


            /*GUILabel infolabel = (GUILabel)guimanager.GetComponent("texturelabel");
            if (infolabel == null)
            {
                if (selectedArt == null)
                {
                    guimanager.AddLabel(new Vector2(400, 10), "texturelabel", "Selected Texture:");
                }
                else
                {
                    guimanager.AddLabel(new Vector2(400, 10), "texturelabel", "Selected Texture: " + selectedArt.Name);
                }
            }
            else
            {
                if (selectedArt == null)
                {
                    infolabel.Text = "Selected Texture: ";
                }
                else
                {
                    infolabel.Text = "Selected Texture: " + selectedArt.Name;
                }
            }*/
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (editing)
            {
                if (showGrid)
                {
                    int xbounds = GraphicsDevice.Viewport.Width / gridSize;
                    int ybounds = GraphicsDevice.Viewport.Height / gridSize;
                    Color gridColor = Color.Gray;
                    gridColor.A = 128;
                    gridBatch.Begin(PrimitiveType.LineList);
                    for (int x = 0; x < xbounds; x++)
                    {
                        gridBatch.AddVertex(new Vector2((x * gridSize), 0), gridColor);
                        gridBatch.AddVertex(new Vector2((x * gridSize), GraphicsDevice.Viewport.Height), gridColor);
                    }

                    for (int y = 0; y < ybounds; y++)
                    {
                        gridBatch.AddVertex(new Vector2(0, y * gridSize), gridColor);
                        gridBatch.AddVertex(new Vector2(GraphicsDevice.Viewport.Width, y * gridSize), gridColor);
                    }
                    gridBatch.End();
                }
            }

            artBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            foreach (LevelArt la in Art)
            {
                artBatch.Draw(la.Texture, la.GetScreenPosition(), null, la.Tint, la.Rotation, la.Origin, camera.ZoomLevel, SpriteEffects.None, 1 - la.ScreenDepth);
                if (editing && la.EditorSelected)
                    la.DrawBoundingBox();
            }

            foreach (DynamicLevelObject da in DynamicObjects)
            {
                if (da.Texture != null)
                {
                    //artBatch.Draw(da.Texture, new Rectangle((int)da.GetScreenPosition().X, (int)da.GetScreenPosition().Y, da.Texture.Width, da.Texture.Height), null, Color.White, da.Rotation,da.Origin, SpriteEffects.None, 0);
                    artBatch.Draw(da.Texture, da.GetScreenPosition(), null, Color.White, da.Rotation, da.Origin, camera.ZoomLevel, SpriteEffects.None, 0);
                }
            }
            artBatch.End();
        }
        /// <summary>
        /// Lists all the files in a given directory
        /// </summary>
        /// <param name="name">Name of the directory to list the files in.</param>
        public void listDirectory(string name)
        {
            string[] filenames = Directory.GetFiles(name);
            for (int i = 0; i < filenames.Length; i++)
            {
                console.Write(" * " + filenames[i]);
            }
            console.Write("Listing Complete, " + filenames.Length + " files");
            console.Write("");
        }

        /// <summary>
        /// Resets all the elements in the world
        /// </summary>
        private void resetLevel()
        {
            // Reset Physics Objects
            foreach (DynamicLevelObject dlo in DynamicObjects)
            {
                dlo.WorldPosition = dlo.StartingPosition;
                dlo.Rotation = dlo.StartingRotation;
                // How to reset physics object ? 
                dlo.PhysicsBody.SetXForm(new Box2DX.Common.Vec2(dlo.StartingPosition.X, dlo.StartingPosition.Y), dlo.Rotation);
                dlo.PhysicsBody.SetLinearVelocity(new Box2DX.Common.Vec2(0, 0));
                dlo.PhysicsBody.SetAngularVelocity(0);
            }
        }

        /// <summary>
        /// Listens for certain commands from the console and processes them
        /// </summary>
        /// <param name="command">The console command</param>
        /// <param name="argument">Any arguments passed as a single string.</param>
        public void ConsoleMessageHandler(string command, string argument)
        {
            switch (command.ToLower())
            {
                case "listtextures" :
                    console.Write("Textures Listed : ");
                    listDirectory("Content\\LevelArt");
                    console.CommandHandled = true;
                    break;
                case "resetlevel":
                    console.Write("Level Reset");
                    resetLevel();
                    console.CommandHandled = true;
                    break;
                case "listlevels":
                    console.Write("Levels Listed : ");
                    listDirectory("Content\\Levels");
                    console.CommandHandled = true;
                    break;
                case "loadtexture" :
                    LoadTexture(argument, new Vector2(0, 0));
                    console.CommandHandled = true;
                    break;
                case "loadlevel" :
                    ClearLevel();
                    LoadLevel(argument);
                    console.CommandHandled = true;
                    break;
                case "savelevel" :
                    SaveLevel(argument);
                    console.CommandHandled = true;
                    break;
                case "gridsize" :
                    if (String.IsNullOrEmpty(argument))
                    {
                        console.Write("Gridsize is " + gridSize);
                        return;
                    }

                    int size = Convert.ToInt32(argument);
                    if (size <= 0)
                    {
                        console.Write("Error: gridsize cannot be <= 0", ConsoleMessageType.Error);
                    }
                    else
                    {
                        gridSize = size;
                        console.Write("Gridsize changed to " + gridSize);
                    }
                    console.CommandHandled = true;
                    break;
            }
        }

        /// <summary>
        /// Completely clears out all level data.
        /// </summary>
        public void ClearLevel()
        {
            Art.Clear();
            selectedArt = null;
        }

        /// <summary>
        /// Saves the level to the specified filename
        /// </summary>
        /// <param name="filename">Filename to save the level too.</param>
        public void SaveLevel(string filename)
        {
            LevelFilename = filename;
            if (string.IsNullOrEmpty(LevelFilename))
            {
                console.Write("SaveLevel failed: No filename specified", ConsoleMessageType.Error);
                return;
            }

            try {
                Directory.CreateDirectory(@"Content\Levels");
                BinaryWriter file = new BinaryWriter(File.Open(@"Content\Levels\" +LevelFilename + ".pld", FileMode.Create));
                file.Write(Art.Count);
                for (int i = 0; i < Art.Count; i++)
                {
                    Art[i].EditorSelect(false);
                    Art[i].SaveLevelData(file);
                }
                console.Write(@"Level saved as: Content\Levels\" + LevelFilename + ".pld");
                file.Close();
            }
            catch (Exception e)
            {
                console.Write("SaveLevel failed: " + e.Message, ConsoleMessageType.Error);
            }
           
        }
        /// <summary>
        /// Loads and populates a level from a given filename.
        /// </summary>
        /// <param name="filename">Filename to load the level from.</param>
        /// <returns>True if level loaded, false otherwise.</returns>
        public bool LoadLevel(string filename)
        {
            LevelFilename = filename;
            if (String.IsNullOrEmpty(LevelFilename))
            {
                console.Write("LoadLevel failed: No filename specified", ConsoleMessageType.Error);
                return false;
            }
            try
            {
                BinaryReader file = new BinaryReader(File.Open(@"Content\Levels\" + filename + ".pld", FileMode.Open));
                int count = file.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    Art.Add(new LevelArt(Game, file));
                }
                file.Close();
                console.Write("Level loaded as: " + LevelFilename);
                return true;
            }
            catch (Exception e)
            {
                console.Write("LoadLevel failed: " + e.Message, ConsoleMessageType.Error);
                return false;
            }
        }
    }
}
