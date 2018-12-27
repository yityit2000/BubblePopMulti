using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace BubblePop
{
    class MainMenuScreen : Screen
    {
        SpriteFont font;
        public MainMenuScreen(ScreenManager screenManager) : base(screenManager)
        {
        }

        public override void LoadContent(ContentManager Content)
        {
            font = Content.Load<SpriteFont>("Score");
        }

        public override void Update(GameTime gameTime, Camera2D camera)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed || TouchPanel.GetState().Count > 0)
            {
                screenManager.SetActiveScreen(new GameScreen(screenManager));
            }
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter, Camera2D camera)
        {
            Vector2 titleSize = font.MeasureString("BUBBLE POP");
            Vector2 tipSize = font.MeasureString("Click or tap to start");
            spriteBatch.DrawString(font, "BUBBLE POP", new Vector2(Constants.WORLD_WIDTH / 2 - titleSize.X / 2, 100), Color.White);
            spriteBatch.DrawString(font, "Click or tap to start", new Vector2(Constants.WORLD_WIDTH / 2 - tipSize.X / 2, 500), Color.White);
        }


        public override void UnloadContent(ContentManager Content)
        {

        }
    }
}
