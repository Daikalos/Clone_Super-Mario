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

        /// <summary>
        /// 0 = Block/#;
        /// 1 = Player/?;
        /// 2 = Flag/*;
        /// </summary>
        private Tile[] mySelections;
        private Rectangle myOffset;
        private char mySelectedTile;
        private int myTile;
        private float
            myTimer,
            myDelay;

        public EditorState(MainGame aGame, GameWindow aWindow) : base(aGame)
        {
            Level.LoadLevel(new Point(32), "Level_Template");

            this.mySelections = new Tile[]
            {
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32)),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 90), new Point(32)),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 138), new Point(32))
            };
            this.myOffset = new Rectangle(-2, -2, 4, 4);
            this.mySelectedTile = ' ';
            this.myTile = -1;
            this.myDelay = 0.50f;
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            Level.Update();

            if (KeyMouseReader.KeyHold(Keys.Left))
            {
                Camera.MoveCamera(aWindow, aGameTime, -12.0f);
            }
            if (KeyMouseReader.KeyHold(Keys.Right))
            {
                Camera.MoveCamera(aWindow, aGameTime, 12.0f);
            }

            if (myTimer > 0)
            {
                myTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }

            for (int i = 0; i < mySelections.Length; i++)
            {
                mySelections[i].Update();

                mySelections[i].BoundingBox = new Rectangle((int)Camera.Position.X + aWindow.ClientBounds.Width - 64, 32 + (48 * i),
                    mySelections[i].BoundingBox.Width, mySelections[i].BoundingBox.Height);

                Rectangle tempRect = mySelections[i].BoundingBox;

                if (tempRect.Contains(Camera.Position.ToPoint() + KeyMouseReader.GetCurrentMouseState.Position))
                {
                    mySelections[i].BoundingBox = new Rectangle(tempRect.X + myOffset.X, tempRect.Y + myOffset.Y,
                        tempRect.Width + myOffset.Width, tempRect.Height + myOffset.Height);

                    if (KeyMouseReader.LeftClick())
                    {
                        mySelectedTile = mySelections[i].TileType;
                        myTile = i;

                        myTimer = myDelay;
                    }
                }
            }

            if (KeyMouseReader.LeftHold() && mySelectedTile != ' ' && myTimer <= 0)
            {
                Tile tempTile = Level.GetTileAtPos(Camera.Position + KeyMouseReader.GetCurrentMouseState.Position.ToVector2()).Item1;
                tempTile.TileType = mySelectedTile;
                tempTile.TileForm = 0;
                tempTile.SetTextureEditor();
            }
            if (KeyMouseReader.RightHold())
            {
                mySelectedTile = ' ';
                myTile = -1;

                Tile tempTile = Level.GetTileAtPos(Camera.Position + KeyMouseReader.GetCurrentMouseState.Position.ToVector2()).Item1;
                tempTile.TileType = mySelectedTile;
                tempTile.TileForm = 0;
                tempTile.SetTextureEditor();
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

            Array.ForEach(mySelections, t => t.Draw(aSpriteBatch));

            if (myTile >= 0 && myTile < mySelections.Length)
            {
                aSpriteBatch.Draw(mySelections[myTile].Texture, (Camera.Position + KeyMouseReader.GetCurrentMouseState.Position.ToVector2()), null, Color.White);
            }

            StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press return to go back to menu", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.3f);
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

            mySelections[0].TileType = '#';
            mySelections[1].TileType = '?';
            mySelections[2].TileType = '*';

            Array.ForEach(mySelections, t => t.SetTextureEditor());

            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
