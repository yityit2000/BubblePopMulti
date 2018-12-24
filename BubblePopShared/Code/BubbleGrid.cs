using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;

namespace BubblePop
{
    class BubbleGrid
    {
        Random random;

        MouseState oldState = Mouse.GetState();

        List<Bubble> bubbles;
        public List<Bubble> Bubbles
        {
            get { return bubbles; }
        }

        readonly Texture2D bubbleSprite;
        readonly Texture2D clearColorPowerupSprite;
        readonly Texture2D activateBubbleOverlaySprite;
        readonly Texture2D activatePowerupOverlaySprite;
        
        ContentManager content;

        public BubbleGrid(ContentManager Content)
        {
            bubbles = new List<Bubble>();
            random = new Random();
            this.content = Content;
            bubbleSprite = Content.Load<Texture2D>("bubble");
            clearColorPowerupSprite = Content.Load<Texture2D>("powerup_clear_color");
            activateBubbleOverlaySprite = Content.Load<Texture2D>("bubble_outline");
            activatePowerupOverlaySprite = Content.Load<Texture2D>("powerup_outline");
        }

        public void Initialize(int difficulty)
        {
            bubbles.Clear();
            /* Set up initial grid of bubbles. Poweups will be places in random spots within the grid, though not
             * in place of any of the outermost bubbles */
            int randomCoordinateX1 = random.Next(Constants.GRID_WIDTH_IN_UNITS - 1);
            if (randomCoordinateX1 == 0) { randomCoordinateX1++; }
            int randomCoordinateY1 = random.Next(Constants.GRID_HEIGHT_IN_UNITS - 1);
            if (randomCoordinateY1 == 0) { randomCoordinateY1++; }
            int randomCoordinateX2 = random.Next(Constants.GRID_WIDTH_IN_UNITS - 1);
            if (randomCoordinateX2 == 0) { randomCoordinateX2++; }
            int randomCoordinateY2 = random.Next(Constants.GRID_HEIGHT_IN_UNITS - 1);
            if (randomCoordinateY1 == 0) { randomCoordinateY1++; }
            for (int i = 0; i < Constants.GRID_HEIGHT_IN_UNITS; i++)
            {
                for (int j = 0; j < Constants.GRID_WIDTH_IN_UNITS; j++)
                {
                    Vector2 thisBubblesPosition = Vector2.Add(Constants.BUBBLE_GRID_ORIGIN, new Vector2(j * Constants.WORLD_UNIT, i * Constants.WORLD_UNIT));
                    // We're going to start randomly adding powerups for testing. For now, we add one at the top left.
                    Bubble bubble;
                    if (difficulty > 3 && difficulty < 5)
                    {
                        if (i == randomCoordinateX1 && j == randomCoordinateY1)
                        {
                            bubble = new ClearColorPowerup(thisBubblesPosition, GenerateRandomColor(difficulty), clearColorPowerupSprite, content);
                        }
                        else
                        {
                            bubble = new Bubble(thisBubblesPosition, GenerateRandomColor(difficulty), bubbleSprite);
                        }
                    }
                    else if (difficulty > 4)
                    {
                        if (i == randomCoordinateX1 && j == randomCoordinateY1)
                        {
                            bubble = new ClearColorPowerup(thisBubblesPosition, GenerateRandomColor(difficulty), clearColorPowerupSprite, content);
                        }
                        else if (i == randomCoordinateX2 && j == randomCoordinateY2)
                        {
                            bubble = new ClearColorPowerup(thisBubblesPosition, GenerateRandomColor(difficulty), clearColorPowerupSprite, content);
                        }
                        else
                        {
                            bubble = new Bubble(thisBubblesPosition, GenerateRandomColor(difficulty), bubbleSprite);
                        }
                    }
                    else
                    {
                        bubble = new Bubble(thisBubblesPosition, GenerateRandomColor(difficulty), bubbleSprite);
                    }
                    bubbles.Add(bubble);

                }
            }
        }

