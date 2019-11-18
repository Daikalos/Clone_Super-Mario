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
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 704;
            graphics.ApplyChanges();

            ResourceManager.Initialize();

            GameInfo.Initialize(Window, 0.5f);
            GameInfo.CurrentLevel = "Level01.txt";
            GameInfo.FolderLevels = "../../../../Levels/";

            Level.LoadLevel(new Point(32));

            myGameState = new PlayState(this, Window);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.AddFont("8-bit", this.Content.Load<SpriteFont>("Fonts/8bit"));

            ResourceManager.AddTexture("Grass-00", this.Content.Load<Texture2D>("Tileset/tile-grass-00"));

            ResourceManager.AddTexture("Mario_Walking", this.Content.Load<Texture2D>("Sprites/mario_walking"));

            Level.SetTileTexture();

            myGameState.LoadContent();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();

            myGameState.Update(Window, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DodgerBlue);

            spriteBatch.Begin();

            myGameState.Draw(spriteBatch, Window, gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
