using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class EditorState : State
    {
        private SpriteFont my8bitFont;
        private Texture2D
            myBlock,
            myEnemy,
            myFlag,
            myPlayer;
        private char mySelectedTile;

        public EditorState(MainGame aGame) : base(aGame)
        {
            Level.LoadLevel(new Point(32), "Level_Template");
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            Level.Update();

            if (KeyMouseReader.KeyHold(Keys.Left))
            {
                Camera.MoveCamera(aWindow, aGameTime, -10.0f);
            }
            if (KeyMouseReader.KeyHold(Keys.Right))
            {
                Camera.MoveCamera(aWindow, aGameTime, 10.0f);
            }

            if (KeyMouseReader.KeyPressed(Keys.Back))
            {
                myGame.ChangeState(new MenuState(myGame, aWindow));
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            aSpriteBatch.End();

            aSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null, Camera.TranslationMatrix);

            Level.DrawTiles(aSpriteBatch);

            aSpriteBatch.Draw(myBlock, new Vector2(Camera.Position.X + aWindow.ClientBounds.Width - 100, 100), null, Color.White);
            aSpriteBatch.Draw(myPlayer, new Vector2(Camera.Position.X + aWindow.ClientBounds.Width - 100, 164), null, Color.White);
            aSpriteBatch.Draw(myFlag, new Vector2(Camera.Position.X + aWindow.ClientBounds.Width - 100, 228), null, Color.White);
        }

        private void SaveLevel()
        {

        }
        private void LoadLevel()
        {

        }

        private void EditMap()
        {
            
        }

        public override void LoadContent()
        {
            Level.SetTileTextureEditor();

            myBlock = ResourceManager.RequestTexture("Grass-00");
            //myEnemy = ResourceManager.RequestTexture("");
            myPlayer = ResourceManager.RequestTexture("Mario_Idle");
            myFlag = ResourceManager.RequestTexture("Flag");

            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
