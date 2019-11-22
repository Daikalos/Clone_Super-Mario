using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class WinState : State
    {
        SpriteFont my8bitFont;

        public WinState(MainGame aGame) : base(aGame)
        {

        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {      
            if (KeyMouseReader.KeyPressed(Keys.Back))
            {
                myGame.ChangeState(new MenuState(myGame, aWindow));
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "YOU WIN", new Vector2(aWindow.ClientBounds.Width / 2, (aWindow.ClientBounds.Height / 2) - 96), Color.Yellow, 1.2f);
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "Score: " + GameInfo.Score.ToString(), new Vector2(aWindow.ClientBounds.Width / 2, (aWindow.ClientBounds.Height / 2) - 56), Color.White, 0.7f);
            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press return to go back to menu", new Vector2(12, aWindow.ClientBounds.Height - 12), Color.DarkOrange, 0.5f);
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
