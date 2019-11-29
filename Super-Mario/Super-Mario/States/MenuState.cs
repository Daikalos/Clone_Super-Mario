using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class MenuState : State
    {
        private SpriteFont my8bitFont;
        private Button[] myButtons;
        private Button[] myLevels;
        private string[] myLevelNames;
        private bool myLoadLevel;

        public MenuState(MainGame aGame, GameWindow aWindow) : base(aGame)
        {
            aGame.IsMouseVisible = true;

            Background.Reset();
            Camera.Reset();

            myButtons = new Button[]
            {
                new Button(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 - 90),
                    new Point(452, 64),
                    null,
                    "PLAY", 1.1f),
                new Button(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 - 10),
                    new Point(452, 64),
                    new Button.OnClick(() => Button.Editor(aGame, aWindow)),
                    "EDITOR", 1.1f),
                new Button(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 + 70),
                    new Point(452, 64),
                    new Button.OnClick(() => Button.Leaderboard(aGame)),
                    "LEADERBOARD", 1.1f),
                new Button(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 + 150),
                    new Point(452, 64),
                    new Button.OnClick(() => Button.Exit(aGame)),
                    "EXIT", 1.1f),
            };
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            Background.Scrolling(aGameTime, -2.0f);

            if (!myLoadLevel)
            {
                Array.ForEach(myButtons, b => b.Update());

                if (myButtons[0].IsClicked())
                {
                    myLoadLevel = true;
                    myLevelNames = FileReader.FindFileNames("../../../../Levels/");

                    myLevels = new Button[myLevelNames.Length];

                    for (int i = 0; i < myLevels.Length; i++)
                    {
                        myLevels[i] = new Button(new Vector2((aWindow.ClientBounds.Width / 2) - 113, (aWindow.ClientBounds.Height / 2) - 64 - 90 + (i * 40)), 
                            new Point(226, 32), null, myLevelNames[i], 0.4f);
                        myLevels[i].LoadContent();
                    }
                }
            }
            else
            {
                foreach (Button button in myLevels)
                {
                    button.Update();
                    if (button.IsClicked())
                    {
                        string tempLevel = button.DisplayText;
                        tempLevel = tempLevel.Replace("Level", "");
                        int tempLoadLevel;

                        if (Int32.TryParse(tempLevel, out tempLoadLevel))
                        {
                            GameInfo.CurrentLevel = tempLoadLevel;
                            myGame.ChangeState(new PlayState(myGame, aWindow));
                        }

                        myLoadLevel = false;
                        myLevels = null;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "SUPER MARIO", new Vector2((aWindow.ClientBounds.Width / 2), (aWindow.ClientBounds.Height / 2) - 200), Color.Red, 1.5f);
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "not really...", new Vector2((aWindow.ClientBounds.Width / 2) + 350, (aWindow.ClientBounds.Height / 2) - 182), Color.Red, 0.25f);

            if (!myLoadLevel)
            {
                Array.ForEach(myButtons, b => b.Draw(aSpriteBatch));
            }
            else
            {
                Array.ForEach(myLevels, b => b.Draw(aSpriteBatch));
            }
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");
            Array.ForEach(myButtons, b => b.LoadContent());
        }
    }
}
