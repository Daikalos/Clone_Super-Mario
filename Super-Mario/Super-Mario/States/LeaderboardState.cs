using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class LeaderboardState : State
    {
        private SpriteFont my8bitFont;
        private int
            mySelection,
            mySelectionAmount;
        private string[] myLevelNames;
        private string myLevelName;

        public LeaderboardState(MainGame aGame) : base(aGame)
        {
            GameInfo.LoadHighScore(GameInfo.LevelName);

            this.myLevelNames = FileReader.FindFileNames(GameInfo.FolderLevels);
            this.mySelectionAmount = myLevelNames.Length - 1;
            this.myLevelName = string.Empty;
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            Background.Scrolling(aGameTime, -2.0f);

            if (KeyMouseReader.KeyPressed(Keys.Up))
            {
                if (mySelection > 0)
                {
                    mySelection--;
                }
            }
            if (KeyMouseReader.KeyPressed(Keys.Down))
            {
                if (mySelection < mySelectionAmount)
                {
                    mySelection++;
                }
            }

            if (KeyMouseReader.KeyPressed(Keys.Escape))
            {
                myGame.ChangeState(new MenuState(myGame, aWindow));
            }

            if (myLevelNames.Length > 0)
            {
                if (myLevelName != myLevelNames[mySelection])
                {
                    myLevelName = myLevelNames[mySelection];

                    string tempName = myLevelName;
                    tempName = tempName.Replace(".txt", "");

                    GameInfo.LoadHighScore(tempName);
                }
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, ">",
                new Vector2(60, 110 + (40 * mySelection)),
                Color.Black, 0.6f);

            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "LEVELS", new Vector2(64, 64), Color.Black, 0.9f);
            for (int i = 0; i < myLevelNames.Length; i++)
            {
                string tempName = myLevelNames[i];
                tempName = tempName.Replace(".txt", "");

                StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, tempName,
                new Vector2(80, 110 + (40 * i)),
                Color.Black, 0.5f);
            }


            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "HIGH SCORE", new Vector2(aWindow.ClientBounds.Width / 2, 64), Color.Black, 0.9f);
            for (int i = 0; i < GameInfo.HighScores.Length; i++)
            {
                StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, GameInfo.HighScores[i].ToString(), new Vector2((aWindow.ClientBounds.Width / 2) + 16, 110 + (40 * i)), Color.Black, 0.7f);
            }

            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press escape to go back to menu", new Vector2(16, aWindow.ClientBounds.Height - 16), Color.Black * 0.8f, 0.5f);
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
