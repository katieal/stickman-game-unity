using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using EventChannel;

namespace UserInterface
{
    public class TrainingPopupController : MonoBehaviour
    {

        [Title("Components")]
        [SerializeField] private GameObject _trainPanel;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _exitButton;

        [Title("Listening to Events")]
        [SerializeField] private StringBoolUniTaskEventSO _trainEvent;

        private void OnEnable()
        {
            _trainPanel.SetActive(false);
            _trainEvent.OnStartEvent += OnTrainEvent;
            _exitButton.onClick.AddListener(OnExit);
        }
        private void OnDisable()
        {
            _trainEvent.OnStartEvent -= OnTrainEvent;
            _exitButton.onClick.RemoveAllListeners();
        }

        private void OnTrainEvent(string npcType)
        {
            _text.text = npcType;
            _trainPanel.SetActive(true);
        }

        private void OnExit()
        {
            _trainPanel.SetActive(false);
            _trainEvent.EndEvent(true);
        }
    }
}