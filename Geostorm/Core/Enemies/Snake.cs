using System.Numerics;
using System.Collections.Generic;

using static System.MathF;
using static MyMathLib.Arithmetic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;

using Geostorm.GameData;

namespace Geostorm.Core
{
    public class SnakeBodyPart : IEntity
    {
        public Vector2 Pos      { get; set; } = Vector2Zero();
        public Vector2 Velocity { get; set; } = Vector2Zero();
        public Vector2 Scale    { get; set; } = Vector2Create(1, 1);
        public float   Rotation { get; set; } = 0;
        public RGBA    Color    { get; set; } = new(1f, 0.83f, 0.54f, 1f);

        public readonly float SegmentLength   = 18*2;
        public Segment2       DirectorSegment = new();

        public SnakeBodyPart() { }
        public SnakeBodyPart(Vector2 pos, float rotation) { Pos = pos; Rotation = rotation; }

        public void Update(Vector2 directorPointA) 
        {
            DirectorSegment.A = directorPointA;
            Vector2 AB = Vector2FromSegment(DirectorSegment);
            AB.SetLength(SegmentLength);
            DirectorSegment.B = DirectorSegment.A + AB;

            // Update position and rotation.
            Pos = DirectorSegment.GetCenterOfMass();
            Rotation = Vector2FromSegment(DirectorSegment).GetNegated().GetAngle();
        }
    }

    public class Snake : Enemy
    {
        public int RotationDir = 0;
        public readonly int   BodyPartsCount = 10;
        public List<SnakeBodyPart> BodyParts = new();

        public Snake() { }
        public Snake(Vector2 pos, float preSpawnDelay = 0, int minLen = 0, int maxLen = 0) : base(pos, new RGBA(0.51f, 0.47f, 0.9f, 1f), preSpawnDelay) 
        {
            System.Random rng = new();
            Rotation = 0;
            Velocity = Vector2Create(3, 0);

            if (minLen == 0 && maxLen == 0)
                BodyPartsCount = rng.Next(10, 15);
            else
                BodyPartsCount = rng.Next(minLen, maxLen);

            // Initialize the body parts.
            for (int i = 0; i < BodyPartsCount; i++) { 
                BodyParts.Add(new SnakeBodyPart(Pos - new Vector2(36*(i+1), 0), Rotation));
                BodyParts[i].Scale = new Vector2(1f, 1 - ClampAbove(i-4, 0) / (float)BodyPartsCount);
            }

            // Once to take their starting positions.
            for (int i = 0; i < BodyPartsCount; i++) 
            { 
                if (i == 0)
                    BodyParts[i].Update(Pos - Vector2FromAngle(Rotation, 18));
                else
                    BodyParts[i].Update(BodyParts[i-1].DirectorSegment.B);
            }
        }

        public override void DoUpdate(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents)
        {
            // Choose a random rotation direction.
            System.Random rng = new();
            int randInt = rng.Next(0, 100);
            if (randInt < 20)
            {
                randInt = rng.Next(0, 100);
                if (randInt < 33)
                    RotationDir = -1;
                else if (randInt < 66)
                    RotationDir = 1;
                else
                    RotationDir = 0;
            }

            // Increment rotation.
            Velocity = Velocity.GetRotated(PI / 100 * RotationDir);
            Rotation = Velocity.GetAngle();

            // Move the snake according to its velocity.
            Pos += Velocity;

            // Bounce on screen borders.
            if (0 > Pos.X || Pos.X > gameState.ScreenSize.X) { 
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
                Rotation = (Rotation+PI) % (2*PI);
            }
            if (0 > Pos.Y || Pos.Y > gameState.ScreenSize.Y) { 
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
                Rotation = (Rotation+PI) % (2*PI);
            }

            // Move the snake parts to follow the head.
            for (int i = 0; i < BodyPartsCount; i++) 
            { 
                if (i == 0)
                    BodyParts[i].Update(Pos - Vector2FromAngle(Rotation, 18));
                else
                    BodyParts[i].Update(BodyParts[i-1].DirectorSegment.B);
            }
        }
    }
}
