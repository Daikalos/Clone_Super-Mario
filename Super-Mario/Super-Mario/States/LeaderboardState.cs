using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class LeaderboardState : State
    {
        private SpriteFont my8bitFont;
        private string[]
            myLevelNames,
            myHighScores;
        private int
            mySelection,
            mySelectionAmount;

        public LeaderboardState(MainGame aGame) : base(aGame)
        {
            myLevelNames = FileReader.FindFileNames(GameInfo.FolderLevels);
            myHighScores = FileReader.FindFileNames(GameInfo.FolderHighScores);
            mySelectionAmount = myLevelNames.Length - 2;
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
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

            if (KeyMouseReader.KeyPressed(Keys.Back))
            {
                myGame.ChangeState(new MenuState(myGame, aWindow));
            }

            if (myLevelNames.Length > 0)
            {
                string tempPrefix = "";
                if (mySelection < 10)
                {
                    tempPrefix = "0";
                }
                if (myLevelNames[mySelection] != "Level" + tempPrefix + mySelection)
                {
                    string tempName = myLevelNames[mySelection];
                    tempName = tempName.Replace(".txt", "");

                    GameInfo.LoadHighScore(GameInfo.FolderHighScores + tempName + "_HighScores.txt");
                }
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, ">",
                new Vector2(60, 110 + (40 * mySelection)),
                Color.GhostWhite, 0.6f);

            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "LEVELS", new Vector2(64, 64), Color.DarkOrange, 0.9f);
            for (int i = 0; i < myLevelNames.Length; i++)
            {
                string tempName = myLevelNames[i];
                tempName = tempName.Replace(".txt", "");

                if (tempName != "Level_Template")
                {
                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, tempName,
                    new Vector2(80, 110 + (40 * i)),
                    Color.White, 0.5f);
                }
            }


            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "HIGH SCORE", new Vector2(aWindow.ClientBounds.Width / 2, 64), Color.DarkOrange, 0.9f);
            for (int i = 0; i < GameInfo.HighScores.Length; i++)
            {
                StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, GameInfo.HighScores[i].ToString(), new Vector2((aWindow.ClientBounds.Width / 2) + 16, 110 + (40 * i)), Color.White, 0.7f);
            }

            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press return to go back to menu", new Vector2(16, aWindow.ClientBounds.Height - 16), Color.DarkOrange, 0.5f);
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
