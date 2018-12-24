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
            font = Content.Load<SpriteFont>("PowerupUi");
        }

        public abstract void DoEffect(BubbleGrid bubbleGrid);

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bubbleTexture, position, bubbleColor);
            /* There is a smaller bubble in the lower righthand corner of the powerup texture. "powerupUiPosition" is doing some math to place the
             * string representing the bubbles required to activate the powerup in the center of that smaller bubble. I don't have a constant for
             * exactly that value because the value is dependant on the width and height of the string itself, which I calculate here. */
            Vector2 sizeOfString = font.MeasureString(bubblesRequiredToActivate.ToString());
            Vector2 powerupUiPosition = new Vector2(position.X + Constants.POWERUP_UI_POSITION_OFFSET.X - sizeOfString.X / 2, 
                                                    position.Y + Constants.POWERUP_UI_POSITION_OFFSET.Y - sizeOfString.Y / 2);
            spriteBatch.DrawString(font, bubblesRequiredToActivate.ToString(), powerupUiPosition, Color.White);
        }
    }
}
