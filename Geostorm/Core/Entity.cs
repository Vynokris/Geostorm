using System.Numerics;
using static MyMathLib.Geometry2D;

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
        public Entity(Vector2 _pos, Vector2 _velocity, float _rotation) { Pos = _pos; Velocity = _velocity; Rotation = _rotation; }
        public abstract void Update(in GameInputs inputs, ref GameEvents gameEvents);
    }
}
