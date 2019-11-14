using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    static class EnemyManager
    {
        static List<Enemy> myEnemies;
        static float[] myRespawnTimer;
        static bool[] myRespawnEnemy;
        static float myRespawnDelay;

        public static List<Enemy> Enemies
        {
            get => myEnemies;
        }

        public static void Initialize(float aRespawnDelay)
        {
            myEnemies = new List<Enemy>();
            myRespawnDelay = aRespawnDelay;
        }

        public static void Update(GameTime aGameTime)
        {

        }

        public static void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {

        }

        public static void AddEnemies()
        {

        }
        public static void RemoveAll()
        {

        }

        public static void SetTexture()
        {
            
        }
    }
}
