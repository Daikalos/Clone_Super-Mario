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
            myFolderLevels,
            myFolderHighScores;
        private static int[] myHighScores;
        private static int
            myScore,
            myCurrentLevel,
            myDrawScore;
        private static float
            myDSTimer,
            myDSDelay;
        private static bool myIsPaused;

        public static Vector2 DrawPos
        {
            set => myDrawPos = value;
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
        public static int CurrentLevel
        {
            get => myCurrentLevel;
            set => myCurrentLevel = value;
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

        public static string LevelName()
        {
            if (myCurrentLevel < 10)
            {
                return "Level0" + myCurrentLevel;
            }
            return "Level" + myCurrentLevel;
        }

        public static void Initialize(GameWindow aWindow, float aDSDelay)
        {
            myDSDelay = aDSDelay;

            myDrawPos = Vector2.Zero;
            myScore = 0;
            myDSTimer = 0;
        }

        public static void LoadHighScore(string aPath)
        {
            string[] tempScores = FileReader.FindInfo(aPath, "HighScore", '=');
            myHighScores = Array.ConvertAll(tempScores, s => Int32.Parse(s));

            if (myHighScores.Length == 0)
            {
                myHighScores = new int[] { 0 };
            }

            Array.Sort(myHighScores);
            Array.Reverse(myHighScores);
        }
        public static void SaveHighScore(string aPath)
        {

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
            StringManager.DrawStringLeft(aSpriteBatch, aFont, "HighScore: " + HighScore.ToString(), new Vector2(Camera.Position.X + 32, 32), Color.Black, 0.5f);
            StringManager.DrawStringLeft(aSpriteBatch, aFont, "Score: " + myScore.ToString(), new Vector2(Camera.Position.X + 32, 64), Color.Black, 0.5f);

            if (myDSTimer >= 0)
            {
                StringManager.DrawStringMid(aSpriteBatch, aFont, myDrawScore.ToString(), 
                    new Vector2(Camera.Position.X + myDrawPos.X, myDrawPos.Y), Color.White, 0.3f);
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
