using System.Numerics;

namespace Geostorm.Core
{
    public class Grunt : Enemy
    {
        public Grunt() { }
        public Grunt(Vector2 _pos) : base(_pos) { }

        public override void DoUpdate(in GameInputs inputs, ref GameEvents gameEvents)
        {
            Pos += Velocity;
        }
    }
}
