using System.Numerics;
using static MyMathLib.Geometry2D;
using Geostorm.GameData;

namespace Geostorm.Core
{
    public interface IEntity
    {
        public Vector2 Pos      { get; set; }
        public Vector2 Velocity { get; set; }
        public float   Rotation { get; set; }
        public float   Health   { get; set; }
    }

    public abstract class Entity : IEntity
    {
        public Vector2 Pos      { get; set; } = Vector2Zero();
        public Vector2 Velocity { get; set; } = Vector2Zero();
        public float   Rotation { get; set; } = 0;
        public float   Health   { get; set; } = 0;

        public Entity() { }
        public Entity(Vector2 pos, Vector2 velocity, float rotation, float health) { Pos = pos; Velocity = velocity; Rotation = rotation; Health = health; }
        public abstract void Update(in GameState gameState, in GameInputs gameInputs, ref GameEvents gameEvents);
    }
}
