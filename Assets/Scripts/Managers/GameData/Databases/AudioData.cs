using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    [HideLabel, Serializable]
    public struct AudioEntry
    {
        [HideLabel, HorizontalGroup(Gap = 20)] [ReadOnly] public int Id;
        [HideLabel, HorizontalGroup] public AudioClip Audio;
    }

    /// <summary>
    /// Class to hold a database of all audio files for the game
    /// </summary>
    public class AudioData : SerializedMonoBehaviour
    {
        [Title("Background Music")]
        [ListDrawerSettings(CustomAddFunction = "AddNewBackground", DraggableItems = false, NumberOfItemsPerPage = 10, ShowPaging = true)]
        public List<AudioEntry> Background = new List<AudioEntry>();

        [Title("Sound Effects")]
        [ListDrawerSettings(CustomAddFunction = "AddNewSoundEffect", DraggableItems = false, NumberOfItemsPerPage = 10, ShowPaging = true)]
        public List<AudioEntry> SoundEffects = new List<AudioEntry>();

        private static List<AudioEntry> _background;
        private static List<AudioEntry> _soundEffects;

        #region Inspector Methods
        public AudioEntry AddNewBackground()
        {
            AudioEntry sound = new AudioEntry();
            if (Background.Count > 0)
            {
                sound.Id = Background[^1].Id + 1;
            }
            else
            {
                sound.Id = 0;
            }

            return sound;
        }

        public AudioEntry AddNewSoundEffect()
        {
            AudioEntry sound = new AudioEntry();
            if (SoundEffects.Count > 0)
            {
                sound.Id = SoundEffects[^1].Id + 1;
            }
            else
            {
                sound.Id = 0;
            }

            return sound;
        }
        #endregion

        private void Awake()
        {
            _background = Background;
            _soundEffects = SoundEffects;
        }

        public static AudioClip GetBackgroundAudio(int id)
        {
            AudioEntry clip = _background.Find(x => x.Id == id);
            return clip.Audio;
        }

        public static AudioClip GetSoundEffect(int id)
        {
            AudioEntry clip = _soundEffects.Find(x => x.Id == id);
            return clip.Audio;
        }
    }
}