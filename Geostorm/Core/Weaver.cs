using System.Numerics;
using System.Collections.Generic;

using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Weaver : Enemy
    {
        public Weaver() { }
        public Weaver(Vector2 pos, float preSpawnDelay = 0) : base(pos, preSpawnDelay) { }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {

        }
    }
}
