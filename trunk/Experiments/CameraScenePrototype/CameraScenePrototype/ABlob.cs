using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CameraScenePrototype
{
    class ABlob : SceneNode
    {
        private InputHandler input;
        public bool humanControlled;

        public ABlob(Game game, string TextureName, Vector2 StartingPosition)
            : base(game, TextureName, StartingPosition)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandlerComponent));   
        }

        public override void Update()
        {
            if (humanControlled && input.MouseDisplacement() != Vector2.Zero)
            {
                Position = Vector2.Subtract(Position, input.MouseDisplacement());// input.MouseDisplacement();
            }
        }
    }

       
}
