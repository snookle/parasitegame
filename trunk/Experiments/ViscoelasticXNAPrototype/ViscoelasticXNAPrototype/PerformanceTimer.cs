using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace ViscoelasticXNAPrototype
{

    public class SnookTimer
    {
        public string Name;
        private DateTime StartTime;
        private DateTime EndTime;
        public float Duration;
        private bool stop;

        public SnookTimer(string name)
        {
            Name = name;
        }

        public void Start()
        {
            StartTime = DateTime.Now;
            stop = false;
        }

        public void Stop()
        {
            stop = true;
            EndTime = DateTime.Now;
            Duration = EndTime.Millisecond - StartTime.Millisecond;
        }
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PerformanceTimer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Dictionary<string, SnookTimer> timers;
        private SpriteFont font;
        private SpriteBatch batch;

        public PerformanceTimer(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            timers = new Dictionary<string, SnookTimer>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>("TimerFont");
            batch = new SpriteBatch(this.GraphicsDevice);
            base.LoadContent();
        }

        public void StartTimer(string timer)
        {
            if (timers.ContainsKey(timer))
            {
                timers[timer].Start();
                return;
            }
            SnookTimer t = new SnookTimer(timer);
            timers.Add(timer, t);
        }

        public void StopTimer(string timer)
        {
            timers[timer].Stop();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            foreach (SnookTimer t in timers.Values)
            {
               // t.Update();
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            string s = "";
            foreach (SnookTimer t in timers.Values)
            {
                s += t.Name + ": " + ((t.Duration < 1) ? "<1" : t.Duration.ToString()) + "ms\n";  
            }

            batch.Begin();
                batch.DrawString(font, s, new Vector2(10, 10), Color.Black);
            batch.End();
            base.Draw(gameTime);
        }
    }
}