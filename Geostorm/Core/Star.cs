using System.Numerics;
using System.Collections.Generic;

using static System.MathF;
using static MyMathLib.Arithmetic;
using static MyMathLib.Colors;

using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Star : Entity
    {
        public int  Radius { get; private set; } = 0;

        public Star(int screenW, int screenH)
        {
            System.Random rnd = new();

            // Get a random position and radius for the star.
            Pos      = new(rnd.Next(0, screenW), rnd.Next(0, screenH));
            Radius   = (int)ClampAbove(rnd.Next(-1, 4), 1.0f);
            Velocity = new(-0.7f * Radius, 0);

            // Get random red green and blue values.
            float R = rnd.Next(135, 255) / 255.0f;
            float G = rnd.Next(135, 255) / 255.0f;
            float B = rnd.Next(135, 255) / 255.0f;

            // Make the color as white as possible.
            float minVal = Min(1-R, Min(1-G, 1-B));
            R += minVal;
            G += minVal;
            B += minVal;

            // Set the star's color.
            Color = new(R, G, B, 1);
        }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Move the star according to its velocity.
            Pos += Velocity;

            // Screen wrapping.
            if (Pos.X < 0)
                Pos += new Vector2(gameState.ScreenSize.X, 0);
        }
    }
}
