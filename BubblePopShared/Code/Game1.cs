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

        ScreenManager screenManager;        

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
            viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, (int)Constants.WORLD_WIDTH, (int)Constants.WORLD_HEIGHT);
            camera = new Camera2D(viewportAdapter);

            screenManager = new ScreenManager(Content, viewportAdapter, camera);

#if _ANDROID_
            graphics.IsFullScreen = true;
#endif
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            backgroundColor = Color.Navy;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenManager.SetActiveScreen(new MainMenuScreen(screenManager));
        }

        protected override void UnloadContent() { }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            var sourceRectangle = new Rectangle(0, 0, viewportAdapter.VirtualWidth, viewportAdapter.VirtualHeight);
            sourceRectangle.Offset(camera.Position * new Vector2(0.1f));

            spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend, transformMatrix: viewportAdapter.GetScaleMatrix());
            screenManager.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
