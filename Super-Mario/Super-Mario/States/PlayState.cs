﻿using Microsoft.Xna.Framework;
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
            myGame.IsMouseVisible = false;

            EnemyManager.Initialize();

            GameInfo.Initialize(0.5f, 0.4f);
            GameInfo.LoadHighScore(GameInfo.LevelName);

            Level.LoadLevel(new Point(32), GameInfo.LevelName);

            Background.Reset();
            Camera.Reset();

            myPlayer = new Player(Level.PlayerSpawn, new Point(32), new Vector2(4.0f, 2.0f), new Vector2(4.0f, 9.0f), 2.0f, 20.0f, 15.0f, 3);
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
                Camera.FollowObject(aWindow, myPlayer);
                EnemyManager.Update(aGameTime);
                GameInfo.Update(aGameTime);
                myPlayer.Update(aGameTime);

                if (myPlayer.CollisionFlag())
                {
                    GameInfo.SaveHighScore(GameInfo.LevelName);
                    myGame.ChangeState(new WinState(myGame)); //Resets and loads new level
                }
                if (!myPlayer.IsAlive)
                {
                    myGame.ChangeState(new DeadState(myGame));
                }
            }
            else
            {
                myBackButton.Update();
            }

            if (KeyMouseReader.KeyPressed(Keys.Escape))
            {
                GameInfo.IsPaused = !GameInfo.IsPaused;
                myGame.IsMouseVisible = !myGame.IsMouseVisible;
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            aSpriteBatch.End();

            aSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null, Camera.TranslationMatrix);

            Level.DrawTiles(aSpriteBatch, aGameTime);
            EnemyManager.Draw(aSpriteBatch, aGameTime);
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
            EnemyManager.SetTexture();
            myPlayer.SetTexture("Mario_Idle");
            myBackButton.LoadContent();
        }
    }
}