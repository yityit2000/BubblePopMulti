using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace BubblePop
{
    class GameScreen : Screen
    {
        SpriteFont font;
        Texture2D background;

        Score score;

        MouseState oldState;

        // This will house the textures and all the logic concerning the grid of bubbles.
        BubbleGrid bubbleGrid;

        int difficulty = Constants.STARTING_DIFFICULTY;
        int level;

        bool readyToRemoveActivatedBubbles = false;

        Color backgroundColor;

        public GameScreen(ScreenManager screenManager) : base(screenManager)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            score = new Score();
            level = 1;

            font = content.Load<SpriteFont>("Score");
            background = content.Load<Texture2D>("background");

            bubbleGrid = new BubbleGrid(content);
            bubbleGrid.Initialize(difficulty);

            oldState = Mouse.GetState();
            /*These lines are when we want to test larger grids. First switch the values of SCREEN_WIDTH and _HEIGHT in Constants.cs to those
            commented out. */
            //graphics.PreferredBackBufferWidth = (int)(Constants.SCREEN_WIDTH / 1.4f);
            //graphics.PreferredBackBufferHeight = (int)(Constants.WORLD_HEIGHT/ 1.4f);

            backgroundColor = new Color(0, 0, 28);
        }

        public override void Update(GameTime gameTime, Camera2D camera)
        {
            // Update bubble positions
            foreach (Bubble bubble in bubbleGrid.Bubbles)
            {
                Vector2 oldPosition = bubble.Position;
                bubble.Update(gameTime);
                HandleCollision(bubble);
                if (Equals(oldPosition, bubble.Position))
                {
                    bubble.IsFalling = false;
                }
                else
                {
                    bubble.IsFalling = true;
                }
            }

            bubbleGrid.CollapseBubbleColumns();

            //Get current state of inputs so that we can check whether the player has clicked/tapped the screen
            MouseState newState = Mouse.GetState();
            TouchCollection touchCollection = TouchPanel.GetState();

            /* If the player has clicked the left button of the mouse (only if previously unclicked, otherwise this will
            execute multiple times even if the player seemingly only clicked once) or if the player has touched some
            location on the screen, then we check for the rest of the game mechanics */
            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released || touchCollection.Count > 0)
            {
                Vector2 clickLocation = camera.ScreenToWorld(new Vector2(newState.X, newState.Y));
                Vector2 touchLocation = Vector2.Zero;

                /* Only Fire Select Once it's been released. This will make it so that even if the user holds their finger down,
                 * we will only assign a different touch location other than 0,0 when they release their finger. */
                if (touchCollection.Count > 0)
                {
                    if (touchCollection[0].State == TouchLocationState.Pressed)
                    {
                        touchLocation = camera.ScreenToWorld(touchCollection[0].Position);
                    }
                }

                /* First, we see if any bubbles have been clicked on. If it's not an activated bubble, we need to deactivate all
                the bubbles so we can prepare to activate the set that the user just clicked on. If they clicked on a bubble that
                IS activated, then we get ready to clear out all the activated bubbles. */
                foreach (Bubble bubble in bubbleGrid.Bubbles)
                {
                    if (bubble.Intersects(clickLocation) || bubble.Intersects(touchLocation))
                    {
                        if (!bubble.Activated)
                        {
                            bubbleGrid.DeactivateAllBubbles();
                        }
                        else if (bubble.Activated && bubbleGrid.BubbleHasSameColorNeighbor(bubble))
                        {
                            readyToRemoveActivatedBubbles = true;
                            break; //We can exit early before doing anything with any other bubbles remaining in the foreach loop
                        }
                    }
                }

                if (readyToRemoveActivatedBubbles)
                {
                    score.Add(bubbleGrid.NumberOfActivatedBubbles(), level);
                    bubbleGrid.RemoveActivatedBubbles();
                    readyToRemoveActivatedBubbles = false;
                    oldState = newState;
                    if (LevelIsCleared()) //May eventually pass in score.Value
                    {
                        if (difficulty < 6)
                        {
                            level++;
                        }
                        StartNextLevel();
                    }
                    return;
                }

                /* If we've gotten this far, it means that the user hasn't clicked on a bubble that was already activated. So we
                 * again check to see if they clicked on a bubble, and if it's not activated, we activate it. Then we activate
                 * all the connected bubbles of the same color. */
                foreach (Bubble bubble in bubbleGrid.Bubbles)
                {
                    if (bubble.Intersects(clickLocation) || bubble.Intersects(touchLocation))
                    {
                        if (!bubble.Activated)
                        {
                            bubble.Activate();
                            break; //We found the one they clicked on and activated it, so we can leave this loop early
                        }
                    }
                }
                bubbleGrid.ActivateAllConnectedBubbles();
            }
            /* We update the old mouse state so that we can keep accurately acting through single mouse clicks rather than
             * if the mouse is held for a few frames (when the user THINKS they only clicked once) */
            oldState = newState;

        }

        // This method compares the position of one bubble to that of all the other bubbles in the grid. If this bubble tries falling through any
        // other bubbles or through the bottom of the grid, we stop it from doing so.
        private void HandleCollision(Bubble bubbleToHandle)
        {
            foreach(Bubble bubble in bubbleGrid.Bubbles)
            {
                if (bubbleToHandle.Position.Y >= Constants.BOTTOM_OF_GRID)
                {
                    bubbleToHandle.MoveTo(bubbleToHandle.Position.X, Constants.BOTTOM_OF_GRID);
                    return;
                }

                Vector2 topOfBubble = new Vector2(bubble.Position.X + Constants.BUBBLE_RADIUS, bubble.Position.Y);
                if (bubbleToHandle.Intersects(topOfBubble))
                {
                    bubbleToHandle.MoveTo(bubbleToHandle.Position.X, bubble.Position.Y - Constants.WORLD_UNIT);
                    return;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter, Camera2D camera2D)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            bubbleGrid.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Score: " + score.GameScore, new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(font, "Current Batch: " + score.ScoreOfCurrentBatch(bubbleGrid.NumberOfActivatedBubbles(), level), new Vector2(50, 100), Color.White);

        }

        public override void UnloadContent(ContentManager content)
        {

        }

        public bool LevelIsCleared()
        {
            // If we are out of bubbles, then we're obviously done with the level. We don't have to do the rest of this method.
            if (bubbleGrid.Bubbles.Count == 0)
            {
                return true;
            }

            // Here we loop through each bubble and see if any of them have one connected same-color partner. If so, the level
            // isn't over yet, so we return false.
            foreach (Bubble bubble in bubbleGrid.Bubbles)
            {
                if (bubbleGrid.BubbleHasSameColorNeighbor(bubble))
                {
                    return false;
                }
            }

            // We got through all the bubbles and none of them have any neighboring bubbles of the same color, so the level is,
            // for all intents and purposes, cleared (even though there are still bubbles remaining).
            return true;
        }

        public void StartNextLevel()
        {
            if (difficulty < 6)
            {
                difficulty++;
            }
            else
            {
                difficulty = 3;
                level = 1;
                score = new Score();
            }
            bubbleGrid.Initialize(difficulty);
        }
    }
}
