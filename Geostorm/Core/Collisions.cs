using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Geometry2D;
using static MyMathLib.Collisions2D;
using Geostorm.Renderer;
using Geostorm.GameData;

namespace Geostorm.Core
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
                        /*
                        player.Invincibility.Reset();
                        player.Health--;
                        */
                        gameEvents.Add(new PlayerDamagedEvent());
                        if (player.Health <= 1)
                            gameEvents.Add(new PlayerKilledEvent());
                    }
                    
                    // Check collisions between bullets and enemies.
                    foreach (Bullet bullet in bullets)
                    {
                        if (CheckEntityCollisions(bullet, enemy, entityVertices)) 
                        {
                            /*
                            enemy.Health -= 1;
                            bullet.Health = 0;
                            */
                            gameEvents.Add(new EnemyDamagedEvent(enemy, bullet));
                            if (enemy.Health <= 1)
                                gameEvents.Add(new EnemyKilledEvent(enemy));
                        }
                    }
                }
            }
        }

        public static bool CheckEntityCollisions<T1, T2>(in T1 entity1, in T2 entity2, in EntityVertices entityVertices) where T1 : IEntity where T2 : IEntity
        {
            bool colliding = false;

            Vector2[] vertices1 = entityVertices.GetEntityVertices(entity1);
            Vector2[] vertices2 = entityVertices.GetEntityVertices(entity2);

            Vector2 prevVertex1 = vertices1[0];
            Vector2 prevVertex2 = vertices2[0];
            foreach(Vector2 vertex1 in vertices1)
            {
                foreach(Vector2 vertex2 in vertices2)
                {
                    if (prevVertex1 != vertex1 && prevVertex2 != vertex2)
                    {
                        Segment2 segment1 = new(prevVertex1, vertex1);
                        Segment2 segment2 = new(prevVertex2, vertex2);

                        if (CollisionSAT(segment1, segment2))
                            colliding = true;
                    }
                }
            }

            return colliding;
        }
    }
}
