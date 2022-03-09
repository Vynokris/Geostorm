using System.Numerics;
using static MyMathLib.Geometry2D;

namespace Geostorm.Core
{
    public class Bullet : Entity
    {
        public bool destroyed { get; private set; } = false;

        public Bullet() { }
        public Bullet(Vector2 _pos, float _rotation) : base(_pos, Vector2FromAngle(_rotation, 15), _rotation) { }

        public override void Update(in GameInputs inputs, ref GameEvents  gameEvents)
        {
            // Move the bullet according to its velocity.
            Pos += Velocity;

            // Destroy the bullet when it hits the edge of the screen.
            if (-5 > Pos.X || Pos.X > inputs.screenSize.X ||
                -5 > Pos.Y || Pos.Y > inputs.screenSize.Y)
            {
                destroyed = true;
            }
        }
    }
}
