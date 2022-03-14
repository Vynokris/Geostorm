using System.Numerics;
using System.Collections.Generic;

using Geostorm.Core;
using Geostorm.GameData;

namespace Geostorm.Utility
{
    public class ParticleSpawner
    {
        public int ParticlesPerKill = 50;

        public void Update(ref List<GameEvent> gameEvents)
        {
            for (int i = 0; i < gameEvents.Count; i++)
            {
                if (gameEvents[i] is EnemyKilledEvent killEvent) 
                    for (int j = 0; j < ParticlesPerKill; j++)
                        gameEvents.Add(new ParticleSpawnedEvent(new Particle(killEvent.enemy.Pos, killEvent.enemy.Color)));
            }
        }
    }
}
