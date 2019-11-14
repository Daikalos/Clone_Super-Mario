using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        State myGameState;

        Camera myCamera;

        public void ChangeState(State aNewState)
        {
            myGameState = aNewState;
            myGameState.LoadContent();
        }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 704;
            graphics.ApplyChanges();

            GraphicsDevice.Viewport = new Viewport(0, 0, 960, 704);

            ResourceManager.Initialize();

            ChangeState(new PlayState(this, Window));
            myCamera = new Camera(GraphicsDevice.Viewport);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.AddFont("8-bit", this.Content.Load<SpriteFont>("Fonts/8bit"));
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            myCamera.UpdateCamera(GraphicsDevice.Viewport);
            myGameState.Update(Window, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            myGameState.Draw(spriteBatch, Window, gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
