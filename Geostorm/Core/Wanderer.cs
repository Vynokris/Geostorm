using static System.MathF;
using System.Numerics;
using static MyMathLib.Geometry2D;
using Geostorm.GameData;

namespace Geostorm.Core
{
    class Wanderer : Enemy
    {
        public Wanderer() { }
        public Wanderer(Vector2 pos, int spawnDelay) : base(pos, 1, spawnDelay) { }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents)
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
