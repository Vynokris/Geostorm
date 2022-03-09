using System.Numerics;

namespace Geostorm.Core
{
    public abstract class Enemy : Entity
    {
        protected int spawnDelay;

        public sealed override void Update(GameInputs inputs)
        {
            if (spawnDelay > 0)
                spawnDelay--;
            else
                DoUpdate(inputs);
        }

        public abstract void DoUpdate(GameInputs inputs);
    }
}
