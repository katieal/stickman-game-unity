using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{

    public class AudioController : SerializedMonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        
        [SerializeField]
        [Tooltip("Audio Index is the index of the corresponding clip in Managers.AudioData.SoundEffects")]
        [DictionaryDrawerSettings(KeyLabel = "Audio DisplayName", ValueLabel = "Audio Index")]
        private Dictionary<string, int> _audioClips = new()
        {
            { "Footsteps", -1 },
            { "TakeDamage", -1 },
            { "Attack1", -1 },
            { "Attack2", -1 },
        };

        private void Awake()
        {
            if (_audioClips["Footsteps"] != -1) { _audioSource.clip = GetClip("Footsteps"); }
        }

        public void PlayFootsteps(bool isEnabled)
        {
            if (_audioClips["Footsteps"] == -1) { return; }

            if (isEnabled) { _audioSource.Play(); }
            else { _audioSource.Stop(); }
        }

        public void PlayClip(string clipName)
        {
            AudioSource.PlayClipAtPoint(GetClip(clipName), transform.position);
        }

        private AudioClip GetClip(string clipName)
        {
            #region Debug
            Debug.Assert(_audioClips.ContainsKey(clipName), "Audio clip not found!");
            #endregion

            return Managers.AudioData.GetSoundEffect(_audioClips[clipName]);
        }
    }
}