using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BubblePop
{
    class ClearRowAndColumnPowerup : Powerup
    {
        public ClearRowAndColumnPowerup(Vector2 position, Color bubbleColor, Texture2D bubbleTexture, ContentManager Content) : base(position, bubbleColor, bubbleTexture, Content)
        {
            bubblesRequiredToActivate = 3;
        }

        public override void DoEffect(BubbleGrid bubbleGrid)
        {
            foreach (Bubble bubble in bubbleGrid.Bubbles)
            {
                if (bubble.Position.X == position.X || bubble.Position.Y == position.Y)
                {
                    bubble.Activate();
                }
            }
        }
    }
}
