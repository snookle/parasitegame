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
        public delegate void BoxOkHandler(string name, string texturename);
        public event BoxOkHandler OnBoxOk;

        // Texture Information
        //string Tex_name;
        //float Tex_layer;
        //LevelArt Art;
        
        bool useTexture = false;

        // Information
        Vector2 Dimensions = new Vector2(10, 10);

        public GUIAddPhysPanel(Game game) : base(game, new Vector2(700, 0), new Vector2(300, 500), "physmanager", "PhysObj Properties")
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
            GUILabel label = new GUILabel(Game, new Vector2(10, 10), "physobjname", "Physics Object");
            label.Initialize();
            AddComponent(label);

            GUILabel depthlabel = new GUILabel(Game, new Vector2(10, 30), "type", "Type : ");
            depthlabel.Initialize();
            AddComponent(depthlabel);

            // Depth Options
            /*List<GUIListBoxItem> depthItems = new List<GUIListBoxItem>();

            depthItems.Add(new GUIListBoxItem(Game, "0", "Static Background"));
            depthItems.Add(new GUIListBoxItem(Game, "0.1", "Far Background"));
            depthItems.Add(new GUIListBoxItem(Game, "0.449", "Background"));
            depthItems.Add(new GUIListBoxItem(Game, "0.5", "Midground"));
            depthItems.Add(new GUIListBoxItem(Game, "0.501", "Foreground"));
            depthItems.Add(new GUIListBoxItem(Game, "0.9", "Far Foreground"));
            depthItems.Add(new GUIListBoxItem(Game, "1", "Static Foreground"));*/

            //GUIListBox depthEdit = new GUIListBox(Game, new Vector2(80, 30), "depth_edit", depthItems);
            //depthEdit.Initialize();

            // Setting current item
            //string currentDepth = Art.ScreenDepth.ToString();

            //depthEdit.SetItem(currentDepth);

            //AddComponent(depthEdit);

            List<GUIListBoxItem> physTypeItems = new List<GUIListBoxItem>();

            physTypeItems.Add(new GUIListBoxItem(Game, "static", "Static"));
            physTypeItems.Add(new GUIListBoxItem(Game, "rigid", "Rigid"));

            GUIListBox physTypeEdit = new GUIListBox(Game, new Vector2(80, 30), "phystype_edit", physTypeItems);
            physTypeEdit.Initialize();

            // Use Texture
            GUICheckbox useTextureCheck = new GUICheckbox(Game, new Vector2(80, 60), "usetexturecheck", "Use Texture");
            useTextureCheck.Initialize();
            AddComponent(useTextureCheck);
            useTextureCheck.CheckStateChange += new GUICheckbox.CheckChangeEventHandler(updateUseTexture);
            useSizeForObject();

            //GUIEditBox edit = new GUIEditBox(Game, new Vector2(100, 10), "edtOpen", 300, "");
            //edit.Initialize();
            //AddComponent(edit);
            GUIButton ok = new GUIButton(Game, new Vector2(100, 400), "okbutton", "Apply");
            ok.Initialize();
            ok.OnMouseClick += new GUIButton.MouseClickHandler(OkMouseClick);
            ok.Location.X -= ok.Bounds.Width / 2;
            AddComponent(ok);
            GUIButton cancel = new GUIButton(Game, new Vector2(150, 400), "cancelbutton", "Cancel");
            cancel.Initialize();
            cancel.OnMouseClick += new GUIButton.MouseClickHandler(CancelMouseClick);
            cancel.Location.X -= cancel.Bounds.Width / 2;
            AddComponent(cancel);

            AddComponent(physTypeEdit);
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
            // Update Depth
            //GUIListBox depthEdit = (GUIListBox)this.getComponent("depth_edit");
            //Art.ScreenDepth = float.Parse(depthEdit.getSelectedItem());

            string texturename = "";

            // Compile data
            if (useTexture)
            {
                //texturename = 
            }
            else
            {
                GUIEditBox tempwidth = (GUIEditBox)this.getComponent("s_width");
                GUIEditBox tempheight = (GUIEditBox)this.getComponent("s_height");
            }


            if (OnBoxOk != null)
            {
                //fire off any mouseclick event handlers that are listening
                OnBoxOk("test","");
            }
            

            ((GUIManager)Game.Services.GetService(typeof(IGUIManager))).RemoveComponent(Name);
        }

   }
}