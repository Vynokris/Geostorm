using System.Numerics;
using System.Collections.Generic;

using static MyMathLib.Geometry2D;

using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Grunt : Enemy
    {
        public Grunt() { }
        public Grunt(Vector2 pos, float preSpawnDelay = 0) : base(pos, preSpawnDelay) { }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Make the grunt move towards the player.
            Velocity = Velocity.GetModifiedAngle(Vector2FromPoints(Pos, gameState.PlayerPos).GetAngle());

            // Move the grunt according to its velocity.
            Pos += Velocity;
        }
    }
}
