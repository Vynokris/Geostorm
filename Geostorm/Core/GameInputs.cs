using System.Numerics;

namespace Geostorm.Core
{
    public class GameInputs
    {
        public float DeltaTime;
        public Vector2 Movement;    // WASD.
        public bool Dash;           // Left shift.
        public bool Shoot;          // Left click.
        public Vector2 ShootDir;    // Direction given by joystick.
        public Vector2 ShootTarget; // Mouse position.
    }
}
