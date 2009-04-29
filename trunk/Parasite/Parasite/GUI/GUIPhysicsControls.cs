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
    class GUIPhysicsControls : GUIPanel
    {
        // Texture Information
        //string Tex_name;
        //float Tex_layer;
        //LevelArt Art;

        bool useTexture = false;

        // Information
        Vector2 Dimensions = new Vector2(10, 10);

        public GUIPhysicsControls(Game game)
            : base(game, new Vector2(200, 10), new Vector2(125, 135), "physcontroller", "Physics Controls")
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.minimised = false;
            base.Initialize();

            GUIButton btn = new GUIButton(Game, new Vector2(10, 10), "rewind", "<<");
            btn.Initialize();
            AddComponent(btn);

            btn = new GUIButton(Game, new Vector2(40, 10), "pause", "| |");
            btn.Initialize();
            AddComponent(btn);

            btn = new GUIButton(Game, new Vector2(65, 10), "play", " > ");
            btn.Initialize();
            AddComponent(btn);

            GUICheckbox cb = new GUICheckbox(Game, new Vector2(10, 50), "debugdraw", "Debug Draw");
            cb.Initialize();
            AddComponent(cb);

            GUILabel lbl = new GUILabel(Game, new Vector2(10, 80), "status", "Status: Paused");
            lbl.Initialize();
            AddComponent(lbl);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        void CancelMouseClick(GUIComponent sender, OnMouseClickEventArgs args)
        {

            ((GUIManager)Game.Services.GetService(typeof(IGUIManager))).RemoveComponent(Name);
        }
    }
}