using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;
using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public abstract class Enemy : Entity
    {
        public float PreSpawnDelay { get; private set; } = 0;
        public readonly Cooldown SpawnDelay = new(1);
        public float VelocityLength = 3;

        public Enemy() { }
        public Enemy(Vector2 pos, RGBA color, float preSpawnDelay = 0, float speed = 3) : base(pos, Vector2Create(3, 0), 0, color) 
        { 
            PreSpawnDelay = preSpawnDelay; 
            SpawnDelay.ChangeDuration(preSpawnDelay); 
            VelocityLength = speed;
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
    }
}
