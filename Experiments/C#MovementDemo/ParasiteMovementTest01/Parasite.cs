using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ParasiteMovementTest01
{
    // Parasite is made up of parasite body parts
    class Parasite
    {
        public Vector2 position;    // Position of the Head
        public int numSegments;     // Number of Body Segments
        public int linkageDistance; // Distance the bodyparts are from eachother
        
        public List<ParasiteBodyPart> theParasite;

        public ParasiteBodyPart theHead;
        public ParasiteBodyPart theTail;

        public ContentManager content;

        public Parasite(int numSeg, Vector2 _position, ContentManager contMan)
        {
            position = _position;

            numSegments = numSeg;

            theParasite = new List<ParasiteBodyPart>();

            content = contMan;

            createBodyParts();
        }

        public void createBodyParts()
        {
            // Create the Head
            theHead = new ParasiteBodyPart(content.Load<Texture2D>("Sprites\\Head"), 3);
            
            // Create the Body Parts
            for (int i = 0; i < numSegments; i++)
            {
                ParasiteBodyPart bPart = new ParasiteBodyPart(content.Load<Texture2D>("Sprites\\Part"), 1);

                theParasite.Add(bPart);
            }

            // Create the Tail
            theTail = new ParasiteBodyPart(content.Load<Texture2D>("Sprites\\Tail"), 1);
        }

        public void Update()
        {
            theHead.position = position;

            Vector2 thePreviousPosition = theHead.position;
            thePreviousPosition.Y += theHead.centre.Y - 5;
            thePreviousPosition.X += theHead.centre.X / 2;

            foreach (ParasiteBodyPart bodyPart in theParasite)
            {
                // Update the Position relative to this awesome head
                bodyPart.updatePosition(thePreviousPosition);

                thePreviousPosition = bodyPart.position;
            }

            theTail.updatePosition(thePreviousPosition);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the Body Parts
            foreach (ParasiteBodyPart bodyPart in theParasite)
            {
                spriteBatch.Draw(bodyPart.sprite, bodyPart.position, Color.White);
            }

            // Draw the Head
            spriteBatch.Draw(theHead.sprite, theHead.position, Color.White);

            // Draw the Tail
            spriteBatch.Draw(theTail.sprite, theTail.position, Color.White);
        }
    }
}
