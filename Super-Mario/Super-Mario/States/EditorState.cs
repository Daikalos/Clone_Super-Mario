using System;
using System.Text.RegularExpressions;
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
            isSaving,
            isDeleting
        }

        private SpriteFont 
            my8bitFont;

        /// <summary>
        /// 0 = Block|#;
        /// 1 = Player|?;
        /// 2 = Flag|*;
        /// 3 = Ladder|%;
        /// 4 = Goomba|And;
        /// 6 = Koopa|";
        /// 7 = Coin_Block|^;
        /// 8 = Item_Block|/;
        /// 9 = Gravity_Block|¤;
        /// </summary>
        private Tile[] mySelections;
        private Button[] myLevels;
        private Button 
            myLoadButton,
            mySaveButton,
            myDeleteButton;
        private Rectangle 
            myOffset,
            mySelectedSource;
        private EditorStates myEditorState;
        private char mySelectedTile;
        private char[,] myLevel;
        private float
            myTimer,
            myDelay;
        private int myTile;
        private string myLevelName;

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
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '#'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '?'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '*'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '%'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '"'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '&'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '^'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '/'),
                new Tile(new Vector2(aWindow.ClientBounds.Width - 64, 32), new Point(32), '¤'),
            };
            this.myEditorState = EditorStates.isEditing;
            this.myOffset = new Rectangle(
                -Level.TileSize.X / 16, -Level.TileSize.Y / 16, 
                Level.TileSize.X / 8, Level.TileSize.Y / 8);
            this.mySelectedTile = ' ';
            this.myDelay = 0.50f;
            this.myTile = -1;
            this.myLevelName = string.Empty;
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
                case EditorStates.isSaving:
                    TypeNameOfLevel();
                    break;
                case EditorStates.isDeleting:
                    SelectLevelToDelete();
                    break;
            }

            if (KeyMouseReader.KeyPressed(Keys.Escape))
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

            Level.DrawTiles(aSpriteBatch, aGameTime);

            switch (myEditorState)
            {
                case EditorStates.isEditing:
                    Array.ForEach(mySelections, t => t.Draw(aSpriteBatch, aGameTime));

                    if (myTile >= 0 && myTile < mySelections.Length)
                    {
                        aSpriteBatch.Draw(mySelections[myTile].Texture, (Camera.Position + KeyMouseReader.CurrentMouseState.Position.ToVector2()), mySelectedSource, Color.White);
                    }

                    myLoadButton.Draw(aSpriteBatch);
                    mySaveButton.Draw(aSpriteBatch);
                    myDeleteButton.Draw(aSpriteBatch);

                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press escape to go back to menu", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.4f);
                    break;
                case EditorStates.isLoading:
                    Array.ForEach(myLevels, b => b.Draw(aSpriteBatch));
                    StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "LOAD", new Vector2(Camera.Position.X + (aWindow.ClientBounds.Width / 2), 32), Color.Black, 0.9f);
                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press escape to go back to editor", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.4f);
                    break;
                case EditorStates.isSaving:
                    StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "Type name of level", new Vector2(Camera.Position.X + (aWindow.ClientBounds.Width / 2), 32), Color.Black, 0.8f);
                    StringManager.DrawStringMid(aSpriteBatch, my8bitFont, myLevelName + "_", new Vector2(Camera.Position.X + (aWindow.ClientBounds.Width / 2), 96), Color.Black, 0.8f);
                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press escape to go back to editor", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.4f);
                    break;
                case EditorStates.isDeleting:
                    Array.ForEach(myLevels, b => b.Draw(aSpriteBatch));
                    StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "DELETE", new Vector2(Camera.Position.X + aWindow.ClientBounds.Width / 2, 32), Color.Black, 0.9f);
                    StringManager.DrawStringLeft(aSpriteBatch, my8bitFont, "Press escape to go back to editor", new Vector2(Camera.Position.X + 16, aWindow.ClientBounds.Height - 16), Color.Black * 0.50f, 0.4f);
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

                if (tempRect.Contains(Camera.Position.ToPoint() + KeyMouseReader.CurrentMouseState.Position))
                {
                    mySelections[i].BoundingBox = new Rectangle(tempRect.X + myOffset.X, tempRect.Y + myOffset.Y,
                        tempRect.Width + myOffset.Width, tempRect.Height + myOffset.Height);

                    if (KeyMouseReader.LeftClick())
                    {
                        mySelectedTile = mySelections[i].TileType;
                        myTile = i;

                        switch (mySelections[i].TileType)
                        {
                            case '/':
                                mySelectedSource = new Rectangle(0, 0, mySelections[i].Texture.Width / 4, mySelections[i].Texture.Height);
                                break;
                            case '¤':
                                mySelectedSource = new Rectangle(0, 0, mySelections[i].Texture.Width / 4, mySelections[i].Texture.Height / 2);
                                break;
                            default:
                                mySelectedSource = new Rectangle(0, 0, mySelections[i].Texture.Width, mySelections[i].Texture.Height);
                                break;
                        }

                        myTimer = myDelay;
                    }
                }
            }
        }
        private void EditMap()
        {
            if (KeyMouseReader.LeftHold() && mySelectedTile != ' ' && myTimer <= 0)
            {
                Tuple<Tile, bool> tempTile = Level.TileAtPos(Camera.Position + KeyMouseReader.CurrentMouseState.Position.ToVector2());

                if (tempTile.Item2)
                {
                    tempTile.Item1.TileType = mySelectedTile;
                    tempTile.Item1.TileForm = 0;
                    tempTile.Item1.SetTextureEditor();
                }
            }
            if (KeyMouseReader.RightHold())
            {
                mySelectedTile = '-';
                myTile = -1;

                Tuple<Tile, bool> tempTile = Level.TileAtPos(Camera.Position + KeyMouseReader.CurrentMouseState.Position.ToVector2());

                if (tempTile.Item2)
                {
                    tempTile.Item1.TileType = mySelectedTile;
                    tempTile.Item1.TileForm = 0;
                    tempTile.Item1.SetTextureEditor();
                }
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
                myEditorState = EditorStates.isSaving;

                for (int i = 0; i < myLevel.GetLength(0); i++)
                {
                    for (int j = 0; j < myLevel.GetLength(1); j++)
                    {
                        myLevel[i, j] = Level.GetTiles[i, j].TileType;
                    }
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

                int tempAddButton = 0;
                for (int i = 0; i < tempLevelNames.Length; i++)
                {
                    if (tempLevelNames[i] != "Level_Template")
                    {
                        myLevels[tempAddButton] = new Button(new Vector2((aWindow.ClientBounds.Width / 2) - 113, (aWindow.ClientBounds.Height / 2) - 64 - 200 + (tempAddButton * 40)),
                            new Point(226, 32), null, tempLevelNames[i], 0.4f);
                        myLevels[tempAddButton].LoadContent();

                        tempAddButton++;
                    }
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
        private void TypeNameOfLevel()
        {
            Keys[] tempKeys = KeyMouseReader.CurrentKeyState.GetPressedKeys();

            if (tempKeys.Length > 0)
            {
                string tempLetter = tempKeys[0].ToString();

                if (Regex.IsMatch(tempLetter, @"[a-zA-Z]") && tempLetter.Length == 1)
                {
                    if (KeyMouseReader.PreviousKeyState.IsKeyUp(tempKeys[0]))
                    {
                        myLevelName += tempKeys[0].ToString();
                    }
                }

                if (KeyMouseReader.KeyPressed(Keys.Space))
                {
                    myLevelName += "_";
                }
                if (KeyMouseReader.KeyPressed(Keys.Back) && myLevelName.Length > 0)
                {
                    myLevelName = myLevelName.Remove(myLevelName.Length - 1, 1);
                }
                if (KeyMouseReader.KeyPressed(Keys.Enter) && myLevelName.Length > 0)
                {
                    myEditorState = EditorStates.isEditing;
                    Level.SaveLevel(myLevelName, myLevel);
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

            Array.ForEach(mySelections, t => t.SetTextureEditor());

            myLoadButton.LoadContent();
            mySaveButton.LoadContent();
            myDeleteButton.LoadContent();

            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
