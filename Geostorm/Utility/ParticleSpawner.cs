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
                    gameEvents.Add(new ParticleSpawnedEvent(ParticlesPerKill, killEvent.enemy.Pos, killEvent.enemy.Color));

                if (gameEvents[i] is SnakeBodyPartHitEvent hitEvent)
                    gameEvents.Add(new ParticleSpawnedEvent(3, hitEvent.snakeBodyPart.Pos, hitEvent.snakeBodyPart.Color));
            }
        }
    }
}
