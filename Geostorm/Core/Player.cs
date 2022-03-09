using System.Numerics;
using static System.MathF;
using static MyMathLib.Geometry2D;

namespace Geostorm.Core
{
    public class Player : Entity
    {
        public int Score;

        private readonly int maxVelocity = 10;


        public override void Update(GameInputs inputs)
        {
            // -- Accelerate -- //
            if (inputs.Movement != Vector2Zero())
            {
                // Get the direction of the joystick.
                float dirAngle = inputs.Movement.GetAngle();

                // If the player is under the maximum velocity, make him accelerate.
                if (velocity.Length() < maxVelocity)
                    velocity += Vector2FromAngle(dirAngle, 0.6f);

                // If the player is at maximum velocity, stop accelerating.
                else
                    velocity = velocity * 0.7f + Vector2FromAngle(dirAngle, 0.3f);
            }

            // -- Slow down -- //
            else if (velocity.Length() > 0)
            {
                velocity *= 0.97f;
            }

            // -- Turn -- //
            if (inputs.ShootDir != Vector2Zero())
            {
                rotation = inputs.ShootDir.GetAngle();
            }
            else
            {
                rotation = Vector2FromPoints(pos, inputs.ShootTarget).GetAngle();
            }

            // -- Shoot -- //
            if (inputs.Shoot)
            {
                // player_shoot(player);
            }

            // -- Dash -- //
            if (inputs.Dash)
            {
                // player_dash(player);
            }

            // Move the player according to its velocity.
            pos += velocity;
        }
    }
}
