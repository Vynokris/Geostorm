using System.Numerics;

namespace Geostorm.Core
{
    public interface IEntity
    {
        public Vector2 pos      { get; set; }
        public Vector2 velocity { get; set; }
        public float   rotation { get; set; }
    }

    public abstract class Entity : IEntity
    {
        public Vector2 pos      { get; set; }
        public Vector2 velocity { get; set; }
        public float   rotation { get; set; }

        public abstract void Update(GameInputs inputs);
    }
}
