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
    class GUITexureManager : GUIPanel
    {
        public delegate void BoxOkHandler(string file, Vector2 position);
        public event BoxOkHandler OnBoxOk;

        // Texture Information
        string Tex_name;
        float Tex_layer;
        LevelArt Art;

        public GUITexureManager(Game game) : base(game, new Vector2(700, 0), new Vector2(300, 500), "texturemanager", "Texture Properties")
        {         
        }

        private GUIListBox fileList;

        public void initTextureInformation(LevelArt levelArt)
        {
            Art = levelArt;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.minimised = true;
            base.Initialize();
            GUILabel label = new GUILabel(Game, new Vector2(10, 10), "texturename", "Texture : " + Art.Name);
            label.Initialize();
            AddComponent(label);

            GUILabel depthlabel = new GUILabel(Game, new Vector2(10, 30), "depth", "Depth : ");
            depthlabel.Initialize();
            AddComponent(depthlabel);

            GUIEditBox depthEdit = new GUIEditBox(Game, new Vector2(70, 30), "depth_edit", 3, Art.ScreenDepth.ToString());
            depthEdit.Initialize();
            AddComponent(depthEdit);

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
            GUIEditBox depthEdit = (GUIEditBox)this.getComponent("depth_edit");
            Art.ScreenDepth = float.Parse(depthEdit.Text);

            ((GUIManager)Game.Services.GetService(typeof(IGUIManager))).RemoveComponent(Name);
        }

   }
}