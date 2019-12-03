using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    static class Level
    {
        private static Tile[,] myTiles;
        private static Vector2 myPlayerSpawn;
        private static Point
            myTileSize,
            myMapSize;

        public static Vector2 PlayerSpawn
        {
            get => myPlayerSpawn;
        }
        public static Point TileSize
        {
            get => myTileSize;
        }
        public static Point MapSize
        {
            get => myMapSize;
        }

        public static Tile[,] GetTiles
        {
            get => myTiles;
        }

        public static Tuple<Tile, bool> TileAtPos(Vector2 aPos)
        {
            if (aPos.X > 0 && aPos.Y > 0)
            {
                if (((int)aPos.X / myTileSize.X) >= 0 && ((int)aPos.Y / myTileSize.Y) >= 0)
                {
                    if (((int)aPos.X / myTileSize.X) < myTiles.GetLength(0) && ((int)aPos.Y / myTileSize.Y) < myTiles.GetLength(1))
                    {
                        return new Tuple<Tile, bool>(myTiles[(int)aPos.X / myTileSize.X, (int)aPos.Y / myTileSize.Y], true);
                    }
                }
            }
            return new Tuple<Tile, bool>(myTiles[0, 0], false);
        }
        public static List<Tile> TilesAround(DynamicObject aObject)
        {
            List<Tile> tempTiles = new List<Tile>();
            Vector2 tempOffset = new Vector2((aObject.Size.X / 2) - (Level.TileSize.X / 2), (aObject.Size.Y / 2) - (Level.TileSize.Y / 2));

            Vector2 tempPosition;
            Tuple<Tile, bool> tempTile;

            for (int x = 0; x < (aObject.Size.X / Level.TileSize.X) + 2; x++)
            {
                tempPosition = new Vector2(aObject.BoundingBox.Center.X - Level.TileSize.X + (Level.TileSize.X * x) - tempOffset.X, aObject.BoundingBox.Center.Y - Level.TileSize.Y - tempOffset.Y);
                tempTile = TileAtPos(tempPosition);

                if (!tempTiles.Contains(tempTile.Item1) && tempTile.Item2)
                {
                    tempTiles.Add(tempTile.Item1);
                }

                tempPosition = new Vector2(aObject.BoundingBox.Center.X - Level.TileSize.X + (Level.TileSize.X * x) - tempOffset.X, aObject.BoundingBox.Center.Y + Level.TileSize.Y + tempOffset.Y);
                tempTile = TileAtPos(tempPosition);

                if (!tempTiles.Contains(tempTile.Item1) && tempTile.Item2)
                {
                    tempTiles.Add(tempTile.Item1);
                }
            }

            for (int y = 0; y < (aObject.Size.Y / Level.TileSize.Y) + 2; y++)
            {
                tempPosition = new Vector2(aObject.BoundingBox.Center.X - Level.TileSize.X - tempOffset.X, aObject.BoundingBox.Center.Y - Level.TileSize.Y + (Level.TileSize.Y * y) - tempOffset.Y);
                tempTile = TileAtPos(tempPosition);

                if (!tempTiles.Contains(tempTile.Item1) && tempTile.Item2)
                {
                    tempTiles.Add(tempTile.Item1);
                }

                tempPosition = new Vector2(aObject.BoundingBox.Center.X + Level.TileSize.X + tempOffset.X, aObject.BoundingBox.Center.Y - Level.TileSize.Y + (Level.TileSize.Y * y) - tempOffset.Y);
                tempTile = TileAtPos(tempPosition);

                if (!tempTiles.Contains(tempTile.Item1) && tempTile.Item2)
                {
                    tempTiles.Add(tempTile.Item1);
                }
            }

            return tempTiles;
        }
        public static List<Tile> TilesOn(DynamicObject aObject)
        {
            List<Tile> tempTiles = new List<Tile>();
            Vector2 tempOffset = new Vector2(
                (aObject.Size.X / 2) - (Level.TileSize.X / 2), 
                (aObject.Size.Y / 2) - (Level.TileSize.Y / 2));

            Vector2 tempPosition;
            Tuple<Tile, bool> tempTile;

            for (int x = 0; x < (aObject.Size.X / Level.TileSize.X); x++)
            {
                tempPosition = new Vector2(aObject.BoundingBox.Center.X + (Level.TileSize.X * x) - tempOffset.X, aObject.BoundingBox.Center.Y - tempOffset.Y);
                tempTile = TileAtPos(tempPosition);

                if (!tempTiles.Contains(tempTile.Item1) && tempTile.Item2)
                {
                    tempTiles.Add(tempTile.Item1);
                }

                for (int y = 0; y < (aObject.Size.Y / Level.TileSize.Y); y++)
                {
                    tempPosition = new Vector2(aObject.BoundingBox.Center.X + (Level.TileSize.X * x) - tempOffset.X, aObject.BoundingBox.Center.Y + (Level.TileSize.Y * y) - tempOffset.Y);
                    tempTile = TileAtPos(tempPosition);

                    if (!tempTiles.Contains(tempTile.Item1) && tempTile.Item2)
                    {
                        tempTiles.Add(tempTile.Item1);
                    }
                }
            }

            return tempTiles;
        }
        public static Tile ClosestTile(Vector2 aPos)
        {
            Tile tempClosest = null;
            float tempMinDistance = float.MaxValue;

            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    if (myTiles[i, j].TileType != '#')
                    {
                        float tempDistance = Vector2.Distance(myTiles[i, j].GetCenter(), aPos);
                        if (tempDistance < tempMinDistance)
                        {
                            tempClosest = myTiles[i, j];
                            tempMinDistance = tempDistance;
                        }
                    }
                }
            }
            return tempClosest;
        }

        public static void Update()
        {
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    myTiles[i, j].Update();
                }
            }
        }

        public static void DrawTiles(SpriteBatch aSpriteBatch)
        {
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    myTiles[i, j].Draw(aSpriteBatch);
                }
            }
        }

        public static bool LoadLevel(Point aTileSize, string aLevelName)
        {
            if (File.Exists(GameInfo.FolderLevels + aLevelName + ".txt"))
            {
                string[] myLevelBuilder = File.ReadAllLines(GameInfo.FolderLevels + aLevelName + ".txt");
                myTileSize = aTileSize;

                int tempSizeX = myLevelBuilder[0].Length;
                int tempSizeY = myLevelBuilder.Length;

                myTiles = new Tile[tempSizeX, tempSizeY];

                for (int x = 0; x < tempSizeX; x++)
                {
                    for (int y = 0; y < tempSizeY; y++)
                    {
                        myTiles[x, y] = new Tile(
                            new Vector2(x * myTileSize.X, y * myTileSize.Y),
                            myTileSize, myLevelBuilder[y][x]);

                        switch(myTiles[x, y].TileType)
                        {
                            case '?':
                                myPlayerSpawn = myTiles[x, y].Position;
                                break;
                            case '&':
                                EnemyManager.AddPatrolEnemy(myTiles[x, y].Position);
                                break;
                        }
                    }
                }

                myMapSize = new Point(
                    myTiles.GetLength(0) * myTileSize.X,
                    myTiles.GetLength(1) * myTileSize.Y);

                return true;
            }
            return false;
        }
        public static void SaveLevel(string aLevelName, char[,] aLevel)
        {
            string tempPathLevel = GameInfo.FolderLevels + aLevelName + ".txt";

            string tempName = aLevelName;
            tempName = tempName.Replace(".txt", "");

            string tempPathHighScores = GameInfo.FolderHighScores + tempName + "_HighScores.txt";

            if (File.Exists(tempPathLevel))
            {
                File.Delete(tempPathLevel);
            }
            if (File.Exists(tempPathHighScores))
            {
                File.Delete(tempPathHighScores);
            }

            FileStream tempFS = File.Create(tempPathLevel);
            tempFS.Close();

            tempFS = File.Create(tempPathHighScores);
            tempFS.Close();

            for (int i = 0; i < aLevel.GetLength(1); i++)
            {
                string tempLevel = "";

                for (int j = 0; j < aLevel.GetLength(0); j++)
                {
                    tempLevel += aLevel[j, i].ToString();
                }

                File.AppendAllText(tempPathLevel, tempLevel);
                File.AppendAllText(tempPathLevel, Environment.NewLine);
            }
        }
        public static void DeleteLevel(string aLevelName)
        {
            string tempPathLevel = GameInfo.FolderLevels + aLevelName + ".txt";

            string tempName = aLevelName;
            tempName = tempName.Replace(".txt", "");

            string tempPathHighScores = GameInfo.FolderHighScores + tempName + "_HighScores.txt";

            if (File.Exists(tempPathLevel))
            {
                File.Delete(tempPathLevel);
            }
            if (File.Exists(tempPathHighScores))
            {
                File.Delete(tempPathHighScores);
            }
        }

        public static bool CheckIfWon()
        {
            return false;
        }
        public static bool CheckIn(int anX, int anY)
        {
            return 
                anX >= 0 &&
                anX < myTiles.GetLength(0) &&
                anY >= 0 &&
                anY < myTiles.GetLength(1);
        }

        public static void SetTileTexture()
        {
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    if (CheckIn(i, j - 1))
                    {
                        if (myTiles[i, j - 1].TileType != '#')
                        {
                            myTiles[i, j].TileForm = 1;
                        }
                    }

                    myTiles[i, j].SetTexture();
                }
            }
        }
        public static void SetTileTextureEditor()
        {
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    if (CheckIn(i, j - 1))
                    {
                        if (myTiles[i, j - 1].TileType != '#')
                        {
                            myTiles[i, j].TileForm = 1;
                        }
                    }

                    myTiles[i, j].SetTextureEditor();
                }
            }
        }
    }
}
