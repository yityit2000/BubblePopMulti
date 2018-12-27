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
    abstract class Screen
    {
        protected ScreenManager screenManager;

        public Screen(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
        }

        public abstract void LoadContent(ContentManager Content);
        public abstract void UnloadContent(ContentManager Content);
        public abstract void Update(GameTime gameTime, Camera2D camera);
        public abstract void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter, Camera2D camera);
    }
}
