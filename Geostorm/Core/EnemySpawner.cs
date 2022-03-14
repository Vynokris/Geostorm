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
            int totalChance    = 0;
            int wandererChance = 40;
            int gruntChance    = 40;
            int weaverChance   = 20;

            int randInt = RandomGen.Next() % 100;

            totalChance += wandererChance;
            if (randInt < totalChance) {
                SpawnEnemy(ref gameEvents, typeof(Wanderer), gameState.ScreenSize);
                return;
            }
            totalChance += gruntChance;
            if (randInt < totalChance) {
                SpawnEnemy(ref gameEvents, typeof(Grunt), gameState.ScreenSize);
                return;
            }
            totalChance += weaverChance;
            if (randInt < totalChance) {
                SpawnEnemy(ref gameEvents, typeof(Weaver), gameState.ScreenSize);
                return;
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
