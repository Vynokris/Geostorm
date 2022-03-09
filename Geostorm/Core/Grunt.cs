

namespace Geostorm.Core
{
    public class Grunt : Enemy
    {
        public override void DoUpdate(GameInputs inputs)
        {
            pos += velocity;
        }
    }
}
