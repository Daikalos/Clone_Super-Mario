using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class EditorState : State
    {
        enum EditorStates
        {
            isEditing,
            isLoading,
            isDeleting
        }

        private SpriteFont 
            my8bitFont;

        /// <summary>
        /// 0 = Block/#;
        /// 1 = Player/?;
        /// 2 = Flag/*;
        /// 3 = Ladder/%;
        /// 4 = Goomba/And;
        /// </summary>
        private Tile[] mySelections;
        private Button[] myLevels;
        private Button 
            myLoadButton,
            mySaveButton,
            myDeleteButton;
        private Rectangle myOffset;
        private EditorStates myEditorState;
        private char mySelectedTile;
        private char[,] myLevel;
        private int myTile;
        private float
            myTimer,
            myDelay;

        public EditorState(MainGame aGame, GameWindow aWindow) : base(aGame)
        {
            Level.LoadLevel(new Point(32), "Level_Template");

            myLoadButton = new Button(new Vector2(32, 32), new Point(128, 48), null, "LOAD", 0.6f);
            mySaveButton = new Button(new Vector2(32, 96), new Point(128, 48), null, "SAVE", 0.6f);
            myDeleteButton = new Button(new Vector2(32, 160), new Point(128, 48), null, "DEL", 0.6f);

            myLevel = new char[
                Level.MapSize.X / Level.TileSize.X,
                Level.MapSize.Y / Level.TileSize.Y];

            this.mySelections = new Tile[]
            {
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32)),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32)),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32)),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32)),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32)),
            };
            myEditorState = EditorStates.isEditing;
            this.myOffset = new Rectangle(-2, -2, 4, 4);
            this.mySelectedTile = ' ';
            this.myTile = -1;
            this.myDelay = 0.50f;
        }

        public override void Update(GameWindow aWindow, GameTime aGameTime)
        {
            switch(myEditorState)
            {
                case EditorStates.isEditing:
                    Misc(aWindow, aGameTime);

                    SelectTile();
                    EditMap();

                    SaveLevel();
                    LoadLevel(aWindow);
                    DeleteLevel(aWindow);
                    break;
                case EditorStates.isLoading:
                    SelectLevelToLoad();
                    break;
                case EditorStates.isDeleting:
                    SelectLevelToDelete();
                    break;
            }

            if (KeyMouseReader.KeyPressed(Keys.Back))
            {
                switch (myEditorState)
                {
                    case EditorStates.isEditing:
                        myGame.ChangeState(new MenuState(myGame, aWindow));
                        break;
                    default:
                        myEditorState = EditorStates.isEditing;
                        break;
                }
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, GameTime aGameTime)
        {
            aSpriteBatch.End();

            aSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null, Camera.TranslationMatrix);

            Level.DrawTiles(aSpriteBatch);

            switch (myEditorState)
            {
                case EditorStates.isEditing:
                    Array.ForEach(mySelections, t => t.Draw(aSpriteBatch));

                    if (myTile >= 0 && myTile < mySelections.Length)
                    {
                        aSpriteBatch.Draw(mySelections[myTile].Texture, (Camera.Position + KeyMouseReader.GetCurrentMouseState.Position.ToVector2()), null, Color.White);
                    }

                    myLoadButton.Draw(aSpriteBatch);
                    mySaveButton.Draw(aSpriteBatch);
                    myDeleteButton.Draw(aSpriteBatch);

                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press return to go back to menu", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.4f);
                    break;
                case EditorStates.isLoading:
                    Array.ForEach(myLevels, b => b.Draw(aSpriteBatch));
                    StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "LOAD", new Vector2(Camera.Position.X + aWindow.ClientBounds.Width / 2, 32), Color.Black, 0.9f);
                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press return to go back to editor", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.4f);
                    break;
                case EditorStates.isDeleting:
                    Array.ForEach(myLevels, b => b.Draw(aSpriteBatch));
                    StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "DELETE", new Vector2(Camera.Position.X + aWindow.ClientBounds.Width / 2, 32), Color.Black, 0.9f);
                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press return to go back to editor", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.4f);
                    break;
            }
        }

        private void Misc(GameWindow aWindow, GameTime aGameTime)
        {
            Level.Update();
            myLoadButton.Update();
            mySaveButton.Update();
            myDeleteButton.Update();

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
        }

        private void SelectTile()
        {
            for (int i = 0; i < mySelections.Length; i++)
            {
                mySelections[i].Update();

                mySelections[i].BoundingBox = new Rectangle((int)(Camera.Position.X + mySelections[i].Position.X), (int)mySelections[i].Position.Y + (48 * i),
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
        }
        private void EditMap()
        {
            if (KeyMouseReader.LeftHold() && mySelectedTile != ' ' && myTimer <= 0)
            {
                Tile tempTile = Level.TileAtPos(Camera.Position + KeyMouseReader.GetCurrentMouseState.Position.ToVector2()).Item1;
                tempTile.TileType = mySelectedTile;
                tempTile.TileForm = 0;
                tempTile.SetTextureEditor();
            }
            if (KeyMouseReader.RightHold())
            {
                mySelectedTile = '-';
                myTile = -1;

                Tile tempTile = Level.TileAtPos(Camera.Position + KeyMouseReader.GetCurrentMouseState.Position.ToVector2()).Item1;
                tempTile.TileType = mySelectedTile;
                tempTile.TileForm = 0;
                tempTile.SetTextureEditor();
            }
        }

        private void LoadLevel(GameWindow aWindow)
        {
            if (myLoadButton.IsClicked())
            {
                myEditorState = EditorStates.isLoading;
                string[] tempLevelNames = FileReader.FindFileNames(GameInfo.FolderLevels);

                myLevels = new Button[tempLevelNames.Length];

                for (int i = 0; i < myLevels.Length; i++)
                {
                    myLevels[i] = new Button(new Vector2((aWindow.ClientBounds.Width / 2) - 113, (aWindow.ClientBounds.Height / 2) - 64 - 200 + (i * 40)),
                        new Point(226, 32), null, tempLevelNames[i], 0.4f);
                    myLevels[i].LoadContent();
                }
            }
        }
        private void SaveLevel()
        {
            if (mySaveButton.IsClicked())
            {
                for (int i = 0; i < myLevel.GetLength(0); i++)
                {
                    for (int j = 0; j < myLevel.GetLength(1); j++)
                    {
                        myLevel[i, j] = Level.GetTiles[i, j].TileType;
                    }
                }

                int tempLevel = 1;
                string[] tempName = FileReader.FindFileNames(GameInfo.FolderLevels);
                for (int i = 0; i < tempName.Length; i++)
                {
                    tempName[i] = tempName[i].Replace("Level", "");
                    tempName[i] = tempName[i].Replace(".txt", "");
                }
                for (int i = 0; i < tempName.Length; i++)
                {
                    if (tempName[i] != "Level_Template")
                    {
                        int tempResult = 0;
                        Int32.TryParse(tempName[i], out tempResult);

                        if (tempResult != (i + 1) && i > 0)
                        {
                            tempLevel = (i + 1);
                            break;
                        }
                    }
                }

                if (tempLevel < 10)
                {
                    Level.SaveLevel("Level0" + tempLevel, myLevel);
                }
                else
                {
                    Level.SaveLevel("Level" + tempLevel, myLevel);
                }
            }
        }
        private void DeleteLevel(GameWindow aWindow)
        {
            if (myDeleteButton.IsClicked())
            {
                myEditorState = EditorStates.isDeleting;
                string[] tempLevelNames = FileReader.FindFileNames(GameInfo.FolderLevels);

                myLevels = new Button[tempLevelNames.Length - 1];

                for (int i = 0; i < myLevels.Length; i++)
                {
                    myLevels[i] = new Button(new Vector2((aWindow.ClientBounds.Width / 2) - 113, (aWindow.ClientBounds.Height / 2) - 64 - 200 + (i * 40)),
                        new Point(226, 32), null, tempLevelNames[i], 0.4f);
                    myLevels[i].LoadContent();
                }
            }
        }

        private void SelectLevelToLoad()
        {
            foreach (Button button in myLevels)
            {
                button.Update();
                if (button.IsClicked())
                {
                    Level.LoadLevel(new Point(32), button.DisplayText);

                    LoadContent();

                    myEditorState = EditorStates.isEditing;
                    myLevels = null;
                }
            }
        }
        private void SelectLevelToDelete()
        {
            foreach (Button button in myLevels)
            {
                button.Update();
                if (button.IsClicked())
                {
                    Level.DeleteLevel(button.DisplayText);

                    myEditorState = EditorStates.isEditing;
                    myLevels = null;
                }
            }
        }

        public override void LoadContent()
        {
            Level.SetTileTextureEditor();

            mySelections[0].TileType = '#';
            mySelections[1].TileType = '?';
            mySelections[2].TileType = '*';
            mySelections[3].TileType = '%';
            mySelections[4].TileType = '&';

            Array.ForEach(mySelections, t => t.SetTextureEditor());

            myLoadButton.LoadContent();
            mySaveButton.LoadContent();
            myDeleteButton.LoadContent();

            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
