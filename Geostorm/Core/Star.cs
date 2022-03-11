using System.Numerics;
using static System.MathF;
using static MyMathLib.Arithmetic;
using static MyMathLib.Colors;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public class Star : Entity
    {
        public int  Radius { get; private set; } = 0;
        public RGBA Color  { get; private set; } = new(0, 0, 0, 0);

        public Star(int screenW, int screenH)
        {
            System.Random rnd = new();

            // Get a random position and radius for the star.
            Pos      = new(rnd.Next(0, screenW), rnd.Next(0, screenH));
            Radius   = (int)ClampAbove(rnd.Next(-1, 4), 1.0f);
            Velocity = new(-0.7f * Radius, 0);

            // Get a random color.
            Color = new(rnd.Next(135, 255) / 255.0f, 
                        rnd.Next(135, 255) / 255.0f, 
                        rnd.Next(135, 255) / 255.0f, 1);

            // Make the color as white as possible.
            float minVal = Min(1-Color.R, Min(1-Color.G, 1-Color.B));
            Color.R += minVal;
            Color.G += minVal;
            Color.B += minVal;
        }

        public override void Update(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents)
        {
            // Move the star according to its velocity.
            Pos += Velocity;

            // Screen wrapping.
            if (Pos.X < 0)
                Pos += new Vector2(gameState.ScreenSize.X, 0);
        }
    }
}
