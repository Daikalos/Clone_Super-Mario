using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    static class Level
    {
        private static string[] myLevelBuilder;
        private static Tile[,] myTiles;
        private static Point
            myTileSize,
            myMapSize;

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

        public static Tuple<Tile, bool> GetTileAtPos(Vector2 aPos)
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
        public static Tile GetClosestTile(Vector2 aPos)
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

        public static void LoadLevel(Point aTileSize)
        {
            if (File.Exists(GameInfo.FolderLevels + GameInfo.CurrentLevel))
            {
                myLevelBuilder = File.ReadAllLines(GameInfo.FolderLevels + GameInfo.CurrentLevel);
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
                            myTileSize);
                        myTiles[x, y].TileType = myLevelBuilder[y][x];
                    }
                }

                myMapSize = new Point(
                    myTiles.GetLength(0) * myTileSize.X,
                    myTiles.GetLength(1) * myTileSize.Y);
            }
            else //Create custom level
            {
                myTileSize = aTileSize;

                int tempSizeX = 128;
                int tempSizeY = 32;

                myTiles = new Tile[tempSizeX, tempSizeY];

                for (int x = 0; x < tempSizeX; x++)
                {
                    for (int y = 0; y < tempSizeY; y++)
                    {
                        myTiles[x, y] = new Tile(
                            new Vector2(x * myTileSize.X, y * myTileSize.Y),
                            myTileSize);
                        myTiles[x, y].TileType = '-';
                    }
                }

                myMapSize = new Point(
                myTiles.GetLength(0) * myTileSize.X,
                myTiles.GetLength(1) * myTileSize.Y);
            }
        }
        public static void SaveLevel(string aLevelName)
        {

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

        public static bool CheckIfWon()
        {
            return false;
        }
        public static bool CheckIn(int anX, int anY)
        {
            if (anX >= 0 && anX < myTiles.GetLength(0))
            {
                if (anY >= 0 && anY < myTiles.GetLength(1))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
