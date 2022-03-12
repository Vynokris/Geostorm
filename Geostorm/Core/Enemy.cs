﻿using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;
using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public abstract class Enemy : Entity, IEventListener
    {
        public float PreSpawnDelay { get; private set; } = 0;
        public readonly Cooldown SpawnDelay = new(1);

        public Enemy() { }
        public Enemy(Vector2 pos, float health, float preSpawnDelay = 0) : base(pos, Vector2Create(3, 0), 0, health) 
        { 
            PreSpawnDelay = preSpawnDelay; 
            SpawnDelay.ChangeDuration(preSpawnDelay); 
        }

        public sealed override void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            if (SpawnDelay.Update(gameState.DeltaTime))
            {
                if (PreSpawnDelay > 0) {
                    SpawnDelay.ChangeDuration(1);
                    PreSpawnDelay = 0;
                }
                else {
                    DoUpdate(gameState, gameInputs, ref gameEvents);
                }
            }
        }

        public abstract void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents);

        public void HandleEvents(in List<GameEvent> gameEvents)
        {
            foreach (GameEvent Event in gameEvents)
            {
                if (Event is EnemyDamagedEvent damageEvent && damageEvent.enemy == this)
                {
                    Health -= 1;
                    break;
                }
            }
        }
    }
}
