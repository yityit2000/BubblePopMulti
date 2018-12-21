using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BubblePop
{
    abstract class Powerup : Bubble
    {
        public Powerup(Vector2 position, Color bubbleColor, Texture2D bubbleTexture) : base(position, bubbleColor, bubbleTexture)
        {
        }


    }
}
