using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    static class EnemyManager
    {
        static List<Enemy> myEnemies;

        public static List<Enemy> Enemies
        {
            get => myEnemies;
        }

        public static void Initialize()
        {
            myEnemies = new List<Enemy>();
        }

        public static void Update(GameTime aGameTime)
        {
            for (int i = myEnemies.Count - 1; i >= 0; i--)
            {
                myEnemies[i].Update(aGameTime);
                if (!myEnemies[i].IsAlive)
                {
                    myEnemies.RemoveAt(i);
                }
            }
        }

        public static void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            for (int i = myEnemies.Count - 1; i >= 0; i--)
            {
                myEnemies[i].Draw(aSpriteBatch, aGameTime);
            }
        }

        public static void AddPatrolEnemy(Vector2 aPos)
        {
            myEnemies?.Add(new Patrol(aPos, new Point(32), 1.0f, 8.2f));
        }
        public static void AddChaseEnemy(Vector2 aPos)
        {
            myEnemies?.Add(new Chase(aPos, new Point(32), 5.0f, 8.2f));
        }
        public static void RemoveAll()
        {

        }

        public static void SetTexture()
        {
            foreach (Enemy enemy in myEnemies)
            {
                if (enemy is Chase)
                {
                    enemy.SetTexture("Goomba_Walking");
                }
                if (enemy is Patrol)
                {
                    enemy.SetTexture("Goomba_Walking");
                }
            }
        }
    }
}
