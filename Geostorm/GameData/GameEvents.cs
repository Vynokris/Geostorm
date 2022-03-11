using System.Numerics;
using System.Collections.Generic;
using Geostorm.Core;

namespace Geostorm.GameData
{
    public interface IEventListener
    {
        public void HandleEvents(in List<GameEvent> gameEvents);
    }

    public abstract class GameEvent { }

    public class PlayerMoveEvent : GameEvent
    {
        public Vector2 Pos;

        public PlayerMoveEvent(Vector2 newPos) { Pos = newPos; }
    }

    public class PlayerShootEvent : GameEvent
    {
        public Bullet bullet;

        public PlayerShootEvent(Bullet newBullet) { bullet = newBullet; }
    }

    public class PlayerDashEvent : GameEvent
    {
    }

    public class PlayerDamagedEvent : GameEvent
    {
    }

    public class PlayerKilledEvent : GameEvent
    {
    }

    public class EnemySpawnedEvent : GameEvent
    {
        public Enemy enemy;

        public EnemySpawnedEvent(Enemy spawnedEnemy) { enemy = spawnedEnemy; }
    }

    public class EnemyDamagedEvent : GameEvent
    {
        public Enemy  enemy;
        public Bullet bullet;

        public EnemyDamagedEvent(Enemy killedEnemy, Bullet killingBullet) { enemy = killedEnemy; bullet = killingBullet; }
    }

    public class EnemyKilledEvent : GameEvent
    {
        public Enemy  enemy;

        public EnemyKilledEvent(Enemy killedEnemy) { enemy = killedEnemy; }
    }
}
