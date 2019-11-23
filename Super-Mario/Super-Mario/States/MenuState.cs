using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class MenuState : State
    {
        private SpriteFont my8bitFont;
        private Button[] myButtons;

        public MenuState(MainGame aGame, GameWindow aWindow) : base(aGame)
        {
            aGame.IsMouseVisible = true;
            Background.Reset();

            myButtons = new Button[]
            {
                new PlayButton(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 - 90), 
                    new Point(452, 64)),
                new LeaderboardButton(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 - 20), 
                    new Point(452, 64)),
                new EditorButton(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 + 50), 
                    new Point(452, 64)),
                new ExitButton(
                    new Vector2((aWindow.ClientBounds.Width / 2) - 226, (aWindow.ClientBounds.Height / 2) - 32 + 120), 
                    new Point(452, 64))
            };
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            Background.Scrolling(aGameTime, -2.0f);

            Array.ForEach(myButtons, b => b.Update(myGame, aWindow));
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "SUPER MARIO", new Vector2((aWindow.ClientBounds.Width / 2), (aWindow.ClientBounds.Height / 2) - 200), Color.Red, 1.5f);
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "not really...", new Vector2((aWindow.ClientBounds.Width / 2) + 350, (aWindow.ClientBounds.Height / 2) - 182), Color.Red, 0.25f);

            Array.ForEach(myButtons, b => b.Draw(aSpriteBatch));
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");
            Array.ForEach(myButtons, b => b.LoadContent());
        }
    }
}
