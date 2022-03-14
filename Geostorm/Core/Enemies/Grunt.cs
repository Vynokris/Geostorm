using System.Numerics;
using System.Collections.Generic;

using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Grunt : Enemy
    {
        public Grunt() { }
        public Grunt(Vector2 pos, float preSpawnDelay = 0) : base(pos, new RGBA(0, 1, 1, 1), preSpawnDelay) { }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Make the grunt move towards the player.
            Velocity = Vector2FromPoints(Pos, gameState.PlayerPos).GetModifiedLength(VelocityLength);

            // Move the grunt according to its velocity.
            Pos += Velocity;
        }
    }
}
