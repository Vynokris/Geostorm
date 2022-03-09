﻿using System.Numerics;

namespace Geostorm.Core
{
    public class GameInputs
    {
        public Vector2 screenSize;
        public float DeltaTime;
        public Vector2 Movement;    // WASD / left joystick.
        public bool Dash;           // Left shift / left 2nd trigger.
        public bool Shoot;          // Left click / right 2nd trigger.
        public Vector2 ShootDir;    // Direction given by joystick.
        public Vector2 ShootTarget; // Mouse position.
    }
}
