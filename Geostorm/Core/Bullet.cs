using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Bullet : Entity
    {
        public Bullet() { }
        public Bullet(Vector2 pos, float rotation) : base(pos, Vector2FromAngle(rotation, 20), rotation, new RGBA(1, 1, 0, 1)) { }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Move the bullet according to its velocity.
            Pos += Velocity;

            // Destroy the bullet when it hits the edge of the screen.
            if (-5 > Pos.X || Pos.X > gameState.ScreenSize.X ||
                -5 > Pos.Y || Pos.Y > gameState.ScreenSize.Y)
            {
                gameEvents.Add(new BulletDestroyedEvent(this));
            }
        }
    }
}
