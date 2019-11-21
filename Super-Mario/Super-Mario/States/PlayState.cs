using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class PlayState : State
    {
        private SpriteFont my8bitFont;

        private Player myPlayer;

        public PlayState(MainGame aGame, GameWindow aWindow) : base(aGame)
        {
            myPlayer = new Player(GameInfo.PlayerSpawn, new Point(32), 4.0f, 14.0f, -440.0f);
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            Level.Update();
            Camera.FollowObject(aWindow, myPlayer);
            myPlayer.Update(aGameTime);
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            aSpriteBatch.End();

            aSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                null, null, null, null, Camera.TranslationMatrix);

            Level.DrawTiles(aSpriteBatch);
            myPlayer.Draw(aSpriteBatch, aGameTime);
        }

        public override void LoadContent()
        {
            my8bitFont = ResourceManager.RequestFont("8-bit");

            myPlayer.SetTexture("Mario_Walking");
        }
    }
}
