using System.Numerics; 
using System.Collections.Generic;

using static System.MathF;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.GameData;

namespace Geostorm.Core
{
    class Wanderer : Enemy
    {
        public Wanderer() { }
        public Wanderer(Vector2 pos, float preSpawnDelay = 0) : base(pos, new RGBA(1, 0, 1, 1), preSpawnDelay) { }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Roatate the wanderer.
            Rotation += PI/30;

            // Rotate the velocity by a small random amount.
            System.Random rnd = new();
            Velocity = Velocity.GetRotated(rnd.Next() % PI/10 - PI/20);

            // Move the grunt according to its velocity.
            Pos += Velocity;

            // Bounce on the screen borders.
            if (0 > Pos.X || Pos.X > gameState.ScreenSize.X)
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            if (0 > Pos.Y || Pos.Y > gameState.ScreenSize.Y)
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
        }
    }
}
