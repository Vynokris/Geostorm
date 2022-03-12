﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geostorm.Utility
{
    public class Cooldown
    {
        public float Duration { get; private set; } = 0;
        public float Counter  { get; set; }         = 0;

        public Cooldown() {  }
        public Cooldown(in float duration) { Duration = duration; Counter = duration; }

        public bool Update(float deltaTime)
        {
            if (Counter > 0) {
                Counter -= deltaTime;
                return false;
            }
            else {
                Counter = 0;
                return true;
            }
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

        public void ChangeDuration(in float duration)
        {
            Duration = duration;
            Counter  = duration;
        }
    }
}
