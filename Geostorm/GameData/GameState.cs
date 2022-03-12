using System;
using System.Numerics;

namespace Geostorm.GameData
{
    public class GameState
    {
        public Vector2 ScreenSize;
        public float   DeltaTime;
        public double  GameDuration = 0;
        public int     Score        = 0;
        public int     Multiplier   = 1;
        public Vector2 PlayerPos;

        public GameState(int screenW, int screenH) { ScreenSize = new(screenW, screenH); }
    }
}
