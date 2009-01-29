using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GUIManager : Microsoft.Xna.Framework.GameComponent
    {
        private List<GUIComponent> components = new List<GUIComponent>();

        public GUIManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            AddCheckbox(new Vector2(10, 10), "", "Collision Mesh");
            AddCheckbox(new Vector2(10, 30), "", "Background");


            AddButton(new Vector2(10, 100), "", "Load Texture");
            AddButton(new Vector2(10, 130), "", "Clone Texture");
            AddButton(new Vector2(10, 160), "", "Deselect All");


            base.Initialize();
        }
 
        /// <summary>
        /// Shows or hides all the components managed by this gui manager.
        /// </summary>
        /// <param name="show">Whether or not to show the components</param>
        public void ShowComponents(bool show)
        {
            foreach (GUIComponent c in components)
            {
                c.Hidden = !show;
            }
        }

        /// <summary>
        /// Adds a new button to the GUI
        /// </summary>
        /// <param name="location">Screen location of the top left corner</param>
        /// <param name="name">Name of the component</param>
        /// <param name="caption">Caption to be displayed on the button</param>
        public void AddButton(Vector2 location, string name, string caption)
        {
            GUIButton b = new GUIButton(Game, location, name, caption);
            Game.Components.Add(b);
            components.Add(b);
        }

        /// <summary>
        /// Adds a new checkbox to the GUI
        /// </summary>
        /// <param name="location">Screen location of the top left corner</param>
        /// <param name="name">Name of the component</param>
        /// <param name="caption">Caption to be displayed on the checkbox</param>
        public void AddCheckbox(Vector2 location, string name, string caption)
        {
            GUICheckbox c = new GUICheckbox(Game, location, name, caption);
            Game.Components.Add(c);
            components.Add(c);
        }



        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}