using System.Numerics;
using System.Collections.Generic;
using Geostorm.Core;

namespace Geostorm.GameData
{
    public class GameState
    {
        public Vector2 ScreenSize;
        public int     FPS;
        public float   DeltaTime;
        public double  GameDuration = 0;
        public int     Score        = 0;
        public int     Multiplier   = 1;
        public Vector2 PlayerPos;
        public List<Bullet> bullets; // This is used by weaver enemies to dodge.

        public GameState(int screenW, int screenH) { ScreenSize = new(screenW, screenH); }
    }
}
