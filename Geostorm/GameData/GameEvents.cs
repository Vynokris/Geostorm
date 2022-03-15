using System.Numerics;
using System.Collections.Generic;
using static MyMathLib.Colors;
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

        public PlayerMoveEvent(in Vector2 newPos) { Pos = newPos; }
    }

    public class BulletShotEvent : GameEvent
    {
        public Bullet bullet;

        public BulletShotEvent(in Bullet newBullet) { bullet = newBullet; }
    }

    public class PlayerDashEvent : GameEvent
    {
    }

    public class PlayerInvincibilityEndEvent : GameEvent
    {
    }

    public class PlayerDamagedEvent : GameEvent
    {
    }

    public class PlayerKilledEvent : GameEvent
    {
    }

    public class BulletDestroyedEvent : GameEvent
    {
        public Bullet bullet;

        public BulletDestroyedEvent(in Bullet destroyedBullet) { bullet = destroyedBullet; }
    }

    public class GeomPickedUpEvent : GameEvent
    {
        public Geom geom;

        public GeomPickedUpEvent(in Geom pickedUpGeom) { geom = pickedUpGeom; }
    }

    public class GeomDespawnEvent : GameEvent
    {
        public Geom geom;

        public GeomDespawnEvent(in Geom despawnedGeom) { geom = despawnedGeom; }
    }

    public class EnemySpawnedEvent : GameEvent
    {
        public Enemy enemy;

        public EnemySpawnedEvent(in Enemy spawnedEnemy) { enemy = spawnedEnemy; }
    }

    public class EnemyKilledEvent : GameEvent
    {
        public Enemy enemy;

        public EnemyKilledEvent(in Enemy killedEnemy) { enemy = killedEnemy; }
    }

    public class BounceOnSnakeBodyEvent : GameEvent
    {
        public SnakeBodyPart snakeBodyPart;

        public BounceOnSnakeBodyEvent(in SnakeBodyPart bodyPart) { snakeBodyPart = bodyPart; }
    }

    public class SnakeBodyPartHitEvent : GameEvent
    {
        public SnakeBodyPart snakeBodyPart;

        public SnakeBodyPartHitEvent(in SnakeBodyPart bodyPart) { snakeBodyPart = bodyPart; }
    }

    public class ParticleSpawnedEvent : GameEvent
    {
        public int     Count;
        public Vector2 Pos;
        public RGBA    Color;

        public ParticleSpawnedEvent(in int count, Vector2 pos, RGBA color) { Count = count; Pos = pos; Color = color; }
    }

    public class ParticleDespawnEvent : GameEvent
    {
        public Particle particle;

        public ParticleDespawnEvent(in Particle despawnedParticle) { particle = despawnedParticle; }
    }

    public class UiFlickerEvent : GameEvent
    {
    }
}
