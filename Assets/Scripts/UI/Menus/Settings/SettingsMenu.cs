using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

using EventChannel;

namespace UserInterface
{
    public class SettingsMenu : MonoBehaviour
    {
        //[Title("Invoked Events")]
        //[SerializeField] private SettingsDataEventSO _saveSettingsSO;

        [BoxGroup("Background Music")]
        [BoxGroup("Background Music/Toggle")]
        [SerializeField] private Toggle _musicToggle;
        [BoxGroup("Background Music/Toggle")]
        [SerializeField] private BoolEventSO _toggleMusicSO;
        [BoxGroup("Background Music/Volume")]
        [SerializeField] private Slider _backgroundVolumeSlider;
        [BoxGroup("Background Music/Volume")]
        [SerializeField] private FloatEventSO _setBackgroundVolumeSO;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            LoadData();
            _musicToggle.onValueChanged.AddListener(OnToggleMusic);
            _backgroundVolumeSlider.onValueChanged.AddListener(SetBGVolume);
        }
        private void OnDisable()
        {
            // save settings when menu is closed
            SaveData();

            _musicToggle.onValueChanged.RemoveListener(OnToggleMusic);
            _backgroundVolumeSlider.onValueChanged.RemoveListener(SetBGVolume);
        }

        #region SaveData
        private void LoadData()
        {
            _musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 0) != 0;
            _backgroundVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.01f);
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("MusicEnabled", (_musicToggle.isOn) ? 1 : 0);
            PlayerPrefs.SetFloat("MusicVolume", _backgroundVolumeSlider.value);
        }
        #endregion

        private void OnToggleMusic(bool isToggled)
        {
            _toggleMusicSO.InvokeEvent(isToggled);
        }

        private void SetBGVolume(float value)
        {
            _setBackgroundVolumeSO.InvokeEvent(value);
        }
    }
}