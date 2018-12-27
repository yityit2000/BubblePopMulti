using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubblePop
{
    class ScreenManager
    {
        Screen activeScreen;
        ViewportAdapter viewportAdapter;
        Camera2D camera;
        ContentManager Content;

        public ScreenManager(ContentManager Content, ViewportAdapter viewportAdapter, Camera2D camera)
        {
            this.Content = Content;
            this.viewportAdapter = viewportAdapter;
            this.camera = camera;
        }

        public void Update(GameTime gameTime)
        {
            activeScreen.Update(gameTime, camera);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            activeScreen.Draw(spriteBatch, viewportAdapter, camera);
        }

        public void SetActiveScreen(Screen newActiveScreen)
        {
            if (activeScreen != null)
            {
                activeScreen.UnloadContent(Content);
            }

            activeScreen = newActiveScreen;
            activeScreen.LoadContent(Content);
        }
    }
}
