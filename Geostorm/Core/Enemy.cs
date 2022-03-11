using System.Numerics;
using static MyMathLib.Geometry2D;
using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Core
{
    public abstract class Enemy : Entity
    {
        public readonly Cooldown SpawnDelay = new(0);

        public Enemy() { }
        public Enemy(Vector2 pos, float health, int spawnDelay) : base(pos, Vector2Create(3, 0), 0, health) { SpawnDelay.ChangeDuration(spawnDelay); }

        public sealed override void Update(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents)
        {
            if (SpawnDelay.Update())
                DoUpdate(gameState, gameInputs, ref gameEvents);
        }

        public abstract void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents);
    }
}
