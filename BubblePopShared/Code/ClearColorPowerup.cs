using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BubblePop
{
    class ClearColorPowerup : Powerup
    {
        public ClearColorPowerup(Vector2 position, Color bubbleColor, Texture2D bubbleTexture, ContentManager Content) : base(position, bubbleColor, bubbleTexture, Content)
        {
            bubblesRequiredToActivate = 5;
        }

        public override void DoEffect(BubbleGrid bubbleGrid)
        {
            if (bubbleGrid.NumberOfActivatedBubbles() < bubblesRequiredToActivate)
            {
                return;
            }
            foreach (Bubble bubble in bubbleGrid.Bubbles)
            {
                if (bubbleColor == bubble.BubbleColor)
                {
                    bubble.Activate();
                }
            }
            Console.WriteLine("This powerup has been activated!");
        }
    }
}
