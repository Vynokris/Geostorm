using System;
using System.Collections.Generic;
using System.Linq;

using Raylib_cs;

using Geostorm.GameData;
using Geostorm.Utility;

namespace Geostorm.Renderer
{
    public enum SoundNames: int
    {
        UiFlicker              = 0,
        PlayerDash             = 1,
        PlayerInvincibilityEnd = 2,
        PlayerDamaged          = 3,
        PlayerKilled           = 4,
        EnemyKilled            = 5,
        BulletShot             = 6,
        GeomPickedUp           = 7,
        SnakeBodyPartHit       = 8,
    }

    public enum AmbianceNames: int
    {
        None        = -1,
        Monolith    =  0,
        EeryVoid    =  1,
        DeepSpace   =  2,
        AlienVoices =  3,
    }

    public class SoundController : IEventListener
    {
        public List<Sound> sounds = new();
        public List<bool>  soundsPlayedThisFrame;

        public Music    Ambiance     = Raylib.LoadMusicStream("Sounds/Ambiance.ogg");
        public Sound    BassTheme    = Raylib.LoadSound("Sounds/BassTheme.ogg");
        public Cooldown BassCooldown = new(20);

        public Random Rng = new();

        
        public SoundController()
        {
            // Load all of the game's sounds.
            for (int i = 0; i < 9; i++)
            {
                string soundName = "Sounds/" + ((SoundNames)i).ToString() + ".ogg";
                sounds.Add(Raylib.LoadSound(soundName));
            }

            // Change the sound volumes.
            Raylib.SetSoundVolume(sounds[(int)SoundNames.UiFlicker],    0.6f);
            Raylib.SetSoundVolume(sounds[(int)SoundNames.PlayerDash],   0.1f);
            Raylib.SetSoundVolume(sounds[(int)SoundNames.BulletShot],   0.3f);
            Raylib.SetSoundVolume(sounds[(int)SoundNames.EnemyKilled],  0.4f);
            Raylib.SetSoundVolume(sounds[(int)SoundNames.GeomPickedUp], 0.7f);

            // Change the ambiance volume.
            Raylib.SetMusicVolume(Ambiance, 0.2f);

            // Play the ambiance and loop it.
            Ambiance.looping = true;
            Raylib.PlayMusicStream(Ambiance);
        }

        ~SoundController()
        {
            foreach (Sound sound in sounds)
                Raylib.UnloadSound(sound);
            Raylib.UnloadMusicStream(Ambiance);
        }

        public void UpdateAmbiance(float deltaTime)
        {
            // Update ambiance.
            Raylib.UpdateMusicStream(Ambiance);

            // Play the bass theme if the bass cooldown is finished,
            if (!Raylib.IsSoundPlaying(BassTheme) && BassCooldown.Update(deltaTime))
            {
                Raylib.PlaySound(BassTheme);
                BassCooldown.ChangeDuration(Rng.Next(20, 30));
            }
        }
        
        public void HandleEvents(in List<GameEvent> gameEvents)
        {
            // Reset the boolean list.
            soundsPlayedThisFrame = Enumerable.Repeat(false, sounds.Count).ToList();

            // Listen to events and play sounds accordingly.
            foreach (GameEvent gameEvent in gameEvents)
            {
                Type eventType = gameEvent.GetType();

                if      (eventType == typeof(UiFlickerEvent))
                    PlaySound(SoundNames.UiFlicker);

                else if (eventType == typeof(PlayerDashEvent))
                    PlaySound(SoundNames.PlayerDash);

                else if (eventType == typeof(PlayerInvincibilityEndEvent))
                    PlaySound(SoundNames.PlayerInvincibilityEnd);
                
                else if (eventType == typeof(PlayerDamagedEvent)) 
                    PlaySound(SoundNames.PlayerDamaged);

                else if (eventType == typeof(PlayerKilledEvent)) 
                    PlaySound(SoundNames.PlayerKilled);

                else if (eventType == typeof(EnemyKilledEvent))
                    PlaySound(SoundNames.EnemyKilled);

                else if (eventType == typeof(BulletShotEvent))
                    PlaySound(SoundNames.BulletShot);

                else if (eventType == typeof(GeomPickedUpEvent))
                    PlaySound(SoundNames.GeomPickedUp);

                else if (eventType == typeof(SnakeBodyPartHitEvent))
                    PlaySound(SoundNames.SnakeBodyPartHit);
            }
        }

        private void PlaySound(in SoundNames soundName)
        {
            if (!soundsPlayedThisFrame[(int)soundName])
            {
                Raylib.PlaySoundMulti(sounds[(int)soundName]);
                soundsPlayedThisFrame[(int)soundName] = true;
            }
        }
    }
}
