using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BubblePop
{
    abstract class Powerup : Bubble
    {
        protected int bubblesRequiredToActivate;
        protected SpriteFont font;

        public Powerup(Vector2 position, Color bubbleColor, Texture2D bubbleTexture, ContentManager Content) : base(position, bubbleColor, bubbleTexture)
        {
            font = Content.Load<SpriteFont>("Score");
        }

        public abstract void DoEffect(BubbleGrid bubbleGrid);

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bubbleTexture, position, bubbleColor);
            // Right now, we are just identifying powerups based on them having a number inside them that displays the bubbles needed to activate them.
            // Eventually, we'll probably have a different texture associated with different powerups
            Vector2 sizeOfString = font.MeasureString(bubblesRequiredToActivate.ToString());
            spriteBatch.DrawString(font, bubblesRequiredToActivate.ToString(), new Vector2(center.X - sizeOfString.X / 2, center.Y - sizeOfString.Y / 2), Color.White);
        }
    }
}
