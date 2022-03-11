using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;
using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public abstract class Enemy : Entity, IEventListener
    {
        public readonly Cooldown SpawnDelay = new(0);

        public Enemy() { }
        public Enemy(Vector2 pos, float health, int spawnDelay) : base(pos, Vector2Create(3, 0), 0, health) { SpawnDelay.ChangeDuration(spawnDelay); }

        public sealed override void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            if (SpawnDelay.Update())
                DoUpdate(gameState, gameInputs, ref gameEvents);
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
