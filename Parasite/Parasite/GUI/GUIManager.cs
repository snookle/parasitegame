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
    interface IGUIManager { };
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class GUIManager : DrawableGameComponent, IGUIManager
    {
        private List<GUIComponent> components = new List<GUIComponent>();

        public GUIManager(Game game)
            : base(game)
        {
            Game.Services.AddService(typeof(IGUIManager), this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            //GUIPanel gp = new GUIPanel(Game, new Vector2(100, 100), new Vector2(400, 400), "panel1", "This is a panel");
            //gp.Initialize();
            //components.Add(gp);
            base.Initialize();
        }

        /// <summary>
        /// Finds a component from their name
        /// </summary>
        /// <param name="name">name of the component to findd</param>
        /// <returns>the component named by name, false otherwise</returns>
        public GUIComponent GetComponent(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            foreach (GUIComponent g in components)
            {
                if (g.Name.ToLower() == name.ToLower())
                {
                    return g;
                }
            }
            return null;
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
        public GUIButton AddButton(Vector2 location, string name, string caption)
        {
            GUIButton b = new GUIButton(Game, location, name, caption);
            b.Initialize();
            components.Add(b);
            return b;
        }

        public void AddComponent(GUIComponent component)
        {
            components.Add(component);
        }
        
        /// <summary>
        /// Adds a new button to the GUI with an associated event handler
        /// </summary>
        /// <param name="location">Screen location of the top left corner</param>
        /// <param name="name">Name of the component</param>
        /// <param name="caption">Caption to be displayed on the button</param>
        public GUIButton AddButton(Vector2 location, string name, string caption, GUIButton.MouseClickHandler OnMouseClick)
        {
            GUIButton b = new GUIButton(Game, location, name, caption);
            b.Initialize();
            b.OnMouseClick += OnMouseClick;
            components.Add(b);
            return b;
        }
        /// <summary>
        /// Adds a new checkbox to the GUI
        /// </summary>
        /// <param name="location">Screen location of the top left corner</param>
        /// <param name="name">Name of the component</param>
        /// <param name="caption">Caption to be displayed on the checkbox</param>
        internal GUICheckbox AddCheckbox(Vector2 location, string name, string caption)
        {
            GUICheckbox c = new GUICheckbox(Game, location, name, caption);
            c.Initialize();
            components.Add(c);
            return c;
        }

        internal GUIEditBox AddEditBox(Vector2 location, string name, int length, string startingText)
        {
            GUIEditBox e = new GUIEditBox(Game, location, name, length, startingText);
            e.Initialize();
            components.Add(e);
            return e;
        }

        internal GUILabel AddLabel(Vector2 location, string name, string text)
        {
            GUILabel l = new GUILabel(Game, location, name, text);
            l.Initialize();
            components.Add(l);
            return l;
        }

        internal bool RemoveComponent(string name)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Name.ToLower() == name.ToLower())
                {
                    GUIComponent c = components[i];
                    components.RemoveAt(i);
                    c.Dispose();
                    c = null;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Update(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GUIComponent c in components)
            {
                c.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
