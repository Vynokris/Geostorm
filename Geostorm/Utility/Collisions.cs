using System.Numerics;
using System.Collections.Generic;

using static MyMathLib.Geometry2D;
using static MyMathLib.Collisions2D;

using Geostorm.Core;
using Geostorm.GameData;
using Geostorm.Renderer;

namespace Geostorm.Utility
{
    public static class Collisions
    {
        public static void DoCollisions(in Player player, in List<Bullet> bullets, in List<Enemy> enemies, in EntityVertices entityVertices, ref List<GameEvent> gameEvents)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.SpawnDelay.Counter <= 0)
                { 
                    // Check collisions between player and enemies.
                    if (player.Invincibility.HasEnded() && CheckEntityCollisions(player, enemy, entityVertices)) 
                    { 
                        gameEvents.Add(new PlayerDamagedEvent());
                        if (player.Health <= 1)
                            gameEvents.Add(new PlayerKilledEvent());
                    }
                    
                    // Check collisions between bullets and enemies.
                    foreach (Bullet bullet in bullets)
                    {
                        if (CheckEntityCollisions(bullet, enemy, entityVertices)) 
                        {
                            gameEvents.Add(new BulletDestroyedEvent(bullet));
                            gameEvents.Add(new EnemyKilledEvent(enemy));
                        }
                    }
                }
            }
        }

        public static bool CheckEntityCollisions<T1, T2>(in T1 entity1, in T2 entity2, in EntityVertices entityVertices) where T1 : IEntity where T2 : IEntity
        {
            bool colliding = false;

            if (entity1.Pos.GetDistanceFromPoint(entity2.Pos) < 30)
            {
                Vector2[] vertices1 = entityVertices.GetEntityVertices(entity1);
                Vector2[] vertices2 = entityVertices.GetEntityVertices(entity2);

                for (int i = 0; i < vertices1.Length-1; i++)
                {
                    for (int j = 0; j < vertices2.Length-1; j++)
                    {
                        Segment2 segment1 = new(vertices1[i], vertices1[i+1]);
                        Segment2 segment2 = new(vertices2[j], vertices2[j+1]);

                        if (CollisionSAT(segment1, segment2))
                            colliding = true;
                    }
                }
            }

            return colliding;
        }
    }
}
