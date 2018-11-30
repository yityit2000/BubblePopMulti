using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BubblePop
{
    class BubbleGrid
    {
        Random random;

        MouseState oldState = Mouse.GetState();

        List<Bubble> bubbles;

        bool readyToRemoveActivatedBubbles = false;

        // Represents the amount of colors in the grid.
        int difficulty;

        readonly Texture2D bubbleSprite;
        readonly Texture2D activateBubbleOverlaySprite;

        public BubbleGrid(ContentManager Content)
        {
            bubbles = new List<Bubble>();
            random = new Random();
            bubbleSprite = Content.Load<Texture2D>("bubble");
            difficulty = Constants.STARTING_DIFFICULTY;
            activateBubbleOverlaySprite = Content.Load<Texture2D>("bubble_outline");
        }
        
        public void Initialize()
        {
            int bubbleCounter = 0;
            // Set up initial grid of bubbles.
            for (int i = 0; i < Constants.GRID_HEIGHT_IN_UNITS ; i++)
            {
                for (int j = 0; j < Constants.GRID_WIDTH_IN_UNITS; j++)
                {
                    Vector2 thisBubblesPosition = Vector2.Add(Constants.BUBBLE_GRID_ORIGIN, new Vector2(j * Constants.WORLD_UNIT, i * Constants.WORLD_UNIT));
                    Bubble bubble = new Bubble(thisBubblesPosition, GenerateRandomColor(), bubbleSprite);
                    Console.WriteLine("Bubble " + bubbleCounter + ": (" + thisBubblesPosition.X + ", " + thisBubblesPosition.Y + ")");
                    bubbles.Add(bubble);

                }
            }
            
            
        }

        public void Update(GameTime gameTime, Camera2D camera)
        {
            MouseState newState = Mouse.GetState();

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                Console.WriteLine(newState.X + ", " + newState.Y);
                Vector2 clickLocation = camera.ScreenToWorld(new Vector2(newState.X, newState.Y));
                foreach (Bubble bubble in bubbles)
                {
                    // If we click on a deactivated bubble, we want to deactivate whatever has been activated first
                    if (bubble.Intersects(clickLocation) && !bubble.Activated)
                    {
                        DeactivateAllBubbles();
                    }
                    else if(bubble.Intersects(clickLocation) && bubble.Activated)
                    {
                        readyToRemoveActivatedBubbles = true;
                        break;
                    }
                }

                // We need to do this outside of the above loop because it's still looping through all the bubbles on the list. We can't
                // delete bubbles that it's going to try to loop through later, so we instead tell the program "Hey, by the way, when you're
                // done with that loop, go ahead and remove all those activated bubbles, yeah?" And the program is like "Yeah, man, got it."
                if (readyToRemoveActivatedBubbles)
                {
                    RemoveActivatedBubbles();
                    DropFloatingBubbles();
                    CollapseBubbleColumns();
                    readyToRemoveActivatedBubbles = false;
                    // This is the last thing we need to do in this function after a click, so we leave before doing more loops.
                    oldState = newState;
                    return;
                }

                foreach (Bubble bubble in bubbles)
                {
                    if (bubble.Intersects(clickLocation))
                    {
                        bubble.Activate();
                    }
                }
                // This loop is to activate all the connected bubbles to the one clicked. We loop it the number of times there are
                // rows in the grid to make sure we cover all possible connected bubbles. Ideally we would do a recursive method call
                // on ActivateConnectedBubbles, but c# isn't optimized for such an operation.
                for (int i = 0; i < Constants.GRID_HEIGHT_IN_UNITS; i++)
                {
                    foreach (Bubble bubble in bubbles)
                    {
                        if (bubble.Activated)
                        {
                            ActivateConnectedBubbles(bubbles.IndexOf(bubble));
                        }
                    }
                }
            }

            oldState = newState;

            // Check to see if level has been cleared
            if (bubbles.Count == 0)
            {
                if (difficulty < 7)
                {
                    difficulty++;
                }
                Initialize();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch);
                if (bubble.Activated)
                {
                    spriteBatch.Draw(activateBubbleOverlaySprite, bubble.Position, bubble.BubbleColor);
                }
            }
        }

        public void ActivateConnectedBubbles(int index)
        {

            int leftBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X - Constants.WORLD_UNIT, bubbles[index].Position.Y);
            int rightBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X + Constants.WORLD_UNIT, bubbles[index].Position.Y);
            int upBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X, bubbles[index].Position.Y - Constants.WORLD_UNIT);
            int downBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X, bubbles[index].Position.Y + Constants.WORLD_UNIT);

            // For all of these, we make sure that the index is greater than -1, because when we get the bubble index from the position using
            // the corresponding method, we designed it so that it returned -1 if that bubble didn't exist.
            
            if (leftBubbleIndex != Constants.NO_BUBBLE && OnSameRow(index, leftBubbleIndex)) //AKA, if a bubble at this index exists
            {
                if (bubbles[index].BubbleColor == bubbles[leftBubbleIndex].BubbleColor)
                {
                    bubbles[leftBubbleIndex].Activate();
                }

            }
            
            if (rightBubbleIndex != Constants.NO_BUBBLE && rightBubbleIndex < bubbles.Count && OnSameRow(index, rightBubbleIndex))
            {
                if (bubbles[index].BubbleColor == bubbles[rightBubbleIndex].BubbleColor)
                {
                    bubbles[rightBubbleIndex].Activate();
                }
            }
            
            if (upBubbleIndex != Constants.NO_BUBBLE)
            {
                if (bubbles[index].BubbleColor == bubbles[upBubbleIndex].BubbleColor)
                {
                    bubbles[upBubbleIndex].Activate();
                }
            }
            
            if (downBubbleIndex != Constants.NO_BUBBLE && downBubbleIndex < bubbles.Count)
            {
                if (bubbles[index].BubbleColor == bubbles[downBubbleIndex].BubbleColor)
                {
                    bubbles[downBubbleIndex].Activate();
                }
            }
        }

        public void RemoveActivatedBubbles()
        {
            foreach (Bubble bubble in bubbles)
            {
                if (bubble.Activated)
                {
                    // This is something fancy that removes every bubble from bubbles if a given condition is met. In this case,
                    // it's if that bubble is activated. Needed to call it bubble2 because the name bubble was already being used
                    // in this context.
                    bubbles.RemoveAll(bubble2 => bubble2.Activated == true);
                    break;
                }
            }
        }
        
        public void DropFloatingBubbles()
        {
            for (int i = bubbles.Count-1; i >= 0; i--)
            {
                if (bubbles[i].Position.Y >= Constants.BOTTOM_OF_GRID)
                {
                    continue;
                }

                // While there is no bubble under the current bubble we're checking...
                int counter = 0;
                while(GetBubbleIndexFromPosition(bubbles[i].Position.X, bubbles[i].Position.Y + Constants.WORLD_UNIT) < 0)
                {
                    counter++;
                    if (counter > 8)
                    {
                        Console.WriteLine("Something went wrong, we're continuously dropping...");
                        return;
                    }
                    
                    //Drop the current bubble down a WORLD_UNIT.
                    bubbles[i].Drop();

                    // If we found the bottom, then we're done with this bubble and can move to the next one
                    if (bubbles[i].Position.Y >= Constants.BOTTOM_OF_GRID)
                    {
                        break;
                    }                                     
                }
            }
        }
        
        public void CollapseBubbleColumns()
        {
            //Check each element in the last row (in reverse) to see if there's an element to its left, based on position. We end at 1 because we will never need
            //to collapse the leftmost row.
            for (int i = Constants.GRID_WIDTH_IN_UNITS - 1; i > -1; i--)
            {
                int currentIndex = GetBubbleIndexFromPosition(Constants.BUBBLE_GRID_ORIGIN.X + i * Constants.WORLD_UNIT, Constants.BOTTOM_OF_GRID);

                if (currentIndex == Constants.NO_BUBBLE) { continue;  }

                // If there's no bubble to the left of the current bubble on the bottom row, then...
                if (GetBubbleIndexFromPosition(bubbles[currentIndex].Position.X - Constants.WORLD_UNIT, Constants.BOTTOM_OF_GRID) == Constants.NO_BUBBLE)
                {
                    // Look through the rest of the columns to the left.
                    for (int j = i - 1; j > -1; j--)
                    {
                        if (GetBubbleIndexFromPosition(Constants.BUBBLE_GRID_ORIGIN.X + (j * Constants.WORLD_UNIT), Constants.BOTTOM_OF_GRID) != Constants.NO_BUBBLE)
                        {
                            MoveColumnOfBubbles(Constants.BUBBLE_GRID_ORIGIN.X + j * Constants.WORLD_UNIT, bubbles[currentIndex].Position.X - Constants.WORLD_UNIT);
                            break;
                        }
                    }
                }
            }
        }

        public void MoveColumnOfBubbles(float oldPositionX, float newPositionX)
        {
            foreach (Bubble bubble in bubbles)
            {
                // If the current bubble has the same X position of the bubble we want to move (AKA, if it's in the same column) then...
                if (bubble.Position.X == oldPositionX)
                {
                    // ...move that bubble to the new position, while retaining the row it's in (AKA Position.Y)
                    bubble.MoveTo(newPositionX, bubble.Position.Y);
                }
            }
        }

        public void DeactivateAllBubbles()
        {
            foreach (Bubble bubble in bubbles)
            {
                bubble.Deactivate();
            }
        }

        // Checks if two bubbles are on the same row for the purpose of knowing which bubbles are next to one
        // that's been activated. If the bubbles are indeed on different rows, then they're not next to eachother.
        private bool OnSameRow(int indexOne, int indexTwo)
        {
            // This if statement makes sure that a bubble at indexOne or indexTwo even exists. If the index is
            // less than or equal to -1, or if it's greater than or equal to the amount of bubbles in the bubbles
            // list, then it doesn't and we should return false before trying to pull up a bubble at that index.
            if (indexOne >= bubbles.Count || indexOne <= -1 || indexTwo >= bubbles.Count || indexTwo <= -1)
            {
                return false;
            }

            // The bubbles are on the same row, the positions will share the same Y-value (height).
            if (bubbles[indexOne].Position.Y == bubbles[indexTwo].Position.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetBubbleIndexFromPosition(float proposedX, float proposedY)
        {
            // We start with the assumption that we won't find a bubble at the given position,
            // feeding a bogus index that we handle other places in code.
            int bubbleIndex = Constants.NO_BUBBLE;
            foreach (Bubble bubble in bubbles)
            {
                if (bubble.Position.X == proposedX && bubble.Position.Y == proposedY)
                {
                    // if we DO find a bubble in the proposed position, then we set a new value for bubbleIndex
                    // corresponding to the index of the bubble we found.
                    bubbleIndex = bubbles.IndexOf(bubble);
                    break;
                }
            }
            // Lastly, we return bubbleIndex come hell or high water.
            return bubbleIndex;

        }
        // Generates a random color for the purpose of initializing the game.
        private Color GenerateRandomColor()
        {
            Color randomColor = Color.White;

            switch (random.Next(difficulty))
            {
                case 0:
                    randomColor = Color.Blue;
                    break;
                case 1:
                    randomColor = Color.Green;
                    break;
                case 2:
                    randomColor = Color.Red;
                    break;
                case 3:
                    randomColor = Color.Orange;
                    break;
                case 4:
                    randomColor = Color.Cyan;
                    break;
                case 5:
                    randomColor = Color.White;
                    break;
                case 6:
                    randomColor = Color.Pink;
                    break;
                case 7:
                    randomColor = Color.Purple;
                    break;
                case 8:
                    randomColor = Color.Yellow;
                    break;
                case 9:
                    randomColor = Color.DarkRed;
                    break;
                default:
                    break;
            }
            return randomColor;
        }
    }
}
