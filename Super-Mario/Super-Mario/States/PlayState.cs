using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class PlayState : State
    {
        private SpriteFont my8bitFont;
        private Button myBackButton;
        private Player myPlayer;

        public PlayState(MainGame aGame, GameWindow aWindow) : base(aGame)
        {
            Level.LoadLevel(new Point(32), GameInfo.LevelName());

            Background.Reset();
            Camera.Reset();

            myPlayer = new Player(Level.PlayerSpawn, new Point(32), 4.0f, 14.0f, -440.0f);
            myBackButton = new Button(new Vector2(aWindow.ClientBounds.Width - 128 - 16, aWindow.ClientBounds.Height - 48 - 16),
                    new Point(128, 48),
                    new Button.OnClick(() => Button.Back(aGame, aWindow)),
                    "MENU", 0.6f);
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            if (!GameInfo.IsPaused)
            {
                Level.Update();
                Camera.FollowObject(aWindow, aGameTime, myPlayer);
                myPlayer.Update(aWindow, aGameTime);

                if (myPlayer.CollisionFlag())
                {
                    GameInfo.CurrentLevel++;
                    myGame.ChangeState(new PlayState(myGame, aWindow)); //Resets and loads new level
                }
            }
            else
            {
                myBackButton.Update();
            }

            if (KeyMouseReader.KeyPressed(Keys.Escape))
            {
                GameInfo.IsPaused = !GameInfo.IsPaused;
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            aSpriteBatch.End();

            aSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null, Camera.TranslationMatrix);

            Level.DrawTiles(aSpriteBatch);
            myPlayer.Draw(aSpriteBatch, aGameTime);
            GameInfo.Draw(aSpriteBatch, aWindow, my8bitFont, myPlayer);

            if (GameInfo.IsPaused)
            {
                StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "PAUSED", 
                    new Vector2(Camera.Position.X + (aWindow.ClientBounds.Width / 2), aWindow.ClientBounds.Height / 2), Color.Black, 1.5f);
                myBackButton.Draw(aSpriteBatch);
            }
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");

            Level.SetTileTexture();
            myPlayer.SetTexture("Mario_Idle");
            myBackButton.LoadContent();
        }
    }
}
