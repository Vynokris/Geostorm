using System;
using System.Collections.Generic;
using System.Linq;

using Raylib_cs;

using Geostorm.GameData;

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

        public List<Sound>   ambiances       = new();
        public AmbianceNames currentAmbiance = AmbianceNames.None;

        public Music BassTheme = Raylib.LoadMusicStream("Sounds/Ambiance/BassTheme.ogg");

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
            Raylib.SetSoundVolume(sounds[(int)SoundNames.EnemyKilled],  0.6f);
            Raylib.SetSoundVolume(sounds[(int)SoundNames.GeomPickedUp], 0.7f);

            // Load all of the game's ambiances.
            for (int i = 0; i < 4; i++)
            {
                string ambianceName = "Sounds/Ambiance/" + ((AmbianceNames)i).ToString() + ".ogg";
                ambiances.Add(Raylib.LoadSound(ambianceName));
            }

            // Change the ambiance volumes.
            Raylib.SetSoundVolume(ambiances[(int)AmbianceNames.Monolith],    0.2f);
            Raylib.SetSoundVolume(ambiances[(int)AmbianceNames.EeryVoid],    0.2f);
            Raylib.SetSoundVolume(ambiances[(int)AmbianceNames.DeepSpace],   0.2f);
            Raylib.SetSoundVolume(ambiances[(int)AmbianceNames.AlienVoices], 0.2f);
        }

        ~SoundController()
        {
            foreach (Sound sound in sounds)
                Raylib.UnloadSound(sound);
            foreach (Sound ambiance in ambiances)
                Raylib.UnloadSound(ambiance);
        }

        public void UpdateAmbiance()
        {
            // Update ambiance.
            Raylib.UpdateMusicStream(BassTheme);

            // 0.01% chance to play the bass loop.
            if (!Raylib.IsMusicStreamPlaying(BassTheme) && Rng.Next(0, 10000) < 1)
            {
                Raylib.PlayMusicStream(BassTheme);
            }

            // 0.3% chance to play a random ambiance sounds.
            if (currentAmbiance == AmbianceNames.None && Rng.Next(0, 10000) < 30)
            {
                currentAmbiance = (AmbianceNames)Rng.Next(0, ambiances.Count);
                Raylib.PlaySoundMulti(ambiances[(int)currentAmbiance]);
            }

            // Check if the current ambiance if still playing.
            if (currentAmbiance != AmbianceNames.None && !Raylib.IsSoundPlaying(ambiances[(int)currentAmbiance]))
            {
                currentAmbiance = AmbianceNames.None;
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
