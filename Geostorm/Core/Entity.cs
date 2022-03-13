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
        public float   Rotation { get; set; }
    }

    public abstract class Entity : IEntity
    {
        public Vector2 Pos      { get; set; } = Vector2Zero();
        public Vector2 Velocity { get; set; } = Vector2Zero();
        public float   Rotation { get; set; } = 0;

        public Entity() { }
        public Entity(Vector2 pos, Vector2 velocity, float rotation) { Pos = pos; Velocity = velocity; Rotation = rotation; }
        public abstract void Update(in GameState gameState, in GameInputs gameInputs, ref List<GameEvent> gameEvents);
    }
}
