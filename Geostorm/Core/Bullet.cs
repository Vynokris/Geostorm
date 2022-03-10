using System.Numerics;
using static MyMathLib.Geometry2D;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Bullet : Entity
    {
        public Bullet() { }
        public Bullet(Vector2 pos, float rotation) : base(pos, Vector2FromAngle(rotation, 20), rotation, 1) { }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents)
        {
            // Move the bullet according to its velocity.
            Pos += Velocity;

            // Destroy the bullet when it hits the edge of the screen.
            if (-5 > Pos.X || Pos.X > gameState.ScreenSize.X ||
                -5 > Pos.Y || Pos.Y > gameState.ScreenSize.Y)
            {
                Health = 0;
            }
        }
    }
}
