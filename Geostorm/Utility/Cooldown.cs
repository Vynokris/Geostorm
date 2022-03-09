using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geostorm.Utility
{
    public class Cooldown
    {
        public int Duration { get; private set; } = 0;
        public int Counter  { get; set; }         = 0;

        public Cooldown() {  }
        public Cooldown(in int duration) { Duration = duration; Counter = duration; }

        public bool Update()
        {
            if (Counter > 0)
            {
                Counter--;
                return false;
            }
            return true;
        }

        public bool HasEnded()
        {
            if (Counter > 0)
                return false;
            return true;
        }

        public float CompletionRatio()
        {
            return (float)Counter / (float)Duration;
        }

        public void Reset()
        {
            Counter = Duration;
        }

        public void ChangeDuration(in int duration)
        {
            Duration = duration;
            Counter  = duration;
        }
    }
}
