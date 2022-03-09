using System.Numerics;

namespace Geostorm.Core
{
    public class Bullet : Entity
    {
        public override void Update(GameInputs inputs)
        {
            pos += velocity;
        }
    }
}
