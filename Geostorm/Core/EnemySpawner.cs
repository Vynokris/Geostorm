using Geostorm.Utility;
using System.Numerics;
using System.Collections.Generic;

namespace Geostorm.Core
{
    public class EnemySpawner
    {
        private Vector2 ScreenSize      = new();
        private System.Random RandomGen = new();
        private Cooldown SpawnCooldown  = new(60);

        public EnemySpawner(int screenW, int screenH) { ScreenSize.X = screenW; ScreenSize.Y = screenH; }

        public void Update(ref List<Enemy> enemies, double gameDuration)
        {
            if (SpawnCooldown.Update())
            {
                // TODO: change cooldown according to game duration.
                SpawnCooldown.ChangeDuration(RandomGen.Next() % 60*2 + 60);
                SpawnRandomEnemy(ref enemies);
            }
        }

        public void SpawnRandomEnemy(ref List<Enemy> enemies)
        {
            int randType = RandomGen.Next() % 2;

            switch (randType)
            { 
            case 0:
                SpawnEnemy(ref enemies, typeof(Wanderer));
                break;
            case 1:
                SpawnEnemy(ref enemies, typeof(Grunt));
                break;
            default:
                break;
            }
        }

        public void SpawnEnemy(ref List<Enemy> enemies, System.Type enemyType)
        {
            Vector2 pos = new Vector2(RandomGen.Next() % ScreenSize.X, 
                                      RandomGen.Next() % ScreenSize.Y);

            if (enemyType == typeof(Wanderer))
                enemies.Add(new Wanderer(pos, 60));
            if (enemyType == typeof(Grunt))
                enemies.Add(new Grunt(pos, 60));
        }
    }
}
