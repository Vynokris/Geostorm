using Geostorm.Utility;
using System.Numerics;
using System.Collections.Generic;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class EnemySpawner
    {
        private System.Random RandomGen = new();
        private Cooldown SpawnCooldown  = new(1);

        public EnemySpawner() { }

        public void Update(in GameState gameState, ref List<GameEvent> gameEvents)
        {
            if (SpawnCooldown.Update(gameState.DeltaTime))
            {
                // TODO: change cooldown according to game duration.
                SpawnCooldown.ChangeDuration(RandomGen.Next() % 2);
                for (int i = 0; i < RandomGen.Next(1, 3); i++)
                    SpawnRandomEnemy(gameState, ref gameEvents);
            }
        }

        public void SpawnRandomEnemy(in GameState gameState, ref List<GameEvent> gameEvents)
        {
            int randType = RandomGen.Next() % 3;

            switch (randType)
            { 
            case 0:
                SpawnEnemy(ref gameEvents, typeof(Wanderer), gameState.ScreenSize);
                break;
            case 1:
                SpawnEnemy(ref gameEvents, typeof(Grunt), gameState.ScreenSize);
                break;
            case 2:
                SpawnEnemy(ref gameEvents, typeof(Weaver), gameState.ScreenSize);
                break;
            default:
                break;
            }
        }

        public void SpawnEnemy(ref List<GameEvent> gameEvents, System.Type enemyType, Vector2 screenSize)
        {
            Vector2 pos = new(RandomGen.Next() % screenSize.X, 
                              RandomGen.Next() % screenSize.Y);
            float preSpawnDelay = RandomGen.Next(0, 120) / 60f;

            if      (enemyType == typeof(Wanderer)) {
                gameEvents.Add(new EnemySpawnedEvent(new Wanderer(pos, preSpawnDelay)));
            }
            else if (enemyType == typeof(Grunt)) {
                gameEvents.Add(new EnemySpawnedEvent(new Grunt   (pos, preSpawnDelay)));
            }
            else if (enemyType == typeof(Weaver)) {
                gameEvents.Add(new EnemySpawnedEvent(new Weaver  (pos, preSpawnDelay)));
            }
        }
    }
}
