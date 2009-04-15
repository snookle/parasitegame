using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// A panel is a container of GUI components. Acts sort of like a moveable window.
    /// </summary>
    class GUIAddPhysPanel : GUIPanel
    {
        public delegate void BoxOkHandler(string texture, float mass, float bouncy, float friction);
        public event BoxOkHandler OnBoxOk;

        // Texture Information
        //string Tex_name;
        //float Tex_layer;
        //LevelArt Art;
        
        bool useTexture = false;

        // Information
        Vector2 Dimensions = new Vector2(10, 10);

        public GUIAddPhysPanel(Game game) : base(game, new Vector2(400, 10), new Vector2(400, 450), "physmanager", "PhysObj Properties")
        {         
        }

        private GUIListBox fileList;

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.minimised = false;
            base.Initialize();
            GUILabel label = new GUILabel(Game, new Vector2(10, 10), "physobjname", "Texture:");
            label.Initialize();
            AddComponent(label);

            // Use Texture
            List<GUIListBoxItem> guiListItems = new List<GUIListBoxItem>();

            string[] filenames = Directory.GetFiles("Content\\LevelArt");

            for (int i = 0; i < filenames.Length; i++)
            {
                string shortName = (string)filenames[i].Substring(17);
                guiListItems.Add(new GUIListBoxItem(Game, filenames[i], shortName));
            }

            fileList = new GUIListBox(Game, new Vector2(10, 30), "list box", guiListItems);
            fileList.Initialize();


            GUILabel weightlbl = new GUILabel(Game, new Vector2(10, 60), "lblMass", "Mass");
            weightlbl.Initialize();
            AddComponent(weightlbl);
            
            GUIEditBox weight = new GUIEditBox(Game, new Vector2(100, 60), "edtMass", 50, "10");
            weight.Initialize();
            AddComponent(weight);

            GUILabel bouncylbl = new GUILabel(Game, new Vector2(10, 90), "lblBounce", "Bounciness");
            bouncylbl.Initialize();
            AddComponent(bouncylbl);

            GUIEditBox bouncy = new GUIEditBox(Game, new Vector2(100, 90), "edtBouncy", 50, "500");
            bouncy.Initialize();
            AddComponent(bouncy);

            GUILabel frictionlbl = new GUILabel(Game, new Vector2(10, 120), "lblFriction", "Friction");
            frictionlbl.Initialize();
            AddComponent(frictionlbl);

            GUIEditBox friction = new GUIEditBox(Game, new Vector2(100, 120), "edtFriction", 50, "500");
            friction.Initialize();
            AddComponent(friction);

            GUIButton ok = new GUIButton(Game, new Vector2(150, 375), "okbutton", "Apply");
            ok.Initialize();
            ok.OnMouseClick += new GUIButton.MouseClickHandler(OkMouseClick);
            ok.Location.X -= ok.Bounds.Width / 2;
            AddComponent(ok);
            GUIButton cancel = new GUIButton(Game, new Vector2(200, 375), "cancelbutton", "Cancel");
            cancel.Initialize();
            cancel.OnMouseClick += new GUIButton.MouseClickHandler(CancelMouseClick);
            cancel.Location.X -= cancel.Bounds.Width / 2;
            AddComponent(cancel);

            AddComponent(fileList);
        }

        private void useTextureForObject()
        {
            GUILabel t_label = new GUILabel(Game, new Vector2(10, 100), "t_label", "Texture Name : ");
            t_label.Initialize();
            AddComponent(t_label);
        }

        private void useSizeForObject()
        {
            GUILabel s_label = new GUILabel(Game, new Vector2(10, 100), "s_label", "Dimensions : ");
            s_label.Initialize();
            AddComponent(s_label);

            GUIEditBox s_width = new GUIEditBox(Game, new Vector2(100, 100), "s_width", 50, Dimensions.X.ToString());
            s_width.Initialize();
            AddComponent(s_width);

            GUIEditBox s_height = new GUIEditBox(Game, new Vector2(160, 100), "s_height", 50, Dimensions.Y.ToString());
            s_height.Initialize();
            AddComponent(s_height);
        }

        private void resetTextureAndSize()
        {
            GUIComponent gc;
            for (int i = components.Count - 1; i > 0; i--)
            {
                gc = components[i];
                if (gc.Name.Substring(0, 2) == "s_" || gc.Name.Substring(0, 2) == "t_")
                {
                    gc.Dispose();
                    components.Remove(gc);
                }
            }
        }

        void updateUseTexture(GUIComponent sender, CheckStateEventArgs args)
        {
            resetTextureAndSize();
            if (useTexture)
            {
                useSizeForObject();
            }
            else
            {
                useTextureForObject();
            }

            useTexture = !useTexture;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        void CancelMouseClick(GUIComponent sender, OnMouseClickEventArgs args)
        {

            ((GUIManager)Game.Services.GetService(typeof(IGUIManager))).RemoveComponent(Name);
        }

        void OkMouseClick(GUIComponent sender, OnMouseClickEventArgs args)
        {
            float bounciness = Convert.ToSingle(((GUIEditBox)getComponent("edtBouncy")).Text)/1000;
            float mass = Convert.ToSingle(((GUIEditBox)getComponent("edtMass")).Text);
            float friction = Convert.ToSingle(((GUIEditBox)getComponent("edtFriction")).Text) / 1000;
            string texture = ((GUIListBox)getComponent("list box")).getSelectedLabel();
            texture = "LevelArt\\" + texture.Substring(0, texture.Length - 4);
            

            if (OnBoxOk != null)
            {
                //fire off any mouseclick event handlers that are listening
                OnBoxOk(texture, mass, bounciness, friction);
            }
            

            ((GUIManager)Game.Services.GetService(typeof(IGUIManager))).RemoveComponent(Name);
        }

   }
}