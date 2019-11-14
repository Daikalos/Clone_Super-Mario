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

        }
        public static void SaveLevel(string aLevelName)
        {

        }

        public static bool CheckIfWon()
        {
            return false;
        }

        public static void SetTileTexture()
        {

        }
        public static void SetTileTextureEditor()
        {

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
