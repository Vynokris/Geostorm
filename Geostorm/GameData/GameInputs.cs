using System.Numerics;

namespace Geostorm.GameData
{
    public class GameInputs
    {
        public Vector2 Movement;    // WASD / left joystick.
        public bool    Dash;        // Space / Left Shift / left 2nd trigger.
        public bool    Shoot;       // Left click / right 2nd trigger.
        public Vector2 ShootDir;    // Direction given by joystick.
        public Vector2 ShootTarget; // Mouse position.
        public bool    CheatMenu;   // Alt+C / Start.
        public bool    DebugMenu;   // Alt+D / Select.
    }
}
