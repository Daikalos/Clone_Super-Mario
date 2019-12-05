using System;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    static class GameInfo
    {
        private static Vector2 myDrawPos;
        private static string
            myLevelName,
            myFolderLevels,
            myFolderHighScores;
        private static int[] myHighScores;
        private static int
            myScore,
            myDrawScore;
        private static float
            myDSTimer,
            myDSDelay;
        private static bool myIsPaused;

        public static string LevelName
        {
            get => myLevelName;
            set => myLevelName = value;
        }
        public static string FolderLevels
        {
            get => myFolderLevels;
            set => myFolderLevels = value;
        }
        public static string FolderHighScores
        {
            get => myFolderHighScores;
            set => myFolderHighScores = value;
        }
        public static int[] HighScores
        {
            get => myHighScores;
        }
        public static int Score
        {
            get => myScore;
            set => myScore = value;
        }
        public static int HighScore
        {
            get => myHighScores.Max();
        }
        public static bool IsPaused
        {
            get => myIsPaused;
            set => myIsPaused = value;
        }

        public static void Initialize(float aDSDelay)
        {
            myDSDelay = aDSDelay;

            myDrawPos = Vector2.Zero;
            myScore = 0;
            myDSTimer = 0;
        }

        public static void LoadHighScore(string aLevelName)
        {
            string tempPath = GameInfo.FolderHighScores + aLevelName + "_HighScores.txt";

            string[] tempScores = FileReader.FindInfo(tempPath, "HighScore", '=');
            myHighScores = Array.ConvertAll(tempScores, s => Int32.Parse(s));

            if (myHighScores.Length == 0)
            {
                myHighScores = new int[] { 0 };
            }

            Array.Sort(myHighScores);
            Array.Reverse(myHighScores);
        }
        public static void SaveHighScore(string aLevelName)
        {
            string tempPath = GameInfo.FolderHighScores + aLevelName + "_HighScores.txt";

            if (myHighScores.Length > 0)
            {
                if (myScore > 0)
                {
                    if (myHighScores[0] != 0)
                    {
                        File.AppendAllText(tempPath, Environment.NewLine + "HighScore=" + myScore.ToString());
                    }
                    else
                    {
                        File.AppendAllText(tempPath, "HighScore=" + myScore.ToString());
                    }
                }
            }
        }

        public static void Update(GameTime aGameTime)
        {
            if (myDSTimer >= 0)
            {
                myDSTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public static void Draw(SpriteBatch aSpriteBatch, GameWindow aWindow, SpriteFont aFont, Player aPlayer)
        {
            StringManager.DrawStringLeft(aSpriteBatch, aFont, "Lives: " + aPlayer.Lives, new Vector2(Camera.Position.X + 32, 32), Color.Black, 0.5f);
            StringManager.DrawStringLeft(aSpriteBatch, aFont, "Score: " + myScore.ToString(), new Vector2(Camera.Position.X + 32, 64), Color.Black, 0.5f);
            StringManager.DrawStringMid(aSpriteBatch, aFont, "HighScore: " + HighScore.ToString(), new Vector2(Camera.Position.X + (aWindow.ClientBounds.Width / 2), 32), Color.Black, 0.5f);
            StringManager.DrawStringRight(aSpriteBatch, aFont, GameInfo.LevelName, new Vector2(Camera.Position.X + (aWindow.ClientBounds.Width - 32), 32), Color.Black, 0.5f);

            if (myDSTimer >= 0)
            {
                StringManager.DrawStringMid(aSpriteBatch, aFont, myDrawScore.ToString(), myDrawPos, Color.Black, 0.3f);
            }
        }

        public static void AddScore(Vector2 aPos, int someScore)
        {
            myDrawPos = new Vector2(aPos.X, aPos.Y - Level.TileSize.Y);
            myScore += someScore;
            myDrawScore = someScore;
            myDSTimer = myDSDelay;
        }
    }
}
