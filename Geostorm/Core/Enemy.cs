using System.Numerics;
using static MyMathLib.Geometry2D;

namespace Geostorm.Core
{
    public abstract class Enemy : Entity
    {
        protected int spawnDelay;

        public Enemy() { }
        public Enemy(Vector2 _pos) : base(_pos, Vector2Zero(), 0) { }

        public sealed override void Update(in GameInputs inputs, ref GameEvents gameEvents)
        {
            if (spawnDelay > 0)
                spawnDelay--;
            else
                DoUpdate(inputs, ref gameEvents);
        }

        public abstract void DoUpdate(in GameInputs inputs, ref GameEvents gameEvents);
    }
}
