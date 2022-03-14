﻿using System;
using System.Numerics;
using System.Collections.Generic;

using Geostorm.Core;
using Geostorm.GameData;

namespace Geostorm.Utility
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
            // Chance percentages for random enemy type.
            Type[] enemyTypes   = new Type[] { typeof(Wanderer), typeof(Rocket), typeof(Grunt), typeof(Weaver) };
            int[]  enemyChances = new int[]  { 35,               20,             30,            15             };
            int    totalChance  = 0;

            int randInt = RandomGen.Next() % 100;
            for (int i = 0; i < enemyChances.Length; i++)
            {
                totalChance += enemyChances[i];
                if (randInt < totalChance) { 
                    SpawnEnemy(ref gameEvents, enemyTypes[i], gameState.ScreenSize);
                    return;
                }
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
            else if (enemyType == typeof(Rocket)) {
                gameEvents.Add(new EnemySpawnedEvent(new Rocket  (pos, preSpawnDelay)));
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
