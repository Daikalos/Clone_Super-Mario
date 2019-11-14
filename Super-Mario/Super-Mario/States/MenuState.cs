using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class MenuState : State
    {
        private SpriteFont my8bitFont;

        public MenuState(MainGame aGame) : base(aGame)
        {

        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {

        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
