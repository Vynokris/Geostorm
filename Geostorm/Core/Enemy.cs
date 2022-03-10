using System.Numerics;
using static MyMathLib.Geometry2D;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public abstract class Enemy : Entity
    {
        protected int SpawnDelay = 0;

        public Enemy() { }
        public Enemy(Vector2 pos, float health, int spawnDelay) : base(pos, Vector2Create(3, 0), 0, health) { SpawnDelay = spawnDelay; }

        public sealed override void Update(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents)
        {
            if (SpawnDelay <= 0)
                DoUpdate(gameState, gameInputs, ref gameEvents);
            else
                SpawnDelay--;
        }

        public abstract void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents);
    }
}
