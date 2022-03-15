using System;
using System.Numerics;
using System.Collections.Generic;

using static MyMathLib.Arithmetic;

using Geostorm.Core;
using Geostorm.GameData;

namespace Geostorm.Utility
{
    public class EnemySpawner
    {
        private Type[] EnemyTypes   = new Type[] { typeof(Wanderer), typeof(Rocket), typeof(Grunt), typeof(Weaver), typeof(Snake) };
        public  int [] EnemyChances = new int[]  { 30,               20,             25,            15,             10 };

        public bool  AdaptativeSpawnSpeed = true;
        public float TimeBetweenWaves     = 1f;
        public int   SnakeMinLen          = 5;
        public int   SnakeMaxLen          = 15;

        private Random   Rng           = new();
        private Cooldown SpawnCooldown = new(0);

        public EnemySpawner() { }

        public void Update(in GameState gameState, ref List<GameEvent> gameEvents)
        {
            if (SpawnCooldown.Update(gameState.DeltaTime))
            {
                // Change the time between waves.
                if (AdaptativeSpawnSpeed)
                { 
                    TimeBetweenWaves = Remap(ClampUnder(gameState.GameDuration, 60), 0, 60, 1, -0.5f);
                }

                // Spawn a wave of enemies.
                SpawnCooldown.ChangeDuration(ClampAbove(Rng.Next() % 2 + TimeBetweenWaves, 0));
                for (int i = 0; i < Rng.Next(1, 3); i++)
                    SpawnRandomEnemy(gameState, ref gameEvents);
            }
        }

        public void SpawnRandomEnemy(in GameState gameState, ref List<GameEvent> gameEvents)
        {
            // Chance percentages for random enemy type.
            int    totalChance  = 0;

            int randInt = Rng.Next() % 100;
            for (int i = 0; i < EnemyChances.Length; i++)
            {
                totalChance += EnemyChances[i];
                if (randInt < totalChance) { 
                    SpawnEnemy(ref gameEvents, EnemyTypes[i], gameState.ScreenSize);
                    return;
                }
            }
        }

        public void SpawnEnemy(ref List<GameEvent> gameEvents, System.Type enemyType, Vector2 screenSize)
        {
            Vector2 pos = new(Rng.Next() % screenSize.X, 
                              Rng.Next() % screenSize.Y);
            float preSpawnDelay = Rng.Next(0, 120) / 60f;

            if      (enemyType == typeof(Wanderer)) {
                gameEvents.Add(new EnemySpawnedEvent(new Wanderer(pos, preSpawnDelay)));
            }
            else if (enemyType == typeof(Rocket)) {
                gameEvents.Add(new EnemySpawnedEvent(new Rocket  (pos, preSpawnDelay)));
            }
            else if (enemyType == typeof(Grunt)) {
                gameEvents.Add(new EnemySpawnedEvent(new Grunt   (pos, preSpawnDelay)));
            }
            else if (enemyType == typeof(Weaver)) {
                gameEvents.Add(new EnemySpawnedEvent(new Weaver  (pos, preSpawnDelay)));
            }
            else if (enemyType == typeof(Snake)) { 
                gameEvents.Add(new EnemySpawnedEvent(new Snake   (pos, preSpawnDelay, SnakeMinLen, SnakeMaxLen)));
            }
        }
    }
}
