using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Colors;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public interface IEntity
    {
        public Vector2 Pos      { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Scale    { get; set; }
        public float   Rotation { get; set; }
        public RGBA    Color    { get; set; }
    }

    public abstract class Entity : IEntity
    {
        public Vector2 Pos      { get; set; } = Vector2Zero();
        public Vector2 Velocity { get; set; } = Vector2Zero();
        public Vector2 Scale    { get; set; } = Vector2Create(1, 1);
        public float   Rotation { get; set; } = 0;
        public RGBA    Color    { get; set; } = new();

        public Entity() { }
        public Entity(Vector2 pos, Vector2 velocity, float rotation, RGBA color) { Pos = pos; Velocity = velocity; Rotation = rotation; Color = color; }
        public abstract void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents);
    }
}
