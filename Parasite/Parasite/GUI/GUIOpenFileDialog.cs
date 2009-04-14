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
    class GUIOpenFileDialog : GUIPanel
    {
        public delegate void BoxOkHandler(string file, Vector2 position, bool dynamic);
        public event BoxOkHandler OnBoxOk;

        public GUIOpenFileDialog(Game game) : base(game, new Vector2(100, 100), new Vector2(600, 300), "ofdialog", "Open File...")
        {         
        }

        private GUIListBox fileList;

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            GUILabel label = new GUILabel(Game, new Vector2(10, 10), "lblOpen", "Filename:");
            label.Initialize();
            AddComponent(label);
            //GUIEditBox edit = new GUIEditBox(Game, new Vector2(100, 10), "edtOpen", 300, "");
            //edit.Initialize();
            //AddComponent(edit);
            GUIButton ok = new GUIButton(Game, new Vector2(480, 50), "okbutton", "Ok");
            ok.Initialize();
            ok.OnMouseClick += new GUIButton.MouseClickHandler(OkMouseClick);
            AddComponent(ok);
            GUIButton cancel = new GUIButton(Game, new Vector2(520, 50), "cancelbutton", "Cancel");
            cancel.Initialize();
            cancel.OnMouseClick += new GUIButton.MouseClickHandler(CancelMouseClick);
            AddComponent(cancel);

            GUICheckbox isphys = new GUICheckbox(Game, new Vector2(100, 50), "isphys", "Generate Collision Mesh");
            isphys.Initialize();
            AddComponent(isphys);

            // Add List Box
            List<GUIListBoxItem> guiListItems = new List<GUIListBoxItem>();

            string[] filenames = Directory.GetFiles("Content\\LevelArt");

            for (int i = 0; i < filenames.Length; i++)
            {
                string shortName = (string)filenames[i].Substring(17);
                guiListItems.Add(new GUIListBoxItem(Game, filenames[i], shortName));
            }

            fileList = new GUIListBox(Game, new Vector2(100, 10), "list box", guiListItems);
            fileList.Initialize();
            AddComponent(fileList);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        void CancelMouseClick(GUIComponent sender, OnMouseClickEventArgs args)
        {
            Close();
        }

        void OkMouseClick(GUIComponent sender, OnMouseClickEventArgs args)
        {
            string selectedItem = fileList.getSelectedLabel();
            //throw new NotImplementedException();
            if (selectedItem != null)
            {
                // load fileList.getSelectedItem();
                if (OnBoxOk != null)
                {
                    //fire off any mouseclick event handlers that are listening
                    OnBoxOk(selectedItem, camera.Position, ((GUICheckbox)getComponent("isphys")).Checked);
                }
            }
            Close();
        }

   }
}