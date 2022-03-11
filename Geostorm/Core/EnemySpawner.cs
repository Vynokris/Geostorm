using Geostorm.Utility;
using System.Numerics;
using System.Collections.Generic;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class EnemySpawner
    {
        private Vector2 ScreenSize      = new();
        private System.Random RandomGen = new();
        private Cooldown SpawnCooldown  = new(60);

        public EnemySpawner(int screenW, int screenH) { ScreenSize.X = screenW; ScreenSize.Y = screenH; }

        public void Update(ref List<GameEvent> gameEvents, double gameDuration)
        {
            if (SpawnCooldown.Update())
            {
                // TODO: change cooldown according to game duration.
                SpawnCooldown.ChangeDuration(RandomGen.Next() % 60*2);
                for (int i = 0; i < RandomGen.Next(1, 3); i++)
                    SpawnRandomEnemy(ref gameEvents);
            }
        }

        public void SpawnRandomEnemy(ref List<GameEvent> gameEvents)
        {
            int randType = RandomGen.Next() % 2;

            switch (randType)
            { 
            case 0:
                SpawnEnemy(ref gameEvents, typeof(Wanderer));
                break;
            case 1:
                SpawnEnemy(ref gameEvents, typeof(Grunt));
                break;
            default:
                break;
            }
        }

        public void SpawnEnemy(ref List<GameEvent> gameEvents, System.Type enemyType)
        {
            Vector2 pos = new Vector2(RandomGen.Next() % ScreenSize.X, 
                                      RandomGen.Next() % ScreenSize.Y);

            if (enemyType == typeof(Wanderer))
                gameEvents.Add(new EnemySpawnedEvent(new Wanderer(pos, 60)));
            if (enemyType == typeof(Grunt))
                gameEvents.Add(new EnemySpawnedEvent(new Grunt(pos, 60)));
        }
    }
}
