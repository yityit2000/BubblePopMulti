using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace BubblePop
{
    public class Game1 : Game
    {
        // Monogame related variables for running any game
        GraphicsDeviceManager graphics;
        Camera2D camera;
        ViewportAdapter viewportAdapter;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Score score;

        MouseState oldState;

        // This will house the textures and all the logic concerning the grid of bubbles.
        BubbleGrid bubbleGrid;

        int difficulty = Constants.STARTING_DIFFICULTY;
        int level;

        bool readyToRemoveActivatedBubbles = false;

        Color backgroundColor;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            Window.AllowUserResizing = true;
            //Window.Position = Point.Zero;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Viewport Adapter and Camera2d makes sure we always output to given virtual dimensions, even through resizing.
            viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, (int)Constants.SCREEN_WIDTH, (int)Constants.SCREEN_HEIGHT);
            camera = new Camera2D(viewportAdapter);
            score = new Score();
            level = 1;

            font = Content.Load<SpriteFont>("Score");

            bubbleGrid = new BubbleGrid(this.Content);
            bubbleGrid.Initialize(difficulty);

            oldState = Mouse.GetState();
            /*These lines are when we want to test larger grids. First switch the values of SCREEN_WIDTH and _HEIGHT in Constants.cs to those
            commented out. */
            //graphics.PreferredBackBufferWidth = (int)(Constants.SCREEN_WIDTH / 1.4f);
            //graphics.PreferredBackBufferHeight = (int)(Constants.SCREEN_HEIGHT/ 1.4f);

#if _ANDROID_
            graphics.IsFullScreen = true;
#endif
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
            
            backgroundColor = new Color(0,0,28);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent() { }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            bubbleGrid.Update(gameTime, camera);

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
                    // Before removing the activated bubbles, we check to see if any of them are powerups. If they are, we have the powerup do its effect.
                    foreach (Bubble bubble in bubbleGrid.Bubbles)
                    {
                        if (bubbleGrid.ThisBubbleIsAPowerup(bubble) && bubble.Activated)
                        {
                            /* All powerups are bubbles, but not all bubbles are powerups. Therefore, we can't go calling bubble.DoEffect(...) on any old
                             bubble willy-nilly. We have to use a tool called "Reflection" to be able to use the DoEffect method in the Powerup class. 
                             When we DO find a powerup to use it on, it will have different syntax, corresponding to the two lines of code below.*/
                            Object[] variablesToPassIntoDoEffect = new Object[] { bubbleGrid };
                            //For all intents and purposes, you can read the line of code below as:
                            //bubble.DoEffect(bubbleGrid)
                            bubble.GetType().InvokeMember("DoEffect", System.Reflection.BindingFlags.InvokeMethod, null, bubble, variablesToPassIntoDoEffect);
                        }
                    }
                    score.Add(bubbleGrid.NumberOfActivatedBubbles(), level);
                    bubbleGrid.RemoveActivatedBubbles();
                    bubbleGrid.DropFloatingBubbles();
                    bubbleGrid.CollapseBubbleColumns();
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            var sourceRectangle = new Rectangle(0, 0, viewportAdapter.VirtualWidth, viewportAdapter.VirtualHeight);
            sourceRectangle.Offset(camera.Position * new Vector2(0.1f));

            spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend, transformMatrix: viewportAdapter.GetScaleMatrix());
            bubbleGrid.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Score: " + score.GameScore, new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(font, "Current Batch: " + score.ScoreOfCurrentBatch(bubbleGrid.NumberOfActivatedBubbles(), level), new Vector2(50, 100), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            spriteBatch.End();

            base.Draw(gameTime);
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
