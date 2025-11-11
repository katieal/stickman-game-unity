using EventChannel;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        [Title("Audio Sources")]
        [SerializeField] private AudioSource _backgroundAudio;
        [SerializeField] private AudioMixer _backgroundMixer;
        [SerializeField] private AudioSource _effectsAudio;
        [SerializeField] private AudioMixer _effectsMixer;

        [Title("Listening to Events")]
        [SerializeField] private BoolEventSO _toggleMusicSO;
        [SerializeField] private FloatEventSO _setBackgroundVolumeSO;

        private void Awake()
        {
            LoadData();
        }

        private void OnEnable()
        {
            _toggleMusicSO.OnInvokeEvent += OnPlayBackgroundMusic; 
            _setBackgroundVolumeSO.OnInvokeEvent += OnSetBackgroundVolume;
        }
        private void OnDisable()
        {
            _toggleMusicSO.OnInvokeEvent -= OnPlayBackgroundMusic;
            _setBackgroundVolumeSO.OnInvokeEvent -= OnSetBackgroundVolume;
        }

        #region SaveData
        // load  data from playerprefs
        private void LoadData()
        {
            _backgroundMixer.SetFloat("BackgroundMusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 0.01f)) * 20);
            OnPlayBackgroundMusic(PlayerPrefs.GetInt("MusicEnabled", 0) != 0);
        }
        #endregion

        private void OnSetBackgroundVolume(float value)
        {
            _backgroundMixer.SetFloat("BackgroundMusicVolume", Mathf.Log10(value) * 20);
        }


        // TODO: assign audio indexes to scenes and listen to sceneloaded event
        // to change background music based on scene
        private void OnPlayBackgroundMusic(bool isEnabled)
        {
            if (isEnabled) { _backgroundAudio.Play(); }
            else { _backgroundAudio.Stop(); }
            
        }



        public static void PlaySoundEffect(int id, Vector3 position)
        {
            AudioSource.PlayClipAtPoint(AudioData.GetSoundEffect(id), position);
        }
    }
}