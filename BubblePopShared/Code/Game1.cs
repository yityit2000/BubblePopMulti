using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace BubblePop
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        Camera2D camera;
        ViewportAdapter viewportAdapter;
        SpriteBatch spriteBatch;

        // This will house the textures and all the logic concerning the grid of bubbles.
        BubbleGrid bubbleGrid;

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
            // Don't FULLY understand everything about these next two lines. But they let us have Resolution Indepentent drawing, meaning
            // that it will scale to any resolution we want without us having to worry about accounting for different screen resolutions.
            // This involves a transform matrix in the camera that converts any clicks in the virtual screen to that of the actual screen
            // that we are drawing to. Need to do more research on it to comprehend, but hey, it's working!

            // Viewport Adapter and Camera2d makes sure we always output to given virtual dimensions, even through resizing.
            viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, (int)Constants.SCREEN_WIDTH, (int)Constants.SCREEN_HEIGHT);
            camera = new Camera2D(viewportAdapter);

            bubbleGrid = new BubbleGrid(this.Content);
            bubbleGrid.Initialize();

            // These lines are when we want to test larger grids. First switch the values of SCREEN_WIDTH and _HEIGHT in Constants.cs to those
            // commented out.
            //graphics.PreferredBackBufferWidth = (int)(Constants.SCREEN_WIDTH / 1.4f);
            //graphics.PreferredBackBufferHeight = (int)(Constants.SCREEN_HEIGHT/ 1.4f);

#if _ANDROID_
            graphics.IsFullScreen = true;
#endif
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            //backgroundColor = Color.LightBlue;
            backgroundColor = new Color(0,0,28);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            bubbleGrid.Update(gameTime, camera);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            var sourceRectangle = new Rectangle(0, 0, viewportAdapter.VirtualWidth, viewportAdapter.VirtualHeight);
            sourceRectangle.Offset(camera.Position * new Vector2(0.1f));

            spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.AlphaBlend, transformMatrix: viewportAdapter.GetScaleMatrix());
            bubbleGrid.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