        public void Update(GameTime gameTime, Camera2D camera)
        {
            //A remnant from when we had the update code in this class. We moved this to the main game class. This can be removed eventually.
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bubble bubble in bubbles)
            {
                bubble.Draw(spriteBatch);
                if (bubble.Activated)
                {
                    if (ThisBubbleIsAPowerup(bubble))
                    {
                        spriteBatch.Draw(activatePowerupOverlaySprite, bubble.Position, bubble.BubbleColor);
                    }
                    else
                    {
                        spriteBatch.Draw(activateBubbleOverlaySprite, bubble.Position, bubble.BubbleColor);
                    }
                }
            }
        }

        public void ActivateAllConnectedBubbles()
        {
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

        // This method activates all the bubbles connected to the bubble at the index given, provided that those bubbles are of the same color.
        public void ActivateConnectedBubbles(int index)
        {
            int leftBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X - Constants.WORLD_UNIT, bubbles[index].Position.Y);
            int rightBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X + Constants.WORLD_UNIT, bubbles[index].Position.Y);
            int upBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X, bubbles[index].Position.Y - Constants.WORLD_UNIT);
            int downBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X, bubbles[index].Position.Y + Constants.WORLD_UNIT);

            // For all of these, we make sure that the index corresponds to a bubble that exists, because when we get the bubble index from
            //the position using the corresponding method, we designed it so that it returned Constants.NO_BUBBLE if that bubble didn't exist.

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

        public bool BubbleHasSameColorNeighbor(Bubble bubble)
        {
            int index = bubbles.IndexOf(bubble);

            int leftBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X - Constants.WORLD_UNIT, bubbles[index].Position.Y);
            int rightBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X + Constants.WORLD_UNIT, bubbles[index].Position.Y);
            int upBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X, bubbles[index].Position.Y - Constants.WORLD_UNIT);
            int downBubbleIndex = GetBubbleIndexFromPosition(bubbles[index].Position.X, bubbles[index].Position.Y + Constants.WORLD_UNIT);

            // For all of these, we make sure that the index corresponds to a bubble that exists, because when we get the bubble index from
            //the position using the corresponding method, we designed it so that it returned Constants.NO_BUBBLE if that bubble didn't exist.

            if (leftBubbleIndex != Constants.NO_BUBBLE && OnSameRow(index, leftBubbleIndex)) //AKA, if a bubble at this index exists
            {
                if (bubbles[index].BubbleColor == bubbles[leftBubbleIndex].BubbleColor)
                {
                    return true;
                }
            }

            if (rightBubbleIndex != Constants.NO_BUBBLE && rightBubbleIndex < bubbles.Count && OnSameRow(index, rightBubbleIndex))
            {
                if (bubbles[index].BubbleColor == bubbles[rightBubbleIndex].BubbleColor)
                {
                    return true;
                }
            }

            if (upBubbleIndex != Constants.NO_BUBBLE)
            {
                if (bubbles[index].BubbleColor == bubbles[upBubbleIndex].BubbleColor)
                {
                    return true;
                }
            }

            if (downBubbleIndex != Constants.NO_BUBBLE && downBubbleIndex < bubbles.Count)
            {
                if (bubbles[index].BubbleColor == bubbles[downBubbleIndex].BubbleColor)
                {
                    return true;
                }
            }

            return false;
        }

        public int NumberOfActivatedBubbles()
        {
            int numberOfActivatedBubbles = 0;
            foreach (Bubble bubble in bubbles)
            {
                if (bubble.Activated)
                {
                    numberOfActivatedBubbles++;
                }
            }

            return numberOfActivatedBubbles;
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

        public bool ThisBubbleIsAPowerup(Bubble bubble)
        {
            // This loops through all the powerup types in the game defined in the Constants class and compares it to the class name
            // of the bubble. If the class name of the bubble matches the name of one of the powerups, we return true
            for (int i = 0; i < Constants.POWERUP_TYPES.Length; i++)
            {
                if (bubble.GetType().Name == Constants.POWERUP_TYPES[i])
                {
                    return true;
                }
            }
            // If this bubble didn't match with any of the names of powerups, then we return false
            return false;
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
                    if (counter > Constants.GRID_HEIGHT_IN_UNITS)
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

                if (currentIndex == Constants.NO_BUBBLE) { continue; }

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
        private Color GenerateRandomColor(int numberOfColors)
        {
            Color randomColor = Color.White;

            switch (random.Next(numberOfColors))
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
                default:
                    break;
            }
            return randomColor;
        }
    }
}
