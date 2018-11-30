using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubblePop
{
    class Bubble
    {
        private Vector2 position;
        Color bubbleColor;
        private Vector2 center;
        
        // activated refers to when either a user clicks on a bubble or when it's next to one of the same color that's been activated
        private bool activated;

        private Texture2D bubbleTexture;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Color BubbleColor
        {
            get { return bubbleColor; }
        }

        public bool Activated
        {
            get { return activated; }
            set { activated = value;  }
        }

        public Bubble(Vector2 position, Color bubbleColor, Texture2D bubbleTexture)
        {
            this.position = position;
            this.bubbleColor = bubbleColor;
            this.bubbleTexture = bubbleTexture;
            activated = false;
            center = new Vector2(position.X + Constants.BUBBLE_RADIUS, position.Y + Constants.BUBBLE_RADIUS);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(bubbleTexture, position, bubbleColor);     
        }

        // This method checks to see whether a point is within the bubble. This is easy to tell by making sure the distance
        // between the two points is less than the radius. First we get the distance using Pythagorean Theorem. The radius
        // and the point make a right triangle if you trace the x and y paths and then connect the points. So using X^2 + Y^2 = D^2,
        // where "D" is the distance, we can see whether D is less than the radius. That being said, it's programatically cheaper
        // to see if the square distance is less than the radius (D^2 < radius ^ 2). If it is, then the click is within the circle,
        // and we have collision!
        public bool Intersects(Vector2 click)
        {
            // Pythagorus strikes again
            float squareDistance = (click.X - center.X) * (click.X - center.X) + (click.Y - center.Y) * (click.Y - center.Y);
            if (squareDistance < (Constants.BUBBLE_RADIUS * Constants.BUBBLE_RADIUS))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // This method moves the bubble down one position.
        public void Drop()
        {
            position = new Vector2(position.X, position.Y + Constants.WORLD_UNIT);
            // It took me a loooong time to add the following line. The game was running super funky before!
            center = Vector2.Add(position, Constants.RADIUS_VECTOR);
        }

        public void MoveTo(float x, float y)
        {
            position = new Vector2(x,y);
            center = Vector2.Add(position, Constants.RADIUS_VECTOR);
        }

        public void Activate()
        {
            if (!activated)
            {
                activated = true;
            }
        }

        public void Deactivate()
        {
            if (activated)
            {
                activated = false;
            }
        }
    }
}
